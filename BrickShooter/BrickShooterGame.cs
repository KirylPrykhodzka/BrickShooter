using BrickShooter.Configuration;
using BrickShooter.Drawing;
using BrickShooter.GameObjects;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

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

        private int collectionCount_0 = 0;
        private int collectionCount_1 = 0;
        private int collectionCount_2 = 0;

        protected override void Update(GameTime gameTime)
        {
            var cc_0 = GC.CollectionCount(0);
            if(cc_0 > collectionCount_0)
            {
                collectionCount_0 = cc_0;
                Debug.WriteLine("Collected 0");
            }
            var cc_1 = GC.CollectionCount(1);
            if (cc_1 > collectionCount_1)
            {
                collectionCount_1 = cc_1;
                Debug.WriteLine("Collected 1");
            }
            var cc_2 = GC.CollectionCount(2);
            if (cc_2 > collectionCount_2)
            {
                collectionCount_2 = cc_2;
                Debug.WriteLine("Collected 2");
            }

            GlobalObjects.GameTime = gameTime;
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