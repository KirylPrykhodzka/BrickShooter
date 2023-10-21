using BrickShooter.Helpers;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IMaterialObjectMover materialObjectMover;

        public PhysicsSystem(
            IPotentialCollisionsDetector potentialCollisionsDetector,
            IExistingCollisionsCalculator existingCollisionsCalculator,
            IFutureCollisionsCalculator futureCollisionsCalculator,
            IMaterialObjectMover materialObjectMover)
        {
            this.potentialCollisionsDetector = potentialCollisionsDetector;
            this.existingCollisionsCalculator = existingCollisionsCalculator;
            this.futureCollisionsCalculator = futureCollisionsCalculator;
            this.materialObjectMover = materialObjectMover;
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
                var potentialCollisions = potentialCollisionsDetector.DetectPotentialCollisions(currentObject, mobileObjects.Where(x => x != currentObject).Concat(immobileObjects));
                if (potentialCollisions.Count == 0)
                {
                    materialObjectMover.MoveWithoutObstruction(currentObject);
                    continue;
                }
                if(currentObject.DidRotate)
                {
                    var existingCollisions = existingCollisionsCalculator.GetExistingCollisions(currentObject, potentialCollisions);
                    if (existingCollisions.Count > 0)
                    {
                        materialObjectMover.ProcessExistingCollisions(currentObject, existingCollisions);
                    }
                }
                if (currentObject.Velocity != Vector2.Zero)
                {
                    var before = GC.GetTotalAllocatedBytes(true);
                    var nextCollisions = futureCollisionsCalculator.FindNextCollisions(currentObject, potentialCollisions);
                    var after = GC.GetTotalAllocatedBytes(true);
                    Debug.WriteLine(after - before);
                    materialObjectMover.ProcessNextCollisions(currentObject, nextCollisions);
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
