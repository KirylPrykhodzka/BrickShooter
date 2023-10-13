using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;

namespace BrickShooter.Tests.Mocks
{
    public class MaterialObjectMock : MaterialObject
    {
        public MaterialObjectMock() { }

        public MaterialObjectMock(Vector2[] initialColliderPoints)
        {
            this.initialColliderPoints = initialColliderPoints;
        }
    }
}
