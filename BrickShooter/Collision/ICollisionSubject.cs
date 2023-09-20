using Microsoft.Xna.Framework;

namespace BrickShooter.Collision
{
    public interface ICollisionSubject : ICollisionActor
    {
        Point Position { get; set; }
        Vector2 Velocity { get; set; }
        void OnCollision(ICollisionActor collisionActor);
    }
}
