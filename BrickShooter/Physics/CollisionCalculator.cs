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

            //find width of an object relative to its movement direction
            IEnumerable<(Vector2 key, Vector2 value)> perpendicularProjections = localColliderPoints
                //need to add * new Vector2(1, -1) because in Monogame Y axis is inverted
                .Select(x => (x, x.Project(perpendicularAxis)));
            var ordered = perpendicularProjections.OrderBy(x => Math.Abs(x.value.X) + Math.Abs(x.value.Y)).Take(2).ToArray();

            //it does not actually matter which one is min and which one is max
            Vector2 min = ordered[0].key;
            Vector2 max = ordered[1].key;

            //find all points that are closer to velocity than the min-max vector
            var threshold = max - min;

            return new List<Vector2>();
        }
    }
}
