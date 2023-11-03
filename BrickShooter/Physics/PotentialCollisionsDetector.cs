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

        public (IList<IMaterialObject> existing, IList<IMaterialObject> future) GetPotentialCollisions(IMaterialObject currentObject, IEnumerable<IMaterialObject> allObjects)
        {
            var existing = new List<IMaterialObject>();
            var future = new List<IMaterialObject>();

            foreach(var otherObject in allObjects.Where(x => x != currentObject && !(IgnoredCollisions.TryGetValue(currentObject.CollisionLayer, out var ignoredCollisions) && ignoredCollisions.Contains(x.CollisionLayer))))
            {
                var currentObjectBounds = currentObject.ColliderPolygon.Bounds;
                var otherObjectBounds = otherObject.ColliderPolygon.Bounds;
                if (currentObjectBounds.Intersects(otherObjectBounds))
                {
                    existing.Add(otherObject);
                }
                if (DoProjectedBoundsOverlap(currentObjectBounds, currentObject.Velocity, otherObjectBounds, otherObject.Velocity))
                {
                    future.Add(otherObject);
                }
            }
            return (existing, future);
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
