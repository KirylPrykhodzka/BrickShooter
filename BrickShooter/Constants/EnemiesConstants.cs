using Microsoft.Xna.Framework;

namespace BrickShooter.Constants
{
    public static class EnemiesConstants
    {
        public const int SPAWN_COOLDOWN_MS = 1000;
        public const float VELOCITY = 100f;
        public static readonly Vector2[] INITIAL_COLLIDER_POINTS = new Vector2[]
            {
                new(-16, -32),
                new(16, -32),
                new(16, 32),
                new(-16, 32),
            };
    }
}
