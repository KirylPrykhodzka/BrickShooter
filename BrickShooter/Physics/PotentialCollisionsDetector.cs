using System.Collections.Generic;
using System.Linq;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using BrickShooter.Helpers;
using System.Diagnostics;
using System;
using Microsoft.Xna.Framework;

namespace BrickShooter.Physics
{
    public class PotentialCollisionsDetector : IPotentialCollisionsDetector
    {

        //each Key is a name of a type that inherits from MaterialObject class.
        //Values are names of types that implement MaterialObject interface
        //If an object of Key type collides with an object contained in Value, collision is ignored completely
        private static readonly Dictionary<string, HashSet<string>> IgnoredCollisions = new()
        {
            { "Bullet", new() { "Bullet" } }
        };

        public IList<MaterialObject> DetectPotentialCollisions(MaterialObject currentObject, IEnumerable<MaterialObject> otherObjects)
        {
            var potentialCollisions = otherObjects
                .Where(x => !IgnoredCollisions.TryGetValue(currentObject.CollisionLayer, out var ignoredCollisions) || !ignoredCollisions.Contains(x.CollisionLayer))
                .Where(x => IsCollisionPossible(currentObject, x));

            return potentialCollisions.ToList();
        }

        private static bool IsCollisionPossible(MaterialObject first, MaterialObject second)
        {
            var firstPolygon = first.ColliderPolygon;
            var secondPolygon = second.ColliderPolygon;
            return DoBoundsOverlap(firstPolygon, secondPolygon) || DoProjectedBoundsOverlap(firstPolygon, first.Velocity, secondPolygon, second.Velocity);
        }

        private static bool DoBoundsOverlap(ColliderPolygon first, ColliderPolygon second)
        {
            return
                first.MaxX > second.MinX &&
                first.MaxY > second.MinY &&
                first.MinX < second.MaxX &&
                first.MinY < second.MaxY;
        }

        private static bool DoProjectedBoundsOverlap(ColliderPolygon first, Vector2 firstVelocity, ColliderPolygon second, Vector2 secondVelocity)
        {
            var firstFixedVelocity = firstVelocity * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds;
            var secondFixedVelocity = secondVelocity * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds;

            return
                first.MaxX + firstFixedVelocity.X > second.MinX + secondFixedVelocity.X &&
                first.MinX + firstFixedVelocity.X < second.MaxX + secondFixedVelocity.X &&
                first.MaxY + firstFixedVelocity.Y > second.MinY + secondFixedVelocity.Y &&
                first.MinY + firstFixedVelocity.Y < second.MaxY + secondFixedVelocity.Y;
        }
    }
}
