using BrickShooter.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BrickShooter
{
    public static class GlobalObjects
    {
        public static GraphicsDeviceManager Graphics { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static IContentManager Content { get; set; }
        public static float DeltaTime { get; set; }
        public static IKeyboardState KeyboardState { get; set; } = new KeyboardState();
        public static IMouseState MouseState { get; set; } = new MouseState();
    }
}
