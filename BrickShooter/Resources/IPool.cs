namespace BrickShooter.Resources
{
    public interface IPool<T>
    {
        T GetItem();
        void Return(T item);
    }
}
