using Microsoft.Xna.Framework;

namespace BrickShooter.Constants
{
    public class PlayerConstants
    {
        public const float MAX_MOVE_VELOCITY = 300f;
        // determines how long it takes the player to reach MAX_MOVE_VELOCITY
        public const float ACCELERATION_FACTOR = 0.1f;
        // determines how long it takes the player to stop moving
        public const float DECELERATION_FACTOR = 0.2f;

        public const int DODGE_COOLDOWN_MS = 2500;
        public const float DODGE_VELOCITY = 1200f;
        // determines how quickly player loses velocity after a dodge
        public const float DODGE_RECOVERY_DECELERATION_FACTOR = 0.05f;
        //the period after dodge during which the player cannot take any action
        public const int DODGE_RECOVERY_MS = 500;

        public const int SHOOTING_COOLDOWN_MS = 1000;

        /// <summary>
        /// position of player's gun relative to player's position
        /// </summary>
        public static readonly Vector2 PLAYER_GUN_POSITION = new(13, 10);
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
