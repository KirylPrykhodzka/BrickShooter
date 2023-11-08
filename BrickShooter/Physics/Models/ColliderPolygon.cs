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
        public string CollisionLayer { get; }

        //points of this polygon in world space
        public IList<Vector2> Points
        {
            get
            {
                RecalculateIfNeeded();
                return points;
            }
        }
        private IList<Vector2> points;

        //bounds of this polygon in world space
        public RectangleF Bounds
        {
            get
            {
                RecalculateIfNeeded();
                return bounds;
            }
        }
        private RectangleF bounds;

        //center of this polygon in world space
        public Vector2 Center
        {
            get
            {
                RecalculateIfNeeded();
                return center;
            }
        }
        private Vector2 center;

        //polygon position and rotation should be always equal to owner's position and rotation
        //whenever discrepancy is found, points are recalculated
        private Vector2 position;
        private float rotation;

        //points of the polygon relative to its position
        private readonly IList<Vector2> localPoints;

        public ColliderPolygon(IMaterialObject owner, string collisionLayer, IList<Vector2> localPoints)
        {
            Owner = owner;
            CollisionLayer = collisionLayer;
            this.localPoints = localPoints.ToList();
            position = owner.Position;
            rotation = owner.Rotation;
            Recalculate();
        }

        private void RecalculateIfNeeded()
        {
            if (position != Owner.Position || rotation != Owner.Rotation)
            {
                position = Owner.Position;
                rotation = Owner.Rotation;
                Recalculate();
            }
        }

        private void Recalculate()
        {
            points = localPoints.Select(x => x.Rotate(Vector2.Zero, rotation) + position).ToList();

            center = new Vector2(points.Average(x => x.X), points.Average(x => x.Y));

            var maxX = points.Max(x => x.X);
            var minX = points.Min(x => x.X);
            var maxY = points.Max(x => x.Y);
            var minY = points.Min(x => x.Y);
            bounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }
    }
}
