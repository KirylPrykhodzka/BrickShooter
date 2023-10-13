using BrickShooter.Constants;
using BrickShooter.Drawing;
using BrickShooter.Models;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrickShooter.GameObjects
{
    public class Level
    {
        private readonly IFactory<Bullet> bulletFactory;
        private readonly IPhysicsSystem physicsSystem;
        private readonly IDrawingSystem drawingSystem;

        private Rectangle levelBounds;
        private LevelData levelData;
        private Background background;
        private Player player;
        //for the cooldown calculation purposes
        private long lastBulletShot = 0;
        private readonly List<Wall> walls = new();

        public Level(IFactory<Bullet> bulletFactory,
            IPhysicsSystem physicsSystem,
            IDrawingSystem drawingSystem)
        {
            this.bulletFactory = bulletFactory;
            this.physicsSystem = physicsSystem;
            this.drawingSystem = drawingSystem;
        }

        public void Load(string name)
        {
            levelData = GlobalObjects.Content.Load<LevelData>($"Levels/{name}");

            //make sure level is rendered in the center of the screen
            levelBounds = new Rectangle(
                (GlobalObjects.Graphics.GraphicsDevice.Viewport.Bounds.Width - levelData.Width) / 2,
                Math.Abs(levelData.Height - GlobalObjects.Graphics.GraphicsDevice.Viewport.Bounds.Height) / 2,
                levelData.Width,
                levelData.Height);

            background = new Background(levelData.BackgroundTexture, levelBounds);
            drawingSystem.Register(background);

            //LevelData.InitialPlayerPosition is of type System.Drawing.Point, so we have to convert it here
            player = new Player(new Vector2(levelBounds.X + levelData.InitialPlayerPosition.X, levelBounds.Y + levelData.InitialPlayerPosition.Y));
            physicsSystem.RegisterMobileObject(player);
            drawingSystem.Register(player);

            walls.Clear();
            walls.AddRange(levelData.Walls.Placements.Select(placement =>
                new Wall(
                    levelData.Walls.Texture,
                    new Rectangle(
                        levelBounds.X + placement.X * levelData.Walls.TileWidth + levelData.Walls.OffsetX,
                        levelBounds.Y + placement.Y * levelData.Walls.TileHeight + levelData.Walls.OffsetY,
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
            physicsSystem.Reset();
            drawingSystem.Reset();
        }

        public void Update()
        {
            player.Update();

            if (GlobalObjects.MouseState.LeftButton == ButtonState.Pressed)
            {
                var now = DateTime.Now.Ticks / 10000;
                if (now - lastBulletShot > PlayerConstants.SHOOTING_COOLDOWN_MS)
                {
                    Shoot();
                    lastBulletShot = now;
                }
            }
        }

        private void Shoot()
        {
            var bullet = bulletFactory.GetItem();
            physicsSystem.RegisterMobileObject(bullet);
            drawingSystem.Register(bullet);
            bullet.OnPlayerHit = OnBulletHitPlayer;
            var initialPosition = player.InitialBulletPosition;
            bullet.Move(initialPosition, player.Rotation);
        }

        private void OnBulletHitPlayer(Bullet bullet)
        {
            bullet.OnPlayerHit = null;
            physicsSystem.UnregisterMobileObject(bullet);
            drawingSystem.Unregister(bullet);
            bulletFactory.Return(bullet);
        }
    }
}
