using Microsoft.Xna.Framework;

namespace BrickShooter.Models
{
    public record Tilemap
    {
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public string TextureName { get; set; }
        public Vector2[] Placements { get; set; }
    }
}
