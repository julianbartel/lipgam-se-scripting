// <copyright file="IExtendedBlock.cs" company="LipGam">
//      Copyright © LipGam Gaming Community. All rights reserved.
// </copyright>

namespace LipGam.SE.Scripting.Common
{
    using System.Collections.Generic;
    using Sandbox.ModAPI.Ingame;

    /// <summary>
    /// Extension wrapper for in-game terminal blocks.
    /// </summary>
    /// <typeparam name="TBlock">The actual type of the wrapped block.</typeparam>
    public interface IExtendedBlock<out TBlock>
        where TBlock : IMyTerminalBlock
    {
        /// <summary>
        /// Gets the actual in-game block object.
        /// </summary>
        TBlock Block { get; }

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
        /// Reloads the data of this block.
        /// </summary>
        void ReloadData();
    }
}