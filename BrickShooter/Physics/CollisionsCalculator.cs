using Microsoft.Xna.Framework;

namespace BrickShooter.Physics
{
    public class CollisionsCalculator : ICollisionsCalculator
    {
        public CollisionCalculationResult CalculateCollision(MaterialObject collisionSubject, MaterialObject collisionObject)
        {
            CollisionCalculationResult result = new();
            result.RelativeVelocity = collisionSubject.Velocity - collisionObject.Velocity;

            return result;
        }
    }
}
