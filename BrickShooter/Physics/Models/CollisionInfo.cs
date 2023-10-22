using Microsoft.Xna.Framework;

namespace BrickShooter.Physics.Models
{
    public record struct CollisionInfo
    {
        public bool IsColliding { get; set; }
        public Vector2 MinimalTranslationVector { get; set; }

        public CollisionInfo(bool isColliding, Vector2 minimalTranslationVector)
        {
            IsColliding = isColliding;
            MinimalTranslationVector = minimalTranslationVector;
        }
    }
}
