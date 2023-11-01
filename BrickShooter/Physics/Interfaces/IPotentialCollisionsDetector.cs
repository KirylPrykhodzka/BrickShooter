using System.Collections.Generic;
using BrickShooter.Physics.Models;

namespace BrickShooter.Physics.Interfaces
{
    public interface IPotentialCollisionsDetector
    {
        (IList<IMaterialObject> existing, IList<IMaterialObject> future) GetPotentialCollisions(IMaterialObject currentObject, IEnumerable<IMaterialObject> otherObjects);
    }
}
