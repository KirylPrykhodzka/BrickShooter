using System.Collections.Generic;
using BrickShooter.Physics.Models;

namespace BrickShooter.Physics.Interfaces
{
    public interface IPotentialCollisionsDetector
    {
        (IList<MaterialObject> existing, IList<MaterialObject> future) GetPotentialCollisions(MaterialObject currentObject, IEnumerable<MaterialObject> otherObjects);
    }
}
