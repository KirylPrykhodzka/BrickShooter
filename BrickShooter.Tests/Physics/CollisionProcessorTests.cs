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
            GlobalObjects.AbsoluteDeltaTime = 2f;
            futureCollisionsCalculatorMock = new Mock<IFutureCollisionsCalculator>();
            materialObjectMoverMock = new Mock<IMaterialObjectMover>();
            collisionProcessor = new CollisionProcessor(futureCollisionsCalculatorMock.Object, materialObjectMoverMock.Object);
        }

        [Test]
        public void ProcessExistingCollisions_Should_TranslateObject_ByTheTranslationVectorsSum()
        {
            // Arrange
            var position = fixture.Create<Vector2>();
            var materialObject = new MaterialObjectMock { Position = position };
            fixture.Register<IColliderPolygon>(() => new Mock<ColliderPolygon>().Object);
            var collisions = new List<RotationCollisionInfo>
            {
                new RotationCollisionInfo
                {
                    MinimalTranslationVector = fixture.Create<Vector2>(),
                },
                new RotationCollisionInfo
                {
                    MinimalTranslationVector = fixture.Create<Vector2>(),
                },
            };

            // Act
            collisionProcessor.ProcessExistingCollisions(materialObject, collisions);

            // Assert
            var translationVectorsSum = collisions.Aggregate(Vector2.Zero, (sum, x) => sum + x.MinimalTranslationVector);
            materialObjectMoverMock.Verify(x => x.MoveObject(materialObject, translationVectorsSum), Times.Once);
        }

        [Test]
        public void FindAndProcessNextCollisions_Should_Not_ChangeVelocity()
        {
            // Arrange
            var originalVelocity = fixture.Create<Vector2>();
            var materialObject = new MaterialObjectMock { Position = new Vector2(1, 1), Velocity = originalVelocity };
            var collisions = new List<CollisionPair>
            {
                new Mock<CollisionPair>().Object,
            };

            futureCollisionsCalculatorMock.Setup(x => x.FindNextCollisions(It.IsAny<IList<CollisionPair>>()))
                .Returns(new List<MovementCollisionInfo>());

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
            GlobalObjects.AbsoluteDeltaTime = 0.5f;
            var materialObject = new MaterialObjectMock { Position = position, Velocity = velocity };

            // Act
            collisionProcessor.FindAndProcessNextCollisions(materialObject, new List<CollisionPair>());

            //Assert
            materialObjectMoverMock.Verify(x => x.MoveObject(materialObject, velocity * GlobalObjects.ScaledDeltaTime), Times.Once);
        }

        [Test]
        public void FindAndProcessNextCollisions_DidNotFindCollisions_ShouldScheduleMovementWithoutObstruction()
        {
            // Arrange
            GlobalObjects.AbsoluteDeltaTime = 0.5f;
            var position = new Vector2(3, 5);
            var velocity = new Vector2(5, 5);
            var materialObject = new MaterialObjectMock { Position = position, Velocity = velocity };
            var collisions = new List<CollisionPair>
            {
                new Mock<CollisionPair>().Object,
            };

            futureCollisionsCalculatorMock.Setup(x => x.FindNextCollisions(It.IsAny<IList<CollisionPair>>()))
                .Returns(new List<MovementCollisionInfo>());

            // Act
            collisionProcessor.FindAndProcessNextCollisions(materialObject, collisions);

            // Assert
            futureCollisionsCalculatorMock.Verify(x => x.FindNextCollisions(It.Is<IList<CollisionPair>>(x => x.Count() == 1)), Times.Once);
            futureCollisionsCalculatorMock.Verify(x => x.FindNextCollisions(It.Is<IList<CollisionPair>>(x => !x.Any())), Times.Never);
            materialObjectMoverMock.Verify(x => x.MoveObject(materialObject, velocity * GlobalObjects.ScaledDeltaTime), Times.Once);
        }

        [Test]
        public void FindAndProcessNextCollisions_FoundMultipleNextCollisions_ShouldProcessThemInOrder()
        {
            // Arrange
            GlobalObjects.AbsoluteDeltaTime = 0.5f;
            var position = new Vector2(3, 5);
            var velocity = new Vector2(100, 100);
            var materialObject = new MaterialObjectMock { Position = position, Velocity = velocity };
            var futureCollision1 = new MovementCollisionInfo
            {
                CollisionPair = new Mock<CollisionPair>().Object,
                DistanceToCollision = 1f,
                CollisionEdge = new Vector2(1, 1)
            };
            var futureCollision2 = new MovementCollisionInfo
            {
                CollisionPair = new Mock<CollisionPair>().Object,
                DistanceToCollision = 2f,
                CollisionEdge = new Vector2(1, 1)
            };

            //translation: there are two potential future collisions, and upon calling FindNextCollisions collision processor finds out that both of they will in fact occur
            futureCollisionsCalculatorMock.Setup(x =>
                x.FindNextCollisions(It.Is<IList<CollisionPair>>(l =>
                    l.Contains(futureCollision1.CollisionPair) && l.Contains(futureCollision2.CollisionPair))))
                .Returns(new List<MovementCollisionInfo> { futureCollision1, futureCollision2 });

            //translation: after processing closest collision, collision processor checks whether second collision is still possible and finds out that it is
            futureCollisionsCalculatorMock.Setup(x =>
                x.FindNextCollisions(It.Is<IList<CollisionPair>>(x =>
                    x.Count() == 1 && x.First() == futureCollision2.CollisionPair)))
                .Returns(new List<MovementCollisionInfo> { futureCollision2 });

            // Act
            collisionProcessor.FindAndProcessNextCollisions(materialObject, new List<CollisionPair> { futureCollision1.CollisionPair, futureCollision2.CollisionPair });

            // Assert
            materialObjectMoverMock.Verify(x => x.MoveObject(materialObject, It.IsAny<Vector2>()), Times.Exactly(3));
        }

        [Test]
        public void FindAndProcessNextCollisions_FoundMultiplCollisions_ShouldBounceOffTheFirstCollision()
        {
            // Arrange
            GlobalObjects.AbsoluteDeltaTime = 0.5f;
            var position = new Vector2(3, 5);
            var velocity = new Vector2(100, 100);
            var materialObject = new MaterialObjectMock { Position = position, Velocity = velocity, Bounciness = 0.5f };
            var futureCollision1 = new MovementCollisionInfo
            {
                CollisionPair = new Mock<CollisionPair>().Object,
                DistanceToCollision = 1f,
                CollisionEdge = new Vector2(1, 1),
                Normal = new Vector2(-1, -1),
            };
            var futureCollision2 = new MovementCollisionInfo
            {
                CollisionPair = new Mock<CollisionPair>().Object,
                DistanceToCollision = 2f,
                CollisionEdge = new Vector2(1, 1)
            };

            //translation: there are two potential future collisions, and upon calling FindNextCollisions collision processor finds out that both of they will in fact occur
            futureCollisionsCalculatorMock.Setup(x =>
                x.FindNextCollisions(It.Is<IList<CollisionPair>>(l =>
                    l.Contains(futureCollision1.CollisionPair) && l.Contains(futureCollision2.CollisionPair))))
                .Returns(new List<MovementCollisionInfo> { futureCollision1, futureCollision2 });

            //translation: after processing closest collision, collision processor checks whether second collision is still possible and finds out that it is
            futureCollisionsCalculatorMock.Setup(x =>
                x.FindNextCollisions(It.Is<IList<CollisionPair>>(x =>
                    x.Count() == 1 && x.First() == futureCollision2.CollisionPair)))
                .Returns(new List<MovementCollisionInfo> { futureCollision2 });

            // Act
            collisionProcessor.FindAndProcessNextCollisions(materialObject, new List<CollisionPair> { futureCollision1.CollisionPair, futureCollision2.CollisionPair });

            // Assert
            materialObjectMoverMock.Verify(x => x.MoveObject(materialObject, It.IsAny<Vector2>()), Times.Exactly(2));
            materialObject.Velocity.Should().Be(Vector2.Reflect(velocity, futureCollision1.Normal) * materialObject.Bounciness);
        }
    }
}
