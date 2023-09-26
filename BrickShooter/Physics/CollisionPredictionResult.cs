using Microsoft.Xna.Framework;

namespace BrickShooter.Physics
{
    public record CollisionPredictionResult
    {
        public MaterialObject CollisionSubject { get; set; }
        public MaterialObject CollisionObject { get; set; }
        public Vector2 RelativeVelocity { get; set; }
        public bool WillCollide { get; set; }
        public Vector2 DistanceToCollision { get; set; }
        public Point[] PointsOfContact { get; set; }
        public Vector2[] ContactEdges { get; set; }
    }
}
