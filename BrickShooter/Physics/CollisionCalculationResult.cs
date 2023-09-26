using Microsoft.Xna.Framework;

namespace BrickShooter.Physics
{
    public record CollisionCalculationResult
    {
        public MaterialObject CollisionSubject { get; set; }
        public MaterialObject CollisionObject { get; set; }
        public bool Collides { get; set; }
        public bool WillCollide { get; set; }
        public Vector2 RelativeVelocity { get; set; }
        public Point ClosestCollisionPoint { get; set; }
        public Vector2 ClosestCollisionEdge { get; set; }
    }
}
