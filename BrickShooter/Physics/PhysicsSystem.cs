using BrickShooter.Helpers;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
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
        private readonly ICollisionCalculator collisionCalculator;
        private readonly IMaterialObjectMover materialObjectMover;

        public PhysicsSystem(
            IPotentialCollisionsDetector potentialCollisionsDetector,
            ICollisionCalculator collisionCalculator,
            IMaterialObjectMover materialObjectMover)
        {
            this.potentialCollisionsDetector = potentialCollisionsDetector;
            this.collisionCalculator = collisionCalculator;
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
            foreach (var currentObject in mobileObjects.Where(x => x.Velocity != Vector2.Zero || x.DidRotate).ToList())
            {
                var potentialCollisions = potentialCollisionsDetector.DetectPotentialCollisions(currentObject, mobileObjects.Where(x => x != currentObject).Concat(immobileObjects));
                if(!potentialCollisions.Any())
                {
                    materialObjectMover.MoveWithoutObstruction(currentObject);
                    continue;
                }
                if (currentObject.DidRotate)
                {
                    var existingCollisions = collisionCalculator.GetExistingCollisions(currentObject, potentialCollisions);
                    materialObjectMover.ProcessExistingCollisions(currentObject, existingCollisions);
                }
                if (currentObject.Velocity != Vector2.Zero)
                {
                    var nextCollision = collisionCalculator.FindNextCollision(currentObject, potentialCollisions);
                    if (nextCollision is null)
                    {
                        materialObjectMover.MoveWithoutObstruction(currentObject);
                    }
                    else
                    {
                        materialObjectMover.ProcessNextCollision(currentObject, nextCollision);
                    }
                }
            }
        }

        public void Visualize()
        {
#if DEBUG
            foreach (var collisionActor in immobileObjects.Concat(mobileObjects))
            {
                VisualizationHelper.VisualizeCollider(collisionActor.GlobalColliderPolygon);
            }
#endif
        }
    }
}
