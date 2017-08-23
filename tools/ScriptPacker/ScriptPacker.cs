// <copyright file="ScriptPacker.cs" company="Julian Bartel">
//      Copyright © Julian Bartel. All rights reserved.
// </copyright>
namespace JulianBartel.SE.Tools.ScriptPacker
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.MSBuild;
    using Microsoft.CodeAnalysis.Text;

    public class ScriptPacker
    {
        /// <summary>
        /// The original workspace.
        /// </summary>
        private readonly MSBuildWorkspace workspace = MSBuildWorkspace.Create();

        /// <summary>
        /// The workspace for creating the final script.
        /// </summary>
        private readonly AdhocWorkspace temporaryWorkspace = new AdhocWorkspace();

        /// <summary>
        /// The solutionPath.
        /// </summary>
        private readonly Solution solution;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptPacker"/> class.
        /// </summary>
        /// <param name="solutionPath">Path to the solution.</param>
        public ScriptPacker(string solutionPath)
        {
            this.solution = workspace.OpenSolutionAsync(solutionPath).Result;
        }

        /// <summary>
        /// Packs the specified project into a single code file.
        /// </summary>
        /// <param name="projectPath">Path to the primary project.</param>
        /// <param name="programTypeName">Type name of the program holding the actual script.</param>
        /// <returns>Code file usable as in-game script.</returns>
        public string PackProject(string projectPath, string programTypeName)
        {
            Project rootProject = solution.Projects.Single(p => Path.GetFullPath(p.FilePath) == Path.GetFullPath(projectPath));
            IEnumerable<Project> requiredProjects = CollectRequiredProjects(rootProject);
            string mergedCode = MergeProjects(requiredProjects);

            (ClassDeclarationSyntax program, CompilationUnitSyntax additionalCode) parts = SeparateProgram(mergedCode, programTypeName);

            string program = FinalizeProgramCode(parts.program);
            string dependentCode = FinalizeAdditionalCode(parts.additionalCode);

            string finalScriptCode = MergeProgramAndAdditionalCode(program, dependentCode);

            return finalScriptCode;
        }

        /// <summary>
        /// Merges the code from all <paramref name="projects"/> into 
        /// </summary>
        /// <param name="projects">The projects to merge.</param>
        /// <returns>Merged code of all projects.</returns>
        private static string MergeProjects(IEnumerable<Project> projects)
        {
            // compile projects
            SyntaxTreeMergerMerger merger = new SyntaxTreeMergerMerger();
            foreach (Compilation compilation in projects.Select(p => p.GetCompilationAsync().Result))
            {
                foreach (SyntaxTree compilationSyntaxTree in compilation.SyntaxTrees)
                {
                    merger.AddUnit(compilationSyntaxTree.GetCompilationUnitRoot());
                }
            }

            return merger.GetMergedCode();
        }

        /// <summary>
        /// Separates the program code from the rest of the code base.
        /// </summary>
        /// <param name="mergedCode">The whole codebase.</param>
        /// <param name="programTypeName">The name of the program.</param>
        /// <returns>Tuple containing program declaration and the remaining code.</returns>
        private (ClassDeclarationSyntax program, CompilationUnitSyntax additionalCode) SeparateProgram(string mergedCode, string programTypeName)
        {
            ClassDeclarationSyntax program = null;
            CompilationUnitSyntax dependendCode = null;

            Document document = CreateScriptDocument(mergedCode);
            SemanticModel semanticModel = document.GetSemanticModelAsync().Result;
            CompilationUnitSyntax syntaxNode = (CompilationUnitSyntax)document.GetSyntaxRootAsync().Result;
            foreach (ClassDeclarationSyntax classDeclaration in syntaxNode.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>())
            {
                INamedTypeSymbol symbol = semanticModel.GetDeclaredSymbol(classDeclaration);
                if ((symbol.ContainingNamespace.Name + "." + symbol.Name).Equals(programTypeName))
                {
                    program = classDeclaration;
                    dependendCode = syntaxNode.RemoveNode(classDeclaration, SyntaxRemoveOptions.KeepNoTrivia);
                    break;
                }
            }

            if (program == null)
            {
                Console.Error.WriteLine($"Unable to resolve script program '{programTypeName}'");
            }
            return (program, dependendCode);
        }

        /// <summary>
        /// Finalizes the program code so that it does not contain the wrapping class declaration.
        /// </summary>
        /// <param name="program">The complete program declaration.</param>
        /// <returns>The stripped program code for the final script.</returns>
        private string FinalizeProgramCode(ClassDeclarationSyntax program)
        {
            CompilationUnitSyntax programUnit = SyntaxFactory.CompilationUnit(
                new SyntaxList<ExternAliasDirectiveSyntax>(),
                new SyntaxList<UsingDirectiveSyntax>(),
                new SyntaxList<AttributeListSyntax>(),
                program.Members);

            return Formatter.Format(programUnit, this.temporaryWorkspace).ToFullString();
        }

        /// <summary>
        /// Performs a cleanup of the dependent code.
        /// </summary>
        /// <param name="additionalCode">The dependent code.</param>
        /// <returns>The cleaned merged code.</returns>
        private string FinalizeAdditionalCode(CompilationUnitSyntax additionalCode)
        {
            Document doc = CreateScriptDocument(additionalCode.ToString());
            SemanticModel semanticModel = doc.GetSemanticModelAsync().Result;
            SyntaxNode root = doc.GetSyntaxRootAsync().Result;

            MergeNamespacesRewriter rewriter = new MergeNamespacesRewriter(semanticModel);
            SyntaxNode node = rewriter.Visit(root);
            return Formatter.Format(node, this.temporaryWorkspace).ToFullString();
        }

        /// <summary>
        /// Merges program and additional code to the final script.
        /// </summary>
        /// <param name="program">The program code.</param>
        /// <param name="additionalCode">The additional code.</param>
        /// <returns>The final script code.</returns>
        private string MergeProgramAndAdditionalCode(string program, string additionalCode)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(program);
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine(Regex.Replace(additionalCode, "\\s*}\\s*$", string.Empty));
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Creates a script document from the merged code in the current project.
        /// </summary>
        /// <param name="mergedCode">The code to create the document for.</param>
        /// <returns>The document containing the specified code.</returns>
        private Document CreateScriptDocument(string mergedCode)
        {
            Project project = this.temporaryWorkspace.AddProject("IngameScript", LanguageNames.CSharp);
            Document doc = project.AddDocument("IngameScript", mergedCode);
            return doc;
        }

        /// <summary>
        /// Collects all required projects for packing the specified <paramref name="rootProject"/>.
        /// </summary>
        /// <param name="rootProject">The root project to pack.</param>
        /// <returns>All projects required to be included in the package.</returns>
        private IEnumerable<Project> CollectRequiredProjects(Project rootProject)
        {
            yield return rootProject;
            foreach (ProjectReference reference in rootProject.ProjectReferences)
            {
                yield return solution.GetProject(reference.ProjectId);
            }
        }
    }
}