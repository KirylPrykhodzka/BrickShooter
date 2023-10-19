using BrickShooter.Resources;
using Microsoft.Xna.Framework;

namespace BrickShooter.Physics.Models
{
    public class CollisionInfo : IResetable
    {
        public MaterialObject CollisionSubject { get; set; }
        public MaterialObject CollisionObject { get; set; }
        public bool IsColliding { get; set; }
        public Vector2 MinimalTranslationVector { get; set; }

        public void Reset()
        {
            CollisionSubject = null;
            CollisionObject = null;
            IsColliding = false;
            MinimalTranslationVector = Vector2.Zero;
        }
    }
}
