using BrickShooter.Configuration;
using BrickShooter.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System.Linq;

namespace BrickShooter.Physics
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
        public float Bounciness { get; set; }
        public ColliderPolygon ColliderBounds => GetGlobalColliderBounds();
        protected Vector2[] localColliderBounds;

        public virtual void OnCollision(MaterialObject otherCollider) { }

        /// <summary>
        /// ColliderBounds depend on position, rotation and scale of the object
        /// To avoid re-calculating them upon each update of position / rotation / scale,
        /// calculation logic is placed here to be called when needed
        /// </summary>
        /// <returns></returns>
        private ColliderPolygon GetGlobalColliderBounds()
        {
            var result = new ColliderPolygon();
            result.Points.AddRange(localColliderBounds.Select(x => GetGlobalPosition(x)));
            result.BuildEdges();
            return result;

            Vector2 GetGlobalPosition(Vector2 localPoint)
            {
                //get global position
                Vector2 globalPosition = Position + localPoint;
                //rotate collider
                return globalPosition.ToPoint().Rotate(Position.ToPoint(), Rotation).ToVector2();
            }
        }
    }
}
