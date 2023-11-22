using Myra.Graphics2D.UI;

namespace BrickShooter.Framework
{
    public class BrickShooterGameWrapper : IBrickShooterGame
    {
        private BrickShooterGame game;

        public BrickShooterGameWrapper(BrickShooterGame game)
        {
            this.game = game;
        }

        public void Exit()
        {
            game.Exit();
        }

        public void SetIsMouseVisible(bool isMouseVisible)
        {
            game.IsMouseVisible = isMouseVisible;
        }

        public void SetUI(Widget ui)
        {
            game.UI.Root = ui;
        }
    }
}
