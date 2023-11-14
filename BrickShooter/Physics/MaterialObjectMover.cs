using BrickShooter.Physics.Interfaces;
using Microsoft.Xna.Framework;

namespace BrickShooter.Physics
{
    public class MaterialObjectMover : IMaterialObjectMover
    {
        public void MoveObject(IMaterialObject materialObject, Vector2 movement)
        {
            materialObject.Position += movement;
        }
    }
}
