using BrickShooter.Constants;
using BrickShooter.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using System;
using System.Linq;

namespace BrickShooter.GameObjects
{
    public class Level : IDisposable
    {
        private readonly Rectangle levelBounds;
        private LevelData levelData;
        private Player player;
        private Wall[] walls;
        private readonly CollisionComponent collisionComponent;

        private bool disposedValue;

        public Level(string name)
        {
            levelData = GlobalObjects.Content.Load<LevelData>($"Levels/{name}");

            //make sure level is rendered in the center of the screen
            levelBounds = new Rectangle(
                (GlobalObjects.Graphics.GraphicsDevice.Viewport.Bounds.Width - levelData.Width) / 2,
                Math.Abs(levelData.Height - GlobalObjects.Graphics.GraphicsDevice.Viewport.Bounds.Height) / 2,
                levelData.Width,
                levelData.Height);
            collisionComponent = new CollisionComponent(levelBounds);

            //LevelData.InitialPlayerPosition is of type System.Drawing.Point, so we have to convert it here
            player = new Player(new Vector2(levelBounds.X + levelData.InitialPlayerPosition.X, levelBounds.Y + levelData.InitialPlayerPosition.Y));

            walls = levelData.Walls.Placements.Select(placement => new Wall
            {
                Texture = levelData.Walls.Texture,
                RectBounds = new Rectangle(
                    levelBounds.X + placement.X * levelData.Walls.TileWidth + levelData.Walls.OffsetX,
                    levelBounds.Y + placement.Y * levelData.Walls.TileHeight + levelData.Walls.OffsetY,
                    levelData.Walls.TileWidth,
                    levelData.Walls.TileHeight)
            }).ToArray();

            collisionComponent.Insert(player);
            foreach(var wall in walls)
            {
                collisionComponent.Insert(wall);
            }
        }

        public void Update()
        {
            player.Update();
            collisionComponent.Update(GlobalObjects.GameTime);
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

            player.Draw();

            foreach(var wall in walls)
            {
                wall.Draw();
            }

            //draw bricks
            //draw bullets
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    collisionComponent.Dispose();
                }

                levelData = null;
                player = null;
                walls = null;

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
