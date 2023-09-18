using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace BrickShooter.GameObjects
{
    public abstract class GameObject : ICollisionActor
    {
        public IShapeF Bounds { get; protected set; }
        protected Point currentPosition;
        protected float rotation;
        protected Vector2 velocity;

        public virtual void OnCollision(CollisionEventArgs collisionInfo) { }
    }
}
