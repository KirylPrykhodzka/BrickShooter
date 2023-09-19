using BrickShooter.Constants;
using BrickShooter.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BrickShooter.Helpers
{
    public static class VisualizationHelper
    {
        public static void VisualizeCollider(List<Vector2> points)
        {
            var blackTexture = new Texture2D(GlobalObjects.Graphics.GraphicsDevice, 1, 1);
            blackTexture.SetData(new Color[] { Color.GreenYellow });
            for (int i = 0; i < points.Count; i++)
            {
                var destination = i < points.Count - 1 ? new Vector2(points[i + 1].X, points[i + 1].Y) : new Vector2(points[0].X, points[0].Y);
                GlobalObjects.SpriteBatch.DrawLine(
                    blackTexture,
                    new Vector2(points[i].X, points[i].Y),
                    destination,
                    layerDepth: Layers.UI);
            }
        }
    }
}
