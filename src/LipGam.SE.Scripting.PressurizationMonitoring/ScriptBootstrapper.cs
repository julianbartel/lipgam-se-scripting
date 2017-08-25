// <copyright file="ScriptBootstrapper.cs" company="LipGam">
//      Copyright © LipGam Gaming Community. All rights reserved.
// </copyright>

namespace IngameScript
{
    using Sandbox.ModAPI.Ingame;

    /// <summary>
    /// Bootstrapper of the script program.
    /// </summary>
    public class ScriptBootstrapper : MyGridProgram
    {
        /// <summary>
        /// The actual program.
        /// </summary>
        private LipGam.SE.Scripting.PressurizationMonitoring.Program program;

        /// <summary>
        /// Entry point of the script.
        /// </summary>
        /// <param name="argument">The script argument provided by executing block.</param>
        public void Main(string argument)
        {
            if (program == null)
            {
                program = new LipGam.SE.Scripting.PressurizationMonitoring.Program(this);
            }

            program.Main(argument);
        }
    }
}