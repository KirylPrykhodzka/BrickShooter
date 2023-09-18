using System.Drawing;

namespace BrickShooter.Models
{
    public record LevelData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string BackgroundTexture { get; set; }
        public Point InitialPlayerPosition { get; set; }
        public Tilemap[] Tilemaps { get; set; }
    }
}
