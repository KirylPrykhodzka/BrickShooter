using System.Drawing;

namespace BrickShooter.Models
{
    public record LevelData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string BackgroundTexture { get; set; }
        /// <summary>
        /// initial player position relative to level bounds
        /// System.Drawing.Point is used because it is more concise when serialized
        /// </summary>
        public Point InitialPlayerPosition { get; set; }
        public Tilemap[] Tilemaps { get; set; }
    }
}
