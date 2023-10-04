using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;

namespace BrickShooter.Physics
{
    public class MaterialObjectMover : IMaterialObjectMover
    {
        public void Move(MaterialObject materialObject, Vector2 positionDiff)
        {
            materialObject.Position += positionDiff;
        }

        public void MoveWithoutObstruction(MaterialObject materialObject)
        {
            materialObject.Position += materialObject.Velocity * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
