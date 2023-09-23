using BrickShooter.Collision;
using Microsoft.Xna.Framework;

namespace BrickShooter.Physics
{
    public record CollisionInfo
    {
        public MobileMaterialObject CollisionSubject { get; set; }
        public IMaterialObject CollisionObject { get; set; }
        public Vector2 MinimumTranslationVector { get; set; }
    }
}
