using BrickShooter.Extensions;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using System.Collections.Generic;

namespace BrickShooter.Physics
{
    public class MaterialObjectMover : IMaterialObjectMover
    {
        public void MoveWithoutObstruction(MaterialObject materialObject)
        {
            materialObject.Position += materialObject.Velocity * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds;
        }

        public void ProcessExistingCollisions(MaterialObject materialObject, IList<CollisionData> existingCollisions)
        {
            if(existingCollisions.Count == 1)
            {
                materialObject.Position += existingCollisions[0].MinimalTranslationVector;
            }
        }

        public void ProcessNextCollision(MaterialObject currentObject, CollisionPredictionResult nextCollision)
        {
            var fullMovement = currentObject.Velocity * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds;
            var fullMovementDistance = fullMovement.Magnitude();

            currentObject.Position += fullMovement * nextCollision.DistanceToCollision / fullMovementDistance;
        }
    }
}
