using Myra.Graphics2D.UI;

namespace BrickShooter.Framework
{
    public interface IBrickShooterGame
    {
        void SetUI(Widget ui);
        void SetIsMouseVisible(bool isMouseVisible);
        void Exit();
    }
}
