using BrickShooter.Extensions;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
            materialObject.Position += existingCollisions.MaxBy(x => Math.Abs(x.MinimalTranslationVector.X) + Math.Abs(x.MinimalTranslationVector.Y)).MinimalTranslationVector;
        }

        public void ProcessNextCollision(MaterialObject currentObject, CollisionPredictionResult nextCollision)
        {
            var fullMovement = currentObject.Velocity * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds;
            var fullMovementDistance = fullMovement.Magnitude();

            currentObject.Position += fullMovement * nextCollision.DistanceToCollision / fullMovementDistance;
        }
    }
}
