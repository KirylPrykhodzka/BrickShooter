using BrickShooter.Helpers;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using BrickShooter.Resources;
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
        private readonly IMaterialObjectMover materialObjectMover;
        private readonly IPool<CollisionInfo> collisionInfoPool;
        private readonly IPool<FutureCollisionInfo> futureCollisionInfoPool;

        public PhysicsSystem(
            IPotentialCollisionsDetector potentialCollisionsDetector,
            IExistingCollisionsCalculator existingCollisionsCalculator,
            IFutureCollisionsCalculator futureCollisionsCalculator,
            IMaterialObjectMover materialObjectMover,
            IPool<CollisionInfo> collisionInfoPool,
            IPool<FutureCollisionInfo> futureCollisionInfoPool)
        {
            this.potentialCollisionsDetector = potentialCollisionsDetector;
            this.existingCollisionsCalculator = existingCollisionsCalculator;
            this.futureCollisionsCalculator = futureCollisionsCalculator;
            this.materialObjectMover = materialObjectMover;
            this.collisionInfoPool = collisionInfoPool;
            this.futureCollisionInfoPool = futureCollisionInfoPool;
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
                if(!potentialCollisions.Any())
                {
                    materialObjectMover.MoveWithoutObstruction(currentObject);
                    continue;
                }
                if(currentObject.DidRotate)
                {
                    var existingCollisions = existingCollisionsCalculator.GetExistingCollisions(currentObject, potentialCollisions);
                    if (existingCollisions.Any())
                    {
                        materialObjectMover.ProcessExistingCollisions(currentObject, existingCollisions);
                    }
                    foreach(var existingCollision in existingCollisions)
                    {
                        collisionInfoPool.Return(existingCollision);
                    }
                }
                if (currentObject.Velocity != Vector2.Zero)
                {
                    var nextCollisions = futureCollisionsCalculator.FindNextCollisions(currentObject, potentialCollisions);
                    materialObjectMover.ProcessNextCollisions(currentObject, nextCollisions);
                    foreach(var nextCollision in nextCollisions)
                    {
                        futureCollisionInfoPool.Return(nextCollision);
                    }
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
