using BrickShooter.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;

namespace BrickShooter
{
    public class BrickShooterGame : Game
    {
        public Desktop UI { get; private set; }

        public BrickShooterGame()
        {
            GlobalObjects.GameManager = new GameManager(new BrickShooterGameWrapper(this));
            GlobalObjects.Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height
            };
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
            GlobalObjects.GameManager.StartGame();
        }
        
        protected override void LoadContent()
        {
            MyraEnvironment.Game = this;
            UI = new();
            GlobalObjects.SpriteBatch = new SpriteBatch(GraphicsDevice);
            GlobalObjects.Content = new ContentManagerWrapper(Content);
            GlobalObjects.TimeScale = 1f;

        }

        protected override void Update(GameTime gameTime)
        {
            GlobalObjects.AbsoluteDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds * GlobalObjects.TimeScale;
            GlobalObjects.KeyboardState.Update(Keyboard.GetState());
            GlobalObjects.MouseState.Update(Mouse.GetState());

            if(IsActive && GlobalObjects.GameManager.IsGameActive)
            {
                GlobalObjects.GameManager.Update();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            GlobalObjects.SpriteBatch.Begin(SpriteSortMode.FrontToBack, samplerState: SamplerState.LinearWrap);
            GlobalObjects.GameManager.Draw();
            GlobalObjects.SpriteBatch.End();

            if (UI.Root != null)
            {
                UI.Render();
            }

            base.Draw(gameTime);
        }
    }
}