using Microsoft.Xna.Framework;

namespace BrickShooter.Extensions
{
    public static class Vector2Extensions
    {
        public static float DotProduct(this Vector2 thisVector, Vector2 otherVector)
        {
            return thisVector.X * otherVector.X + thisVector.Y * otherVector.Y;
        }
    }
}
