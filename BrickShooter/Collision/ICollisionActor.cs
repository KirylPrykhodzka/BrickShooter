namespace BrickShooter.Collision
{
    public interface ICollisionActor
    {
        ColliderPolygon ColliderBounds { get; }
    }
}
