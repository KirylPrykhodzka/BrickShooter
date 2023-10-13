using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BrickShooter
{
    public static class GlobalObjects
    {
        public static GraphicsDeviceManager Graphics { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static ContentManager Content { get; set; }
        public static GameTime GameTime { get; set; }
        public static KeyboardState KeyboardState { get; set; }
        public static MouseState MouseState { get; set; }
    }
}
