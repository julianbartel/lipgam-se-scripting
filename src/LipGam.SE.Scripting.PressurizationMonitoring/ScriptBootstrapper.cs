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
        private LipGam.SE.Scripting.PressurizationMonitor.Program program;

        /// <summary>
        /// Entry point of the script.
        /// </summary>
        /// <param name="argument">The script argument provided by executing block.</param>
        public void Main(string argument)
        {
            if (this.program == null)
            {
                this.program = new LipGam.SE.Scripting.PressurizationMonitor.Program(this);
            }

            this.program.Main(argument);
        }
    }
}