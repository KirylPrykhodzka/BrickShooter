using Microsoft.Xna.Framework;

namespace BrickShooter.Physics.Interfaces
{
    public interface IMaterialObjectMover
    {
        /// <summary>
        /// immediately changes the position of an object by the specified value
        /// </summary>
        /// <param name="materialObject"></param>
        /// <param name="movement"></param>
        void MoveObject(IMaterialObject materialObject, Vector2 movement);
    }
}
