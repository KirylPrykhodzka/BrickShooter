using BrickShooter.Physics.Models;
using System.Collections.Generic;

namespace BrickShooter.Physics.Interfaces
{
    public interface ICollisionProcessor
    {
        void ProcessExistingCollisions(IMaterialObject materialObject, IList<CollisionInfo> existingCollisions);
        void FindAndProcessNextCollisions(IMaterialObject currentObject, IList<IColliderPolygon> potentialFutureCollisions);
    }
}
