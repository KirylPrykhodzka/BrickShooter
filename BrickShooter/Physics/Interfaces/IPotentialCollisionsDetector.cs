using BrickShooter.Physics.Models;
using System.Collections.Generic;

namespace BrickShooter.Physics.Interfaces
{
    public interface IPotentialCollisionsDetector
    {
        PotentialCollisions GetPotentialCollisions(IMaterialObject currentObject, IEnumerable<IMaterialObject> otherObjects);
    }
}
