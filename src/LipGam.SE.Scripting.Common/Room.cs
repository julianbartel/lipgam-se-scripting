// <copyright file="Room.cs" company="LipGam">
//      Copyright © LipGam Gaming Community. All rights reserved.
// </copyright>

namespace LipGam.SE.Scripting.Common
{
    using Sandbox.ModAPI.Ingame;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines a room of a ship/station.
    /// </summary>
    public class Room : IRoom
    {
        /// <summary>
        /// The aspects loaded for the room.
        /// </summary>
        private readonly Dictionary<Type, IRoomAspect> aspects = new Dictionary<Type, IRoomAspect>();

        /// <summary>
        /// The blocks of the room.
        /// </summary>
        private readonly List<IExtendedBlock<IMyTerminalBlock>> blocks = new List<IExtendedBlock<IMyTerminalBlock>>();

        /// <summary>
        /// Gets the blocks of this room.
        /// </summary>
        public IEnumerable<IExtendedBlock<IMyTerminalBlock>> Blocks => this.blocks;

        /// <inheritdoc />
        public string Section { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Room"/> class.
        /// </summary>
        /// <param name="section">The section to which this room belongs.</param>
        /// <param name="name">The name of the room.</param>
        public Room(string section, string name)
        {
            Section = section;
            Name = name;
        }

        /// <inheritdoc />
        public TAspect GetAspect<TAspect>()
            where TAspect : class, IRoomAspect
        {
            return this.aspects.ContainsKey(typeof(TAspect))
                        ? this.aspects[typeof(TAspect)] as TAspect
                        : null;
        }

        /// <inheritdoc />
        public void AddBlock(IExtendedBlock<IMyTerminalBlock> block)
        {
            if (this.blocks.Contains(block))
            {
                throw new InvalidOperationException("The block has already been added.");
            }

            this.blocks.Add(block);
        }

        /// <inheritdoc />
        public void AddAspect(IRoomAspect aspect)
        {
            if (this.aspects.ContainsKey(aspect.GetType()))
            {
                throw new InvalidOperationException("An aspect with the same type has already been added.");
            }

            aspect.InitializeAspect(this);
            this.aspects.Add(aspect.GetType(), aspect);
        }
    }
}
