using Microsoft.Xna.Framework;
using System;

namespace BrickShooter.Collision
{
    public interface ICollisionSubject : ICollisionActor
    {
        Vector2 Velocity { get; set; }
        void OnCollision(ICollisionActor collisionActor);
    }
}
