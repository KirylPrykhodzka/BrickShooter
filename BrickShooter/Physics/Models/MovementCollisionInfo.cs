﻿using BrickShooter.Physics.Interfaces;
using Microsoft.Xna.Framework;

namespace BrickShooter.Physics.Models
{
    /// <summary>
    /// information about the collision which happened as a result of an object moving (non-zero velocity)
    /// </summary>
    public record struct MovementCollisionInfo
    {
        public IColliderPolygon CollisionSubject { get; set; }
        public IColliderPolygon CollisionObject { get; set; }
        public Vector2 RelativeVelocity { get; set; }
        public bool WillCollide { get; set; }
        public Vector2 CollisionPoint { get; set; }
        public Vector2 CollisionEdge { get; set; }
        public Vector2 Normal { get; set; }
        public float DistanceToCollision { get; set; }
    }
}
