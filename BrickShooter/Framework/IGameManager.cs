namespace BrickShooter.Framework
{
    public interface IGameManager
    {
        bool IsGameActive { get; }
        void StartGame();
        void OnLoss();
        void RestartLevel();
        void Quit();
        void Update();
        void Draw();
    }
}
