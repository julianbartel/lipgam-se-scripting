// <copyright file="IRoom.cs" company="LipGam">
//      Copyright © LipGam Gaming Community. All rights reserved.
// </copyright>


namespace LipGam.SE.Scripting.Common
{
    using Sandbox.ModAPI.Ingame;
    using System.Collections.Generic;
    
    /// <summary>
    /// Interface of a station's room.
    /// </summary>
    public interface IRoom
    {
        /// <summary>
        /// Gets the blocks of this room.
        /// </summary>
        IEnumerable<IExtendedBlock<IMyTerminalBlock>> Blocks { get; }

        /// <summary>
        /// Gets the section to which this room belongs.
        /// </summary>
        string Section { get; }

        /// <summary>
        /// Gets the name of the room.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets an aspect for the room.
        /// </summary>
        /// <typeparam name="TAspect">The type of the aspect.</typeparam>
        /// <returns>The aspect or <c>null</c>, if the aspect is not available.</returns>
        TAspect GetAspect<TAspect>()
            where TAspect : class, IRoomAspect;

        /// <summary>
        /// Adds a block to this room.
        /// </summary>
        /// <param name="block">The block to add.</param>
        /// <exception cref="InvalidOperationException">The block has already been added.</exception>
        void AddBlock(IExtendedBlock<IMyTerminalBlock> block);

        /// <summary>
        /// Adds an aspect to this room.
        /// </summary>
        /// <param name="aspect">The aspect to add.</param>
        /// <exception cref="InvalidOperationException">An aspect of the same type has already been added.</exception>
        void AddAspect(IRoomAspect aspect);
    }
}
