using System.Collections.Generic;
using BrickShooter.Physics.Models;

namespace BrickShooter.Physics.Interfaces
{
    public interface IPotentialCollisionsDetector
    {
        IList<MaterialObject> DetectPotentialCollisions(MaterialObject currentObject, IEnumerable<MaterialObject> otherObjects);
    }
}
