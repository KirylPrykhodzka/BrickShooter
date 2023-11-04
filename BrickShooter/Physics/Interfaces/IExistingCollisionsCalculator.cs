using BrickShooter.Physics.Models;
using System.Collections.Generic;

namespace BrickShooter.Physics.Interfaces
{
    public interface IExistingCollisionsCalculator
    {
        IList<CollisionInfo> GetExistingCollisions(IMaterialObject collisionSubject, IList<IColliderPolygon> potentialCollisions);
    }
}
