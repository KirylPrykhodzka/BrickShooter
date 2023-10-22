using Microsoft.Xna.Framework.Input;

namespace BrickShooter.Framework
{
    public interface IKeyboardState
    {
        Keys[] PressedKeys { get; set; }

        void Update(Microsoft.Xna.Framework.Input.KeyboardState keyboardState);
    }
}
