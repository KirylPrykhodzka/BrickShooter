namespace BrickShooter.Constants
{
    public class PlayerConstants
    {
        public const float MAX_VELOCITY = 300f;
        //when player velocity goes below this value, it will reset to zero
        public const float MIN_VELOCITY = 10f;
        //max velocity will be reached in 5 updates
        public const float ACCELERATION_FACTOR = 0.1f;
        //player decelerates twice as fast as accelerates
        public const float DECELERATION_FACTOR = 0.2f;
        public const int SHOOTING_COOLDOWN_MS = 1000;
    }
}
