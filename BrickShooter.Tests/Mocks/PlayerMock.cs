using BrickShooter.Physics.Models;

namespace BrickShooter.Tests.Mocks
{
    public class PlayerMock : MaterialObject
    {
        public PlayerMock()
        {
            CollisionLayer = "Player";
        }
    }
}
