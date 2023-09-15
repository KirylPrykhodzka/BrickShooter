using BrickShooter.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Numerics;

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

            var gameBackground = new Texture2D(GlobalObjects.Graphics.GraphicsDevice, GlobalObjects.Graphics.GraphicsDevice.Viewport.Width, GlobalObjects.Graphics.GraphicsDevice.Viewport.Height);
            gameBackground.SetData(new Color[] { Color.Black });
            GlobalObjects.SpriteBatch.Draw(
                gameBackground,
                Microsoft.Xna.Framework.Vector2.Zero,
                null,
                Color.White,
                0f,
                Microsoft.Xna.Framework.Vector2.Zero,
                Microsoft.Xna.Framework.Vector2.One,
                SpriteEffects.None,
                -1);

            level.Draw();

            GlobalObjects.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}