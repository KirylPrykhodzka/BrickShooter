/* Generated by MyraPad at 11/19/2023 9:13:33 PM */
namespace BrickShooter.UI
{
	public partial class LossScreen
	{
		public LossScreen()
		{
			BuildUI();
            _retryButton.Click += RetryButton_Click;
            _quitButton.Click += QuitButton_Click;
		}

        private void QuitButton_Click(object sender, System.EventArgs e)
        {
            GlobalObjects.GameManager.Quit();
        }

        private void RetryButton_Click(object sender, System.EventArgs e)
        {
			GlobalObjects.GameManager.RestartLevel();
        }
    }
}