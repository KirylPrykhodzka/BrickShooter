using BrickShooter.Constants;
using BrickShooter.Extensions;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BrickShooter.Physics
{
    public class MaterialObjectMover : IMaterialObjectMover
    {
        private readonly ICollisionCalculator collisionCalculator;

        public MaterialObjectMover(ICollisionCalculator collisionCalculator)
        {
            this.collisionCalculator = collisionCalculator;
        }

        public void MoveWithoutObstruction(MaterialObject materialObject)
        {
            materialObject.Position += materialObject.Velocity * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds;
        }

        public void ProcessExistingCollisions(MaterialObject materialObject, IList<Vector2> translationVectors)
        {
            //apply the biggest minimal translation vector, hoping that it will fix the rest of the collisions too
            var longestTranslationVector = translationVectors
                .MaxBy(x => x.Magnitude());
            materialObject.Position += longestTranslationVector;
        }

        //recursively moves close to the collision point, then starts moving along its collision edge until velocity is expired
        public void ProcessNextCollisions(MaterialObject currentObject, IList<CollisionPredictionResult> nextCollisions)
        {
            var originalVelocity = currentObject.Velocity;
            while(currentObject.Velocity != Vector2.Zero)
            {
                if(!nextCollisions.Any())
                {
                    MoveWithoutObstruction(currentObject);
                    break;
                }
                var fixedVelocity = currentObject.Velocity * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds;
                var remainingTravelDistance = fixedVelocity.Magnitude();
                var nextCollision = nextCollisions.MinBy(x => x.DistanceToCollision);
                var regularMovementPortion = nextCollision.DistanceToCollision / remainingTravelDistance;
                var regularMovement = fixedVelocity * regularMovementPortion;
                //the fractions are cut because it causes clipping sometimes.
                //in worst case scenario the object stops 1 unit from collision which does not matter
                currentObject.Position += new Vector2((int)regularMovement.X, (int)regularMovement.Y);
                currentObject.Velocity = currentObject.Velocity.Project(nextCollision.CollisionEdge.point2 - nextCollision.CollisionEdge.point1) * (1 - regularMovementPortion);
                if(Math.Abs(currentObject.Velocity.X) < PhysicsConstants.MIN_VELOCITY && Math.Abs(currentObject.Velocity.Y) < PhysicsConstants.MIN_VELOCITY)
                {
                    break;
                }
                nextCollisions = collisionCalculator.FindNextCollisions(currentObject, nextCollisions.Where(x => x != nextCollision).Select(x => x.CollisionObject));
            }
            currentObject.Velocity = originalVelocity;
        }
    }
}
