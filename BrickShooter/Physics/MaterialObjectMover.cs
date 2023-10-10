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
            //apply the biggest minimal translation vector, hoping that it will fix the rest of the collisions too
            var biggestTranslationVector = existingCollisions
                .MaxBy(x => Math.Abs(x.MinimalTranslationVector.X) + Math.Abs(x.MinimalTranslationVector.Y))
                .MinimalTranslationVector;
            materialObject.Position += biggestTranslationVector;
        }

        public void ProcessNextCollision(MaterialObject currentObject, CollisionPredictionResult nextCollision)
        {
            var fullMovement = currentObject.Velocity * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds;
            var fullMovementDistancePortion = nextCollision.DistanceToCollision < 1f ? 0f : nextCollision.DistanceToCollision / fullMovement.Magnitude();

            currentObject.Position += fullMovement * fullMovementDistancePortion;

            var adjustedVelocity = currentObject.Velocity.Project(nextCollision.CollisionEdge.point2 - nextCollision.CollisionEdge.point1);
            currentObject.Position += adjustedVelocity * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds * (1 - fullMovementDistancePortion);
        }
    }
}
