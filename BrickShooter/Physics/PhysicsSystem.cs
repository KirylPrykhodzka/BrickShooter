using BrickShooter.Constants;
using BrickShooter.Helpers;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using BrickShooter.Resources;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IPotentialCollisionsDetector potentialCollisionsDetector;
        private readonly IExistingCollisionsCalculator existingCollisionsCalculator;
        private readonly ICollisionProcessor collisionProcessor;
        private readonly IMaterialObjectMover materialObjectMover;
        private readonly IPool<CollisionPair> collisionPairPool;

        public PhysicsSystem(
            IPotentialCollisionsDetector potentialCollisionsDetector,
            IExistingCollisionsCalculator existingCollisionsCalculator,
            ICollisionProcessor collisionProcessor,
            IMaterialObjectMover materialObjectMover,
            IPool<CollisionPair> collisionPairPool)
        {
            this.potentialCollisionsDetector = potentialCollisionsDetector;
            this.existingCollisionsCalculator = existingCollisionsCalculator;
            this.collisionProcessor = collisionProcessor;
            this.materialObjectMover = materialObjectMover;
            this.collisionPairPool = collisionPairPool;
        }

        //all objects that can be repositioned in space based on their velocity and initiate collisions
        private readonly HashSet<IMaterialObject> mobileObjects = new();
        //objects that mobile objects can collide with
        private readonly HashSet<IMaterialObject> immobileObjects = new();

        public void RegisterMobileObject(IMaterialObject mobileObject)
        {
            mobileObjects.Add(mobileObject);
        }

        public void UnregisterMobileObject(IMaterialObject mobileObject)
        {
            mobileObjects.Remove(mobileObject);
        }

        public void RegisterImmobileObject(IMaterialObject immobileObject)
        {
            immobileObjects.Add(immobileObject);
        }

        public void UnregisterImmobileObject(IMaterialObject immobileObject)
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
            foreach (var currentObject in mobileObjects.Where(x => x.Velocity.Length() >= PhysicsConstants.MIN_VELOCITY || x.DidRotate))
            {
                var potentialCollisions = potentialCollisionsDetector.GetPotentialCollisions(currentObject, mobileObjects.Concat(immobileObjects));
                if (currentObject.DidRotate && potentialCollisions.Existing.Count > 0)
                {
                    var existingCollisions = existingCollisionsCalculator.GetExistingCollisions(potentialCollisions.Existing);
                    if (existingCollisions.Count > 0)
                    {
                        collisionProcessor.ProcessExistingCollisions(currentObject, existingCollisions);
                    }
                }
                if(currentObject.Velocity.Length() >= PhysicsConstants.MIN_VELOCITY)
                {
                    collisionProcessor.FindAndProcessNextCollisions(currentObject, potentialCollisions.Future);
                }
                collisionPairPool.Return(potentialCollisions.Existing.Concat(potentialCollisions.Future));
            }
            materialObjectMover.ApplyScheduledMovements();
        }

        public void Visualize()
        {
#if DEBUG
            foreach (var collider in immobileObjects.Concat(mobileObjects).SelectMany(x => x.Colliders))
            {
                VisualizationHelper.VisualizeCollider(collider);
            }
#endif
        }
    }
}
