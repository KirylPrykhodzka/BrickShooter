using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Collections.Generic;

namespace BrickShooter.Physics.Interfaces
{
    public interface IColliderPolygon
    {
        IMaterialObject Owner { get; }
        string CollisionLayer { get; }
        IList<Vector2> Points { get; }
        Vector2 Center { get; }
        void SetPoints(IEnumerable<Vector2> points);
        RectangleF Bounds { get; }
    }
}
