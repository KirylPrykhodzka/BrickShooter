namespace BrickShooter.Resources
{
    public interface IFactory<T>
    {
        T GetItem();
        void Return(T item);
    }
}
