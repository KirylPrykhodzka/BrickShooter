using BrickShooter.Physics.Models;

namespace BrickShooter.Physics.Interfaces
{
    public interface ICollisionCalculator
    {
        CollisionCalculationResult CalculateCollision(MaterialObject collisionSubject, MaterialObject collisionObject);
    }
}
