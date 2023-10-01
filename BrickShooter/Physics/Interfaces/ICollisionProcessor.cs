using System.Collections.Generic;
using BrickShooter.Physics.Models;

namespace BrickShooter.Physics.Interfaces
{
    public interface ICollisionProcessor
    {
        void ProcessFutureCollisions(MaterialObject currentObject, IReadOnlyCollection<CollisionCalculationResult> futureCollisions);
    }
}
