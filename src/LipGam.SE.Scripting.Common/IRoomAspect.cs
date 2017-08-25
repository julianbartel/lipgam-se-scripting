// <copyright file="IRoomAspect.cs" company="LipGam">
//      Copyright © LipGam Gaming Community. All rights reserved.
// </copyright>

namespace LipGam.SE.Scripting.Common
{
    /// <summary>
    /// Base interface for a specific aspect of a room.
    /// </summary>
    public interface IRoomAspect
    {
        /// <summary>
        /// Initializes the aspect for the specified <paramref name="room"/>.
        /// </summary>
        /// <param name="room">The room to initialize the aspect for.</param>
        void InitializeAspect(IRoom room);

        /// <summary>
        /// Updates the state of the aspect.
        /// </summary>
        void UpdateAspect();
    }
}
