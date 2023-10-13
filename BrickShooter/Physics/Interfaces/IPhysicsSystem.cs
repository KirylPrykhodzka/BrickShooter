using BrickShooter.Physics.Models;

namespace BrickShooter.Physics.Interfaces
{
    public interface IPhysicsSystem
    {
        void Run();
        void Visualize();
        void RegisterMobileObject(MaterialObject mobileMaterialObject);
        void UnregisterMobileObject(MaterialObject mobileMaterialObject);
        void RegisterImmobileObject(MaterialObject immobileObject);
        void UnregisterImmobileObject(MaterialObject immobileObject);
        void Reset();
    }
}
