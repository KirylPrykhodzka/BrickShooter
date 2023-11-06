using BrickShooter.Physics;
using FluentAssertions;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using AutoFixture;
using BrickShooter.Physics.Interfaces;
using Moq;
using BrickShooter.GameObjects;
using MonoGame.Extended;

namespace BrickShooter.Tests.Physics
{
    [TestFixture]
    public class PotentialCollisionsDetectorTests
    {
        private PotentialCollisionsDetector _potentialCollisionsDetector;
        private Fixture fixture;

        [SetUp]
        public void SetUp()
        {
            _potentialCollisionsDetector = new PotentialCollisionsDetector();
            fixture = new Fixture();
        }

        [Test]
        public void DetectPotentialCollisions_Should_IgnoreCollisionsInIgnoredCollisionsDictionary()
        {
            // Arrange
            var currentObjectBodyMock = new Mock<IColliderPolygon>();
            currentObjectBodyMock.SetupGet(x => x.CollisionLayer).Returns(nameof(Bullet));
            currentObjectBodyMock.SetupGet(x => x.Bounds).Returns(new RectangleF(-1, -1, 2, 2));
            var currentObject = new Mock<IMaterialObject>();
            currentObject.SetupGet(x => x.Position).Returns(Vector2.Zero);
            currentObject.SetupGet(x => x.Velocity).Returns(Vector2.Zero);
            currentObject.SetupGet(x => x.SingleCollider).Returns(currentObjectBodyMock.Object);

            var otherObjectBodyMock = new Mock<IColliderPolygon>();
            otherObjectBodyMock.SetupGet(x => x.CollisionLayer).Returns(nameof(Bullet));
            otherObjectBodyMock.SetupGet(x => x.Bounds).Returns(new RectangleF(0, 0, 2, 2));
            var otherObject = new Mock<IMaterialObject>();
            otherObject.SetupGet(x => x.Position).Returns(Vector2.Zero);
            otherObject.SetupGet(x => x.Velocity).Returns(Vector2.Zero);
            otherObject.SetupGet(x => x.SingleCollider).Returns(otherObjectBodyMock.Object);

            // Act
            var potentialCollisions = _potentialCollisionsDetector.GetPotentialCollisions(currentObject.Object, new List<IMaterialObject> { otherObject.Object });

            // Assert
            potentialCollisions.Existing.Should().BeEmpty();
        }

        [Test]
        public void DetectPotentialCollisions_Should_DetectPotentialCollisionsBasedOnBounds()
        {
            // Arrange
            var currentObjectBodyMock = new Mock<IColliderPolygon>();
            currentObjectBodyMock.SetupGet(x => x.CollisionLayer).Returns(nameof(Player));
            currentObjectBodyMock.SetupGet(x => x.Bounds).Returns(new RectangleF(-1, -1, 2, 2));
            var currentObject = new Mock<IMaterialObject>();
            currentObject.SetupGet(x => x.Position).Returns(Vector2.Zero);
            currentObject.SetupGet(x => x.Velocity).Returns(Vector2.Zero);
            currentObject.SetupGet(x => x.SingleCollider).Returns(currentObjectBodyMock.Object);

            var potentialCollision1BodyMock = new Mock<IColliderPolygon>();
            potentialCollision1BodyMock.SetupGet(x => x.CollisionLayer).Returns(fixture.Create("CollisionLayer"));
            potentialCollision1BodyMock.SetupGet(x => x.Bounds).Returns(new RectangleF(0, 0, 2, 2));
            var potentialCollision1 = new Mock<IMaterialObject>();
            potentialCollision1.SetupGet(x => x.Position).Returns(Vector2.Zero);
            potentialCollision1.SetupGet(x => x.Velocity).Returns(Vector2.Zero);
            potentialCollision1.SetupGet(x => x.SingleCollider).Returns(potentialCollision1BodyMock.Object);

            var potentialCollision2BodyMock = new Mock<IColliderPolygon>();
            potentialCollision2BodyMock.SetupGet(x => x.CollisionLayer).Returns(fixture.Create("CollisionLayer"));
            potentialCollision2BodyMock.SetupGet(x => x.Bounds).Returns(new RectangleF(3, 3, 2, 2));
            var potentialCollision2 = new Mock<IMaterialObject>();
            potentialCollision2.SetupGet(x => x.Position).Returns(Vector2.Zero);
            potentialCollision2.SetupGet(x => x.Velocity).Returns(Vector2.Zero);
            potentialCollision2.SetupGet(x => x.SingleCollider).Returns(potentialCollision2BodyMock.Object);

            // Act
            var potentialCollisions = _potentialCollisionsDetector.GetPotentialCollisions(currentObject.Object, new List<IMaterialObject> { potentialCollision1.Object, potentialCollision2.Object });

            // Assert
            potentialCollisions.Existing.Should().HaveCount(1);
            potentialCollisions.Existing.Should().Contain(potentialCollision1.Object.SingleCollider);
        }

        [Test]
        public void DetectPotentialCollisions_Should_DetectPotentialCollisionsBasedOnProjectedBounds()
        {
            // Arrange
            GlobalObjects.DeltaTime = 1f;

            var currentObjectBodyMock = new Mock<IColliderPolygon>();
            currentObjectBodyMock.SetupGet(x => x.CollisionLayer).Returns(nameof(Player));
            currentObjectBodyMock.SetupGet(x => x.Bounds).Returns(new RectangleF(-1, -1, 2, 2));
            var currentObject = new Mock<IMaterialObject>();
            currentObject.SetupGet(x => x.Position).Returns(Vector2.Zero);
            currentObject.SetupGet(x => x.Velocity).Returns(Vector2.Zero);
            currentObject.SetupGet(x => x.SingleCollider).Returns(currentObjectBodyMock.Object);

            var potentialCollisionBodyMock = new Mock<IColliderPolygon>();
            potentialCollisionBodyMock.SetupGet(x => x.CollisionLayer).Returns(fixture.Create("CollisionLayer"));
            potentialCollisionBodyMock.SetupGet(x => x.Bounds).Returns(new RectangleF(4, 4, 2, 2));
            var potentialCollision = new Mock<IMaterialObject>();
            potentialCollision.SetupGet(x => x.Position).Returns(new Vector2(5,5));
            potentialCollision.SetupGet(x => x.Velocity).Returns(Vector2.Zero);
            potentialCollision.SetupGet(x => x.SingleCollider).Returns(potentialCollisionBodyMock.Object);

            // Act
            var potentialCollisionsWhileStill = _potentialCollisionsDetector.GetPotentialCollisions(currentObject.Object, new List<IMaterialObject> { potentialCollision.Object });
            currentObject.SetupGet(x => x.Velocity).Returns(new Vector2(4, 4));
            var potentialCollisionsWhileMoving = _potentialCollisionsDetector.GetPotentialCollisions(currentObject.Object, new List<IMaterialObject> { potentialCollision.Object });

            // Assert
            potentialCollisionsWhileStill.Future.Should().BeEmpty();
            potentialCollisionsWhileMoving.Future.Should().HaveCount(1);
            potentialCollisionsWhileMoving.Future.Should().Contain(potentialCollision.Object.SingleCollider);
        }
    }
}
