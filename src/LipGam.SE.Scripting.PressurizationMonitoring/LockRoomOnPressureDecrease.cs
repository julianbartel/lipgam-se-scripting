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
        /// Gets the doors of the room.
        /// </summary>
        private List<IMyDoor> Doors { get; } = new List<IMyDoor>();

        private RoomPressure RoomPressure { get; set; }

        public Dictionary<IMyDoor, DateTime> TemporaryOpenDoors { get; }= new Dictionary<IMyDoor, DateTime>();

        public bool Locked { get; set; }

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

        private void Lock()
        {
            if (!Locked)
            {
                Locked = true;
                TemporaryOpenDoors.Clear();
            }

            ControlTemporaryOpenDoors();
        }

        private void Unlock()
        {
            if (Locked)
            {
                Locked = false;
                TemporaryOpenDoors.Clear();
            }
        }

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