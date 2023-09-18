using Microsoft.Xna.Framework.Graphics;
using System.Drawing;

namespace BrickShooter.Models
{
    public record Tilemap
    {
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }

        private string textureName;
        public string TextureName
        {
            get
            {
                return textureName;
            }
            set
            {
                textureName = value;
                Texture = GlobalObjects.Content.Load<Texture2D>($"Walls/{textureName}");
            }
        }

        public Texture2D Texture { get; set; }
        //System.Drawing.Point is used because it is more concise when serialized
        public Point[] Placements { get; set; }
    }
}
