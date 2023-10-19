using BrickShooter.Physics.Models;
using System.Collections.Generic;

namespace BrickShooter.Physics.Interfaces
{
    public interface IFutureCollisionsCalculator
    {
        IEnumerable<FutureCollisionInfo> FindNextCollisions(MaterialObject collisionSubject, IEnumerable<MaterialObject> potentialCollisions);
    }
}
