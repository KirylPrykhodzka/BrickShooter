using System.Collections.Generic;
using System.Linq;
using BrickShooter.GameObjects;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
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
            { "PlayerGun", new() { nameof(Bullet) } },
        };

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
                        ProcessCollisionPair(new CollisionPair(currentObjectSingleCollider, otherObjectSingleCollider), result);
                    }
                    //...and other object has >1 collider
                    else
                    {
                        foreach(var otherObjectCollider in otherObject.Colliders)
                        {
                            ProcessCollisionPair(new CollisionPair(currentObjectSingleCollider, otherObjectCollider), result);
                        }
                    }
                }
                //if current object has >1 collider and other object has 1 collider
                else if (otherObjectSingleCollider != null)
                {
                    foreach (var curentObjectCollider in currentObject.Colliders)
                    {
                        ProcessCollisionPair(new CollisionPair(curentObjectCollider, otherObjectSingleCollider), result);
                    }
                }
                //if current object has >1 collider and other object has >1 collider
                else
                {
                    foreach(var currentObjectCollider in currentObject.Colliders)
                    {
                        foreach(var otherObjectCollider in otherObject.Colliders)
                        {
                            ProcessCollisionPair(new CollisionPair(currentObjectCollider, otherObjectCollider), result);
                        }
                    }
                }
            }

            return result;
        }

        private static void ProcessCollisionPair(CollisionPair collisionPair, PotentialCollisions result)
        {
            if (IgnoredCollisions.TryGetValue(collisionPair.CollisionSubject.CollisionLayer, out var ignoredCollisions) && ignoredCollisions.Contains(collisionPair.CollisionObject.CollisionLayer))
            {
                return;
            }
            
            if (collisionPair.CollisionSubject.Bounds.Intersects(collisionPair.CollisionObject.Bounds))
            {
                result.Existing.Add(collisionPair);
            }
            if (DoProjectedBoundsOverlap(collisionPair.CollisionSubject.Bounds, collisionPair.CollisionSubject.Owner.Velocity, collisionPair.CollisionObject.Bounds, collisionPair.CollisionObject.Owner.Velocity))
            {
                result.Future.Add(collisionPair);
            }
        }

        private static bool DoProjectedBoundsOverlap(RectangleF first, Vector2 firstVelocity, RectangleF second, Vector2 secondVelocity)
        {
            var firstFixedVelocity = firstVelocity * GlobalObjects.DeltaTime;
            var secondFixedVelocity = secondVelocity * GlobalObjects.DeltaTime;
            first.Offset(firstFixedVelocity);
            second.Offset(secondFixedVelocity);
            return first.Intersects(second);
        }
    }
}
