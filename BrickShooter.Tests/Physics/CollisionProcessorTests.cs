﻿using AutoFixture;
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
        public void ProcessExistingCollisions_Should_MoveObject_AlongLongestTranslationVector()
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
            materialObjectMoverMock.Verify(x => x.MoveObject(materialObject, longestTranslationVector), Times.Once);
        }

        [Test]
        public void FindAndProcessNextCollisions_Should_Not_ChangeVelocity()
        {
            // Arrange
            var originalVelocity = fixture.Create<Vector2>();
            var materialObject = new MaterialObjectMock { Position = new Vector2(1, 1), Velocity = originalVelocity };
            var collisions = new List<IMaterialObject>
            {
                fixture.Create<MaterialObjectMock>()
            };

            futureCollisionsCalculatorMock.Setup(x => x.FindNextCollisions(It.IsAny<IMaterialObject>(), It.IsAny<IEnumerable<IMaterialObject>>()))
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
            collisionProcessor.FindAndProcessNextCollisions(materialObject, new List<IMaterialObject>());

            //Assert
            materialObjectMoverMock.Verify(x => x.ScheduleMovement(materialObject, velocity * GlobalObjects.DeltaTime), Times.Once);
        }

        [Test]
        public void FindAndProcessNextCollisions_DidNotFindCollisions_ShouldScheduleMovementWithoutObstruction()
        {
            // Arrange
            GlobalObjects.DeltaTime = 0.5f;
            var position = new Vector2(3, 5);
            var velocity = new Vector2(5, 5);
            var materialObject = new MaterialObjectMock { Position = position, Velocity = velocity };
            var collisions = new List<IMaterialObject>
            {
                fixture.Create<MaterialObjectMock>()
            };

            futureCollisionsCalculatorMock.Setup(x => x.FindNextCollisions(It.IsAny<IMaterialObject>(), It.IsAny<IEnumerable<IMaterialObject>>()))
                .Returns(new List<FutureCollisionInfo>());

            // Act
            collisionProcessor.FindAndProcessNextCollisions(materialObject, collisions);

            // Assert
            futureCollisionsCalculatorMock.Verify(x => x.FindNextCollisions(materialObject, It.Is<IEnumerable<IMaterialObject>>(x => x.Count() == 1)), Times.Once);
            futureCollisionsCalculatorMock.Verify(x => x.FindNextCollisions(materialObject, It.Is<IEnumerable<IMaterialObject>>(x => !x.Any())), Times.Never);
            materialObjectMoverMock.Verify(x => x.ScheduleMovement(materialObject, velocity * GlobalObjects.DeltaTime), Times.Once);
        }

        [Test]
        public void FindAndProcessNextCollisions_FoundMultipleNextCollisions_ShouldProcessThemInOrder()
        {
            // Arrange
            GlobalObjects.DeltaTime = 0.5f;
            var position = new Vector2(3, 5);
            var velocity = new Vector2(100, 100);
            var materialObject = new MaterialObjectMock { Position = position, Velocity = velocity };
            var futureCollision1 = new FutureCollisionInfo
            {
                CollisionObject = fixture.Create<MaterialObjectMock>(),
                DistanceToCollision = 1f,
                CollisionEdge = (new Vector2(1, 1), new Vector2(2, 2))
            };
            var futureCollision2 = new FutureCollisionInfo
            {
                CollisionObject = fixture.Create<MaterialObjectMock>(),
                DistanceToCollision = 2f,
                CollisionEdge = (new Vector2(2, 2), new Vector2(3, 3))
            };

            //translation: there are two potential future collisions, and upon calling FindNextCollisions collision processor finds out that both of they will in fact occur
            futureCollisionsCalculatorMock.Setup(x =>
                x.FindNextCollisions(materialObject, It.Is<IEnumerable<IMaterialObject>>(x =>
                    x.Contains(futureCollision1.CollisionObject) && x.Contains(futureCollision2.CollisionObject))))
                .Returns(new List<FutureCollisionInfo> { futureCollision1, futureCollision2 });

            //translation: after processing closest collision, collision processor checks whether second collision is still possible and finds out that it is
            futureCollisionsCalculatorMock.Setup(x =>
                x.FindNextCollisions(materialObject, It.Is<IEnumerable<IMaterialObject>>(x =>
                    x.Count() == 1 && x.First() == futureCollision2.CollisionObject)))
                .Returns(new List<FutureCollisionInfo> { futureCollision2 });

            // Act
            collisionProcessor.FindAndProcessNextCollisions(materialObject, new List<IMaterialObject> { futureCollision1.CollisionObject, futureCollision2.CollisionObject });

            // Assert
            materialObjectMoverMock.Verify(x => x.MoveObject(materialObject, It.IsAny<Vector2>()), Times.Exactly(2));
            materialObjectMoverMock.Verify(x => x.ScheduleMovement(materialObject, It.IsAny<Vector2>()), Times.Once);
        }
    }
}
