using Microsoft.Xna.Framework.Input;

namespace BrickShooter.Input
{
    public interface IKeyboardState
    {
        Keys[] PressedKeys { get; set; }

        void Update(Microsoft.Xna.Framework.Input.KeyboardState keyboardState);
    }
}
