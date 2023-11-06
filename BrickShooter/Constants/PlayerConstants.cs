using Microsoft.Xna.Framework;

namespace BrickShooter.Constants
{
    public class PlayerConstants
    {
        public const float MAX_VELOCITY = 300f;
        //max velocity will be reached in 10 updates
        public const float ACCELERATION_FACTOR = 0.1f;
        //player decelerates twice as fast as accelerates
        public const float DECELERATION_FACTOR = ACCELERATION_FACTOR * 2;

        public const int SHOOTING_COOLDOWN_MS = 100;

        /// <summary>
        /// position of player's gun relative to player's position
        /// </summary>
        public static readonly Vector2 PLAYER_GUN_POSITION = new(10, 10);
        public static readonly Vector2[] INITIAL_GUN_COLLIDER_POINTS = new Vector2[]
            {
                new(2, 5),
                new(40, 5),
                new(40, 18),
                new(2, 18),
            };
        public static readonly Vector2[] INITIAL_COLLIDER_POINTS = new Vector2[]
            {
                new(-24, -21),
                new(0, -21),
                new(0, 21),
                new(-24, 21),
            };
    }
}
