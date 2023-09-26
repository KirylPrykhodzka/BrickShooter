using System.Collections.Generic;

namespace BrickShooter.Physics
{
    public interface ICollisionProcessor
    {
        void ProcessFutureCollisions(MaterialObject currentObject, IReadOnlyCollection<CollisionPredictionResult> futureCollisions);
    }
}
