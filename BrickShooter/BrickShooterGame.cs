using BrickShooter.Configuration;
using BrickShooter.Drawing;
using BrickShooter.GameObjects;
using BrickShooter.Physics.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BrickShooter
{
    public class BrickShooterGame : Game
    {
        private IPhysicsSystem physicsSystem;
        private Level level;

        public BrickShooterGame()
        {
            GlobalObjects.Graphics = new GraphicsDeviceManager(this);
            GlobalObjects.Graphics.PreferredBackBufferWidth = 1920;
            GlobalObjects.Graphics.PreferredBackBufferHeight = 1080;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            physicsSystem = ServiceProviderFactory.ServiceProvider.GetService<IPhysicsSystem>();
            base.Initialize();
        }

         protected override void LoadContent()
        {
            GlobalObjects.SpriteBatch = new SpriteBatch(GraphicsDevice);
            GlobalObjects.Content = Content;

            level = new Level("level1");
        }

        protected override void Update(GameTime gameTime)
        {
            GlobalObjects.GameTime = gameTime;

            if(IsActive)
            {
                level.Update();
            }
            physicsSystem.Run();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GlobalObjects.SpriteBatch.Begin(SpriteSortMode.FrontToBack);

            DrawingSystem.Run();
#if DEBUG
            physicsSystem.Visualize();
#endif

            GlobalObjects.SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}