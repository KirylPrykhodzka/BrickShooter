using BrickShooter.Extensions;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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

        public void ProcessNextCollisions(MaterialObject currentObject, IList<CollisionPredictionResult> nextCollisions)
        {
            //moves along nextCollisions edges starting from the closest one, without moving towards them
            //for correct simulation, the object needs to move toward nextCollision until it reaches it, but that does not work for some reason
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
                currentObject.Velocity = currentObject.Velocity.Project(nextCollision.CollisionEdge.point2 - nextCollision.CollisionEdge.point1) * (1 - regularMovementPortion);
                nextCollisions = collisionCalculator.FindNextCollisions(currentObject, nextCollisions.Where(x => x != nextCollision).Select(x => x.CollisionObject));
            }
            currentObject.Velocity = originalVelocity;
        }
    }
}
