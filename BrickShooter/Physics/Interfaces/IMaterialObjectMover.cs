using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BrickShooter.Physics.Interfaces
{
    public interface IMaterialObjectMover
    {
        void MoveWithoutObstruction(MaterialObject materialObject);
        void ProcessExistingCollisions(MaterialObject materialObject, IList<CollisionInfo> translationVectors);
        void ProcessNextCollisions(MaterialObject currentObject, IList<FutureCollisionInfo> nextCollisions);
    }
}
