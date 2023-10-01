using Microsoft.Xna.Framework;

namespace BrickShooter.Helpers
{
    public static class LinearEquationHelper
    {
        //builds an equation of a line that connects two points
        //for equation A*x + B*y + C = 0, result.X will be A, result.Y will be B and result.Z will be C
        public static Vector3 BuildLinearEquation(Vector2 point1, Vector2 point2)
        {
            var A = point2.Y - point1.Y;
            var B = point2.X - point1.X;
            return new Vector3
            {
                X = A,
                Y = B,
                Z = A * point1.X + B * point2.Y,
            };
        }
    }
}
