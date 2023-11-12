using Microsoft.Xna.Framework;

namespace BrickShooter.Physics.Models
{
    public record struct FutureCollisionInfo
    {
        public CollisionPair CollisionPair { get; set; }
        public Vector2 RelativeVelocity { get; set; }
        public bool WillCollide { get; set; }
        public Vector2 ClosestCollisionPoint { get; set; }
        public Vector2 CollisionEdge { get; set; }
        public float DistanceToCollision { get; set; }
    }
}
