using Microsoft.Xna.Framework;

namespace BrickShooter.Models
{
    public record LevelData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string BackgroundTextureName { get; set; }
        public Vector2 InitialPlayerPosition { get; set; }
        public Tilemap Walls { get; set; }
    }
}
