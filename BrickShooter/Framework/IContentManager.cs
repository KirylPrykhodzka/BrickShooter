namespace BrickShooter.Framework
{
    public interface IContentManager
    {
        T Load<T>(string path);
    }
}
