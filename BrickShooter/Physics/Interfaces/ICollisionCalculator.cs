using BrickShooter.Physics.Models;
using System.Collections.Generic;

namespace BrickShooter.Physics.Interfaces
{
    public interface ICollisionCalculator
    {        
        IList<CollisionData> GetExistingCollisions(MaterialObject collisionSubject, IEnumerable<MaterialObject> potentialCollisions);
        IList<CollisionPredictionResult> FindNextCollisions(MaterialObject collisionSubject, IEnumerable<MaterialObject> potentialCollisions);
    }
}
