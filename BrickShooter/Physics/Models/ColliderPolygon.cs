using BrickShooter.Physics.Interfaces;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Linq;

namespace BrickShooter.Physics.Models
{
    public class ColliderPolygon : IColliderPolygon
    {
        public string CollisionLayer { get; private set; }
        public IList<Vector2> Points => points;

        private IList<Vector2> points;
        public RectangleF Bounds { get; private set; }
        public Vector2 Center { get; private set; }

        public ColliderPolygon(string collisionLayer)
        {
            CollisionLayer = collisionLayer;
        }


        public void SetPoints(IEnumerable<Vector2> points)
        {
            this.points = points.ToList();
            var maxX = points.Max(x => x.X);
            var minX = points.Min(x => x.X);
            var maxY = points.Max(x => x.Y);
            var minY = points.Min(x => x.Y);
            Bounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);
            Center = new Vector2(points.Average(x => x.X), points.Average(x => x.Y));
        }

        public void Offset(Vector2 v)
        {
            Offset(v.X, v.Y);
        }

        public void Offset(float x, float y)
        {
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 p = points[i];
                points[i] = new Vector2(p.X + x, p.Y + y);
            }
        }
    }
}
