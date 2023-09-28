using BrickShooter.Configuration;
using BrickShooter.Extensions;
using BrickShooter.Physics.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System.Linq;

namespace BrickShooter.Physics.Models
{
    public abstract class MaterialObject
    {
        protected static readonly IPhysicsSystem physicsSystem;

        static MaterialObject()
        {
            physicsSystem = ServiceProviderFactory.ServiceProvider.GetService<IPhysicsSystem>();
        }

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Rotation { get; set; }
        public bool DidRotate { get; set; }
        public float Bounciness { get; set; }

        protected Vector2[] initialLocalColliderPoints;

        /// <summary>
        /// global collider bounds are affected by local bounds + position and should be recalculated whenever any of these values changes
        /// </summary>
        private (Vector2 Position, ColliderPolygon localPolygon, ColliderPolygon Value) globalColliderPolygonCache = new();
        public ColliderPolygon GlobalColliderPolygon
        {
            get
            {
                var localPolygon = LocalColliderPolygon;
                if (globalColliderPolygonCache.Value == null || Position != globalColliderPolygonCache.Position || localPolygon != globalColliderPolygonCache.localPolygon)
                {
                    var globalPolygon = new ColliderPolygon(localPolygon.Points);
                    globalPolygon.Offset(Position);
                    globalColliderPolygonCache = (Position, localPolygon, globalPolygon);
                }
                return globalColliderPolygonCache.Value;
            }
        }

        /// <summary>
        /// positions of collider points relative to this object's position are at any time determined by this object's rotation, and should be
        /// recalculated whenever rotation changes. If scale parameter is added in the future, it will affect this value as well
        /// </summary>
        /// <returns></returns>
        private (float rotation, ColliderPolygon Value) localColliderPolygonCache = new();
        public ColliderPolygon LocalColliderPolygon
        {
            get
            {
                if (localColliderPolygonCache.Value == null || Rotation != localColliderPolygonCache.rotation)
                {
                    localColliderPolygonCache = (Rotation, new ColliderPolygon(initialLocalColliderPoints.Select(x => x.Rotate(Vector2.Zero, Rotation)).ToList()));
                }
                return localColliderPolygonCache.Value;
            }
        }

        public virtual void OnCollision(MaterialObject otherCollider) { }
    }
}
