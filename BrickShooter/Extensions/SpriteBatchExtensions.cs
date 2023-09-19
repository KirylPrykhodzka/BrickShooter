using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BrickShooter.Extensions
{
    public static class SpriteBatchExtensions
    {
        public static void DrawLine(this SpriteBatch spriteBatch, Texture2D texture, Vector2 point1, Vector2 point2, float thickness = 1f, float layerDepth = 0f)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(spriteBatch, texture, point1, distance, angle, Color.White, thickness, layerDepth);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Texture2D texture, Vector2 point, float length, float angle, Color color, float thickness = 1f, float layerDepth = 0f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(texture, point, null, color, angle, origin, scale, SpriteEffects.None, layerDepth);
        }
    }
}
