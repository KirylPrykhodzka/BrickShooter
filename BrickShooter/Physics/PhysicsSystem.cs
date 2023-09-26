using BrickShooter.GameObjects;
using BrickShooter.GameObjects.Bullets;
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
        private readonly ICollisionCalculator collisionCalculator;
        private readonly ICollisionProcessor collisionProcessor;
        private readonly IMaterialObjectMover materialObjectMover;

        public PhysicsSystem(
            IPotentialCollisionsDetector potentialCollisionsDetector,
            ICollisionCalculator collisionCalculator,
            ICollisionProcessor collisionProcessor,
            IMaterialObjectMover materialObjectMover)
        {
            this.potentialCollisionsDetector = potentialCollisionsDetector;
            this.collisionCalculator = collisionCalculator;
            this.collisionProcessor = collisionProcessor;
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

        private HashSet<CollisionPredictionResult> currentObjectPredictedCollisions = new();
        /// <summary>
        /// on each trigger, moves all mobile objects in space based on their velocity (first on X, and then on Y axis)
        /// after movement on each axis, checks and handles collisions
        /// </summary>
        public void Run()
        {
            foreach (var currentObject in mobileObjects.Where(x => x.Velocity != Vector2.Zero || x.DidRotate).ToList())
            {
                currentObjectPredictedCollisions = potentialCollisionsDetector.DetectPotentialCollisions(currentObject, mobileObjects.Where(x => x != currentObject).Concat(immobileObjects))
                    .Select(x => collisionCalculator.CalculateCollision(currentObject, x))
                    .Where(x => x.WillCollide)
                    .ToHashSet();
                if(currentObjectPredictedCollisions.Count == 0)
                {
                    materialObjectMover.MoveWithoutObstruction(currentObject);
                }
                else
                {
                    collisionProcessor.ProcessFutureCollisions(currentObject, currentObjectPredictedCollisions);
                }

                currentObjectPredictedCollisions.Clear();
            }
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
