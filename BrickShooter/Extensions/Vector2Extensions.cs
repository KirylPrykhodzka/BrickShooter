using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;

namespace BrickShooter.Extensions
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// rotates a point around a specified origin by a provided value
        /// </summary>
        /// <param name="point"></param>
        /// <param name="origin"></param>
        /// <param name="angleInRadians"></param>
        /// <returns></returns>
        public static Vector2 Rotate(this Vector2 point, Vector2 origin, double angleInRadians)
        {
            // Translate the point to the origin
            double translatedX = point.X - origin.X;
            double translatedY = point.Y - origin.Y;

            // Perform the rotation using the rotation matrix
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);

            var rotatedX = cosTheta * translatedX - sinTheta * translatedY;
            var rotatedY = sinTheta * translatedX + cosTheta * translatedY;

            // Translate the rotated point back to its original position
            var finalX = rotatedX + origin.X;
            var finalY = origin.Y + rotatedY;

            return new Vector2((float)finalX, (float)finalY);
        }

        public static Vector2 Project(this Vector2 vector, Vector2 axis)
        {
            float dotProduct = vector.Dot(axis);
            float magnitudeSquared = axis.LengthSquared();

            return axis * (dotProduct / magnitudeSquared);
        }
    }
}
