using Microsoft.Xna.Framework;

namespace BrickShooter.Physics
{
    public class CollisionCalculator : ICollisionCalculator
    {
        public CollisionPredictionResult CalculateCollision(MaterialObject collisionSubject, MaterialObject collisionObject)
        {
            CollisionPredictionResult result = new();
            result.RelativeVelocity = collisionSubject.Velocity - collisionObject.Velocity;

            return result;
        }
    }
}
