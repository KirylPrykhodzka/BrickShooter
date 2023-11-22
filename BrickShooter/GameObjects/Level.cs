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

            walls.Clear();
            foreach (var wallGroup in levelData.Walls.GroupBy(x => x.TextureName))
            {
                var wallsTexture = GlobalObjects.Content.Load<Texture2D>($"Walls/{wallGroup.Key}");
                walls.AddRange(wallGroup.Select(wallData => new Wall(
                        wallsTexture,
                        new Point(wallData.X + levelBounds.X, wallData.Y + levelBounds.Y),
                        wallData.Width,
                        wallData.Height,
                        wallData.Rotation)));
            }

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
            GlobalObjects.GameManager.OnLoss();
        }
    }
}
