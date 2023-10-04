using System.Collections.Generic;
using System.Linq;
using BrickShooter.Extensions;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;

namespace BrickShooter.Physics
{
    public class CollisionProcessor : ICollisionProcessor
    {
        public void ProcessCollisions(MaterialObject currentObject, IEnumerable<CollisionPredictionResult> collisions)
        {
            var closestCollision = collisions.MinBy(x => x.CollisionDistance);

            var fullMovement = currentObject.Velocity * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds;
            var fullMovementDistance = fullMovement.Magnitude();

            currentObject.Position += fullMovement * closestCollision.CollisionDistance / fullMovementDistance;
        }
    }
}
