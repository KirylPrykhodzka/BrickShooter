using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BrickShooter.Physics.Interfaces
{
    public interface IMaterialObjectMover
    {
        void MoveWithoutObstruction(MaterialObject materialObject);
        void ProcessExistingCollisions(MaterialObject materialObject, IList<Vector2> translationVectors);
        void ProcessNextCollisions(MaterialObject currentObject, IList<CollisionPredictionResult> nextCollisions);
    }
}
