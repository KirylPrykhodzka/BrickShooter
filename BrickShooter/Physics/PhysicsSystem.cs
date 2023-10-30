using BrickShooter.Helpers;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace BrickShooter.Physics
{
    /// <summary>
    /// moves objects around in accordance with their velocity;
    /// finds all collisions between game objects.
    /// Should be reset upon reloading the level
    /// </summary>
    public class PhysicsSystem : IPhysicsSystem
    {
        private readonly IPotentialCollisionsDetector potentialCollisionsDetector;
        private readonly IExistingCollisionsCalculator existingCollisionsCalculator;
        private readonly IFutureCollisionsCalculator futureCollisionsCalculator;
        private readonly ICollisionProcessor collisionProcessor;

        public PhysicsSystem(
            IPotentialCollisionsDetector potentialCollisionsDetector,
            IExistingCollisionsCalculator existingCollisionsCalculator,
            IFutureCollisionsCalculator futureCollisionsCalculator,
            ICollisionProcessor collisionProcessor)
        {
            this.potentialCollisionsDetector = potentialCollisionsDetector;
            this.existingCollisionsCalculator = existingCollisionsCalculator;
            this.futureCollisionsCalculator = futureCollisionsCalculator;
            this.collisionProcessor = collisionProcessor;
        }

        //all objects that can be repositioned in space based on their velocity and initiate collisions
        private readonly HashSet<MaterialObject> mobileObjects = new();
        //objects that mobile objects can collide with
        private readonly HashSet<MaterialObject> immobileObjects = new();

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

        public void UnregisterImmobileObject(MaterialObject immobileObject)
        {
            immobileObjects.Remove(immobileObject);
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
            foreach (var currentObject in mobileObjects.Where(x => x.Velocity != Vector2.Zero || x.DidRotate))
            {
                var (potentialExistingCollisions, potentialFutureCollisions) = potentialCollisionsDetector.GetPotentialCollisions(currentObject, mobileObjects.Concat(immobileObjects));
                if (currentObject.DidRotate && potentialExistingCollisions.Count > 0)
                {
                    var existingCollisions = existingCollisionsCalculator.GetExistingCollisions(currentObject, potentialExistingCollisions);
                    if (existingCollisions.Count > 0)
                    {
                        collisionProcessor.ProcessExistingCollisions(currentObject, existingCollisions);
                    }
                }
                if(currentObject.Velocity != Vector2.Zero)
                {
                    var nextCollisions = futureCollisionsCalculator.FindNextCollisions(currentObject, potentialFutureCollisions);
                    collisionProcessor.ProcessNextCollisions(currentObject, nextCollisions);
                }
            }
        }

        public void Visualize()
        {
#if DEBUG
            foreach (var collisionActor in immobileObjects.Concat(mobileObjects))
            {
                VisualizationHelper.VisualizeCollider(collisionActor.ColliderPolygon);
            }
#endif
        }
    }
}
