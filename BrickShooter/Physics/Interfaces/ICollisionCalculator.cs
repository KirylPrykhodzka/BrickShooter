using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BrickShooter.Physics.Interfaces
{
    public interface ICollisionCalculator
    {        
        IList<Vector2> GetTranslationVectorsForExistingCollisions(MaterialObject collisionSubject, IEnumerable<MaterialObject> potentialCollisions);
        IList<CollisionPredictionResult> FindNextCollisions(MaterialObject collisionSubject, IEnumerable<MaterialObject> potentialCollisions);
    }
}
