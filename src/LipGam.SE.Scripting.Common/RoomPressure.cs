// <copyright file="RoomPressure.cs" company="LipGam">
//      Copyright © LipGam Gaming Community. All rights reserved.
// </copyright>

namespace LipGam.SE.Scripting.Common
{
    using SpaceEngineers.Game.ModAPI.Ingame;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Aspect for a room's pressurization.
    /// </summary>
    public class RoomPressure : IRoomAspect
    {
        /// <summary>
        /// The status of the room pressure.
        /// </summary>
        public enum PressureStatus
        {
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
            Decreasing,
        }

        /// <summary>
        /// The vents in the room.
        /// </summary>
        private readonly List<IMyAirVent> vents = new List<IMyAirVent>();

        /// <summary>
        /// The pressure of the last update.
        /// </summary>
        private double lastPressure;

        /// <summary>
        /// Gets the current pressure of the room.
        /// </summary>
        public double Pressure => this.vents.Any() ? this.vents.Min(v => v.GetOxygenLevel()) : double.NaN;

        /// <summary>
        /// Gets the room pressurization status.
        /// </summary>
        public PressureStatus Status { get; private set; }

        /// <inheritdoc />
        public void InitializeAspect(IRoom room)
        {
            this.vents.Clear();
            foreach (IMyAirVent vent in room.Blocks.Select(b => b.Block).OfType<IMyAirVent>())
            {
                vents.Add(vent);
            }

            this.lastPressure = Pressure;
            UpdateAspect();
        }

        /// <inheritdoc />
        public void UpdateAspect()
        {
            if (Pressure == this.lastPressure)
            {
                Status = PressureStatus.Stable;
            }
            else if (Pressure > this.lastPressure)
            {
                Status = PressureStatus.Increasing;
            }
            else
            {
                Status = PressureStatus.Decreasing;
            }

            this.lastPressure = Pressure;
        }
    }
}