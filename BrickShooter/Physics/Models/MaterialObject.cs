using BrickShooter.Extensions;
using Microsoft.Xna.Framework;
using System.Linq;

namespace BrickShooter.Physics.Models
{
    public abstract class MaterialObject
    {
        public string CollisionLayer { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Rotation { get; set; }
        public bool DidRotate { get; set; }
        public float Bounciness { get; set; }

        protected Vector2[] initialColliderPoints;

        /// <summary>
        /// global collider bounds are affected by Position + Rotation and should be recalculated whenever any of these values changes
        /// </summary>
        private (Vector2 Position, float Rotation, ColliderPolygon Value) colliderPolygonCache = new();
        public ColliderPolygon ColliderPolygon
        {
            get
            {
                if(colliderPolygonCache.Value == null)
                {
                    var globalPolygon = new ColliderPolygon();
                    globalPolygon.SetPoints(initialColliderPoints.Select(x => x.Rotate(Vector2.Zero, Rotation) + Position));
                    colliderPolygonCache = (Position, Rotation, globalPolygon);
                }
                if (Position != colliderPolygonCache.Position || Rotation != colliderPolygonCache.Rotation)
                {
                    colliderPolygonCache.Value.SetPoints(initialColliderPoints.Select(x => x.Rotate(Vector2.Zero, Rotation) + Position));
                    colliderPolygonCache.Position = Position;
                }
                return colliderPolygonCache.Value;
            }
        }

        public virtual void OnCollision(MaterialObject otherCollider) { }
    }
}
