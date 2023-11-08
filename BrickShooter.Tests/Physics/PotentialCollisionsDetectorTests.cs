using BrickShooter.Physics;
using FluentAssertions;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using AutoFixture;
using BrickShooter.Physics.Interfaces;
using Moq;
using BrickShooter.GameObjects;
using MonoGame.Extended;
using BrickShooter.Physics.Models;

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
            currentObject.SetupGet(x => x.Colliders).Returns(new List<IColliderPolygon> { currentObjectBodyMock.Object });

            var otherObjectBodyMock = new Mock<IColliderPolygon>();
            otherObjectBodyMock.SetupGet(x => x.CollisionLayer).Returns(nameof(Bullet));
            otherObjectBodyMock.SetupGet(x => x.Bounds).Returns(new RectangleF(0, 0, 2, 2));
            var otherObject = new Mock<IMaterialObject>();
            otherObject.SetupGet(x => x.Position).Returns(Vector2.Zero);
            otherObject.SetupGet(x => x.Velocity).Returns(Vector2.Zero);
            otherObject.SetupGet(x => x.Colliders).Returns(new List<IColliderPolygon> { otherObjectBodyMock.Object });

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
            currentObject.SetupGet(x => x.Colliders).Returns(new List<IColliderPolygon> { currentObjectBodyMock.Object });
            currentObjectBodyMock.SetupGet(x => x.Owner).Returns(currentObject.Object);

            var potentialCollision1BodyMock = new Mock<IColliderPolygon>();
            potentialCollision1BodyMock.SetupGet(x => x.CollisionLayer).Returns(fixture.Create("CollisionLayer"));
            potentialCollision1BodyMock.SetupGet(x => x.Bounds).Returns(new RectangleF(0, 0, 2, 2));
            var potentialCollision1 = new Mock<IMaterialObject>();
            potentialCollision1.SetupGet(x => x.Position).Returns(Vector2.Zero);
            potentialCollision1.SetupGet(x => x.Velocity).Returns(Vector2.Zero);
            potentialCollision1.SetupGet(x => x.Colliders).Returns(new List<IColliderPolygon> { potentialCollision1BodyMock.Object });
            potentialCollision1BodyMock.SetupGet(x => x.Owner).Returns(potentialCollision1.Object);

            var potentialCollision2BodyMock = new Mock<IColliderPolygon>();
            potentialCollision2BodyMock.SetupGet(x => x.CollisionLayer).Returns(fixture.Create("CollisionLayer"));
            potentialCollision2BodyMock.SetupGet(x => x.Bounds).Returns(new RectangleF(3, 3, 2, 2));
            var potentialCollision2 = new Mock<IMaterialObject>();
            potentialCollision2.SetupGet(x => x.Position).Returns(Vector2.Zero);
            potentialCollision2.SetupGet(x => x.Velocity).Returns(Vector2.Zero);
            potentialCollision2.SetupGet(x => x.Colliders).Returns(new List<IColliderPolygon> { potentialCollision2BodyMock.Object });
            potentialCollision2BodyMock.SetupGet(x => x.Owner).Returns(potentialCollision2.Object);

            // Act
            var potentialCollisions = _potentialCollisionsDetector.GetPotentialCollisions(currentObject.Object, new List<IMaterialObject> { potentialCollision1.Object, potentialCollision2.Object });

            // Assert
            potentialCollisions.Existing.Should().HaveCount(1);
            potentialCollisions.Existing.First().CollisionSubject.Should().Be(currentObjectBodyMock.Object);
            potentialCollisions.Existing.First().CollisionObject.Should().Be(potentialCollision1BodyMock.Object);
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
            currentObject.SetupGet(x => x.Colliders).Returns(new List<IColliderPolygon> { currentObjectBodyMock.Object });
            currentObjectBodyMock.SetupGet(x => x.Owner).Returns(currentObject.Object);

            var potentialCollisionBodyMock = new Mock<IColliderPolygon>();
            potentialCollisionBodyMock.SetupGet(x => x.CollisionLayer).Returns(fixture.Create("CollisionLayer"));
            potentialCollisionBodyMock.SetupGet(x => x.Bounds).Returns(new RectangleF(4, 4, 2, 2));
            var potentialCollision = new Mock<IMaterialObject>();
            potentialCollision.SetupGet(x => x.Position).Returns(new Vector2(5,5));
            potentialCollision.SetupGet(x => x.Velocity).Returns(Vector2.Zero);
            potentialCollision.SetupGet(x => x.Colliders).Returns(new List<IColliderPolygon> { potentialCollisionBodyMock.Object });
            potentialCollisionBodyMock.SetupGet(x => x.Owner).Returns(potentialCollision.Object);

            // Act
            var potentialCollisionsWhileStill = _potentialCollisionsDetector.GetPotentialCollisions(currentObject.Object, new List<IMaterialObject> { potentialCollision.Object });
            currentObject.SetupGet(x => x.Velocity).Returns(new Vector2(4, 4));
            var potentialCollisionsWhileMoving = _potentialCollisionsDetector.GetPotentialCollisions(currentObject.Object, new List<IMaterialObject> { potentialCollision.Object });

            // Assert
            potentialCollisionsWhileStill.Future.Should().BeEmpty();
            potentialCollisionsWhileMoving.Future.Should().HaveCount(1);
            potentialCollisionsWhileMoving.Future.First().CollisionSubject.Should().Be(currentObjectBodyMock.Object);
            potentialCollisionsWhileMoving.Future.First().CollisionObject.Should().Be(potentialCollisionBodyMock.Object);
        }

        [Test]
        public void DetectPotentialCollisions_SubjectAndObjectHaveMultipleColliders_ShouldDetectExistingCollisionsCorrectly()
        {
            // Arrange
            var currentObjectBodyMock = new Mock<IColliderPolygon>();
            currentObjectBodyMock.SetupGet(x => x.CollisionLayer).Returns(nameof(Player));
            currentObjectBodyMock.SetupGet(x => x.Bounds).Returns(new RectangleF(-1, -1, 2, 2));

            var currentObjectSecondColliderMock = new Mock<IColliderPolygon>();
            currentObjectSecondColliderMock.SetupGet(x => x.CollisionLayer).Returns(nameof(Player));
            currentObjectSecondColliderMock.SetupGet(x => x.Bounds).Returns(new RectangleF(5, 5, 1, 1));

            var currentObject = new Mock<IMaterialObject>();
            currentObject.SetupGet(x => x.Position).Returns(Vector2.Zero);
            currentObject.SetupGet(x => x.Velocity).Returns(Vector2.Zero);
            currentObject.SetupGet(x => x.Colliders).Returns(new List<IColliderPolygon>
            {
                currentObjectSecondColliderMock.Object,
                currentObjectBodyMock.Object,
            });

            currentObjectBodyMock.SetupGet(x => x.Owner).Returns(currentObject.Object);
            currentObjectSecondColliderMock.SetupGet(x => x.Owner).Returns(currentObject.Object);

            var potentialCollisionBodyMock = new Mock<IColliderPolygon>();
            potentialCollisionBodyMock.SetupGet(x => x.CollisionLayer).Returns(fixture.Create("CollisionLayer"));
            potentialCollisionBodyMock.SetupGet(x => x.Bounds).Returns(new RectangleF(0, 0, 2, 2));

            var potentialCollisionSecondColliderMock = new Mock<IColliderPolygon>();
            potentialCollisionSecondColliderMock.SetupGet(x => x.CollisionLayer).Returns(nameof(Player));
            potentialCollisionSecondColliderMock.SetupGet(x => x.Bounds).Returns(new RectangleF(7, 7, 1, 1));

            var potentialCollision = new Mock<IMaterialObject>();
            potentialCollision.SetupGet(x => x.Position).Returns(Vector2.Zero);
            potentialCollision.SetupGet(x => x.Velocity).Returns(Vector2.Zero);
            potentialCollision.SetupGet(x => x.Colliders).Returns(new List<IColliderPolygon>
            {
                potentialCollisionBodyMock.Object,
                potentialCollisionSecondColliderMock.Object
            });
            potentialCollisionBodyMock.SetupGet(x => x.Owner).Returns(potentialCollision.Object);
            potentialCollisionSecondColliderMock.SetupGet(x => x.Owner).Returns(potentialCollision.Object);

            // Act
            var potentialCollisions = _potentialCollisionsDetector.GetPotentialCollisions(currentObject.Object, new List<IMaterialObject> { potentialCollision.Object });

            // Assert
            potentialCollisions.Existing.Should().HaveCount(1);
            potentialCollisions.Existing.First().CollisionSubject.Should().Be(currentObjectBodyMock.Object);
            potentialCollisions.Existing.First().CollisionObject.Should().Be(potentialCollisionBodyMock.Object);
        }

        [Test]
        public void DetectPotentialCollisions_SubjectAndObjectHaveMultipleColliders_ShouldDetectFutureCollisionsCorrectly()
        {
            // Arrange
            var currentObjectBodyMock = new Mock<IColliderPolygon>();
            currentObjectBodyMock.SetupGet(x => x.CollisionLayer).Returns(nameof(Player));
            currentObjectBodyMock.SetupGet(x => x.Bounds).Returns(new RectangleF(-1, -1, 2, 2));

            var currentObjectSecondColliderMock = new Mock<IColliderPolygon>();
            currentObjectSecondColliderMock.SetupGet(x => x.CollisionLayer).Returns(nameof(Player));
            currentObjectSecondColliderMock.SetupGet(x => x.Bounds).Returns(new RectangleF(1, -3, 2, 2));

            var currentObject = new Mock<IMaterialObject>();
            currentObject.SetupGet(x => x.Position).Returns(Vector2.Zero);
            currentObject.SetupGet(x => x.Velocity).Returns(new Vector2(2, 2));
            currentObject.SetupGet(x => x.Colliders).Returns(new List<IColliderPolygon>
            {
                currentObjectSecondColliderMock.Object,
                currentObjectBodyMock.Object,
            });

            currentObjectBodyMock.SetupGet(x => x.Owner).Returns(currentObject.Object);
            currentObjectSecondColliderMock.SetupGet(x => x.Owner).Returns(currentObject.Object);

            var potentialCollisionBodyMock = new Mock<IColliderPolygon>();
            potentialCollisionBodyMock.SetupGet(x => x.CollisionLayer).Returns(fixture.Create("CollisionLayer"));
            potentialCollisionBodyMock.SetupGet(x => x.Bounds).Returns(new RectangleF(1, 2, 2, 2));

            var potentialCollisionSecondColliderMock = new Mock<IColliderPolygon>();
            potentialCollisionSecondColliderMock.SetupGet(x => x.CollisionLayer).Returns(nameof(Player));
            potentialCollisionSecondColliderMock.SetupGet(x => x.Bounds).Returns(new RectangleF(1, 4, 1, 2));

            var potentialCollision = new Mock<IMaterialObject>();
            potentialCollision.SetupGet(x => x.Position).Returns(Vector2.Zero);
            potentialCollision.SetupGet(x => x.Velocity).Returns(new Vector2(-1, -1));
            potentialCollision.SetupGet(x => x.Colliders).Returns(new List<IColliderPolygon>
            {
                potentialCollisionBodyMock.Object,
                potentialCollisionSecondColliderMock.Object
            });
            potentialCollisionBodyMock.SetupGet(x => x.Owner).Returns(potentialCollision.Object);
            potentialCollisionSecondColliderMock.SetupGet(x => x.Owner).Returns(potentialCollision.Object);

            // Act
            var potentialCollisions = _potentialCollisionsDetector.GetPotentialCollisions(currentObject.Object, new List<IMaterialObject> { potentialCollision.Object });

            // Assert
            potentialCollisions.Future.Should().HaveCount(1);
            potentialCollisions.Future.First().CollisionSubject.Should().Be(currentObjectBodyMock.Object);
            potentialCollisions.Future.First().CollisionObject.Should().Be(potentialCollisionBodyMock.Object);
        }

        [Test]
        public void DetectPotentialCollisions_ShouldNotReadAllColliders_IfObjectHasSingleCollider()
        {
            // Arrange
            var currentObjectMock = new Mock<IMaterialObject>();
            currentObjectMock.SetupGet(x => x.SingleCollider).Returns(new ColliderPolygon(currentObjectMock.Object, fixture.Create("CollisionLayer"), fixture.Create<IList<Vector2>>()));
            var otherObjectMock = new Mock<IMaterialObject>();
            otherObjectMock.SetupGet(x => x.SingleCollider).Returns(new ColliderPolygon(currentObjectMock.Object, fixture.Create("CollisionLayer"), fixture.Create<IList<Vector2>>()));

            // Act
            _potentialCollisionsDetector.GetPotentialCollisions(currentObjectMock.Object, new List<IMaterialObject> { otherObjectMock.Object });

            // Assert
            currentObjectMock.Verify(x => x.SingleCollider, Times.Once);
            otherObjectMock.Verify(x => x.SingleCollider, Times.Once);
            currentObjectMock.Verify(x => x.Colliders, Times.Never);
            otherObjectMock.Verify(x => x.Colliders, Times.Never);
        }
    }
}
