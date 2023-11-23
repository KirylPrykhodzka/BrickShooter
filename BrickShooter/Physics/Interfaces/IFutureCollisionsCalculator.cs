using BrickShooter.Physics.Models;
using System.Collections.Generic;

namespace BrickShooter.Physics.Interfaces
{
    public interface IFutureCollisionsCalculator
    {
        IList<MovementCollisionInfo> FindNextCollisions(IList<CollisionPair> potentialCollisions);
    }
}
