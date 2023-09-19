using Microsoft.Xna.Framework;
using System;

namespace BrickShooter.Collision
{
    public interface ICollisionSubject : ICollisionActor
    {
        Point Position { get; set; }
        Vector2 Velocity { get; set; }
        void OnCollision(ICollisionActor collisionActor);
    }
}
