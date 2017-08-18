// <copyright file="Program.cs" company="LipGam">
//      Copyright © LipGam Gaming Community. All rights reserved.
// </copyright>

namespace LipGam.SE.Scripting.PressurizationMonitor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using LipGam.SE.Scripting.Common;
    using Sandbox.ModAPI.Ingame;
    using Sandbox.ModAPI.Interfaces;
    using VRageMath;

    /// <summary>
    /// Script class for door display controlling.
    /// </summary>
    internal class Program
    {
        private Station station;

        /// <summary>
        /// Initializes a new instance of the <see cref="Program" /> class.
        /// </summary>
        /// <param name="gridProgram">The grid Program.</param>
        public Program(MyGridProgram gridProgram)
        {
            GridProgram = gridProgram;
        }

        /// <summary>
        /// Gets the game program.
        /// </summary>
        private MyGridProgram GridProgram { get; }

        /// <summary>
        /// Entry point of the script.
        /// </summary>
        /// <param name="argument">The script argument provided by executing block.</param>
        public void Main(string argument)
        {
            if (station == null)
            {
                station = new Station();
                station.RequiresAspect<RoomPressure>();

                // this.station.RequiresAspect<LockRoomOnPressureDecrease>();
                station.InitializeFromGrid(GridProgram.GridTerminalSystem);
            }

            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (IRoom room in station.Rooms)
                {
                    RoomPressure pressure = room.GetAspect<RoomPressure>();
                    pressure.UpdateAspect();

                    // LockRoomOnPressureDecrease locking = room.GetAspect<LockRoomOnPressureDecrease>();
                    // locking.UpdateAspect();
                    string subSection = !string.IsNullOrWhiteSpace(room.Name) ? $" ({room.Name})" : string.Empty;
                    stringBuilder.AppendLine($"Section {room.Section}{subSection}: {room.Name} {pressure.Pressure * 100:F0}% ({pressure.Status})");
                }

                Color foreground;
                IEnumerable<RoomPressure> allPressures = station.Rooms.Select(r => r.GetAspect<RoomPressure>()).ToList();
                if (allPressures.Any(p => p.Pressure < 0.2))
                {
                    foreground = Color.Red;
                }
                else if (allPressures.Any(p => p.Status == RoomPressure.PressureStatus.Decreasing || p.Pressure < 0.8))
                {
                    foreground = Color.Yellow;
                }
                else
                {
                    foreground = Color.Green;
                }

                IMyTextPanel textPanel = GridProgram.GridTerminalSystem.GetBlockWithName("TestLCD") as IMyTextPanel;
                textPanel.WritePublicText(stringBuilder);
                textPanel.ShowPublicTextOnScreen();
                textPanel.GetProperty("FontColor").AsColor().SetValue(textPanel, foreground);
            }
            catch (Exception e)
            {
                GridProgram.Me.CustomData = e.ToString();
            }
        }

        /// <summary>
        /// Saves the persistent state of the script.
        /// </summary>
        public void Save()
        {
            // the script does not have a state to persist.
        }
    }
}