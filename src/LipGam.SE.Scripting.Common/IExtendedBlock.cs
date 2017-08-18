// <copyright file="IExtendedBlock.cs" company="LipGam">
//      Copyright © LipGam Gaming Community. All rights reserved.
// </copyright>

namespace LipGam.SE.Scripting.Common
{
    using Sandbox.ModAPI.Ingame;
    using System.Collections.Generic;

    /// <summary>
    /// Extension wrapper for ingame terminal blocks.
    /// </summary>
    /// <typeparam name="TBlock">The actual type of the wrapped block.</typeparam>
    public interface IExtendedBlock<TBlock> where TBlock : IMyTerminalBlock
    {
        /// <summary>
        /// Gets the custom data of the block.
        /// </summary>
        IDictionary<string, string> CustomData { get; }

        /// <summary>
        /// Gets the section this block is assigned to.
        /// </summary>
        string Section { get; }

        /// <summary>
        /// Gets the sub section this block is assigned to.
        /// </summary>
        string SubSection { get; }

        /// <summary>
        /// Gets the actual ingame block object.
        /// </summary>
        TBlock Block { get; }

        /// <summary>
        /// Reloads the data of this block.
        /// </summary>
        void ReloadData();
    }
}
