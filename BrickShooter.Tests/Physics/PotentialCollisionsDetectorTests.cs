using BrickShooter.Physics;
using FluentAssertions;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using AutoFixture;
using BrickShooter.Tests.Mocks;

namespace BrickShooter.Tests
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
            var currentObject = fixture.Create<MaterialObjectMock>();
            currentObject.CollisionLayer = "Bullet";

            var otherObject = fixture.Create<MaterialObjectMock>();
            otherObject.CollisionLayer = "Bullet";

            // Act
            var potentialCollisions = _potentialCollisionsDetector.GetPotentialCollisions(currentObject, new List<MaterialObjectMock> { otherObject });

            // Assert
            potentialCollisions.existing.Should().BeEmpty();
        }

        [Test]
        public void DetectPotentialCollisions_Should_DetectPotentialCollisionsBasedOnBounds()
        {
            // Arrange
            var currentObject = new MaterialObjectMock(new Vector2[]
            {
                new(-1, -1),
                new(1, -1),
                new(1, 1),
                new(-1, 1),
            })
            {
                Position = new Vector2(0, 0),
                Velocity = Vector2.Zero,
                CollisionLayer = "Player"
            };

            var potentialCollision1 = new MaterialObjectMock(new Vector2[]
            {
                new(-2, -2),
                new(2, -2),
                new(2, 2),
                new(-2, 2),
            })
            {
                Position = new Vector2(0, 0),
                Velocity = Vector2.Zero,
                CollisionLayer = "PotentialCollision"
            };

            var potentialCollision2 = new MaterialObjectMock(new Vector2[]
            {
                new(-2, -2),
                new(2, -2),
                new(2, 2),
                new(-2, 2),
            })
            {
                Position = new Vector2(10, 10),
                Velocity = Vector2.Zero,
                CollisionLayer = "PotentialCollision"
            };

            // Act
            var potentialCollisions = _potentialCollisionsDetector.GetPotentialCollisions(currentObject, new List<MaterialObjectMock> { potentialCollision1, potentialCollision2 });

            // Assert
            potentialCollisions.existing.Should().HaveCount(1);
            potentialCollisions.existing.Should().Contain(potentialCollision1);
        }

        [Test]
        public void DetectPotentialCollisions_Should_DetectPotentialCollisionsBasedOnProjectedBounds()
        {
            // Arrange
            GlobalObjects.DeltaTime = 1f;
            var currentObject = new MaterialObjectMock(new Vector2[]
            {
                new(-1, -1),
                new(1, -1),
                new(1, 1),
                new(-1, 1),
            })
            {
                Position = Vector2.Zero,
                Velocity = Vector2.Zero,
                CollisionLayer = "Player"
            };

            var potentialCollision = new MaterialObjectMock(new Vector2[]
            {
                new(-2, -2),
                new(2, -2),
                new(2, 2),
                new(-2, 2),
            })
            {
                Position = new Vector2(5, 5),
                Velocity = Vector2.Zero,
                CollisionLayer = "PotentialCollision"
            };

            // Act
            var potentialCollisionsWhileStill = _potentialCollisionsDetector.GetPotentialCollisions(currentObject, new List<MaterialObjectMock> { potentialCollision });
            currentObject.Velocity = new Vector2(4, 4);
            var potentialCollisionsWhileMoving = _potentialCollisionsDetector.GetPotentialCollisions(currentObject, new List<MaterialObjectMock> { potentialCollision });

            // Assert
            potentialCollisionsWhileStill.future.Should().BeEmpty();
            potentialCollisionsWhileMoving.future.Should().HaveCount(1);
            potentialCollisionsWhileMoving.future.Should().Contain(potentialCollision);
        }
    }
}
