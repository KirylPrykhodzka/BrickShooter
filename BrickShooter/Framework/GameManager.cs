using BrickShooter.Configuration;
using BrickShooter.Core;
using BrickShooter.Drawing;
using BrickShooter.GameObjects;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Resources;
using BrickShooter.UI;
using Microsoft.Extensions.DependencyInjection;

namespace BrickShooter.Framework
{
    public class GameManager : IGameManager
    {
        private readonly IBrickShooterGame game;
        private readonly IPhysicsSystem physicsSystem;
        private readonly IDrawingSystem drawingSystem;

        private Level currentLevel;

        public bool IsGameActive { get; private set; }

        public GameManager(IBrickShooterGame game)
        {
            this.game = game;
            physicsSystem = ServiceProviderFactory.ServiceProvider.GetService<IPhysicsSystem>();
            drawingSystem = ServiceProviderFactory.ServiceProvider.GetService<IDrawingSystem>();
        }

        public void StartGame()
        {
            game.SetUI(null);
            game.SetIsMouseVisible(false);
            IsGameActive = true;
            GlobalObjects.TimeScale = 1f;
            currentLevel = new Level(ServiceProviderFactory.ServiceProvider.GetService<IPool<Bullet>>(), physicsSystem, drawingSystem);
            currentLevel.Load("Level1", GlobalObjects.Graphics.GraphicsDevice.Viewport.Bounds);
        }

        public void OnLoss()
        {
            IsGameActive = false;
            GlobalObjects.TimeScale = 0f;
            game.SetIsMouseVisible(true);
            game.SetUI(new LossScreen());
        }

        public void RestartLevel()
        {
            currentLevel.Unload();
            StartGame();
        }

        public void Quit()
        {
            game.Exit();
        }

        public void Update()
        {
            currentLevel.Update();
            physicsSystem.Run();
        }

        public void Draw()
        {
            drawingSystem.Run();
#if DEBUG
            physicsSystem.Visualize();
#endif
        }
    }
}
