// <copyright file="Bootstrapper.cs" company="LipGam">
//      Copyright © LipGam Gaming Community. All rights reserved.
// </copyright>

namespace IngameScript.Properties
{
    using Malware.MDKUtilities;

    /// <summary>
    /// Bootstrapper for test startup.
    /// </summary>
    public class Bootstrapper
    {
        /// <summary>
        /// Initializes the MDK utility framework
        /// </summary>
        static Bootstrapper()
        {
            MDKUtilityFramework.Load();
        }

        /// <summary>
        /// Bootstrapper entry point.
        /// </summary>
        public static void Main()
        {
            // In order for your program to actually run, you will need to provide a mockup of all the facilities 
            // your script uses from the game, since they're not available outside of the game.

            // Create and configure the desired program.
            Program program = MDK.CreateProgram<Program>();
            MDK.Run(program);
        }
    }
}