using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BrickShooter.Physics.Models
{
    public class ColliderPolygon
    {
        public List<Vector2> Points { get; set; }

        public float MaxX => Points.Max(x => x.X);
        public float MinX => Points.Min(x => x.X);
        public float MaxY => Points.Max(x => x.Y);
        public float MinY => Points.Min(x => x.Y);

        public Vector2 Center
        {
            get
            {
                return new Vector2(Points.Average(p => p.X), Points.Average(p => p.Y));
            }
        }

        public void SetPoints(IEnumerable<Vector2> points)
        {
            Points = points.ToList();
        }

        public void Offset(Vector2 v)
        {
            Offset(v.X, v.Y);
        }

        public void Offset(float x, float y)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                Vector2 p = Points[i];
                Points[i] = new Vector2(p.X + x, p.Y + y);
            }
        }
    }
}
