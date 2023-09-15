using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BrickShooter.GameObjects
{
    public class Level
    {
        private Texture2D background;

        public Level()
        {
            background = GlobalObjects.Content.Load<Texture2D>("Background/background_starrysky");
        }

        public void Draw()
        {
            GlobalObjects.SpriteBatch.Draw(background, new Rectangle(0, 0, GlobalObjects.Graphics.PreferredBackBufferWidth, GlobalObjects.Graphics.PreferredBackBufferHeight), Color.White);
            //draw structures
            //draw player
            //draw bricks
            //draw bullets
        }
    }
}
