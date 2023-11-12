using BrickShooter.Physics.Interfaces;
using Microsoft.Xna.Framework;

namespace BrickShooter.Physics.Models
{
    /// <summary>
    /// information about the collision which happened as a result of an object rotating
    /// </summary>
    public record struct RotationCollisionInfo
    {
        public IColliderPolygon CollisionObject { get; set; }
        public bool IsColliding { get; set; }
        public Vector2 MinimalTranslationVector { get; set; }
    }
}
