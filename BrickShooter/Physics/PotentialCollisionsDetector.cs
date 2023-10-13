using BrickShooter.GameObjects;
using System.Collections.Generic;
using System.Linq;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;

namespace BrickShooter.Physics
{
    public class PotentialCollisionsDetector : IPotentialCollisionsDetector
    {

        //each Key is a name of a type that inherits from MaterialObject class.
        //Values are names of types that implement MaterialObject interface
        //If an object of Key type collides with an object contained in Value, collision is ignored completely
        private static readonly Dictionary<string, HashSet<string>> IgnoredCollisions = new()
        {
            { typeof(Bullet).Name, new() { typeof(Bullet).Name, typeof(Player).Name } },
            { typeof(Player).Name, new() { typeof(Bullet).Name, } },
        };

        public IEnumerable<MaterialObject> DetectPotentialCollisions(MaterialObject currentObject, IEnumerable<MaterialObject> otherObjects)
        {
            var potentialCollisions = otherObjects
                .Where(x => IsCollisionPossible(currentObject, x))
                .Where(x => !IgnoredCollisions.TryGetValue(currentObject.GetType().Name, out var ignoredCollisions) || !ignoredCollisions.Contains(x.GetType().Name));

            return potentialCollisions;
        }

        private static bool IsCollisionPossible(MaterialObject first, MaterialObject second)
        {
            return DoBoundsOverlap(first, second) || DoProjectedBoundsOverlap(first, second);
        }

        private static bool DoBoundsOverlap(MaterialObject first, MaterialObject second)
        {
            return
                first.ColliderPolygon.MaxX > second.ColliderPolygon.MinX &&
                first.ColliderPolygon.MaxY > second.ColliderPolygon.MinY &&
                first.ColliderPolygon.MinX < second.ColliderPolygon.MaxX &&
                first.ColliderPolygon.MinY < second.ColliderPolygon.MaxY;
        }

        private static bool DoProjectedBoundsOverlap(MaterialObject first, MaterialObject second)
        {
            var firstFixedVelocity = first.Velocity * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds;
            var secondFixedVelocity = second.Velocity * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds;

            return
                first.ColliderPolygon.MaxX + firstFixedVelocity.X > second.ColliderPolygon.MinX + secondFixedVelocity.X &&
                first.ColliderPolygon.MinX + firstFixedVelocity.X < second.ColliderPolygon.MaxX + secondFixedVelocity.X &&
                first.ColliderPolygon.MaxY + firstFixedVelocity.Y > second.ColliderPolygon.MinY + secondFixedVelocity.Y &&
                first.ColliderPolygon.MinY + firstFixedVelocity.Y < second.ColliderPolygon.MaxY + secondFixedVelocity.Y;
        }
    }
}
