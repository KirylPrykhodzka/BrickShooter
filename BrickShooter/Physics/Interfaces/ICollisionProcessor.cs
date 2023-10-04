using System.Collections.Generic;
using BrickShooter.Physics.Models;

namespace BrickShooter.Physics.Interfaces
{
    public interface ICollisionProcessor
    {
        void ProcessCollisions(MaterialObject currentObject, IEnumerable<CollisionPredictionResult> futureCollisions);
    }
}
