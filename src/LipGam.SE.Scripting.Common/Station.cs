// <copyright file="Station.cs" company="LipGam">
//      Copyright © LipGam Gaming Community. All rights reserved.
// </copyright>

namespace LipGam.SE.Scripting.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sandbox.ModAPI.Ingame;

    /// <summary>
    /// Summarizes rooms of a grid to a station.
    /// </summary>
    public class Station
    {
        /// <summary>
        /// List of required aspects.
        /// </summary>
        private readonly Dictionary<Type, Func<IRoomAspect>> requiredAspectFactories = new Dictionary<Type, Func<IRoomAspect>>();

        /// <summary>
        /// Factory function for initializing rooms.
        /// </summary>
        private readonly Func<string, string, IRoom> roomFactory;

        /// <summary>
        /// List of rooms.
        /// </summary>
        private readonly List<IRoom> rooms = new List<IRoom>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Station" /> class.
        /// </summary>
        public Station()
            : this((section, subSection) => new Room(section, subSection))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Station"/> class.
        /// </summary>
        /// <param name="roomFactory">The room Factory.</param>
        public Station(Func<string, string, IRoom> roomFactory)
        {
            this.roomFactory = roomFactory;
        }

        /// <summary>
        /// Gets the rooms of the station.
        /// </summary>
        public IEnumerable<IRoom> Rooms => rooms;

        /// <summary>
        /// Initializes the station from the specified <paramref name="grid" />.
        /// </summary>
        /// <param name="grid">The grid which this station represents.</param>
        public void InitializeFromGrid(IMyGridTerminalSystem grid)
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            grid.GetBlocks(blocks);

            rooms.Clear();
            foreach (IExtendedBlock<IMyTerminalBlock> block in blocks.Select(b => new ExtendedBlock<IMyTerminalBlock>(b)))
            {
                if (!string.IsNullOrWhiteSpace(block.Section))
                {
                    IRoom room = rooms.SingleOrDefault(r => r.Section == block.Section && r.Name == block.SubSection);
                    if (room == null)
                    {
                        // room not yet created, create a new room.
                        room = roomFactory(block.Section, block.SubSection);

                        rooms.Add(room);
                    }

                    room.AddBlock(block);
                }
            }

            foreach (IRoom room in rooms)
            {
                foreach (Func<IRoomAspect> aspectFactory in requiredAspectFactories.Values)
                {
                    room.AddAspect(aspectFactory());
                }
            }
        }

        /// <summary>
        /// Registers an aspect to the station.
        /// </summary>
        /// <typeparam name="TAspect">The type of the aspect to register.</typeparam>
        public void RequiresAspect<TAspect>()
            where TAspect : IRoomAspect, new()
        {
            if (!requiredAspectFactories.ContainsKey(typeof(TAspect)))
            {
                requiredAspectFactories.Add(typeof(TAspect), () => new TAspect());

                foreach (IRoom room in rooms)
                {
                    room.AddAspect(new TAspect());
                }
            }
        }
    }
}