using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BrickShooter.Physics.Models
{
    public class ColliderPolygon
    {
        public List<Vector2> Points => points.ToList();

        private List<Vector2> points;

        public float MaxX { get; private set; }
        public float MinX { get; private set; }
        public float MaxY { get; private set; }
        public float MinY { get; private set; }
        public Vector2 Center { get; private set; }

        public void SetPoints(IEnumerable<Vector2> points)
        {
            this.points = points.ToList();
            MaxX = points.Max(x => x.X);
            MinX = points.Min(x => x.X);
            MaxY = points.Max(x => x.Y);
            MinY = points.Min(x => x.Y);
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
