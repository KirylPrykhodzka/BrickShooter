using System.Collections.Generic;
using System.Linq;
using BrickShooter.GameObjects;
using BrickShooter.GameObjects.Enemies;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using BrickShooter.Resources;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace BrickShooter.Physics
{
    public class PotentialCollisionsDetector : IPotentialCollisionsDetector
    {
        //each Key is a name of a type that inherits from MaterialObject class.
        //Values are names of types that implement MaterialObject interface
        //If an object of Key type collides with an object contained in Value, collision is ignored completely
        private static readonly Dictionary<string, HashSet<string>> IgnoredCollisions = new()
        {
            { nameof(Bullet), new() { nameof(Bullet), "PlayerGun" } },
            { "PlayerGun", new() { nameof(Bullet), nameof(RedBrick) } },
            { nameof(RedBrick), new() { nameof(RedBrick), "PlayerGun" } }
        };

        private readonly IPool<CollisionPair> collisionPairPool;

        public PotentialCollisionsDetector(IPool<CollisionPair> collisionPairPool)
        {
            this.collisionPairPool = collisionPairPool;
        }

        public PotentialCollisions GetPotentialCollisions(IMaterialObject currentObject, IEnumerable<IMaterialObject> allObjects)
        {
            var result = new PotentialCollisions();

            var currentObjectSingleCollider = currentObject.SingleCollider;

            foreach(var otherObject in allObjects.Where(x => x != currentObject))
            {
                var otherObjectSingleCollider = otherObject.SingleCollider;

                //if current object only has one collider...
                if (currentObjectSingleCollider != null)
                {
                    //...and other object also has only one collider
                    if(otherObjectSingleCollider != null)
                    {
                        CheckCollidersIntersection(currentObjectSingleCollider, otherObjectSingleCollider, result);
                    }
                    //...and other object has >1 collider
                    else
                    {
                        foreach(var otherObjectCollider in otherObject.Colliders)
                        {
                            CheckCollidersIntersection(currentObjectSingleCollider, otherObjectCollider, result);
                        }
                    }
                }
                //if current object has >1 collider and other object has 1 collider
                else if (otherObjectSingleCollider != null)
                {
                    foreach (var curentObjectCollider in currentObject.Colliders)
                    {
                        CheckCollidersIntersection(curentObjectCollider, otherObjectSingleCollider, result);
                    }
                }
                //if current object has >1 collider and other object has >1 collider
                else
                {
                    foreach(var currentObjectCollider in currentObject.Colliders)
                    {
                        foreach(var otherObjectCollider in otherObject.Colliders)
                        {
                            CheckCollidersIntersection(currentObjectCollider, otherObjectCollider, result);
                        }
                    }
                }
            }

            return result;
        }

        private void CheckCollidersIntersection(IColliderPolygon first, IColliderPolygon second, PotentialCollisions result)
        {
            if (IgnoredCollisions.TryGetValue(first.CollisionLayer, out var ignoredCollisions) && ignoredCollisions.Contains(second.CollisionLayer))
            {
                return;
            }
            
            if (first.Bounds.Intersects(second.Bounds))
            {
                var collisionPair = collisionPairPool.GetItem();
                collisionPair.CollisionSubject = first;
                collisionPair.CollisionObject = second;
                result.Existing.Add(collisionPair);
            }
            if (DoProjectedBoundsOverlap(first.Bounds, first.Owner.Velocity, second.Bounds, second.Owner.Velocity))
            {
                var collisionPair = collisionPairPool.GetItem();
                collisionPair.CollisionSubject = first;
                collisionPair.CollisionObject = second;
                result.Future.Add(collisionPair);
            }
        }

        private static bool DoProjectedBoundsOverlap(RectangleF first, Vector2 firstVelocity, RectangleF second, Vector2 secondVelocity)
        {
            var firstFixedVelocity = firstVelocity * GlobalObjects.ScaledDeltaTime;
            var secondFixedVelocity = secondVelocity * GlobalObjects.ScaledDeltaTime;
            first.Offset(firstFixedVelocity);
            second.Offset(secondFixedVelocity);
            return first.Intersects(second);
        }
    }
}
