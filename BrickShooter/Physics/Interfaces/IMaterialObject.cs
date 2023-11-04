using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BrickShooter.Physics.Interfaces
{
    public interface IMaterialObject
    {
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        IColliderPolygon SingleCollider { get; }
        IList<IColliderPolygon> Colliders { get; }
        bool DidRotate { get; }
    }
}
