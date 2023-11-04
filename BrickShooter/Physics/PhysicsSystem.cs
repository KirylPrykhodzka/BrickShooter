using BrickShooter.Constants;
using BrickShooter.Helpers;
using BrickShooter.Physics.Interfaces;
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
        private readonly IPotentialCollisionsDetector potentialCollisionsDetector;
        private readonly IExistingCollisionsCalculator existingCollisionsCalculator;
        private readonly IFutureCollisionsCalculator futureCollisionsCalculator;
        private readonly ICollisionProcessor collisionProcessor;
        private readonly IMaterialObjectMover materialObjectMover;

        public PhysicsSystem(
            IPotentialCollisionsDetector potentialCollisionsDetector,
            IExistingCollisionsCalculator existingCollisionsCalculator,
            IFutureCollisionsCalculator futureCollisionsCalculator,
            ICollisionProcessor collisionProcessor,
            IMaterialObjectMover materialObjectMover)
        {
            this.potentialCollisionsDetector = potentialCollisionsDetector;
            this.existingCollisionsCalculator = existingCollisionsCalculator;
            this.futureCollisionsCalculator = futureCollisionsCalculator;
            this.collisionProcessor = collisionProcessor;
            this.materialObjectMover = materialObjectMover;
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
                    var existingCollisions = existingCollisionsCalculator.GetExistingCollisions(currentObject, potentialCollisions.Existing);
                    if (existingCollisions.Count > 0)
                    {
                        collisionProcessor.ProcessExistingCollisions(currentObject, existingCollisions);
                    }
                }
                if(currentObject.Velocity.Length() >= PhysicsConstants.MIN_VELOCITY)
                {
                    collisionProcessor.FindAndProcessNextCollisions(currentObject, potentialCollisions.Future);
                }
            }
            materialObjectMover.ApplyScheduledMovements();
        }

        public void Visualize()
        {
#if DEBUG
            foreach (var collisionActor in immobileObjects.Concat(mobileObjects))
            {
                VisualizationHelper.VisualizeCollider(collisionActor.SingleCollider);
            }
#endif
        }
    }
}
