using Microsoft.Xna.Framework;
using System;

namespace BrickShooter.Collision
{
    public static class Vector2Extensions
    {
        public static float DotProduct(this Vector2 thisVector, Vector2 otherVector)
        {
            return thisVector.X * otherVector.X + thisVector.Y * otherVector.Y;
        }

        public static void Normalize(this Vector2 thisVector)
        {
            float magnitude = (float)Math.Sqrt(thisVector.X * thisVector.X + thisVector.Y * thisVector.Y);
            thisVector.X /= magnitude;
            thisVector.Y /= magnitude;
        }
    }
}
