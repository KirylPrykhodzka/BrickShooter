using AutoFixture;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;

namespace BrickShooter.Tests.Mocks
{
    public class MaterialObjectMock : MaterialObject
    {
        private static readonly IFixture fixture = new Fixture();

        public MaterialObjectMock() : this(fixture.CreateMany<Vector2>(4).ToArray())
        {
        }

        public MaterialObjectMock(IList<Vector2> initialColliderPoints)
        {
            Colliders.Add(new ColliderPolygon(this, fixture.Create("CollisionLayer"), initialColliderPoints));
        }
    }
}
