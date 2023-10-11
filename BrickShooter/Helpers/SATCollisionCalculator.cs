using BrickShooter.Extensions;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrickShooter.Helpers
{
    public static class SATCollisionCalculator
    {
        public static (bool isColliding, Vector2 minimalTranslationVector) GetCollisionResult(MaterialObject collisionSubject, MaterialObject collisionObject)
        {
            var subjectEdges = BuildEdges(collisionSubject.ColliderPolygon.Points).ToList();
            var objectEdges = BuildEdges(collisionObject.ColliderPolygon.Points).ToList();

            bool isColliding = true;
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

                // ===== 1. Find if the polygons are currently intersecting =====

                // Find the axis perpendicular to the current edge
                Vector2 axis = new Vector2(-edge.Y, edge.X);
                axis.Normalize();

                // Find the projection of the polygon on the current axis
                float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
                ProjectPolygon(axis, collisionSubject.ColliderPolygon, ref minA, ref maxA);
                ProjectPolygon(axis, collisionObject.ColliderPolygon, ref minB, ref maxB);

                // Check if the polygon projections are currentlty intersecting
                float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance > 0)
                {
                    return (false, Vector2.Zero);
                }

                // Check if the current interval distance is the minimum one. If so store
                // the interval distance and the current distance.
                // This will be used to calculate the minimum translation vector
                intervalDistance = Math.Abs(intervalDistance);
                if (intervalDistance < minIntervalDistance)
                {
                    minIntervalDistance = intervalDistance;
                    translationAxis = axis;

                    Vector2 d = collisionSubject.ColliderPolygon.Center - collisionObject.ColliderPolygon.Center;
                    if (Vector2.Dot(d, translationAxis) < 0)
                        translationAxis = -translationAxis;
                }
            }

            var minimalTranslationVector = translationAxis * minIntervalDistance;
            //increase translation by 1px to move the object outside of colliding object's borders
            minimalTranslationVector += new Vector2(Math.Sign(minimalTranslationVector.X), Math.Sign(minimalTranslationVector.Y));

            return (isColliding, minimalTranslationVector);
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

        private static IEnumerable<Vector2> BuildEdges(IList<Vector2> points)
        {
            var result = new List<Vector2>();
            Vector2 p1;
            Vector2 p2;
            for (int i = 0; i < points.Count; i++)
            {
                p1 = points[i];
                if (i + 1 >= points.Count)
                {
                    p2 = points[0];
                }
                else
                {
                    p2 = points[i + 1];
                }
                result.Add(p2 - p1);
            }

            return result;
        }
    }
}
