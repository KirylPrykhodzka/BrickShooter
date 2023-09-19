using Microsoft.Xna.Framework;
using System;

namespace BrickShooter.Extensions
{
    public static class PointExtensions
    {
        public static Point Rotate(this Point point, Point origin, float angle)
        {
            double cosTheta = Math.Cos(angle);
            double sinTheta = Math.Sin(angle);
            return new Point
            {
                X =
                    (int)
                    (cosTheta * (point.X - origin.X) -
                    sinTheta * (point.Y - origin.Y) + origin.X),
                Y =
                    (int)
                    (sinTheta * (point.X - origin.X) +
                    cosTheta * (point.Y - origin.Y) + origin.Y)
            };
        }
    }
}
