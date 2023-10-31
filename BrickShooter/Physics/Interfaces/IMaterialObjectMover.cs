using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;

namespace BrickShooter.Physics.Interfaces
{
    public interface IMaterialObjectMover
    {
        /// <summary>
        /// states that a specified object should move a specified distance when Move will be called
        /// </summary>
        /// <param name="materialObject"></param>
        /// <param name="movement"></param>
        void ScheduleMovement(MaterialObject materialObject, Vector2 movement);
        /// <summary>
        /// applies all previously scheduled movements
        /// </summary>
        void MoveObjects();
    }
}
