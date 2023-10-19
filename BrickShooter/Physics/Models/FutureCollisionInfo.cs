using BrickShooter.Resources;
using Microsoft.Xna.Framework;

namespace BrickShooter.Physics.Models
{
    public record FutureCollisionInfo : IResetable
    {
        public MaterialObject CollisionSubject { get; set; }
        public MaterialObject CollisionObject { get; set; }
        public Vector2 RelativeVelocity { get; set; }
        public bool WillCollide { get; set; }
        public Vector2 ClosestCollisionPoint { get; set; }
        public (Vector2 point1, Vector2 point2) CollisionEdge { get; set; }
        //if WillCollide, shows distance still left between CollisionSubject and CollisionObject
        //if IsColliding, shows penetration depth
        public float DistanceToCollision { get; set; }

        public void Reset()
        {
            CollisionSubject = null;
            CollisionObject = null;
            RelativeVelocity = Vector2.Zero;
            WillCollide = false;
            ClosestCollisionPoint = Vector2.Zero;
            CollisionEdge = (Vector2.Zero, Vector2.Zero);
            DistanceToCollision = 0f;
        }
    }
}
