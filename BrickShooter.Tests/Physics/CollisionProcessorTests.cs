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
        private Mock<IMaterialObjectMover> materialObjectMoverMock;
        private CollisionProcessor collisionProcessor;

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
            GlobalObjects.DeltaTime = 2f;
            futureCollisionsCalculatorMock = new Mock<IFutureCollisionsCalculator>();
            materialObjectMoverMock = new Mock<IMaterialObjectMover>();
            collisionProcessor = new CollisionProcessor(futureCollisionsCalculatorMock.Object, materialObjectMoverMock.Object);
        }

        [Test]
        public void ProcessExistingCollisions_Should_SheduleMovement_AlongLongestTranslationVector()
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
            var longestTranslationVector = collisions.MaxBy(x => x.MinimalTranslationVector.Length()).MinimalTranslationVector;
            materialObjectMoverMock.Verify(x => x.ScheduleMovement(materialObject, longestTranslationVector), Times.Once);
        }

        [Test]
        public void FindAndProcessNextCollisions_Should_Not_ChangeVelocity()
        {
            // Arrange
            var originalVelocity = fixture.Create<Vector2>();
            var materialObject = new MaterialObjectMock { Position = new Vector2(1, 1), Velocity = originalVelocity };
            var collisions = new List<MaterialObject>
            {
                fixture.Create<MaterialObjectMock>()
            };

            futureCollisionsCalculatorMock.Setup(x => x.FindNextCollisions(It.IsAny<MaterialObject>(), It.IsAny<IEnumerable<MaterialObject>>()))
                .Returns(new List<FutureCollisionInfo>());

            // Act
            collisionProcessor.FindAndProcessNextCollisions(materialObject, collisions);

            // Assert
            materialObject.Velocity.Should().Be(originalVelocity);
        }

        [Test]
        public void FindAndProcessNextCollisions_NoNextCollisions_ShouldMoveWithoutObstruction()
        {
            // Arrange
            var position = fixture.Create<Vector2>();
            var velocity = fixture.Create<Vector2>();
            GlobalObjects.DeltaTime = 0.5f;
            var materialObject = new MaterialObjectMock { Position = position, Velocity = velocity };

            // Act
            collisionProcessor.FindAndProcessNextCollisions(materialObject, new List<MaterialObject>());

            //Assert
            materialObjectMoverMock.Verify(x => x.ScheduleMovement(materialObject, velocity * GlobalObjects.DeltaTime), Times.Once);
        }

        [Test]
        public void FindAndProcessNextCollisions_Should_HandleCollisionProperly()
        {
            // Arrange
            var position = new Vector2(3, 5);
            var velocity = new Vector2(5, 5);
            var materialObject = new MaterialObjectMock { Position = position, Velocity = velocity };
            var collisions = new List<MaterialObject>
            {
                fixture.Create<MaterialObjectMock>()
            };

            futureCollisionsCalculatorMock.Setup(x => x.FindNextCollisions(It.IsAny<MaterialObject>(), It.IsAny<IEnumerable<MaterialObject>>()))
                .Returns(new List<FutureCollisionInfo>());

            // Act
            collisionProcessor.FindAndProcessNextCollisions(materialObject, collisions);

            // Assert
            futureCollisionsCalculatorMock.Verify(x => x.FindNextCollisions(materialObject, It.Is<IEnumerable<MaterialObject>>(x => x.Count() == 1)), Times.Once);
            futureCollisionsCalculatorMock.Verify(x => x.FindNextCollisions(materialObject, It.Is<IEnumerable<MaterialObject>>(x => !x.Any())), Times.Never);
        }
    }
}
