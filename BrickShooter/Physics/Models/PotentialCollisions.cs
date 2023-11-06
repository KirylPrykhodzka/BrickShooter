using System.Collections.Generic;

namespace BrickShooter.Physics.Models
{
    public class PotentialCollisions
    {
        public IList<CollisionPair> Existing { get; set; } = new List<CollisionPair>();
        public IList<CollisionPair> Future { get; set; } = new List<CollisionPair>();
    }
}
