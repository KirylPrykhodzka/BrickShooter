using BrickShooter.Collision;
using BrickShooter.Constants;
using BrickShooter.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace BrickShooter.GameObjects
{
    public class Level
    {
        private readonly Rectangle levelBounds;
        private readonly LevelData levelData;
        private readonly Player player;
        private readonly Wall[] walls;

        public Level(string name)
        {
            levelData = GlobalObjects.Content.Load<LevelData>($"Levels/{name}");

            //make sure level is rendered in the center of the screen
            levelBounds = new Rectangle(
                (GlobalObjects.Graphics.GraphicsDevice.Viewport.Bounds.Width - levelData.Width) / 2,
                Math.Abs(levelData.Height - GlobalObjects.Graphics.GraphicsDevice.Viewport.Bounds.Height) / 2,
                levelData.Width,
                levelData.Height);

            //LevelData.InitialPlayerPosition is of type System.Drawing.Point, so we have to convert it here
            player = new Player(new Point(levelBounds.X + levelData.InitialPlayerPosition.X, levelBounds.Y + levelData.InitialPlayerPosition.Y));

            walls = levelData.Walls.Placements.Select(placement =>
                new Wall(
                    levelData.Walls.Texture,
                    new Rectangle(
                        levelBounds.X + placement.X * levelData.Walls.TileWidth + levelData.Walls.OffsetX,
                        levelBounds.Y + placement.Y * levelData.Walls.TileHeight + levelData.Walls.OffsetY,
                        levelData.Walls.TileWidth,
                        levelData.Walls.TileHeight)
                    )
                ).ToArray();
        }

        public void Update()
        {
            player.Update();
        }

        public void Draw()
        {
            GlobalObjects.SpriteBatch.Draw(
                levelData.BackgroundTexture,
                levelBounds,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                Layers.BACKGROUND);

            foreach(var wall in walls)
            {
                wall.Draw();
            }

            player.Draw();

            //draw bricks
            //draw bullets
        }
    }
}
