using AutoFixture;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;

namespace BrickShooter.Tests.Mocks
{
    public class MaterialObjectMock : MaterialObject
    {
        private static readonly IFixture fixture = new Fixture();

        public MaterialObjectMock()
        {
            initialColliderPoints = fixture.CreateMany<Vector2>(4).ToArray();
        }

        public MaterialObjectMock(Vector2[] initialColliderPoints)
        {
            this.initialColliderPoints = initialColliderPoints;
        }
    }
}
