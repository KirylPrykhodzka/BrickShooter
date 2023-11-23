using BrickShooter.Physics.Interfaces;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BrickShooter.Physics.Models
{
    public abstract class MaterialObject : IMaterialObject
    {
        public IColliderPolygon SingleCollider => Colliders.Count == 1 ? Colliders[0] : null;
        public IList<IColliderPolygon> Colliders { get; } = new List<IColliderPolygon>();
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Rotation { get; set; }
        public bool DidRotate { get; set; }
        public float Bounciness { get; set; }

        public virtual void OnMovementCollision(MovementCollisionInfo collisionInfo) { }
    }
}
