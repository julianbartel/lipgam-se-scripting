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

        //public bool Locked { get; set; }

        //public Dictionary<IMyDoor, DateTime> TemporaryUnlockTimers { get; } = new Dictionary<IMyDoor, DateTime>();



        //public List<IMyDoor> Doors { get; } = new List<IMyDoor>();

        //public VentStatus Status => Vents.Any() ? Vents.Min(v => v.Status) : VentStatus.Depressurized;

        //internal Room(string section, string subSection = null)
        //{
        //    Locked = Pressure < 0.8;
        //    Section = section;
        //    SubSection = subSection ?? string.Empty;
        //}

        //public void EnforceRoomLocking()
        //{
        //    if (Pressure < 0.8)
        //    {
        //        if (!Locked)
        //        {
        //            Locked = true;
        //            Doors.ForEach(d => d.CloseDoor());
        //        }
        //        else
        //        {
        //            HandleTemporaryUnlock();
        //        }
        //    }
        //    else if (Locked)
        //    {
        //        Locked = false;
        //        TemporaryUnlockTimers.Clear();
        //        Doors.ForEach(d => d.OpenDoor());
        //    }
        //}

        //public void HandleTemporaryUnlock()
        //{
        //    foreach (IMyDoor door in Doors.Where(d => d.Status == DoorStatus.Open))
        //    {
        //        DateTime timer;
        //        if (TemporaryUnlockTimers.TryGetValue(door, out timer) && ((DateTime.Now - timer) >= TimeSpan.FromSeconds(2)))
        //        {
        //            door.CloseDoor();
        //            TemporaryUnlockTimers.Remove(door);
        //        }
        //        else
        //        {
        //            timer = DateTime.Now;
        //            TemporaryUnlockTimers.Add(door, timer);
        //        }
        //    }
        //}
    }
}
