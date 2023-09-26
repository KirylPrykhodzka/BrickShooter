namespace BrickShooter.Physics
{
    public interface IPhysicsSystem
    {
        void Run();
        void Visualize();
        void RegisterMobileObject(MaterialObject mobileMaterialObject);
        void UnregisterMobileObject(MaterialObject mobileMaterialObject);
        void RegisterImmobileObject(MaterialObject immobileObject);
    }
}
