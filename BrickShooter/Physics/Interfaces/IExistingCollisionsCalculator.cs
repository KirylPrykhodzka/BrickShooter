using BrickShooter.Physics.Models;
using System.Collections.Generic;

namespace BrickShooter.Physics.Interfaces
{
    public interface IExistingCollisionsCalculator
    {
        IList<RotationCollisionInfo> GetExistingCollisions(IList<CollisionPair> potentialCollisions);
    }
}
