using BrickShooter.Constants;
using BrickShooter.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BrickShooter.GameObjects
{
    public class Background : IDrawableObject
    {
        private readonly Texture2D texture;
        private readonly Rectangle bounds;

        public Background(Texture2D texture, Rectangle bounds)
        {
            this.texture = texture;
            this.bounds = bounds;
            DrawingSystem.Register(this);
        }

        public void Draw()
        {
            GlobalObjects.SpriteBatch.Draw(
                texture,
                bounds,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                Layers.BACKGROUND);
        }
    }
}
