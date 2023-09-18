using Microsoft.Xna.Framework.Graphics;
using System.Drawing;

namespace BrickShooter.Models
{
    public record LevelData
    {
        public int Width { get; set; }
        public int Height { get; set; }

        private string backgroundTextureName;
        public string BackgroundTextureName
        {
            get
            {
                return backgroundTextureName;
            }
            set
            {
                backgroundTextureName = value;
                BackgroundTexture = GlobalObjects.Content.Load<Texture2D>($"Backgrounds/{backgroundTextureName}");
            }
        }

        public Texture2D BackgroundTexture { get; set; }
        /// <summary>
        /// initial player position relative to level bounds
        /// System.Drawing.Point is used because it is more concise when serialized
        /// </summary>
        public Point InitialPlayerPosition { get; set; }
        public Tilemap Walls { get; set; }

        public LevelData()
        {
        }
    }
}
