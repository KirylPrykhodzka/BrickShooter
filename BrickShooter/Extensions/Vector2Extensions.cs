﻿using Microsoft.Xna.Framework;
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

        public static float Magnitude(this Vector2 vector) => (float)Math.Sqrt((Math.Pow(vector.X, 2) + Math.Pow(vector.Y, 2)));

        public static Vector2 Project(this Vector2 point, Vector2 axis)
        {
            var dotProduct = point.DotProduct(axis);
            var magnitude = axis.Magnitude();
            var result = axis * dotProduct / (float)Math.Pow(magnitude, 2);
            return result;
        }
    }
}
