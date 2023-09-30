using BrickShooter.Extensions;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrickShooter.Physics
{
    public class CollisionCalculator : ICollisionCalculator
    {
        public CollisionPredictionResult CalculateCollision(MaterialObject collisionSubject, MaterialObject collisionObject)
        {
            CollisionPredictionResult result = new()
            {
                RelativeVelocity = collisionSubject.Velocity - collisionObject.Velocity
            };

            var subjectFrontFacingPoints = GetFrontFacingPoints(collisionSubject);
            var objectFrontFacingPoints = GetFrontFacingPoints(collisionObject);

            return result;
        }

        public static IList<Vector2> GetFrontFacingPoints(MaterialObject materialObject)
        {
            var localColliderPoints = materialObject.LocalColliderPolygon.Points;
            if (materialObject.Velocity == Vector2.Zero || localColliderPoints.Count <= 2)
            {
                return localColliderPoints;
            }
            var perpendicularAxis = new Vector2(materialObject.Velocity.Y, -materialObject.Velocity.X);
            char projectionComparisonAxis = Math.Abs(perpendicularAxis.X) > Math.Abs(perpendicularAxis.Y) ? 'x' : 'y';

            //find width of an object relative to its movement direction
            IEnumerable<(Vector2 key, Vector2 value)> perpendicularProjections = localColliderPoints
                //need to add * new Vector2(1, -1) because in Monogame Y axis is inverted
                .Select(x => (x, x.Project(perpendicularAxis)));

            Vector2 min = Vector2.Zero;
            Vector2 max = Vector2.Zero;
            if (projectionComparisonAxis == 'x')
            {
                var minX = perpendicularProjections.Min(x => x.value.X);
                var maxX = perpendicularProjections.Max(x => x.value.X);
                var minGroup = perpendicularProjections.Where(x => x.value.X == minX);
                var maxGroup = perpendicularProjections.Where(x => x.value.X == maxX);
                if(materialObject.Velocity.Y > 0)
                {
                    min = minGroup.MaxBy(x => x.key.Y).key;
                    max = maxGroup.MaxBy(x => x.key.Y).key;
                }
                else
                {
                    min = minGroup.MinBy(x => x.key.Y).key;
                    max = maxGroup.MinBy(x => x.key.Y).key;
                }
            }
            else
            {
                var minY = perpendicularProjections.Min(x => x.value.Y);
                var maxY = perpendicularProjections.Max(x => x.value.Y);
                var minGroup = perpendicularProjections.Where(x => x.value.Y == minY);
                var maxGroup = perpendicularProjections.Where(x => x.value.Y == maxY);
                if (materialObject.Velocity.X > 0)
                {
                    min = minGroup.MaxBy(x => x.key.X).key;
                    max = maxGroup.MaxBy(x => x.key.X).key;
                }
                else
                {
                    min = minGroup.MinBy(x => x.key.X).key;
                    max = maxGroup.MinBy(x => x.key.X).key;
                }
            }

            //find all points that are closer to velocity than the min-max line
            var slope = (min.Y - max.Y) / (min.X - max.X);

            return new List<Vector2>();
        }
    }
}
