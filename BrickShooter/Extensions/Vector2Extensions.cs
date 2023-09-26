using Microsoft.Xna.Framework;
using System;

namespace BrickShooter.Extensions
{
    public static class Vector2Extensions
    {
        public static float DotProduct(this Vector2 thisVector, Vector2 otherVector)
        {
            return thisVector.X * otherVector.X + thisVector.Y * otherVector.Y;
        }
        public static Vector2 Rotate(this Vector2 point, Vector2 origin, float angle)
        {
            double cosTheta = Math.Cos(angle);
            double sinTheta = Math.Sin(angle);
            return new Vector2
            {
                X =
                    (float)
                    (cosTheta * (point.X - origin.X) -
                    sinTheta * (point.Y - origin.Y) + origin.X),
                Y =
                    (float)
                    (sinTheta * (point.X - origin.X) +
                    cosTheta * (point.Y - origin.Y) + origin.Y)
            };
        }
    }
}
