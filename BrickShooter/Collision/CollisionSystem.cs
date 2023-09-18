using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace BrickShooter.Collision
{
    /// <summary>
    /// finds all collisions between game objects.
    /// Should be reset upon reloading the level
    /// </summary>
    public static class CollisionSystem
    {
        private readonly static List<ICollisionSubject> collisionSubjects = new();
        private readonly static List<ICollisionActor> collisionObjects = new();

        public static void AddSubject(ICollisionSubject subject)
        {
            collisionSubjects.Add(subject);
        }

        public static void AddObject(ICollisionActor collisionObject)
        {
            collisionObjects.Add(collisionObject);
        }

        /// <summary>
        /// on each update, checks all collisionSubjects for collision with each other or collisionObjects
        /// if there is a collision, OnCollision is called on the subject
        /// </summary>
        public static void Run()
        {
            for (int i = 0; i < collisionSubjects.Count; i++)
            {
                var currentElement = collisionSubjects[i];

                //check collision with other subjects
                for (int j = i + 1; j < collisionSubjects.Count; j++)
                {
                    if (CheckCollision(currentElement.ColliderBounds, collisionSubjects[j].ColliderBounds).collides)
                    {
                        //in this game, only subjects are bullets and player, so no physics calculation is needed upon collision
                        currentElement.OnCollision(collisionSubjects[j]);
                        collisionSubjects[j].OnCollision(currentElement);
                    }
                }

                //check collision with objects
                for (int j = 0; j < collisionObjects.Count; j++)
                {
                    var collisionResult = CheckCollision(currentElement.ColliderBounds, collisionObjects[j].ColliderBounds);
                    if (collisionResult.collides)
                    {
                        currentElement.Velocity += collisionResult.minimumTranslationVector;
                        //translate velocity to avoid further collision
                        currentElement.OnCollision(collisionObjects[j]);
                    }
                }
            }
        }

        /// <summary>
        /// checks whether two objects collides, and if yes, calculates minimum translation to apply to the first polygon to push the polygons apart
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private static (bool collides, Vector2 minimumTranslationVector) CheckCollision(ColliderPolygon first, ColliderPolygon second)
        {
            bool collides = true;

            int edgeCountA = first.Edges.Count;
            int edgeCountB = second.Edges.Count;
            float minIntervalDistance = float.PositiveInfinity;
            Vector2 translationAxis = new();
            Vector2 edge;

            // Loop through all the edges of both polygons
            for (int edgeIndex = 0; edgeIndex < edgeCountA + edgeCountB; edgeIndex++)
            {
                if (edgeIndex < edgeCountA)
                {
                    edge = first.Edges[edgeIndex];
                }
                else
                {
                    edge = second.Edges[edgeIndex - edgeCountA];
                }

                // ===== 1. Find if the polygons are currently intersecting =====

                // Find the axis perpendicular to the current edge
                Vector2 axis = new Vector2(-edge.Y, edge.X);
                axis.Normalize();

                // Find the projection of the polygon on the current axis
                float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
                ProjectPolygon(axis, first, ref minA, ref maxA);
                ProjectPolygon(axis, second, ref minB, ref maxB);

                // Check if the polygon projections are currentlty intersecting
                if (IntervalDistance(minA, maxA, minB, maxB) > 0)
                {
                    collides = false;
                }

                // Do the same test as above for the new projection
                float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);

                // Check if the current interval distance is the minimum one. If so store
                // the interval distance and the current distance.
                // This will be used to calculate the minimum translation vector
                intervalDistance = Math.Abs(intervalDistance);
                if (intervalDistance < minIntervalDistance)
                {
                    minIntervalDistance = intervalDistance;
                    translationAxis = axis;

                    Vector2 d = first.Center - second.Center;
                    if (d.DotProduct(translationAxis) < 0)
                        translationAxis = -translationAxis;
                }
            }

            return (collides, translationAxis * minIntervalDistance);

            static void ProjectPolygon(Vector2 axis, ColliderPolygon polygon, ref float min, ref float max)
            {
                // To project a point on an axis use the dot product
                float dotProduct = axis.DotProduct(polygon.Points[0]);
                min = dotProduct;
                max = dotProduct;
                for (int i = 0; i < polygon.Points.Count; i++)
                {
                    dotProduct = polygon.Points[i].DotProduct(axis);
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

            static float IntervalDistance(float minA, float maxA, float minB, float maxB)
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
        }

        public static void Reset()
        {
            collisionSubjects.Clear();
            collisionObjects.Clear();
        }
    }
}
