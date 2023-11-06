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
            { nameof(Bullet), new() { nameof(Bullet) } }
        };

        public PotentialCollisions GetPotentialCollisions(IMaterialObject currentObject, IEnumerable<IMaterialObject> allObjects)
        {
            var result = new PotentialCollisions();

            //create collision pairs for each current object collider + each collider of each other object
            //check bounds overlap

            foreach (var otherObjectCollider in allObjects.Where(x => x != currentObject).SelectMany(x => x.Colliders))
            {
                foreach(var currentObjectCollider in currentObject.Colliders)
                {
                    if(!(IgnoredCollisions.TryGetValue(currentObjectCollider.CollisionLayer, out var ignoredCollisions) && ignoredCollisions.Contains(otherObjectCollider.CollisionLayer)))
                    {
                        ProcessCollisionPair(new CollisionPair(currentObjectCollider, otherObjectCollider), result);
                    }
                }
            }
            return result;
        }

        private static void ProcessCollisionPair(CollisionPair collisionPair, PotentialCollisions result)
        {
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
