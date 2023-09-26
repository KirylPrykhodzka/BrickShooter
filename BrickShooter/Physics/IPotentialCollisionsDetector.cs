using System.Collections.Generic;

namespace BrickShooter.Physics
{
    public interface IPotentialCollisionsDetector
    {
        IEnumerable<MaterialObject> DetectPotentialCollisions(MaterialObject currentObject, IEnumerable<MaterialObject> otherObjects);
    }
}
