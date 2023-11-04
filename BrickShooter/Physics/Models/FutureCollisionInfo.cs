using BrickShooter.Physics.Interfaces;
using Microsoft.Xna.Framework;

namespace BrickShooter.Physics.Models
{
    public record struct FutureCollisionInfo
    {
        public IColliderPolygon CollisionObject { get; set; }
        public Vector2 RelativeVelocity { get; set; }
        public bool WillCollide { get; set; }
        public Vector2 ClosestCollisionPoint { get; set; }
        public (Vector2 point1, Vector2 point2) CollisionEdge { get; set; }
        public float DistanceToCollision { get; set; }

        public FutureCollisionInfo(
            IColliderPolygon collisionObject,
            Vector2 relativeVelocity,
            bool willCollide,
            Vector2 closestCollisionPoint,
            (Vector2 point1, Vector2 point2) collisionEdge,
            float distanceToCollision)
        {
            CollisionObject = collisionObject;
            WillCollide = willCollide;
            RelativeVelocity = relativeVelocity;
            ClosestCollisionPoint = closestCollisionPoint;
            CollisionEdge = collisionEdge;
            DistanceToCollision = distanceToCollision;
        }
    }
}
