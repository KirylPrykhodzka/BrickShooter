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

        public void ProcessExistingCollisions(MaterialObject materialObject, IList<CollisionData> existingCollisions)
        {
            //apply the biggest minimal translation vector, hoping that it will fix the rest of the collisions too
            var biggestTranslationVector = existingCollisions
                .MaxBy(x => Math.Abs(x.MinimalTranslationVector.X) + Math.Abs(x.MinimalTranslationVector.Y))
                .MinimalTranslationVector;
            materialObject.Position += biggestTranslationVector;
        }

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
                var remainingTravelDistance = currentObject.Velocity * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds;
                var nextCollision = nextCollisions.MinBy(x => x.DistanceToCollision);

                var collision = collisionCalculator.GetExistingCollisions(currentObject, new List<MaterialObject>() { nextCollision.CollisionObject }).Where(x => x.MinimalTranslationVector != Vector2.Zero).FirstOrDefault();
                if (collision != null)
                {

                }

                var unobstructedMovementPortion = nextCollision.DistanceToCollision / remainingTravelDistance.Magnitude();
                currentObject.Position += remainingTravelDistance * unobstructedMovementPortion / 2;

                collision = collisionCalculator.GetExistingCollisions(currentObject, new List<MaterialObject>() { nextCollision.CollisionObject }).Where(x => x.MinimalTranslationVector != Vector2.Zero).FirstOrDefault();
                if (collision != null)
                {
                    
                }

                currentObject.Velocity = currentObject.Velocity.Project(nextCollision.CollisionEdge.point2 - nextCollision.CollisionEdge.point1) * (1 - unobstructedMovementPortion);
                nextCollisions = collisionCalculator.FindNextCollisions(currentObject, nextCollisions.Where(x => x != nextCollision).Select(x => x.CollisionObject));
            }
            currentObject.Velocity = originalVelocity;
        }
    }
}
