using BrickShooter.Physics.Models;
using System.Collections.Generic;

namespace BrickShooter.Physics.Interfaces
{
    public interface ICollisionCalculator
    {        
        IList<CollisionData> GetExistingCollisions(MaterialObject collisionSubject, IEnumerable<MaterialObject> potentialCollisions);
        CollisionPredictionResult FindNextCollision(MaterialObject collisionSubject, IEnumerable<MaterialObject> potentialCollisions);
    }
}
