using BrickShooter.Collision;
using Microsoft.Xna.Framework;

namespace BrickShooter.Physics
{
    public interface IMobileMaterialObject : IMaterialObject
    {
        Point Position { get; set; }
        Vector2 Velocity { get; }
        void OnCollision(IMaterialObject collisionActor);
    }
}
