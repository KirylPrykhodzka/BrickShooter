using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BrickShooter.Models
{
    public record LevelData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string BackgroundTextureName { get; set; }
        public Vector2 InitialPlayerPosition { get; set; }
        public List<WallData> Walls { get; set; }
        public EnemiesData EnemiesData { get; set; }
    }
}
