namespace BrickShooter.Constants
{
    public class PlayerConstants
    {
        public const int MAX_VELOCITY = 300;
        //when player velocity goes below this value, it will reset to zero
        public const int MIN_VELOCITY = 10;
        //max velocity will be reached in 5 updates
        public const float ACCELERATION_FACTOR = 0.1f;
        //player decelerates twice as fast as accelerates
        public const float DECELERATION_FACTOR = 0.2f;
    }
}
