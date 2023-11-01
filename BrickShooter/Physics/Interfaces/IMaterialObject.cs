using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;

namespace BrickShooter.Physics.Interfaces
{
    public interface IMaterialObject
    {
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        ColliderPolygon ColliderPolygon { get; }
        string CollisionLayer { get; }
        bool DidRotate { get; }
    }
}
