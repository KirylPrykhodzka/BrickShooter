namespace BrickShooter.Physics
{
    public interface ICollisionsCalculator
    {
        CollisionCalculationResult CalculateCollision(MaterialObject collisionSubject, MaterialObject collisionObject);
    }
}
