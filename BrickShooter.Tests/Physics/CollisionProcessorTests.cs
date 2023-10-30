using AutoFixture;
using BrickShooter.Physics;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using BrickShooter.Tests.Mocks;
using FluentAssertions;
using Microsoft.Xna.Framework;
using Moq;
using NUnit.Framework;

namespace BrickShooter.Tests.Physics
{
    public class CollisionProcessorTests
    {
        private IFixture fixture;
        private Mock<IFutureCollisionsCalculator> futureCollisionsCalculatorMock;
        private CollisionProcessor collisionProcessor;

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
            GlobalObjects.DeltaTime = 2f;
            futureCollisionsCalculatorMock = new Mock<IFutureCollisionsCalculator>();
            collisionProcessor = new CollisionProcessor(futureCollisionsCalculatorMock.Object);
        }

        [Test]
        public void ProcessExistingCollisions_Should_ApplyLongestTranslationVector()
        {
            // Arrange
            var position = fixture.Create<Vector2>();
            var materialObject = new MaterialObjectMock { Position = position };
            var collisions = fixture.Build<CollisionInfo>()
                .CreateMany()
                .ToList();

            // Act
            collisionProcessor.ProcessExistingCollisions(materialObject, collisions);

            // Assert
            materialObject.Position.Should().Be(position + collisions.MaxBy(x => x.MinimalTranslationVector.Length()).MinimalTranslationVector);
        }

        [Test]
        public void ProcessNextCollisions_Should_Not_ChangeVelocity()
        {
            // Arrange
            var originalVelocity = fixture.Create<Vector2>();
            var materialObject = new MaterialObjectMock { Position = new Vector2(1, 1), Velocity = originalVelocity };
            var collisions = new List<FutureCollisionInfo>
            {
                new FutureCollisionInfo
                {
                    DistanceToCollision = 2,
                    CollisionEdge = (new Vector2(1, 1), new Vector2(3, 1)),
                    CollisionObject = new MaterialObjectMock()
                }
            };

            futureCollisionsCalculatorMock.Setup(x => x.FindNextCollisions(It.IsAny<MaterialObject>(), It.IsAny<IEnumerable<MaterialObject>>()))
                .Returns(new List<FutureCollisionInfo>());

            // Act
            collisionProcessor.ProcessNextCollisions(materialObject, collisions);

            // Assert
            materialObject.Velocity.Should().Be(originalVelocity);
        }

        [Test]
        public void ProcessNextCollisions_NoNextCollisions_ShouldMoveWithoutObstruction()
        {
            // Arrange
            var position = fixture.Create<Vector2>();
            var velocity = fixture.Create<Vector2>();
            GlobalObjects.DeltaTime = 0.5f;
            var materialObject = new MaterialObjectMock { Position = position, Velocity = velocity };

            // Act
            collisionProcessor.ProcessNextCollisions(materialObject, new List<FutureCollisionInfo>());

            //Assert
            materialObject.Position.Should().Be(position + velocity * GlobalObjects.DeltaTime);
        }

        [Test]
        public void ProcessNextCollisions_Should_HandleCollisionProperly()
        {
            // Arrange
            var position = new Vector2(3, 5);
            var velocity = new Vector2(5, 5);
            var materialObject = new MaterialObjectMock { Position = position, Velocity = velocity };
            var collisions = new List<FutureCollisionInfo>
            {
                new FutureCollisionInfo
                {
                    DistanceToCollision = 1.5f,
                    CollisionEdge = (new Vector2(2, 7), new Vector2(6, 1)),
                    CollisionObject = new MaterialObjectMock()
                }
            };

            futureCollisionsCalculatorMock.Setup(x => x.FindNextCollisions(It.IsAny<MaterialObject>(), It.IsAny<IEnumerable<MaterialObject>>()))
                .Returns(new List<FutureCollisionInfo>());

            // Act
            collisionProcessor.ProcessNextCollisions(materialObject, collisions);

            // Assert
            futureCollisionsCalculatorMock.Verify(x => x.FindNextCollisions(materialObject, It.Is<IEnumerable<MaterialObject>>(x => !x.Any())), Times.Once);
        }

        [Test]
        public void ProcessNextCollisions_Should_HandleMultipleCollisions()
        {
            // Arrange
            var position = fixture.Create<Vector2>();
            var velocity = fixture.Create<Vector2>();
            var materialObject = new MaterialObjectMock { Position = position, Velocity = velocity };
            var collisionInfo1 = new FutureCollisionInfo
            {
                DistanceToCollision = 1,
                CollisionEdge = (fixture.Create<Vector2>(), fixture.Create<Vector2>()),
                CollisionObject = new MaterialObjectMock()
            };
            var collisionInfo2 = new FutureCollisionInfo
            {
                DistanceToCollision = 2,
                CollisionEdge = (fixture.Create<Vector2>(), fixture.Create<Vector2>()),
                CollisionObject = new MaterialObjectMock()
            };
            var collisions = new List<FutureCollisionInfo>
            {
                collisionInfo1,
                collisionInfo2
            };

            futureCollisionsCalculatorMock.Setup(x => x.FindNextCollisions(It.IsAny<MaterialObject>(), It.Is<IEnumerable<MaterialObject>>(x => x.Single() == collisionInfo2.CollisionObject)))
                .Returns(new List<FutureCollisionInfo> { collisionInfo2 });
            futureCollisionsCalculatorMock.Setup(x => x.FindNextCollisions(It.IsAny<MaterialObject>(), It.Is<IEnumerable<MaterialObject>>(x => !x.Any())))
                .Returns(new List<FutureCollisionInfo> { });

            // Act
            collisionProcessor.ProcessNextCollisions(materialObject, collisions);

            // Assert
            futureCollisionsCalculatorMock.Verify(x => x.FindNextCollisions(materialObject, It.Is<IEnumerable<MaterialObject>>(x => x.Count() == 1)), Times.Once);
            futureCollisionsCalculatorMock.Verify(x => x.FindNextCollisions(materialObject, It.Is<IEnumerable<MaterialObject>>(x => !x.Any())), Times.Once);
        }
    }
}
