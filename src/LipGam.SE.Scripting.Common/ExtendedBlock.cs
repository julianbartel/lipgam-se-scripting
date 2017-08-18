// <copyright file="ExtendedBlock.cs" company="LipGam">
//      Copyright © LipGam Gaming Community. All rights reserved.
// </copyright>

namespace LipGam.SE.Scripting.Common
{
    using System;
    using System.Collections.Generic;
    using Sandbox.ModAPI.Ingame;

    public class ExtendedBlock<TBlock> : IExtendedBlock<TBlock>
        where TBlock : IMyTerminalBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedBlock{TBlock}" /> class.
        /// </summary>
        /// <param name="block">The block that is being wrapped.</param>
        public ExtendedBlock(TBlock block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            Block = block;
            ReloadData();
        }

        /// <inheritdoc />
        public TBlock Block { get; }

        /// <inheritdoc />
        public IDictionary<string, string> CustomData { get; } = new Dictionary<string, string>();

        /// <inheritdoc />
        public string Section { get; private set; }

        /// <inheritdoc />
        public string SubSection { get; private set; }

        /// <inheritdoc />
        public void ReloadData()
        {
            CustomData.Clear();
            foreach (string line in Block.CustomData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int split = line.IndexOf(':');
                if (split > 0 && split + 1 < line.Length)
                {
                    string key = line.Substring(0, split).Trim();
                    string value = line.Substring(split + 1).Trim();
                    switch (key)
                    {
                        case "Section":
                            Section = value;
                            break;
                        case "SubSection":
                            SubSection = value;
                            break;
                        default:
                            CustomData.Add(line.Substring(0, split).Trim(), line.Substring(split + 1).Trim());
                            break;
                    }
                }
            }
        }
    }
}