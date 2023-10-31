using BrickShooter.Physics.Models;
using System.Collections.Generic;

namespace BrickShooter.Physics.Interfaces
{
    public interface ICollisionProcessor
    {
        void ProcessExistingCollisions(MaterialObject materialObject, IList<CollisionInfo> existingCollisions);
        void FindAndProcessNextCollisions(MaterialObject currentObject, IList<MaterialObject> potentialFutureCollisions);
    }
}
