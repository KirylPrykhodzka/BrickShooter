using Microsoft.Xna.Framework;

namespace BrickShooter.Physics.Models
{
    public class CollisionInfo
    {
        public MaterialObject CollisionSubject { get; set; }
        public MaterialObject CollisionObject { get; set; }
        public bool IsColliding { get; set; }
        public Vector2 MinimalTranslationVector { get; set; }
    }
}
