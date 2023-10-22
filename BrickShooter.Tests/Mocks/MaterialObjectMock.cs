using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;

namespace BrickShooter.Tests.Mocks
{
    public class MaterialObjectMock : MaterialObject
    {
        public MaterialObjectMock()
        {
            initialColliderPoints = Array.Empty<Vector2>();
        }

        public MaterialObjectMock(Vector2[] initialColliderPoints)
        {
            this.initialColliderPoints = initialColliderPoints;
        }
    }
}
