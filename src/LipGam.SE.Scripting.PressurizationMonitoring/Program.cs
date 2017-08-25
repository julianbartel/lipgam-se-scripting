// <copyright file="Program.cs" company="LipGam">
//      Copyright © LipGam Gaming Community. All rights reserved.
// </copyright>

namespace LipGam.SE.Scripting.PressurizationMonitoring
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
        /// <summary>
        /// The station in which the script runs.
        /// </summary>
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
            StringBuilder debugStringBuilder = new StringBuilder();
            IMyTextPanel debugPanel = GridProgram.GridTerminalSystem.GetBlockWithName("DebugLCD") as IMyTextPanel;
            debugPanel?.GetProperty("FontColor").AsColor().SetValue(debugPanel, Color.White);
            if (station == null)
            {
                station = new Station();
                station.RequiresAspect<RoomPressure>();
                station.RequiresAspect<LockRoomOnPressureDecrease>();
                station.InitializeFromGrid(GridProgram.GridTerminalSystem);
            }

            try
            {
                int roomNameWidth = station.Rooms.Max(r => r.Name?.Length ?? 0);
                int severity = 0;
                StringBuilder stringBuilder = new StringBuilder();
                foreach (IGrouping<string, IRoom> grouping in station.Rooms.GroupBy(r => r.Section).OrderBy(g => g.Key))
                {
                    stringBuilder.AppendLine("SEKTION " + grouping.Key);
                    foreach (IRoom room in grouping.OrderBy(r => r.Section))
                    {
                        RoomPressure pressure = room.GetAspect<RoomPressure>();
                        pressure.UpdateAspect();
                        if (pressure.Status == RoomPressure.PressureStatus.Unknown)
                        {
                            debugStringBuilder.AppendLine($"Room without pressure data: Section {room.Section} {room.Name}");
                            continue;
                        }

                        if ((pressure.Status == RoomPressure.PressureStatus.Decreasing) || (pressure.Pressure < 0.2))
                        {
                            severity = 2;
                        }
                        else if ((pressure.Status == RoomPressure.PressureStatus.Decreasing) || (pressure.Pressure < 0.8))
                        {
                            severity = Math.Max(severity, 1);
                        }

                        LockRoomOnPressureDecrease locking = room.GetAspect<LockRoomOnPressureDecrease>();
                        locking.UpdateAspect();

                        string roomText = (room.Name ?? string.Empty).PadRight(roomNameWidth);
                        string pressureText = (pressure.Pressure * 100).ToString("F0").PadLeft(3);
                        string statusText = pressure.Status.ToString().PadRight(10);
                        string lockedText = locking.Locked ? "! Locked !" : string.Empty;
                        stringBuilder.AppendLine($"    {roomText}  {pressureText}% {statusText}   {lockedText}");
                    }
                    stringBuilder.AppendLine();
                }

                Color foreground;
                switch (severity)
                {
                    case 0:
                        foreground = Color.Green;
                        break;
                    case 1:
                        foreground = Color.Yellow;
                        break;
                    default:
                        foreground = Color.Red;
                        break;
                }

                IMyTextPanel textPanel = GridProgram.GridTerminalSystem.GetBlockWithName("PressureLCD") as IMyTextPanel;
                textPanel.WritePublicText(stringBuilder);
                textPanel.ShowPublicTextOnScreen();
                textPanel.GetProperty("FontColor").AsColor().SetValue(textPanel, foreground);
            }
            catch (Exception e)
            {
                debugPanel?.GetProperty("FontColor").AsColor().SetValue(debugPanel, Color.DarkRed);
                debugStringBuilder.AppendLine(e.ToString());
            }

            debugPanel?.WritePublicText(debugStringBuilder);
            debugPanel?.ShowPublicTextOnScreen();
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