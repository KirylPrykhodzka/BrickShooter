using BrickShooter.Physics.Interfaces;
using System.Collections.Generic;

namespace BrickShooter.Physics.Models
{
    public class PotentialCollisions
    {
        public IList<IColliderPolygon> Existing { get; set; } = new List<IColliderPolygon>();
        public IList<IColliderPolygon> Future { get; set; } = new List<IColliderPolygon>();
    }
}
