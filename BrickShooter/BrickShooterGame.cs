using BrickShooter.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BrickShooter
{
    public class BrickShooterGame : Game
    {
        private Level level;

        public BrickShooterGame()
        {
            GlobalObjects.Graphics = new GraphicsDeviceManager(this);
            GlobalObjects.Graphics.IsFullScreen = true;
            GlobalObjects.Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            GlobalObjects.Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
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

            level = new Level();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GlobalObjects.SpriteBatch.Begin();

            level.Draw();

            GlobalObjects.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}