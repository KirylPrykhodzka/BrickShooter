using BrickShooter.Extensions;
using BrickShooter.Helpers;
using BrickShooter.Physics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrickShooter.Collision
{
    /// <summary>
    /// moves objects around in accordance with their velocity;
    /// finds all collisions between game objects.
    /// Should be reset upon reloading the level
    /// </summary>
    public static class PhysicsSystem
    {
        //all objects that can be repositioned in space based on their velocity and initiate collisions
        private readonly static List<MobileMaterialObject> mobileObjects = new();
        //objects that mobile objects can collide with
        private readonly static List<IMaterialObject> immobileObjects = new();

        public static void RegisterMobileObject(MobileMaterialObject mobileObject)
        {
            mobileObjects.Add(mobileObject);
        }

        public static void RemoveMobileObject(MobileMaterialObject mobileObject)
        {
            mobileObjects.Remove(mobileObject);
        }

        public static void RegisterImmobileObject(IMaterialObject immobileObject)
        {
            immobileObjects.Add(immobileObject);
        }

        public static void Reset()
        {
            mobileObjects.Clear();
            immobileObjects.Clear();
        }

        /// <summary>
        /// on each trigger, moves all mobile objects in space based on their velocity (first on X, and then on Y axis)
        /// after movement on each axis, checks and handles collisions
        /// </summary>
        public static void Run()
        {
            for (int i = 0; i < mobileObjects.Count; i++)
            {
                var currentElement = mobileObjects[i];
                if(currentElement.Velocity.X != 0)
                {
                    var fixedVelocity = currentElement.Velocity.X * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds;
                    currentElement.Position += new Point((int)fixedVelocity, 0);
                    CheckCollisions();
                }
                if (currentElement.Velocity.Y != 0)
                {
                    var fixedVelocity = currentElement.Velocity.Y * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds;
                    currentElement.Position += new Point(0, (int)fixedVelocity);
                    CheckCollisions();
                }

                void CheckCollisions()
                {
                    //check collision with other mobile objects
                    for (int j = i + 1; j < mobileObjects.Count; j++)
                    {
                        if (DefinitelyDoNotCollide(currentElement, mobileObjects[j]))
                        {
                            continue;
                        }
                        if (CheckCollision(currentElement.ColliderBounds, mobileObjects[j].ColliderBounds).collides)
                        {
                            //in this game, only subjects are bullets and player, so no physics calculation is needed upon collision
                            currentElement.OnCollision(mobileObjects[j]);
                            mobileObjects[j].OnCollision(currentElement);
                        }
                    }

                    //check collision with objects
                    for (int j = 0; j < immobileObjects.Count; j++)
                    {
                        if (DefinitelyDoNotCollide(currentElement, immobileObjects[j]))
                        {
                            continue;
                        }
                        var collisionResult = CheckCollision(currentElement.ColliderBounds, immobileObjects[j].ColliderBounds);
                        if (collisionResult.collides)
                        {
                            if (collisionResult.minimumTranslationVector != Vector2.Zero)
                            {
                                currentElement.Position += collisionResult.minimumTranslationVector.ToPoint();
                            }

                            currentElement.OnCollision(immobileObjects[j]);
                        }
                    }
                }
            }
        }

        public static void Visualize()
        {
#if DEBUG
            foreach (var collisionActor in immobileObjects.Concat(mobileObjects))
            {
                VisualizationHelper.VisualizeCollider(collisionActor.ColliderBounds);
            }
#endif
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

        private static bool DefinitelyDoNotCollide(MobileMaterialObject first, IMaterialObject second)
        {
            return
                first.ColliderBounds.MaxX < second.ColliderBounds.MinX ||
                first.ColliderBounds.MinX > second.ColliderBounds.MaxX ||
                first.ColliderBounds.MaxY < second.ColliderBounds.MinY ||
                first.ColliderBounds.MinY > second.ColliderBounds.MaxY;
        }
    }
}
