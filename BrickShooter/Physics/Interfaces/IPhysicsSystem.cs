namespace BrickShooter.Physics.Interfaces
{
    public interface IPhysicsSystem
    {
        void Run();
        void Visualize();
        void RegisterMobileObject(IMaterialObject mobileMaterialObject);
        void UnregisterMobileObject(IMaterialObject mobileMaterialObject);
        void RegisterImmobileObject(IMaterialObject immobileObject);
        void UnregisterImmobileObject(IMaterialObject immobileObject);
        void Reset();
    }
}
