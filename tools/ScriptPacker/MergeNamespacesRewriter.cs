// <copyright file="MergeNamespacesRewriter.cs" company="Julian Bartel">
//      Copyright © Julian Bartel. All rights reserved.
// </copyright>

namespace JulianBartel.SE.Tools.ScriptPacker
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Merges all same namespace declaration into one declaration and unifies using directives.
    /// </summary>
    internal class MergeNamespacesRewriter : CSharpSyntaxRewriter
    {
        /// <summary>
        /// The semantic model
        /// </summary>
        private readonly SemanticModel model;

        /// <summary>
        /// The namespace declarations grouped by their name.
        /// </summary>
        private readonly Dictionary<string, List<NamespaceDeclarationSyntax>> namespaceGrouping = new Dictionary<string, List<NamespaceDeclarationSyntax>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeNamespacesRewriter" /> class.
        /// </summary>
        /// <param name="model">The semantic model of the tree.</param>
        public MergeNamespacesRewriter(SemanticModel model)
        {
            this.model = model;
        }

        /// <inheritdoc />
        public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
        {
            CompilationUnitSyntax visitCompilationUnit = base.VisitCompilationUnit(node) as CompilationUnitSyntax;
            IEnumerable<NamespaceDeclarationSyntax> members = namespaceGrouping.Select(kvp => CreateNewNamespaceDeclaration(kvp.Key, kvp.Value));
            return visitCompilationUnit?.AddMembers(members.ToArray());
        }

        /// <inheritdoc />
        public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            if (node.Members.Any())
            {
                INamespaceSymbol namespaceSymbol = model.GetDeclaredSymbol(node);
                string name = namespaceSymbol.ToDisplayString();
                List<NamespaceDeclarationSyntax> containingNodes;
                if (!namespaceGrouping.TryGetValue(name, out containingNodes))
                {
                    containingNodes = new List<NamespaceDeclarationSyntax>();
                    namespaceGrouping.Add(name, containingNodes);
                }

                containingNodes.Add(node);
            }

            return null;
        }

        /// <summary>
        /// Creates a merged <see cref="NamespaceDeclarationSyntax" /> containing content of all separate namespace nodes.
        /// </summary>
        /// <param name="name">The name of the merged namespace.</param>
        /// <param name="nodes">The separate namespaces.</param>
        /// <returns>The merged namespace syntax.</returns>
        private NamespaceDeclarationSyntax CreateNewNamespaceDeclaration(string name, List<NamespaceDeclarationSyntax> nodes)
        {
            SyntaxList<ExternAliasDirectiveSyntax> externs = new SyntaxList<ExternAliasDirectiveSyntax>();
            externs = externs.AddRange(nodes.SelectMany(n => n.Externs));
            SyntaxList<UsingDirectiveSyntax> usings = new SyntaxList<UsingDirectiveSyntax>();
            usings = usings.AddRange(nodes.SelectMany(n => n.Usings).Distinct(new UsingComparer()));

            NamespaceDeclarationSyntax newNamespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(name));
            newNamespace = newNamespace.WithExterns(externs);
            newNamespace = newNamespace.WithUsings(usings);

            foreach (NamespaceDeclarationSyntax originalNamespace in nodes)
            {
                foreach (MemberDeclarationSyntax member in originalNamespace.Members)
                {
                    newNamespace = newNamespace.AddMembers(member);
                }
            }

            return newNamespace;
        }
    }
}