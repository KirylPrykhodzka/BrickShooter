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

        public void ProcessExistingCollisions(IMaterialObject materialObject, IList<RotationCollisionInfo> existingCollisions)
        {
            materialObjectMover.MoveObject(materialObject, existingCollisions.Aggregate(Vector2.Zero, (sum, x) => sum + x.MinimalTranslationVector));
        }

        //recursively moves close to the collision point, then starts moving along its collision edge until velocity is expired
        public void FindAndProcessNextCollisions(IMaterialObject currentObject, IList<CollisionPair> potentialFutureCollisions)
        {
            var originalVelocity = currentObject.Velocity;
            while (currentObject.Velocity.Length() >= PhysicsConstants.MIN_VELOCITY)
            {
                if (potentialFutureCollisions.Count == 0)
                {
                    materialObjectMover.MoveObject(currentObject, currentObject.Velocity * GlobalObjects.ScaledDeltaTime);
                    break;
                }
                var nextCollisions = futureCollisionCalculator.FindNextCollisions(potentialFutureCollisions);
                if(nextCollisions.Count == 0)
                {
                    materialObjectMover.MoveObject(currentObject, currentObject.Velocity * GlobalObjects.ScaledDeltaTime);
                    break;
                }
                var fixedVelocity = currentObject.Velocity * GlobalObjects.ScaledDeltaTime;
                var remainingTravelDistance = fixedVelocity.Length();
                var nextCollision = nextCollisions.MinBy(x => x.DistanceToCollision);
                var regularMovementPortion = nextCollision.DistanceToCollision / remainingTravelDistance;
                var regularMovement = fixedVelocity * regularMovementPortion;

                //without casting regularMovement to int the movement bugs, and I have no idea why
                materialObjectMover.MoveObject(currentObject, new Vector2((int)regularMovement.X, (int)regularMovement.Y));
                currentObject.OnVelocityCollision(nextCollision);

                //if an object is not bouncy, we just move it alone the collision edge
                //otherwise, we need to bounce it off of it and move it in the bounced direction as long as there is velocity remaining
                if(currentObject.Bounciness == 0)
                {
                    currentObject.Velocity = currentObject.Velocity.Project(nextCollision.CollisionEdge) * (1 - regularMovementPortion);
                    potentialFutureCollisions.Remove(nextCollision.CollisionPair);
                }
                else
                {
                    originalVelocity = Vector2.Reflect(originalVelocity, nextCollision.Normal) * currentObject.Bounciness;
                    materialObjectMover.MoveObject(currentObject, originalVelocity * (1 - regularMovementPortion) * GlobalObjects.ScaledDeltaTime);
                    break;
                }
            }
            currentObject.Velocity = originalVelocity;
        }
    }
}
