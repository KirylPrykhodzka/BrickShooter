namespace BrickShooter.Collision
{
    public interface IMaterialObject
    {
        ColliderPolygon ColliderBounds { get; }
        float Bounciness { get; }
    }
}
