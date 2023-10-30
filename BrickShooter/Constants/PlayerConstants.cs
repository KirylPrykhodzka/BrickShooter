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
        public static readonly Vector2 BARREL_TIP_OFFSET = new(20, 12);
        public static readonly Vector2[] INITIAL_COLLIDER_POINTS = new Vector2[]
            {
                new(-24, -21),
                new(0, -21),
                new(0, 21),
                new(-24, 21),
            };
    }
}
