using BrickShooter.Configuration;
using BrickShooter.Drawing;
using BrickShooter.GameObjects;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BrickShooter
{
    public class BrickShooterGame : Game
    {
        private IPhysicsSystem physicsSystem;
        private IDrawingSystem drawingSystem;
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
            drawingSystem = ServiceProviderFactory.ServiceProvider.GetService<IDrawingSystem>();
            base.Initialize();
        }

         protected override void LoadContent()
        {
            GlobalObjects.SpriteBatch = new SpriteBatch(GraphicsDevice);
            GlobalObjects.Content = Content;

            level = new Level(ServiceProviderFactory.ServiceProvider.GetService<IPool<Bullet>>(),
                physicsSystem,
                drawingSystem);
            level.Load("Level1");
        }

        protected override void Update(GameTime gameTime)
        {

            GlobalObjects.DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            GlobalObjects.KeyboardState.Update(Keyboard.GetState());
            GlobalObjects.MouseState.Update(Mouse.GetState());

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

            drawingSystem.Run();
#if DEBUG
            physicsSystem.Visualize();
#endif

            GlobalObjects.SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}