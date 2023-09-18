using System.Numerics;

namespace BrickShooter.Collision
{
    public interface ICollisionActor
    {
        ColliderPolygon ColliderBounds { get; }
    }
}
