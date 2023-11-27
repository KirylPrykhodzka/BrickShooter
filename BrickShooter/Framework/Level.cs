using BrickShooter.Constants;
using BrickShooter.Drawing;
using BrickShooter.GameObjects;
using BrickShooter.GameObjects.Enemies;
using BrickShooter.Models;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrickShooter.Framework
{
    public delegate void OnPlayerHit(IMaterialObject hitter);
    public delegate void OnDeath(Enemy deadEnemy);
    public delegate void OnBulletDestroy(Bullet bullet);

    public class Level
    {
        private readonly IPool<Bullet> bulletPool;
        private readonly IPhysicsSystem physicsSystem;
        private readonly IDrawingSystem drawingSystem;

        private Background background;
        private Rectangle levelBounds;

        private Player player;
        //for the cooldown calculation purposes
        private long lastBulletShot = 0;
        private long lastEnemySpawn = 0;
        private readonly List<Wall> walls = new();
        private int enemySpawnCooldownMS;

        private Queue<string> enemySpawnOrder = new();
        private List<Vector2> enemySpawnPoints = new();
        private readonly List<Enemy> aliveEnemies = new();

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
            var levelData = GlobalObjects.Content.Load<LevelData>($"Levels/{name}");

            //make sure level is rendered in the center of the screen
            levelBounds = new Rectangle(
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

            foreach (var wall in walls)
            {
                drawingSystem.Register(wall);
                physicsSystem.RegisterImmobileObject(wall);
            }

            enemySpawnPoints = levelData.EnemiesData.SpawnPoints.ToList();
            CreateSpawnOrder(levelData.EnemiesData);
            enemySpawnCooldownMS = EnemiesConstants.SPAWN_COOLDOWN_MS;
        }

        public void Unload()
        {
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

            if (now - lastEnemySpawn > enemySpawnCooldownMS && enemySpawnOrder.TryDequeue(out var nextEnemyType))
            {
                var nextEnemy = ResolveNextEnemy(nextEnemyType);
                nextEnemy.Position = FindAvailableSpawnPoint();
                nextEnemy.OnPlayerHit = OnEnemyHitPlayer;
                nextEnemy.OnDeath = OnEnemyDeath;
                aliveEnemies.Add(nextEnemy);
                physicsSystem.RegisterMobileObject(nextEnemy);
                drawingSystem.Register(nextEnemy);
                lastEnemySpawn = now;
            }

            foreach (var enemy in aliveEnemies)
            {
                enemy.Update();
            }
        }

        private void Shoot()
        {
            var bullet = bulletPool.GetItem();
            physicsSystem.RegisterMobileObject(bullet);
            drawingSystem.Register(bullet);
            bullet.OnPlayerHit = OnBulletHitPlayer;
            bullet.OnBulletDestroy = OnBulletDestroy;
            var initialPosition = player.BulletSpawnPoint;
            bullet.Move(initialPosition, player.Rotation);
        }

        private void OnBulletHitPlayer(IMaterialObject bullet)
        {
            GlobalObjects.GameManager.OnLoss();
        }

        private void OnBulletDestroy(Bullet bullet)
        {
            physicsSystem.UnregisterMobileObject(bullet);
            drawingSystem.Unregister(bullet);
            bulletPool.Return(bullet);
        }

        private void OnEnemyHitPlayer(IMaterialObject enemy)
        {
            GlobalObjects.GameManager.OnLoss();
        }

        private void OnEnemyDeath(Enemy enemy)
        {
            physicsSystem.UnregisterMobileObject(enemy);
            drawingSystem.Unregister(enemy);
        }

        private Vector2 FindAvailableSpawnPoint()
        {
            var availableSpawnPoint = enemySpawnPoints[new Random().Next(0, enemySpawnPoints.Count)];
            return new Vector2(availableSpawnPoint.X + levelBounds.X, availableSpawnPoint.Y + levelBounds.Y);
        }

        private Enemy ResolveNextEnemy(string enemyType)
        {
            return enemyType switch
            {
                "redBrick" => new RedBrick(player),
                _ => null
            };
        }

        private void CreateSpawnOrder(EnemiesData enemies)
        {
            for (int i = 0; i < enemies.RedBricks; i++)
            {
                enemySpawnOrder.Enqueue("redBrick");
            }
        }
    }
}
