using BrickShooter.GameObjects;
using BrickShooter.GameObjects.Bullets;
using BrickShooter.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace BrickShooter.Physics
{
    /// <summary>
    /// moves objects around in accordance with their velocity;
    /// finds all collisions between game objects.
    /// Should be reset upon reloading the level
    /// </summary>
    public class PhysicsSystem : IPhysicsSystem
    {
        private readonly ICollisionsCalculator collisionsCalculator;

        public PhysicsSystem(ICollisionsCalculator collisionsCalculator)
        {
            this.collisionsCalculator = collisionsCalculator;
        }

        //all objects that can be repositioned in space based on their velocity and initiate collisions
        private readonly HashSet<MaterialObject> mobileObjects = new();
        //objects that mobile objects can collide with
        private readonly HashSet<MaterialObject> immobileObjects = new();

        //each Key is a name of a type that inherits from MaterialObject class.
        //Values are names of types that implement MaterialObject interface
        //If an object of Key type collides with an object contained in Value, collision is ignored completely
        private static readonly Dictionary<string, HashSet<string>> IgnoredCollisions = new()
        {
            { typeof(Bullet).Name, new() { typeof(Bullet).Name, typeof(Player).Name } },
            { typeof(Player).Name, new() { typeof(Bullet).Name, } },
        };

        public void RegisterMobileObject(MaterialObject mobileObject)
        {
            mobileObjects.Add(mobileObject);
        }

        public void UnregisterMobileObject(MaterialObject mobileObject)
        {
            mobileObjects.Remove(mobileObject);
        }

        public void RegisterImmobileObject(MaterialObject immobileObject)
        {
            immobileObjects.Add(immobileObject);
        }

        public void Reset()
        {
            mobileObjects.Clear();
            immobileObjects.Clear();
        }

        /// <summary>
        /// on each trigger, moves all mobile objects in space based on their velocity (first on X, and then on Y axis)
        /// after movement on each axis, checks and handles collisions
        /// </summary>
        public void Run()
        {
            //TODO: iterate only through mobileObjects that have velocity != 0 && !didRotate
            foreach(var currentObject in mobileObjects)
            {
                var potentialCollisions = GetPotentialCollisions(currentObject);
                foreach (var otherObject in potentialCollisions)
                {
                    //get all collisions
                    var collisionResult = collisionsCalculator.CalculateCollision(currentObject, otherObject);
                }
            }
        }

        //collision detection broad phase
        private IEnumerable<MaterialObject> GetPotentialCollisions(MaterialObject currentObject)
        {
            var potentialCollisions = mobileObjects
                .Where(x => x != currentObject)
                .Concat(immobileObjects)
                //since I will be projecting objects, this might break things
                //.Where(x => !DefinitelyDoNotCollide(currentObject, x))
                .Where(x => !IgnoredCollisions.TryGetValue(currentObject.GetType().Name, out var ignoredCollisions) || !ignoredCollisions.Contains(x.GetType().Name));

            return potentialCollisions;
        }

        private static bool DefinitelyDoNotCollide(MaterialObject first, MaterialObject second)
        {
            return
                first.ColliderBounds.MaxX < second.ColliderBounds.MinX ||
                second.ColliderBounds.MaxX < first.ColliderBounds.MinX ||
                first.ColliderBounds.MaxY < second.ColliderBounds.MinY ||
                second.ColliderBounds.MaxY < first.ColliderBounds.MinY;
        }

        public void Visualize()
        {
#if DEBUG
            foreach (var collisionActor in immobileObjects.Concat(mobileObjects))
            {
                VisualizationHelper.VisualizeCollider(collisionActor.ColliderBounds);
            }
#endif
        }
    }
}
