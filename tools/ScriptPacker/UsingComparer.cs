// <copyright file="UsingComparer.cs" company="Julian Bartel">
//      Copyright © Julian Bartel. All rights reserved.
// </copyright>

namespace JulianBartel.SE.Tools.ScriptPacker
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Comparer for using directives.
    /// </summary>
    internal class UsingComparer : IEqualityComparer<UsingDirectiveSyntax>
    {
        /// <inheritdoc />
        public bool Equals(UsingDirectiveSyntax x, UsingDirectiveSyntax y)
        {
            return x.Name.ToString() == y.Name.ToString();
        }

        /// <inheritdoc />
        public int GetHashCode(UsingDirectiveSyntax obj) => obj.Name.ToString().GetHashCode();
    }
}