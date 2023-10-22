using Microsoft.Xna.Framework.Input;

namespace BrickShooter.Framework
{
    public class KeyboardState : IKeyboardState
    {
        public Keys[] PressedKeys { get; set; }

        public void Update(Microsoft.Xna.Framework.Input.KeyboardState keyboardState)
        {
            PressedKeys = keyboardState.GetPressedKeys();
        }
    }
}
