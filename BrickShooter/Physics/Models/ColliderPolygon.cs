using BrickShooter.Extensions;
using BrickShooter.Physics.Interfaces;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Linq;

namespace BrickShooter.Physics.Models
{
    public class ColliderPolygon : IColliderPolygon
    {
        public IMaterialObject Owner { get; }
        public string CollisionLayer { get; private set; }
        public IList<Vector2> Points => GetPoints();
        public RectangleF Bounds { get; private set; }
        public Vector2 Center { get; private set; }

        //points of this polygon in world space
        private IList<Vector2> points;
        //polygon position and rotation should be always equal to owner's position and rotation
        //whenever discrepancy is found, points are recalculated
        private Vector2 position;
        private float rotation;
        //points of the polygon relative to its position
        private IList<Vector2> localPoints;

        public ColliderPolygon(IMaterialObject owner, string collisionLayer, IList<Vector2> localPoints)
        {
            Owner = owner;
            CollisionLayer = collisionLayer;
            this.localPoints = localPoints;
            position = owner.Position;
            rotation = owner.Rotation;
            RecalculatePoints();
        }

        private IList<Vector2> GetPoints()
        {
            if(position != Owner.Position || rotation != Owner.Rotation)
            {
                position = Owner.Position;
                rotation = Owner.Rotation;
                RecalculatePoints();
            }

            return points;
        }

        private void RecalculatePoints()
        {
            points = localPoints.Select(x => x.Rotate(Vector2.Zero, rotation) + position).ToList();
            var maxX = points.Max(x => x.X);
            var minX = points.Min(x => x.X);
            var maxY = points.Max(x => x.Y);
            var minY = points.Min(x => x.Y);
            Bounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);
            Center = new Vector2(points.Average(x => x.X), points.Average(x => x.Y));
        }
    }
}
