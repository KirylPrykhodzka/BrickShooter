namespace BrickShooter.Drawing
{
    public interface IDrawingSystem
    {
        void Register(IDrawableObject drawableObject);
        void Unregister(IDrawableObject drawableObject);
        void Reset();
        void Run();
    }
}
