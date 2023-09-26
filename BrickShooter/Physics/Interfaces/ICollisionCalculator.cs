namespace BrickShooter.Physics.Interfaces
{
    public interface ICollisionCalculator
    {
        CollisionPredictionResult CalculateCollision(MaterialObject collisionSubject, MaterialObject collisionObject);
    }
}
