using BrickShooter.Constants;
using BrickShooter.Extensions;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BrickShooter.Physics
{
    public class CollisionProcessor : ICollisionProcessor
    {
        private readonly IFutureCollisionsCalculator futureCollisionCalculator;
        private readonly IMaterialObjectMover materialObjectMover;

        public CollisionProcessor(
            IFutureCollisionsCalculator futureCollisionCalculator,
            IMaterialObjectMover materialObjectMover)
        {
            this.futureCollisionCalculator = futureCollisionCalculator;
            this.materialObjectMover = materialObjectMover;
        }

        public void ProcessExistingCollisions(IMaterialObject materialObject, IList<CollisionInfo> existingCollisions)
        {
            //apply the biggest minimal translation vector, hoping that it will fix the rest of the collisions too
            var longestTranslationVector = existingCollisions
                .Select(x => x.MinimalTranslationVector)
                .MaxBy(x => x.Length());
            materialObjectMover.MoveObject(materialObject, longestTranslationVector);
        }

        //recursively moves close to the collision point, then starts moving along its collision edge until velocity is expired
        public void FindAndProcessNextCollisions(IMaterialObject currentObject, IList<CollisionPair> potentialFutureCollisions)
        {
            var originalVelocity = currentObject.Velocity;
            while (currentObject.Velocity.Length() >= PhysicsConstants.MIN_VELOCITY)
            {
                if (potentialFutureCollisions.Count == 0)
                {
                    materialObjectMover.ScheduleMovement(currentObject, currentObject.Velocity * GlobalObjects.DeltaTime);
                    break;
                }
                var nextCollisions = futureCollisionCalculator.FindNextCollisions(potentialFutureCollisions);
                if(nextCollisions.Count == 0)
                {
                    materialObjectMover.ScheduleMovement(currentObject, currentObject.Velocity * GlobalObjects.DeltaTime);
                    break;
                }
                var fixedVelocity = currentObject.Velocity * GlobalObjects.DeltaTime;
                var remainingTravelDistance = fixedVelocity.Length();
                var nextCollision = nextCollisions.MinBy(x => x.DistanceToCollision);
                var regularMovementPortion = nextCollision.DistanceToCollision / remainingTravelDistance;
                var regularMovement = fixedVelocity * regularMovementPortion;
                materialObjectMover.MoveObject(currentObject, new Vector2((int)regularMovement.X, (int)regularMovement.Y));
                currentObject.Velocity = currentObject.Velocity.Project(nextCollision.CollisionEdge.point2 - nextCollision.CollisionEdge.point1) * (1 - regularMovementPortion);
                potentialFutureCollisions.Remove(nextCollision.CollisionPair);
            }
            currentObject.Velocity = originalVelocity;
        }
    }
}
