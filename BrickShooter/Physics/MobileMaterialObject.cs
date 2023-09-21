using BrickShooter.Collision;
using BrickShooter.Extensions;
using Microsoft.Xna.Framework;
using System.Linq;

namespace BrickShooter.Physics
{
    public abstract class MobileMaterialObject : IMaterialObject
    {
        public ColliderPolygon ColliderBounds => GetGlobalColliderBounds();
        public Point Position { get; set; }
        public Vector2 Velocity { get; protected set; }
        protected Point[] localColliderBounds;
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
            var center = Position;
            result.Points.AddRange(localColliderBounds.Select(x => GetGlobalPosition(x)));
            result.BuildEdges();
            return result;

            Vector2 GetGlobalPosition(Point localPoint)
            {
                //get global position
                Point globalPosition = Position + localPoint;
                //rotate collider
                return globalPosition.Rotate(Position, rotation).ToVector2();
            }
        }
    }
}
