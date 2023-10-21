using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrickShooter.Physics
{
    public class ExistingCollisionsCalculator : IExistingCollisionsCalculator
    {
        public IList<CollisionInfo> GetExistingCollisions(MaterialObject collisionSubject, IEnumerable<MaterialObject> potentialCollisions)
        {
            return potentialCollisions
                .Select(x => CalculateExistingCollisionResult(collisionSubject.ColliderPolygon, x.ColliderPolygon))
                .ToList();
        }

        private static CollisionInfo CalculateExistingCollisionResult(ColliderPolygon first, ColliderPolygon second)
        {
            var result = new CollisionInfo
            {
                IsColliding = true
            };

            var subjectEdges = BuildEdges(first.Points).ToList();
            var objectEdges = BuildEdges(second.Points).ToList();
            float minIntervalDistance = float.PositiveInfinity;
            Vector2 translationAxis = Vector2.Zero;
            Vector2 edge;

            // Loop through all the edges of both polygons
            for (int edgeIndex = 0; edgeIndex < subjectEdges.Count + objectEdges.Count; edgeIndex++)
            {
                if (edgeIndex < subjectEdges.Count)
                {
                    edge = subjectEdges[edgeIndex];
                }
                else
                {
                    edge = objectEdges[edgeIndex - subjectEdges.Count];
                }

                // Find the axis perpendicular to the current edge
                Vector2 axis = new(-edge.Y, edge.X);
                axis.Normalize();

                // Find the projection of the polygon on the current axis
                float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
                ProjectPolygon(axis, first, ref minA, ref maxA);
                ProjectPolygon(axis, second, ref minB, ref maxB);

                // Check if the polygon projections are currentlty intersecting
                float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance > 0)
                {
                    result.IsColliding = false;
                    return result;
                }

                // Check if the current interval distance is the minimum one. If so store
                // the interval distance and the current distance.
                // This will be used to calculate the minimum translation vector
                intervalDistance = Math.Abs(intervalDistance);
                if (intervalDistance < minIntervalDistance)
                {
                    minIntervalDistance = intervalDistance;
                    translationAxis = axis;

                    Vector2 d = first.Center - second.Center;
                    if (Vector2.Dot(d, translationAxis) < 0)
                        translationAxis = -translationAxis;
                }
            }

            result.MinimalTranslationVector = translationAxis * minIntervalDistance;
            //increase translation by 1px to move the object outside of colliding object's borders
            result.MinimalTranslationVector += new Vector2(Math.Sign(result.MinimalTranslationVector.X), Math.Sign(result.MinimalTranslationVector.Y));

            return result;
        }

        private static void ProjectPolygon(Vector2 axis, ColliderPolygon polygon, ref float min, ref float max)
        {
            // To project a point on an axis use the dot product
            float dotProduct = Vector2.Dot(axis, polygon.Points[0]);
            min = dotProduct;
            max = dotProduct;
            for (int i = 0; i < polygon.Points.Count; i++)
            {
                dotProduct = Vector2.Dot(axis, polygon.Points[i]);
                if (dotProduct < min)
                {
                    min = dotProduct;
                }
                else
                {
                    if (dotProduct > max)
                    {
                        max = dotProduct;
                    }
                }
            }
        }

        private static float IntervalDistance(float minA, float maxA, float minB, float maxB)
        {
            if (minA < minB)
            {
                return minB - maxA;
            }
            else
            {
                return minA - maxB;
            }
        }

        private static IList<Vector2> BuildEdges(IList<Vector2> points)
        {
            var result = new List<Vector2>();
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 p1 = points[i];
                Vector2 p2 = i + 1 >= points.Count ? points[0] : points[i + 1];
                result.Add(p2 - p1);
            }
            return result;
        }
    }
}
