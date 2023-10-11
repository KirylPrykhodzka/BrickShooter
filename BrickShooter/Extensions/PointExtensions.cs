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

            // Calculate the new coordinates with double precision
            double newX = cosTheta * (point.X - origin.X) - sinTheta * (point.Y - origin.Y) + origin.X;
            double newY = sinTheta * (point.X - origin.X) + cosTheta * (point.Y - origin.Y) + origin.Y;

            // Round to integers and create a new Point
            return new Point((int)Math.Round(newX), (int)Math.Round(newY));
        }
    }
}
