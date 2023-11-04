using System.Collections.Generic;

namespace BrickShooter.Physics.Interfaces
{
    public interface IPotentialCollisionsDetector
    {
        (IList<IMaterialObject> existing, IList<IMaterialObject> future) GetPotentialCollisions(IMaterialObject currentObject, IEnumerable<IMaterialObject> otherObjects);
    }
}
