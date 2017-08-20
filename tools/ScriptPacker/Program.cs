// <copyright file="Program.cs" company="Julian Bartel">
//      Copyright © Julian Bartel. All rights reserved.
// </copyright>

namespace JulianBartel.SE.Tools.ScriptPacker
{
    using System.IO;

    /// <summary>
    /// Entry point of the script packer program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Entry point of the script packer program.
        /// </summary>
        /// <param name="args">The program arguments.</param>
        private static void Main(string[] args)
        {
            ScriptPacker packer = new ScriptPacker(Path.GetFullPath(args[0]));
            string script = packer.PackProject(Path.GetFullPath(args[1]), "IngameScript.ScriptBootstrapper");

            if (!Directory.Exists(Path.GetDirectoryName(args[2])))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(args[2]));
            }

            File.WriteAllText(Path.GetFullPath(args[2]), script);
        }
    }
}