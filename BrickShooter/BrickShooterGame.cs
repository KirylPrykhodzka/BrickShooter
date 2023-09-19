using BrickShooter.Collision;
using BrickShooter.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace BrickShooter
{
    public class BrickShooterGame : Game
    {
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

            level.Update();
            CollisionSystem.Run();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GlobalObjects.SpriteBatch.Begin(SpriteSortMode.Immediate);

            level.Draw();

            GlobalObjects.SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}