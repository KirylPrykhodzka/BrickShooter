using Microsoft.Xna.Framework;

namespace BrickShooter.Physics.Interfaces
{
    public interface IMaterialObject
    {
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        IColliderPolygon Body { get; }
        bool DidRotate { get; }
    }
}
