using BrickShooter.Constants;
using BrickShooter.Extensions;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrickShooter.Physics
{
    public class CollisionProcessor : ICollisionProcessor
    {
        private readonly IFutureCollisionsCalculator futureCollisionCalculator;

        public CollisionProcessor(
            IFutureCollisionsCalculator futureCollisionCalculator)
        {
            this.futureCollisionCalculator = futureCollisionCalculator;
        }

        public void ProcessExistingCollisions(MaterialObject materialObject, IList<CollisionInfo> existingCollisions)
        {
            //apply the biggest minimal translation vector, hoping that it will fix the rest of the collisions too
            var longestTranslationVector = existingCollisions
                .Select(x => x.MinimalTranslationVector)
                .MaxBy(x => x.Length());
            materialObject.Position += longestTranslationVector;
        }

        //recursively moves close to the collision point, then starts moving along its collision edge until velocity is expired
        public void ProcessNextCollisions(MaterialObject currentObject, IList<FutureCollisionInfo> nextCollisions)
        {
            var originalVelocity = currentObject.Velocity;
            while (currentObject.Velocity != Vector2.Zero)
            {
                if (nextCollisions.Count == 0)
                {
                    currentObject.Position += currentObject.Velocity * GlobalObjects.DeltaTime;
                    break;
                }
                var fixedVelocity = currentObject.Velocity * GlobalObjects.DeltaTime;
                var remainingTravelDistance = fixedVelocity.Length();
                var nextCollision = nextCollisions.MinBy(x => x.DistanceToCollision);
                var regularMovementPortion = nextCollision.DistanceToCollision / remainingTravelDistance;
                var regularMovement = fixedVelocity * regularMovementPortion;
                //the fractions are cut because it causes clipping sometimes.
                //in worst case scenario the object stops 1 unit from collision which does not matter
                currentObject.Position += new Vector2((int)regularMovement.X, (int)regularMovement.Y);
                currentObject.Velocity = currentObject.Velocity.Project(nextCollision.CollisionEdge.point2 - nextCollision.CollisionEdge.point1) * (1 - regularMovementPortion);
                if (Math.Abs(currentObject.Velocity.X) < PhysicsConstants.MIN_VELOCITY && Math.Abs(currentObject.Velocity.Y) < PhysicsConstants.MIN_VELOCITY)
                {
                    break;
                }
                nextCollisions = futureCollisionCalculator.FindNextCollisions(currentObject, nextCollisions.Where(x => x != nextCollision).Select(x => x.CollisionObject));
            }
            currentObject.Velocity = originalVelocity;
        }
    }
}
