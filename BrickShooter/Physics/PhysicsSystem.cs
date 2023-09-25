using BrickShooter.Extensions;
using BrickShooter.GameObjects;
using BrickShooter.GameObjects.Bullets;
using BrickShooter.Helpers;
using BrickShooter.Physics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static readonly List<MobileMaterialObject> mobileObjects = new();
        //objects that mobile objects can collide with
        private static readonly List<IMaterialObject> immobileObjects = new();

        //each Key is a name of a type that inherits from MobileMaterialObject class.
        //Values are names of types that implement IMaterialObject interface
        //If an object of Key type collides with an object contained in Value, collision is ignored completely
        private static readonly Dictionary<string, List<string>> IgnoredCollisions = new()
        {
            { typeof(Bullet).Name, new() { typeof(Bullet).Name, typeof(Player).Name } },
            { typeof(Player).Name, new() { typeof(Bullet).Name, } },
        };

        public static void RegisterMobileObject(MobileMaterialObject mobileObject)
        {
            if(mobileObjects.Contains(mobileObject))
            {
                throw new InvalidOperationException("Cannot register same mobile object twice");
            }
            mobileObjects.Add(mobileObject);
        }

        public static void RemoveMobileObject(MobileMaterialObject mobileObject)
        {
            if (!mobileObjects.Contains(mobileObject))
            {
                throw new InvalidOperationException("The object is not registered in the physics system");
            }
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
            var allCollisions = new List<CollisionInfo>();

            var currentObjectCollisions = new List<CollisionInfo>();
            for (int i = 0; i < mobileObjects.Count; i++)
            {
                var currentObject = mobileObjects[i];
                var fixedVelocity = currentObject.Velocity * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds;
                currentObject.Position += fixedVelocity;

                //check collision with other mobile and immobile objects
                foreach (var otherObject in mobileObjects.Where(x => x != currentObject).Concat(immobileObjects))
                {
                    if (DefinitelyDoNotCollide(currentObject, otherObject) ||
                        IgnoredCollisions.TryGetValue(currentObject.GetType().Name, out var ignoredCollisions) && ignoredCollisions.Contains(otherObject.GetType().Name))
                    {
                        continue;
                    }

                    //get all collisions
                    var (collides, minimumTranslation) = GetCollisionResult(currentObject.ColliderBounds, otherObject.ColliderBounds);
                    //.ToPoint() is to avoid detecting miniscule collisions which we do not care about
                    if (collides && minimumTranslation.ToPoint() != Point.Zero)
                    {
                        currentObjectCollisions.Add(new CollisionInfo { CollisionSubject = currentObject, CollisionObject = otherObject, Translation = minimumTranslation });
                    }
                }

                if (currentObjectCollisions.Count == 0)
                {
                    continue;
                }

                //apply each collision and check how many collisions remain
                //if 0, awesome
                //if > 0, try finding the one which leaves 0 collisions among other ones
                //if all collisions leave >0 after applying, apply the one that leaves least collisions and repeat the steps
                var remainingCollisions = currentObjectCollisions;
                HashSet<CollisionInfo> appliedCollisions = new();
                List<(CollisionInfo collisionInfo, List<CollisionInfo> remainingAfterApplication)> collisionApplicationResults = new();
                while (remainingCollisions.Count > 0)
                {
                    foreach (var collision in remainingCollisions)
                    {
                        var previousPosition = currentObject.Position;
                        currentObject.Position += collision.Translation;
                        var stillRemainingCollisions = remainingCollisions.Where(x =>
                        {
                            var (collides, minimumTranslation) = GetCollisionResult(x.CollisionSubject.ColliderBounds, x.CollisionObject.ColliderBounds);
                            return collides && minimumTranslation.ToPoint() != Point.Zero;
                        }).ToList();
                        collisionApplicationResults.Add((collision, stillRemainingCollisions));
                        currentObject.Position = previousPosition;
                        if (!stillRemainingCollisions.Any())
                        {
                            break;
                        }
                    }

                    var (collisionInfo, remainingAfterApplication) = collisionApplicationResults.MinBy(x => x.remainingAfterApplication.Count);
                    remainingCollisions = remainingAfterApplication;
                    collisionApplicationResults.Clear();

                    collisionInfo.CollisionSubject.Position += collisionInfo.Translation;
                    appliedCollisions.Add(collisionInfo);
                }

                //bounce
                var bounceForce = currentObject.Bounciness + appliedCollisions.Average(x => x.CollisionObject.Bounciness);
                if (bounceForce > 0)
                {
                    Vector2 bounceVector = new(appliedCollisions.First().Translation.ToPoint().X == 0 ? 1 : -1, appliedCollisions.First().Translation.ToPoint().Y == 0 ? 1 : -1);
                    if (appliedCollisions.Count > 1)
                    {
                        var allXTranslations = appliedCollisions.Select(x => x.Translation.X).Where(x => x != 0);
                        var areAllXTranslationsInSameDirection = allXTranslations.All(x => x > 0) || allXTranslations.All(x => x < 0);
                        var allYTranslations = appliedCollisions.Select(x => x.Translation.Y).Where(y => y != 0);
                        var areAllYTranslationsInSameDirection = allYTranslations.All(y => y > 0) || allYTranslations.All(y => y < 0);
                        bounceVector = new Vector2(areAllXTranslationsInSameDirection ? -1 : 1, areAllYTranslationsInSameDirection ? -1 : 1);
                    }
                    currentObject.Velocity *= bounceVector * bounceForce;
                }

                allCollisions.AddRange(currentObjectCollisions);
                currentObjectCollisions.Clear();
            }

            //OnCollision call can cause side effects, so it has to be made after each collision is handled by the physics system
            foreach (var collisionInfo in allCollisions)
            {
                collisionInfo.CollisionSubject.OnCollision(collisionInfo.CollisionObject);
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
        private static (bool collides, Vector2 minimumTranslation) GetCollisionResult(ColliderPolygon first, ColliderPolygon second)
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

        //TODO: extend with proper broad phase logic
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
