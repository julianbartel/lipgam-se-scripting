// <copyright file="EliminateGlobalUsingsRewriter.cs" company="Julian Bartel">
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
    /// Removes all using declarations from global scope and moves them to the inner namespaces.
    /// </summary>
    public class EliminateGlobalUsingsRewriter : CSharpSyntaxRewriter
    {
        /// <summary>
        /// The global usings.
        /// </summary>
        private readonly List<UsingDirectiveSyntax> globalUsings = new List<UsingDirectiveSyntax>();

        /// <inheritdoc />
        public override SyntaxNode Visit(SyntaxNode node)
        {
            this.globalUsings.Clear();
            return base.Visit(node);
        }

        /// <inheritdoc />
        public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
        {
            globalUsings.AddRange(node.Usings);
            CompilationUnitSyntax compilationUnit = (CompilationUnitSyntax)base.VisitCompilationUnit(node);
            return compilationUnit.RemoveNodes(compilationUnit.Usings, SyntaxRemoveOptions.KeepNoTrivia);
        }

        /// <inheritdoc />
        public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            SyntaxList<UsingDirectiveSyntax> allUsings = node.Usings.AddRange(this.globalUsings);
            SyntaxList<UsingDirectiveSyntax> distinctUsings = new SyntaxList<UsingDirectiveSyntax>()
                .AddRange(allUsings.ToList().Distinct(new UsingComparer()));

            node = node.RemoveNodes(node.Usings, SyntaxRemoveOptions.KeepNoTrivia);
            return node.WithUsings(distinctUsings);
        }
    }
}