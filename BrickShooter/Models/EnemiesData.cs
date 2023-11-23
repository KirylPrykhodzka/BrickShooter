using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BrickShooter.Models
{
    public class EnemiesData
    {
        public Queue<string> SpawnOrder { get; set; }
        public List<Vector2> SpawnPoints { get; set; }
    }
}
