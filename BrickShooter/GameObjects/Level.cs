using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BrickShooter.GameObjects
{
    public class Level
    {
        private Texture2D background = GlobalObjects.Content.Load<Texture2D>("Background/background_starrysky");
        private Player player = new Player();

        public void Draw()
        {
            int w = (int)(GlobalObjects.Graphics.GraphicsDevice.Viewport.Bounds.Width * 0.8);
            int h = (int)(GlobalObjects.Graphics.GraphicsDevice.Viewport.Bounds.Height * 0.9);
            GlobalObjects.SpriteBatch.Draw(
                background,
                new Rectangle(0, 0, w, h),
                null,
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                Layers.BACKGROUND);
            player.Draw();
            //draw structures
            //draw bricks
            //draw bullets
        }
    }
}
