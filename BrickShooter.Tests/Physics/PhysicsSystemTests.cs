using BrickShooter.Physics;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using Moq;
using NUnit.Framework;
using AutoFixture;
using AutoFixture.AutoMoq;
using System.Numerics;
using BrickShooter.Tests.Mocks;
using BrickShooter.Resources;

namespace BrickShooter.Tests.Physics
{
    [TestFixture]
    public class PhysicsSystemTests
    {
        private PhysicsSystem physicsSystem;
        private Mock<IPotentialCollisionsDetector> potentialCollisionsDetectorMock;
        private Mock<IExistingCollisionsCalculator> existingCollisionsCalculatorMock;
        private Mock<IPool<CollisionPair>> collisionPairPoolMock;
        private Mock<ICollisionProcessor> collisionProcessorMock;
        private Fixture fixture;

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());

            potentialCollisionsDetectorMock = new Mock<IPotentialCollisionsDetector>();
            existingCollisionsCalculatorMock = new Mock<IExistingCollisionsCalculator>();
            collisionPairPoolMock = new Mock<IPool<CollisionPair>>();
            collisionProcessorMock = new Mock<ICollisionProcessor>();

            physicsSystem = new PhysicsSystem(
                potentialCollisionsDetectorMock.Object,
                existingCollisionsCalculatorMock.Object,
                collisionProcessorMock.Object,
                collisionPairPoolMock.Object);
        }

        [Test]
        public void RegisterMobileObject_Should_CallDetectPotentialCollisions()
        {
            // Arrange
            var mobileObject = fixture.Create<MaterialObjectMock>();
            potentialCollisionsDetectorMock.Setup(x => x.GetPotentialCollisions(mobileObject, It.IsAny<IEnumerable<IMaterialObject>>()))
                .Returns(new PotentialCollisions());

            // Act
            physicsSystem.RegisterMobileObject(mobileObject);
            physicsSystem.Run();

            // Assert
            potentialCollisionsDetectorMock.Verify(p => p.GetPotentialCollisions(mobileObject, It.IsAny<IEnumerable<IMaterialObject>>()), Times.Once);
        }

        [Test]
        public void RegisterMultipleMobileObjects_Should_CallDetectPotentialCollisionsForEach()
        {
            // Arrange
            var mobileObject1 = fixture.Create<MaterialObjectMock>();
            var mobileObject2 = fixture.Create<MaterialObjectMock>();
            potentialCollisionsDetectorMock.Setup(x => x.GetPotentialCollisions(It.IsAny<IMaterialObject>(), It.IsAny<IEnumerable<IMaterialObject>>()))
                .Returns(new PotentialCollisions());

            // Act
            physicsSystem.RegisterMobileObject(mobileObject1);
            physicsSystem.RegisterMobileObject(mobileObject2);
            physicsSystem.Run();

            // Assert
            potentialCollisionsDetectorMock.Verify(p => p.GetPotentialCollisions(mobileObject1, It.Is<IEnumerable<IMaterialObject>>(x => x.Contains(mobileObject2))), Times.Once);
            potentialCollisionsDetectorMock.Verify(p => p.GetPotentialCollisions(mobileObject2, It.Is<IEnumerable<IMaterialObject>>(x => x.Contains(mobileObject1))), Times.Once);
        }

        [Test]
        public void UnregisterMobileObject_Should_NotCallDetectPotentialCollisions()
        {
            // Arrange
            var mobileObject = fixture.Create<MaterialObjectMock>();
            physicsSystem.RegisterMobileObject(mobileObject);

            // Act
            physicsSystem.UnregisterMobileObject(mobileObject);
            physicsSystem.Run();

            // Assert
            potentialCollisionsDetectorMock.Verify(p => p.GetPotentialCollisions(mobileObject, It.IsAny<IEnumerable<IMaterialObject>>()), Times.Never);
        }

        [Test]
        public void RegisterImmobileObject_Should_CallDetectPotentialCollisions()
        {
            // Arrange
            var mobileObject = fixture.Create<MaterialObjectMock>();
            physicsSystem.RegisterMobileObject(mobileObject);
            var immobileObject = fixture.Create<MaterialObjectMock>();
            potentialCollisionsDetectorMock.Setup(x => x.GetPotentialCollisions(mobileObject, It.Is<IEnumerable<IMaterialObject>>(x => x.Contains(immobileObject))))
                .Returns(new PotentialCollisions());

            // Act
            physicsSystem.RegisterImmobileObject(immobileObject);
            physicsSystem.Run();

            // Assert
            potentialCollisionsDetectorMock.Verify(p => p.GetPotentialCollisions(mobileObject, It.Is<IEnumerable<IMaterialObject>>(x => x.Contains(immobileObject))), Times.Once);
        }

        [Test]
        public void UnregisterImmobileObject_Should_NotCallDetectPotentialCollisions()
        {
            // Arrange
            var mobileObject = fixture.Create<MaterialObjectMock>();
            physicsSystem.RegisterMobileObject(mobileObject);
            var immobileObject = fixture.Create<MaterialObjectMock>();
            physicsSystem.RegisterImmobileObject(immobileObject);
            potentialCollisionsDetectorMock.Setup(x => x.GetPotentialCollisions(mobileObject, It.IsAny<IEnumerable<IMaterialObject>>()))
                .Returns(new PotentialCollisions());

            // Act
            physicsSystem.UnregisterImmobileObject(immobileObject);
            physicsSystem.Run();

            // Assert
            potentialCollisionsDetectorMock.Verify(p => p.GetPotentialCollisions(mobileObject, It.Is<IEnumerable<IMaterialObject>>(x => x.Contains(immobileObject))), Times.Never);
        }

        [Test]
        public void Run_ShouldCallDetectPotentialCollisions_WhenVelocityIsNonZeroOrDidRotateIsTrue()
        {
            // Arrange
            var mobileObject1 = new MaterialObjectMock { Velocity = new Vector2(1, 0), DidRotate = false };
            var mobileObject2 = new MaterialObjectMock { Velocity = Vector2.Zero, DidRotate = true };
            physicsSystem.RegisterMobileObject(mobileObject1);
            physicsSystem.RegisterMobileObject(mobileObject2);
            potentialCollisionsDetectorMock.Setup(x => x.GetPotentialCollisions(It.IsAny<IMaterialObject>(), It.IsAny<IEnumerable<IMaterialObject>>()))
                .Returns(new PotentialCollisions());

            // Act
            physicsSystem.Run();

            // Assert
            potentialCollisionsDetectorMock.Verify(p => p.GetPotentialCollisions(mobileObject1, It.IsAny<IEnumerable<IMaterialObject>>()), Times.Once);
            potentialCollisionsDetectorMock.Verify(p => p.GetPotentialCollisions(mobileObject2, It.IsAny<IEnumerable<IMaterialObject>>()), Times.Once);
        }

        [Test]
        public void Run_ObjectNotMovingAndDidNotRotate_ShouldSkipObject()
        {
            // Arrange
            var mobileObject = new MaterialObjectMock { Velocity = Vector2.Zero, DidRotate = false };
            physicsSystem.RegisterMobileObject(mobileObject);

            // Act
            physicsSystem.Run();

            // Assert
            potentialCollisionsDetectorMock.Verify(p => p.GetPotentialCollisions(mobileObject, It.IsAny<IEnumerable<IMaterialObject>>()), Times.Never);
        }

        [Test]
        public void Run_ShouldNotCheckExistingCollisions_WhenObjectDidNotRotate()
        {
            // Arrange
            var mobileObject = new MaterialObjectMock { Velocity = new Vector2(1, 0), DidRotate = false };
            physicsSystem.RegisterMobileObject(mobileObject);
            potentialCollisionsDetectorMock.Setup(x => x.GetPotentialCollisions(It.IsAny<IMaterialObject>(), It.IsAny<IEnumerable<IMaterialObject>>()))
                .Returns(fixture.Build<PotentialCollisions>().With(x => x.Existing, new List<CollisionPair> { new CollisionPair() }).Create());

            // Act
            physicsSystem.Run();

            // Assert
            existingCollisionsCalculatorMock.Verify(p => p.GetExistingCollisions(It.IsAny<IList<CollisionPair>>()), Times.Never);
        }

        [Test]
        public void Run_ShouldCallCollisionProcessor_ProcessNextCollisions_EvenIfThereAreNone()
        {
            // Arrange
            var position = fixture.Create<Vector2>();
            var velocity = fixture.Create<Vector2>();
            GlobalObjects.DeltaTime = 0.5f;
            var mobileObject = new MaterialObjectMock { Position = position, Velocity = velocity };
            physicsSystem.RegisterMobileObject(mobileObject);
            potentialCollisionsDetectorMock.Setup(p => p.GetPotentialCollisions(mobileObject, It.IsAny<IEnumerable<IMaterialObject>>()))
                .Returns(new PotentialCollisions());

            // Act
            physicsSystem.Run();

            // Assert
            collisionProcessorMock.Verify(x => x.ProcessExistingCollisions(mobileObject, It.IsAny<List<CollisionInfo>>()), Times.Never);
            collisionProcessorMock.Verify(x => x.FindAndProcessNextCollisions(mobileObject, It.Is<IList<CollisionPair>>(x => x.Count == 0)), Times.Once);
        }

        [Test]
        public void Run_ShouldCallCollisionProcessor_ProcessExistingCollisions_WhenObjectDidRotate()
        {
            // Arrange
            var mobileObject = new MaterialObjectMock { DidRotate = true };
            var potentialExistingCollisions = new List<CollisionPair> { fixture.Create<CollisionPair>() };
            physicsSystem.RegisterMobileObject(mobileObject);
            potentialCollisionsDetectorMock.Setup(p => p.GetPotentialCollisions(mobileObject, It.IsAny<IEnumerable<IMaterialObject>>()))
                .Returns(fixture.Build<PotentialCollisions>().With(x => x.Existing, potentialExistingCollisions).Create());
            existingCollisionsCalculatorMock.Setup(e => e.GetExistingCollisions(potentialExistingCollisions))
                .Returns(new List<CollisionInfo> { new CollisionInfo() });

            // Act
            physicsSystem.Run();

            // Assert
            collisionProcessorMock.Verify(c => c.ProcessExistingCollisions(mobileObject, It.IsAny<IList<CollisionInfo>>()), Times.Once);
        }

        [Test]
        public void Run_ShouldNotCallCollisionProcessor_ProcessExistingCollisions_WhenThereAreNoExistingCollisions()
        {
            // Arrange
            var mobileObject = new MaterialObjectMock { DidRotate = true };
            var potentialExistingCollisions = new List<CollisionPair> { fixture.Create<CollisionPair>() };
            physicsSystem.RegisterMobileObject(mobileObject);
            potentialCollisionsDetectorMock.Setup(p => p.GetPotentialCollisions(mobileObject, It.IsAny<IEnumerable<IMaterialObject>>()))
                .Returns(fixture.Build<PotentialCollisions>().With(x => x.Existing, potentialExistingCollisions).Create());
            existingCollisionsCalculatorMock.Setup(e => e.GetExistingCollisions(potentialExistingCollisions))
                .Returns(new List<CollisionInfo> { });

            // Act
            physicsSystem.Run();

            // Assert
            collisionProcessorMock.Verify(c => c.ProcessExistingCollisions(mobileObject, It.IsAny<IList<CollisionInfo>>()), Times.Never);
        }

        [Test]
        public void Run_ShouldCallCollisionProcessor_ProcessNextCollisions_WhenVelocityIsNonZero()
        {
            // Arrange
            var mobileObject = new MaterialObjectMock { Velocity = new Vector2(1, 0) };
            var potentialFutureCollisions = new List<CollisionPair> { fixture.Create<CollisionPair>() };
            physicsSystem.RegisterMobileObject(mobileObject);
            potentialCollisionsDetectorMock.Setup(p => p.GetPotentialCollisions(mobileObject, It.IsAny<IEnumerable<IMaterialObject>>()))
                .Returns(fixture.Build<PotentialCollisions>().With(x => x.Future, potentialFutureCollisions).Create());

            // Act
            physicsSystem.Run();

            // Assert
            collisionProcessorMock.Verify(c => c.FindAndProcessNextCollisions(mobileObject, It.IsAny<IList<CollisionPair>>()), Times.Once);
        }
    }
}
