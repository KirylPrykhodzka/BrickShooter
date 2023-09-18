using BrickShooter.Constants;
using BrickShooter.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BrickShooter.GameObjects
{
    public class Level
    {
        private LevelData levelData;
        private Texture2D background;
        private Player player = new Player();

        public Level(string name)
        {
            levelData = GlobalObjects.Content.Load<LevelData>($"Levels/{name}");
            background = GlobalObjects.Content.Load<Texture2D>($"Backgrounds/{levelData.BackgroundTexture}");
        }

        public void Update()
        {
            player.Update();
        }

        public void Draw()
        {
            GlobalObjects.SpriteBatch.Draw(
                background,
                new Rectangle((GlobalObjects.Graphics.GraphicsDevice.Viewport.Bounds.Width - levelData.Width) / 2, Math.Abs(levelData.Height - GlobalObjects.Graphics.GraphicsDevice.Viewport.Bounds.Height) / 2, levelData.Width, levelData.Height),
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
