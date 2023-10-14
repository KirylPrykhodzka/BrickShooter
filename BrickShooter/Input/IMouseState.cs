using Microsoft.Xna.Framework.Input;

namespace BrickShooter.Input
{
    public interface IMouseState
    {
        int X { get; set; }
        int Y { get; set; }
        ButtonState LeftButton { get; set; }

        void Update(Microsoft.Xna.Framework.Input.MouseState mouseState);
    }
}
