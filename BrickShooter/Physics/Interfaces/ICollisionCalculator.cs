using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BrickShooter.Physics.Interfaces
{
    public interface ICollisionCalculator
    {        
        Vector2 GetTranslationVectorForExistingCollisions(MaterialObject collisionSubject, IEnumerable<MaterialObject> potentialCollisions);
        IList<CollisionPredictionResult> FindFutureCollisions(MaterialObject collisionSubject, IEnumerable<MaterialObject> potentialCollisions);
    }
}
