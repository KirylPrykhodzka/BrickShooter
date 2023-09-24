using BrickShooter.Collision;
using BrickShooter.Extensions;
using Microsoft.Xna.Framework;
using System.Linq;

namespace BrickShooter.Physics
{
    public abstract class MobileMaterialObject : IMaterialObject
    {
        public ColliderPolygon ColliderBounds => GetGlobalColliderBounds();
        public virtual float Bounciness => 0f;
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        protected Vector2[] localColliderBounds;
        protected float rotation;

        public virtual void OnCollision(IMaterialObject otherCollider) { }

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
                return globalPosition.ToPoint().Rotate(Position.ToPoint(), rotation).ToVector2();
            }
        }
    }
}
