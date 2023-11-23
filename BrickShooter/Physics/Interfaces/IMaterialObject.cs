using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BrickShooter.Physics.Interfaces
{
    public interface IMaterialObject
    {
        Vector2 Position { get; set; }
        float Rotation { get; }
        Vector2 Velocity { get; set; }
        float Bounciness { get; set; }
        IColliderPolygon SingleCollider { get; }
        IList<IColliderPolygon> Colliders { get; }
        void OnMovementCollision(MovementCollisionInfo collisionInfo);
        bool DidRotate { get; }
    }
}
