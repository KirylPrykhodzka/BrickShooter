using BrickShooter.Constants;
using BrickShooter.Extensions;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BrickShooter.Helpers
{
    public static class VisualizationHelper
    {
        private static Texture2D texture;

        static VisualizationHelper()
        {
            texture = new Texture2D(GlobalObjects.Graphics.GraphicsDevice, 1, 1);
        }

        public static void VisualizeCollider(ColliderPolygon collider)
        {
            texture.SetData(new Color[] { Color.GreenYellow });
            var points = collider.Points;
            for (int i = 0; i < points.Count; i++)
            {
                var destination = i < points.Count - 1 ? new Vector2(points[i + 1].X, points[i + 1].Y) : new Vector2(points[0].X, points[0].Y);
                GlobalObjects.SpriteBatch.DrawLine(
                    texture,
                    new Vector2(points[i].X, points[i].Y),
                    destination,
                    layerDepth: Layers.UI);
            }
        }
    }
}
