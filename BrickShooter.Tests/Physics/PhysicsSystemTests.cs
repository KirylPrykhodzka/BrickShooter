using BrickShooter.Physics;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using AutoFixture;
using AutoFixture.AutoMoq;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using BrickShooter.Tests.Mocks;

namespace BrickShooter.Tests
{
    [TestFixture]
    public class PhysicsSystemTests
    {
        private PhysicsSystem physicsSystem;
        private Mock<IPotentialCollisionsDetector> potentialCollisionsDetectorMock;
        private Mock<IExistingCollisionsCalculator> existingCollisionsCalculatorMock;
        private Mock<IFutureCollisionsCalculator> futureCollisionsCalculatorMock;
        private Mock<ICollisionProcessor> collisionProcessorMock;
        private Mock<IMaterialObjectMover> materialObjectMoverMock;
        private Fixture fixture;

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());

            potentialCollisionsDetectorMock = fixture.Create<Mock<IPotentialCollisionsDetector>>();
            existingCollisionsCalculatorMock = fixture.Create<Mock<IExistingCollisionsCalculator>>();
            futureCollisionsCalculatorMock = fixture.Create<Mock<IFutureCollisionsCalculator>>();
            collisionProcessorMock = fixture.Create<Mock<ICollisionProcessor>>();
            materialObjectMoverMock = fixture.Create<Mock<IMaterialObjectMover>>();

            physicsSystem = new PhysicsSystem(
                potentialCollisionsDetectorMock.Object,
                existingCollisionsCalculatorMock.Object,
                futureCollisionsCalculatorMock.Object,
                collisionProcessorMock.Object,
                materialObjectMoverMock.Object
            );
        }

        [Test]
        public void RegisterMobileObject_Should_CallDetectPotentialCollisions()
        {
            // Arrange
            var mobileObject = fixture.Create<MaterialObjectMock>();

            // Act
            physicsSystem.RegisterMobileObject(mobileObject);
            physicsSystem.Run();

            // Assert
            potentialCollisionsDetectorMock.Verify(p => p.DetectPotentialCollisions(mobileObject, It.IsAny<IEnumerable<MaterialObject>>()), Times.Once());
        }

        [Test]
        public void RegisterMultipleMobileObjects_Should_CallDetectPotentialCollisionsForEach()
        {
            // Arrange
            var mobileObject1 = fixture.Create<MaterialObjectMock>();
            var mobileObject2 = fixture.Create<MaterialObjectMock>();

            // Act
            physicsSystem.RegisterMobileObject(mobileObject1);
            physicsSystem.RegisterMobileObject(mobileObject2);
            physicsSystem.Run();

            // Assert
            potentialCollisionsDetectorMock.Verify(p => p.DetectPotentialCollisions(mobileObject1, It.Is<IEnumerable<MaterialObject>>(x => x.Contains(mobileObject2))), Times.Once());
            potentialCollisionsDetectorMock.Verify(p => p.DetectPotentialCollisions(mobileObject2, It.Is<IEnumerable<MaterialObject>>(x => x.Contains(mobileObject1))), Times.Once());
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
            potentialCollisionsDetectorMock.Verify(p => p.DetectPotentialCollisions(mobileObject, It.IsAny<IEnumerable<MaterialObject>>()), Times.Never());
        }

        [Test]
        public void RegisterImmobileObject_Should_CallDetectPotentialCollisions()
        {
            // Arrange
            var mobileObject = fixture.Create<MaterialObjectMock>();
            physicsSystem.RegisterMobileObject(mobileObject);
            var immobileObject = fixture.Create<MaterialObjectMock>();

            // Act
            physicsSystem.RegisterImmobileObject(immobileObject);
            physicsSystem.Run();

            // Assert
            potentialCollisionsDetectorMock.Verify(p => p.DetectPotentialCollisions(mobileObject, It.Is<IEnumerable<MaterialObject>>(x => x.Contains(immobileObject))), Times.Once());
        }

        [Test]
        public void UnregisterImmobileObject_Should_NotCallDetectPotentialCollisions()
        {
            // Arrange
            var mobileObject = fixture.Create<MaterialObjectMock>();
            physicsSystem.RegisterMobileObject(mobileObject);
            var immobileObject = fixture.Create<MaterialObjectMock>();
            physicsSystem.RegisterImmobileObject(immobileObject);

            // Act
            physicsSystem.UnregisterImmobileObject(immobileObject);
            physicsSystem.Run();

            // Assert
            potentialCollisionsDetectorMock.Verify(p => p.DetectPotentialCollisions(mobileObject, It.Is<IEnumerable<MaterialObject>>(x => x.Contains(immobileObject))), Times.Never());
        }

        // Write test cases for the Run method and Visualize method
    }
}
