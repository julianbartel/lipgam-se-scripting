// <copyright file="Program.cs" company="LipGam">
//      Copyright © LipGam Gaming Community. All rights reserved.
// </copyright>

namespace LipGam.SE.Scripting.DoorDisplays
{
    using System;
    using System.Collections.Generic;
    using Sandbox.ModAPI.Ingame;
    using Sandbox.ModAPI.Interfaces;
    using VRageMath;

    /// <summary>
    /// Script class for door display controlling.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Gets the game program.
        /// </summary>
        private MyGridProgram GridProgram { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Program"/> class.
        /// </summary>
        /// <param name="gridProgram">The grid Program.</param>
        public Program(MyGridProgram gridProgram)
        {
            GridProgram = gridProgram;
        }

        /// <summary>
        /// Entry point of the script.
        /// </summary>
        /// <param name="argument">The script argument provided by executing block.</param>
        public void Main(string argument)
        {
            List<IMyTextPanel> doorLcds = new List<IMyTextPanel>();
            GridProgram.GridTerminalSystem.GetBlocksOfType(doorLcds, DoorLcdPredicate);

            foreach (IMyTextPanel doorLcd in doorLcds)
            {
                IDictionary<string, string> customData = ReadCustomData(doorLcd);
                string sectionName = $"Sektion {customData["Section"].ToUpper()}";
                string subName;
                customData.TryGetValue("SubSection", out subName);

                doorLcd.WritePublicText($"{sectionName}\r\n{subName}");
                doorLcd.ShowPublicTextOnScreen();
                doorLcd.GetProperty("FontColor").AsColor().SetValue(doorLcd, Color.Green);
                doorLcd.GetProperty("FontSize").AsFloat().SetValue(doorLcd, 1.3f);
            }
        }

        /// <summary>
        /// Saves the persistent state of the script.
        /// </summary>
        public void Save()
        {
            // the script does not have a state to persist.
        }

        /// <summary>
        /// Reads the custom data of a block as a dictionary.
        /// </summary>
        /// <param name="block">The block to read the custom data from.</param>
        /// <returns>The key-value representation of the custom data.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="block" /> is <c>null</c>.</exception>
        private static IDictionary<string, string> ReadCustomData(IMyTerminalBlock block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            Dictionary<string, string> customDataDictionary = new Dictionary<string, string>();
            foreach (string line in block.CustomData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int split = line.IndexOf(':');
                if (split > 0 && split + 1 < line.Length)
                {
                    customDataDictionary.Add(line.Substring(0, split).Trim(), line.Substring(split + 1).Trim());
                }
            }

            return customDataDictionary;
        }

        /// <summary>
        /// Predicate to select relevant LCD display blocks.
        /// </summary>
        /// <param name="block">The block to check.</param>
        /// <returns><c>true</c> if the block is a door display; otherwise <c>false</c>.</returns>
        private bool DoorLcdPredicate(IMyTextPanel block)
        {
            IDictionary<string, string> customData = ReadCustomData(block);
            string value;
            return customData.TryGetValue("IsDoorLcd", out value) && value == "True";
        }
    }
}