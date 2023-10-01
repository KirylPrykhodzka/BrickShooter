using Microsoft.Xna.Framework;

namespace BrickShooter.Physics.Models
{
    public record CollisionCalculationResult
    {
        public MaterialObject CollisionSubject { get; set; }
        public MaterialObject CollisionObject { get; set; }
        public Vector2 RelativeVelocity { get; set; }
        public bool Collides { get; set; }
        public bool WillCollide { get; set; }
        public Vector2 ClosestCollisionPoint { get; set; }
        public Vector2 CollisionEdge { get; set; }
    }
}
