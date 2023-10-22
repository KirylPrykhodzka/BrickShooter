using Microsoft.Xna.Framework.Input;

namespace BrickShooter.Framework
{
    public class MouseState : IMouseState
    {
        public int X { get; set; }
        public int Y { get; set; }
        public ButtonState LeftButton { get; set; }

        public void Update(Microsoft.Xna.Framework.Input.MouseState mouseState)
        {
            X = mouseState.X;
            Y = mouseState.Y;
            LeftButton = mouseState.LeftButton;
        }
    }
}
