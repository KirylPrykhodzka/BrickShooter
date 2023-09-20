using BrickShooter.Collision;
using BrickShooter.Physics;
using Microsoft.Xna.Framework;

namespace BrickShooter.GameObjects
{
    public class Bullet : IMobileMaterialObject
    {
        public Point Position { get; set; }
        public Vector2 Velocity { get; private set; }

        public ColliderPolygon ColliderBounds => new ColliderPolygon();

        public void OnCollision(IMaterialObject collisionActor) { }
    }
}
