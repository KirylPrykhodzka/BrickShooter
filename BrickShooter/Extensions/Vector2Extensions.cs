using Microsoft.Xna.Framework;
using System;

namespace BrickShooter.Extensions
{
    public static class Vector2Extensions
    {
        public static float DotProduct(this Vector2 thisVector, Vector2 otherVector)
        {
            return (float)Math.Round(thisVector.X * otherVector.X + thisVector.Y * otherVector.Y, 2);
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

        public static float Magnitude(this Vector2 vector) => (float)Math.Round(Math.Sqrt(Math.Pow(vector.X, 2) + Math.Pow(vector.Y, 2)), 2);

        public static Vector2 Project(this Vector2 point, Vector2 axis)
        {
            var dotProduct = point.DotProduct(axis);
            var magnitude = axis.Magnitude();
            var result = axis * dotProduct / (float)Math.Pow(magnitude, 2);
            return result;
        }

        //https://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line
        public static float DistanceTo(this Vector2 point, (Vector2 point1, Vector2 point2) line)
        {
            var doubleResult =
                Math.Abs(
                    (line.point2.X - line.point1.X) * (line.point1.Y - point.Y) - (line.point1.X - point.X) * (line.point2.Y - line.point1.Y)
                ) /
                Math.Sqrt(
                    Math.Pow(line.point2.X - line.point1.X, 2) + Math.Pow(line.point2.Y - line.point1.Y, 2)
                );

            return (float)doubleResult;
        }
    }
}
