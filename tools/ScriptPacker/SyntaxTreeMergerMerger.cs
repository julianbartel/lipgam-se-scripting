// <copyright file="CodeMerger.cs" company="Julian Bartel">
//      Copyright © Julian Bartel. All rights reserved.
// </copyright>
namespace JulianBartel.SE.Tools.ScriptPacker
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Merges syntax trees into a single code file.
    /// </summary>
    public class SyntaxTreeMergerMerger
    {
        /// <summary>
        /// The string builder holding the working text.
        /// </summary>
        private readonly StringBuilder builder = new StringBuilder();

        /// <summary>
        /// Adds the specified unit to the merged code.
        /// </summary>
        /// <param name="unit">The unit to add.</param>
        public void AddUnit(CompilationUnitSyntax unit)
        {
            if (unit.DescendantNodesAndSelf().Any(IsRelevantNode))
            {
                EliminateGlobalUsingsRewriter globalUsingsRewriter = new EliminateGlobalUsingsRewriter();
                SyntaxNode cleanNode = globalUsingsRewriter.Visit(unit);
                this.builder.AppendLine(cleanNode.ToFullString());
            }
        }

        /// <summary>
        /// Returns the merged code.
        /// </summary>
        /// <returns>The text containing all syntax trees.</returns>
        public string GetMergedCode()
        {
            return builder.ToString();
        }

        /// <summary>
        /// Identifies relevant nodes.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns><c>true</c> if the specified node is relevant and must be added to the project; otherwise, <c>false</c>.</returns>
        private static bool IsRelevantNode(SyntaxNode node)
        {
            ////switch (node.Kind())
            ////{
            ////    case SyntaxKind.ClassDeclaration:
            ////    case SyntaxKind.InterfaceDeclaration:
            ////    case SyntaxKind.DelegateDeclaration:
            ////    case SyntaxKind.StructDeclaration:
            ////    case SyntaxKind.EnumDeclaration:
            ////        return = true;
            ////    default:
            ////        return false;
            ////}
            return node is MemberDeclarationSyntax;
        }
    }
}