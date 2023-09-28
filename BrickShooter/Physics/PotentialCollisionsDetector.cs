using BrickShooter.GameObjects.Bullets;
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
                //since I will be projecting objects, this might break things
                //.Where(x => !DefinitelyDoNotCollide(currentObject, x))
                .Where(x => !IgnoredCollisions.TryGetValue(currentObject.GetType().Name, out var ignoredCollisions) || !ignoredCollisions.Contains(x.GetType().Name));

            return potentialCollisions;
        }

        private static bool DefinitelyDoNotCollide(MaterialObject first, MaterialObject second)
        {
            return
                first.GlobalColliderPolygon.MaxX < second.GlobalColliderPolygon.MinX ||
                second.GlobalColliderPolygon.MaxX < first.GlobalColliderPolygon.MinX ||
                first.GlobalColliderPolygon.MaxY < second.GlobalColliderPolygon.MinY ||
                second.GlobalColliderPolygon.MaxY < first.GlobalColliderPolygon.MinY;
        }
    }
}
