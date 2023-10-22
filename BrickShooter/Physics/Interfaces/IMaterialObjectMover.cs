using BrickShooter.Physics.Models;

namespace BrickShooter.Physics.Interfaces
{
    public interface IMaterialObjectMover
    {
        void MoveWithoutObstruction(MaterialObject materialObject);
    }
}
