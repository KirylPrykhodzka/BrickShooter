using BrickShooter.Constants;
using BrickShooter.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BrickShooter.GameObjects
{
    public class Level
    {
        private readonly LevelData levelData;
        private readonly Texture2D background;
        private readonly Rectangle levelBounds;
        private readonly Player player;

        public Level(string name)
        {
            levelData = GlobalObjects.Content.Load<LevelData>($"Levels/{name}");
            background = GlobalObjects.Content.Load<Texture2D>($"Backgrounds/{levelData.BackgroundTexture}");
            //make sure level is rendered in the center of the screen
            levelBounds = new Rectangle(
                (GlobalObjects.Graphics.GraphicsDevice.Viewport.Bounds.Width - levelData.Width) / 2,
                Math.Abs(levelData.Height - GlobalObjects.Graphics.GraphicsDevice.Viewport.Bounds.Height) / 2,
                levelData.Width,
                levelData.Height);
            //LevelData.InitialPlayerPosition is of type System.Drawing.Point, so we have to convert it here
            player = new Player(new Point(levelBounds.X + levelData.InitialPlayerPosition.X, levelBounds.Y + levelData.InitialPlayerPosition.Y));
        }

        public void Update()
        {
            player.Update();
        }

        public void Draw()
        {
            GlobalObjects.SpriteBatch.Draw(
                background,
                levelBounds,
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
