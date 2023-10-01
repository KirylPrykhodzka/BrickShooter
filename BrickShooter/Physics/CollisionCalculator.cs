using BrickShooter.Extensions;
using BrickShooter.Helpers;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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

            if (materialObject.Velocity == Vector2.Zero || materialObject.LocalColliderPolygon.Points.Count <= 2)
            {
                return materialObject.LocalColliderPolygon.Points;
            }

            var localColliderPoints = materialObject.LocalColliderPolygon.Points
                //this makes sure that polygon center is in (0,0) which makes it much easier to operate on
                .Select(x => x - materialObject.LocalColliderPolygon.Center)
                .ToList();

            var perpendicularAxis = new Vector2(materialObject.Velocity.Y, -materialObject.Velocity.X);
            char projectionComparisonAxis = Math.Abs(perpendicularAxis.X) > Math.Abs(perpendicularAxis.Y) ? 'x' : 'y';

            //find width of an object relative to its movement direction
            IEnumerable<(Vector2 key, Vector2 value)> perpendicularProjections = localColliderPoints
                //need to add * new Vector2(1, -1) because in Monogame Y axis is inverted
                .Select(x => (x, x.Project(perpendicularAxis)));

            //left- and right-most points of the polygon relative to its velocity
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

            //find all points closer to the front then min and max
            var result = new List<Vector2> { min, max };
            var otherLocalColliderPoints = localColliderPoints.Where(x => x != min && x != max);

            if (projectionComparisonAxis == 'y')
            {
                var aboveY = otherLocalColliderPoints.Where(x => x.Y < 0);
                var belowY = otherLocalColliderPoints.Where(x => x.Y >= 0);
                var distanceToMin = materialObject.Velocity.X - min.X;
                var distanceToMax = materialObject.Velocity.X - max.X;
                result.AddRange(aboveY.Where(x => Math.Abs(materialObject.Velocity.X - x.X) < Math.Abs(distanceToMin)));
                result.AddRange(belowY.Where(x => Math.Abs(materialObject.Velocity.X - x.X) < distanceToMax));
            }
            else
            {
                var leftOfX = otherLocalColliderPoints.Where(x => x.X < 0);
                var rightToX = otherLocalColliderPoints.Where(x => x.X >= 0);
                var distanceToMin = materialObject.Velocity.Y - min.Y;
                var distanceToMax = materialObject.Velocity.Y - max.Y;
                result.AddRange(leftOfX.Where(x => Math.Abs(materialObject.Velocity.Y - x.Y) < Math.Abs(distanceToMin)));
                result.AddRange(rightToX.Where(x => Math.Abs(materialObject.Velocity.Y - x.Y) < distanceToMax));
            }

            //need to "move" polygon back to its original position relative to object's position before returning
            var result = result.Select(x => x + materialObject.LocalColliderPolygon.Center).ToList();
            return result;
        }
    }
}
