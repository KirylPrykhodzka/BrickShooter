using System.Drawing;

namespace BrickShooter.Models
{
    public class Tilemap
    {
        public string Name { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        //System.Drawing.Point is used because it is more concise when serialized
        public Point[] Placements { get; set; }
    }
}
