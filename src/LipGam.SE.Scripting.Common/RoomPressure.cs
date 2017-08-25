// <copyright file="RoomPressure.cs" company="LipGam">
//      Copyright © LipGam Gaming Community. All rights reserved.
// </copyright>

namespace LipGam.SE.Scripting.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SpaceEngineers.Game.ModAPI.Ingame;

    /// <summary>
    /// Aspect for a room's pressurization.
    /// </summary>
    public class RoomPressure : IRoomAspect
    {
        /// <summary>
        /// The vents in the room.
        /// </summary>
        public readonly List<IMyAirVent> vents = new List<IMyAirVent>();

        /// <summary>
        /// The pressure of the last update.
        /// </summary>
        private double lastPressure;

        /// <summary>
        /// The status of the room pressure.
        /// </summary>
        public enum PressureStatus
        {
            /// <summary>
            /// The pressure can not be determined.
            /// </summary>
            Unknown,

            /// <summary>
            /// Pressure is stable.
            /// </summary>
            Stable,

            /// <summary>
            /// Pressure decreases.
            /// </summary>
            Increasing,

            /// <summary>
            /// Pressure increases.
            /// </summary>
            Decreasing
        }

        /// <summary>
        /// Gets the current pressure of the room.
        /// </summary>
        public double Pressure => vents.Any() ? vents.Min(v => v.GetOxygenLevel()) : double.NaN;

        /// <summary>
        /// Gets the room pressurization status.
        /// </summary>
        public PressureStatus Status { get; private set; }

        /// <inheritdoc />
        public void InitializeAspect(IRoom room)
        {
            vents.Clear();
            foreach (IMyAirVent vent in room.Blocks.Select(b => b.Block).OfType<IMyAirVent>())
            {
                vents.Add(vent);
            }

            lastPressure = Pressure;
            UpdateAspect();
        }

        /// <inheritdoc />
        public void UpdateAspect()
        {
            if (!vents.Any())
            {
                Status = PressureStatus.Unknown;
            }
            else if (Math.Abs(Pressure - lastPressure) <= 0.001)
            {
                Status = PressureStatus.Stable;
            }
            else if (Pressure > lastPressure)
            {
                Status = PressureStatus.Increasing;
            }
            else
            {
                Status = PressureStatus.Decreasing;
            }

            lastPressure = Pressure;
        }
    }
}