namespace BrickShooter.Physics
{
    public interface ICollisionCalculator
    {
        CollisionPredictionResult CalculateCollision(MaterialObject collisionSubject, MaterialObject collisionObject);
    }
}
