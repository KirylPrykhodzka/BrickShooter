using BrickShooter.Constants;
using BrickShooter.Drawing;
using BrickShooter.Models;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrickShooter.GameObjects
{
    public class Level
    {
        private readonly IPool<Bullet> bulletPool;
        private readonly IPhysicsSystem physicsSystem;
        private readonly IDrawingSystem drawingSystem;

        private LevelData levelData;
        private Background background;
        private Player player;
        //for the cooldown calculation purposes
        private long lastBulletShot = 0;
        private readonly List<Wall> walls = new();

        public Level(IPool<Bullet> bulletPool,
            IPhysicsSystem physicsSystem,
            IDrawingSystem drawingSystem)
        {
            this.bulletPool = bulletPool;
            this.physicsSystem = physicsSystem;
            this.drawingSystem = drawingSystem;
        }

        public void Load(string name, Rectangle viewportBounds)
        {
            levelData = GlobalObjects.Content.Load<LevelData>($"Levels/{name}");

            //make sure level is rendered in the center of the screen
            var levelBounds = new Rectangle(
                Math.Abs(viewportBounds.Width - levelData.Width) / 2,
                Math.Abs(viewportBounds.Height - levelData.Height) / 2,
                levelData.Width,
                levelData.Height);

            var backgroundTexture = GlobalObjects.Content.Load<Texture2D>($"Backgrounds/{levelData.BackgroundTextureName}");
            background = new Background(backgroundTexture, levelBounds);
            drawingSystem.Register(background);

            player = new Player(new Vector2(levelBounds.X + levelData.InitialPlayerPosition.X, levelBounds.Y + levelData.InitialPlayerPosition.Y));
            physicsSystem.RegisterMobileObject(player);
            drawingSystem.Register(player);

            var wallsTexture = GlobalObjects.Content.Load<Texture2D>($"Walls/{levelData.Walls.TextureName}");
            walls.Clear();
            walls.AddRange(levelData.Walls.Placements.Select(placement =>
                new Wall(
                    wallsTexture,
                    new Rectangle(
                        levelBounds.X + (int)placement.X * levelData.Walls.TileWidth + levelData.Walls.OffsetX,
                        levelBounds.Y + (int)placement.Y * levelData.Walls.TileHeight + levelData.Walls.OffsetY,
                        levelData.Walls.TileWidth,
                        levelData.Walls.TileHeight)
                    )
                ));
            foreach(var wall in walls)
            {
                drawingSystem.Register(wall);
                physicsSystem.RegisterImmobileObject(wall);
            }
        }

        public void Unload()
        {
            levelData = null;
            background = null;
            player = null;
            walls.Clear();
            lastBulletShot = 0;
            physicsSystem.Reset();
            drawingSystem.Reset();
        }

        public void Update()
        {
            player.Update();

            var now = DateTime.Now.Ticks / 10000;
            if (now - lastBulletShot > PlayerConstants.SHOOTING_COOLDOWN_MS && GlobalObjects.MouseState.LeftButton == ButtonState.Pressed)
            {
                Shoot();
                lastBulletShot = now;
            }
        }

        private void Shoot()
        {
            var bullet = bulletPool.GetItem();
            physicsSystem.RegisterMobileObject(bullet);
            drawingSystem.Register(bullet);
            bullet.OnPlayerHit = OnBulletHitPlayer;
            var initialPosition = player.BulletSpawnPoint;
            bullet.Move(initialPosition, player.Rotation);
        }

        private void OnBulletHitPlayer(Bullet bullet)
        {
            bullet.OnPlayerHit = null;
            physicsSystem.UnregisterMobileObject(bullet);
            drawingSystem.Unregister(bullet);
            bulletPool.Return(bullet);
        }
    }
}
