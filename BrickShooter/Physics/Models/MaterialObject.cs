using BrickShooter.Extensions;
using BrickShooter.Physics.Interfaces;
using Microsoft.Xna.Framework;
using System.Linq;

namespace BrickShooter.Physics.Models
{
    public abstract class MaterialObject : IMaterialObject
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Rotation { get; set; }
        public bool DidRotate { get; set; }
        public float Bounciness { get; set; }

        protected Vector2[] initialColliderPoints;

        /// <summary>
        /// global collider bounds are affected by Position + Rotation and should be recalculated whenever any of these values changes
        /// </summary>
        private (Vector2 Position, float Rotation, ColliderPolygon Value) bodyColliderPolygonCache = new();
        public IColliderPolygon Body
        {
            get
            {
                if(bodyColliderPolygonCache.Value == null)
                {
                    var globalPolygon = new ColliderPolygon(GetType().Name);
                    globalPolygon.SetPoints(initialColliderPoints.Select(x => x.Rotate(Vector2.Zero, Rotation) + Position));
                    bodyColliderPolygonCache = (Position, Rotation, globalPolygon);
                }
                if (Position != bodyColliderPolygonCache.Position || Rotation != bodyColliderPolygonCache.Rotation)
                {
                    bodyColliderPolygonCache.Value.SetPoints(initialColliderPoints.Select(x => x.Rotate(Vector2.Zero, Rotation) + Position));
                    bodyColliderPolygonCache.Position = Position;
                }
                return bodyColliderPolygonCache.Value;
            }
        }

        public virtual void OnCollision(IColliderPolygon otherCollider) { }
    }
}
