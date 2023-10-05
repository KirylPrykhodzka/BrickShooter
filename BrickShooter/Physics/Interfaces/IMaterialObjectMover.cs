using BrickShooter.Physics.Models;
using System.Collections.Generic;

namespace BrickShooter.Physics.Interfaces
{
    public interface IMaterialObjectMover
    {
        void MoveWithoutObstruction(MaterialObject materialObject);
        void ProcessExistingCollisions(MaterialObject materialObject, IList<CollisionData> existingCollisions);
        void ProcessNextCollision(MaterialObject currentObject, CollisionPredictionResult nextCollision);
    }
}
