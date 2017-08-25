// <copyright file="LockRoomOnPressureDecrease.cs" company="LipGam">
//      Copyright © LipGam Gaming Community. All rights reserved.
// </copyright>

namespace LipGam.SE.Scripting.PressurizationMonitoring
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LipGam.SE.Scripting.Common;
    using Sandbox.ModAPI.Ingame;

    /// <summary>
    /// Aspect which locks doors as soon as the pressure decreases.
    /// </summary>
    internal class LockRoomOnPressureDecrease : IRoomAspect
    {
        /// <summary>
        /// Gets a value indicating whether this room is locked.
        /// </summary>
        public bool Locked { get; private set; }

        /// <summary>
        /// Gets the doors of the room.
        /// </summary>
        private List<IMyDoor> Doors { get; } = new List<IMyDoor>();

        /// <summary>
        /// Gets or sets the room pressure aspect.
        /// </summary>
        private RoomPressure RoomPressure { get; set; }

        /// <summary>
        /// Gets a list with temporarily opened doors.
        /// </summary>
        private Dictionary<IMyDoor, DateTime> TemporaryOpenDoors { get; } = new Dictionary<IMyDoor, DateTime>();

        /// <inheritdoc />
        public void InitializeAspect(IRoom room)
        {
            Doors.Clear();
            Doors.AddRange(room.Blocks.Select(b => b.Block).OfType<IMyDoor>());
            RoomPressure = room.GetAspect<RoomPressure>();
        }

        /// <inheritdoc />
        public void UpdateAspect()
        {
            if ((RoomPressure.Status == RoomPressure.PressureStatus.Decreasing) || (RoomPressure.Pressure < 0.8))
            {
                Lock();
            }
            else if (RoomPressure.Status == RoomPressure.PressureStatus.Stable)
            {
                Unlock();
            }
        }

        /// <summary>
        /// Locks this room.
        /// </summary>
        private void Lock()
        {
            if (!Locked)
            {
                Locked = true;
                TemporaryOpenDoors.Clear();
            }

            ControlTemporaryOpenDoors();
        }

        /// <summary>
        /// Unlocks this room.
        /// </summary>
        private void Unlock()
        {
            if (Locked)
            {
                Locked = false;
                TemporaryOpenDoors.Clear();
            }
        }

        /// <summary>
        /// Updates and manages state of opened doors.
        /// </summary>
        private void ControlTemporaryOpenDoors()
        {
            foreach (IMyDoor door in Doors)
            {
                switch (door.Status)
                {
                    case DoorStatus.Opening:
                    case DoorStatus.Open:
                        DateTime openSince;
                        if (TemporaryOpenDoors.TryGetValue(door, out openSince))
                        {
                            if ((DateTime.Now - openSince) > TimeSpan.FromSeconds(1.3))
                            {
                                door.CloseDoor();
                                TemporaryOpenDoors.Remove(door);
                            }
                        }
                        else
                        {
                            TemporaryOpenDoors.Add(door, DateTime.Now);
                        }

                        break;

                    default:
                        TemporaryOpenDoors.Remove(door);
                        break;
                }
            }
        }
    }
}