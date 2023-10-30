﻿using BrickShooter.Physics;
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

            physicsSystem = new PhysicsSystem(
                potentialCollisionsDetectorMock.Object,
                existingCollisionsCalculatorMock.Object,
                futureCollisionsCalculatorMock.Object,
                collisionProcessorMock.Object);
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
            potentialCollisionsDetectorMock.Verify(p => p.GetPotentialCollisions(mobileObject, It.IsAny<IEnumerable<MaterialObject>>()), Times.Once);
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
            potentialCollisionsDetectorMock.Verify(p => p.GetPotentialCollisions(mobileObject1, It.Is<IEnumerable<MaterialObject>>(x => x.Contains(mobileObject2))), Times.Once);
            potentialCollisionsDetectorMock.Verify(p => p.GetPotentialCollisions(mobileObject2, It.Is<IEnumerable<MaterialObject>>(x => x.Contains(mobileObject1))), Times.Once);
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
            potentialCollisionsDetectorMock.Verify(p => p.GetPotentialCollisions(mobileObject, It.IsAny<IEnumerable<MaterialObject>>()), Times.Never);
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
            potentialCollisionsDetectorMock.Verify(p => p.GetPotentialCollisions(mobileObject, It.Is<IEnumerable<MaterialObject>>(x => x.Contains(immobileObject))), Times.Once);
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
            potentialCollisionsDetectorMock.Verify(p => p.GetPotentialCollisions(mobileObject, It.Is<IEnumerable<MaterialObject>>(x => x.Contains(immobileObject))), Times.Never);
        }

        [Test]
        public void Run_ShouldCallDetectPotentialCollisions_WhenVelocityIsNonZeroOrDidRotateIsTrue()
        {
            // Arrange
            var mobileObject1 = new MaterialObjectMock { Velocity = new Vector2(1, 0), DidRotate = false };
            var mobileObject2 = new MaterialObjectMock { Velocity = Vector2.Zero, DidRotate = true };
            physicsSystem.RegisterMobileObject(mobileObject1);
            physicsSystem.RegisterMobileObject(mobileObject2);

            // Act
            physicsSystem.Run();

            // Assert
            potentialCollisionsDetectorMock.Verify(p => p.GetPotentialCollisions(mobileObject1, It.IsAny<IEnumerable<MaterialObject>>()), Times.Once);
            potentialCollisionsDetectorMock.Verify(p => p.GetPotentialCollisions(mobileObject2, It.IsAny<IEnumerable<MaterialObject>>()), Times.Once);
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
            potentialCollisionsDetectorMock.Verify(p => p.GetPotentialCollisions(mobileObject, It.IsAny<IEnumerable<MaterialObject>>()), Times.Never);
        }

        [Test]
        public void Run_ShouldNotCheckExistingCollisions_WhenObjectDidNotRotate()
        {
            // Arrange
            var mobileObject = new MaterialObjectMock { Velocity = new Vector2(1, 0), DidRotate = false };
            physicsSystem.RegisterMobileObject(mobileObject);

            // Act
            physicsSystem.Run();

            // Assert
            existingCollisionsCalculatorMock.Verify(p => p.GetExistingCollisions(mobileObject, It.IsAny<IEnumerable<MaterialObject>>()), Times.Never);
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
            potentialCollisionsDetectorMock.Setup(p => p.GetPotentialCollisions(mobileObject, It.IsAny<IEnumerable<MaterialObject>>()))
                .Returns((new List<MaterialObject>(), new List<MaterialObject>()));
            futureCollisionsCalculatorMock.Setup(x => x.FindNextCollisions(mobileObject, It.IsAny<List<MaterialObject>>())).Returns(new List<FutureCollisionInfo>());

            // Act
            physicsSystem.Run();

            // Assert
            collisionProcessorMock.Verify(x => x.ProcessExistingCollisions(mobileObject, It.IsAny<List<CollisionInfo>>()), Times.Never);
            collisionProcessorMock.Verify(x => x.ProcessNextCollisions(mobileObject, It.Is<List<FutureCollisionInfo>>(x => x.Count == 0)), Times.Once);
        }

        [Test]
        public void Run_ShouldCallCollisionProcessor_ProcessExistingCollisions_WhenObjectDidRotate()
        {
            // Arrange
            var mobileObject = new MaterialObjectMock { DidRotate = true };
            var potentialExistingCollisions = new List<MaterialObject> { new MaterialObjectMock() };
            physicsSystem.RegisterMobileObject(mobileObject);
            potentialCollisionsDetectorMock.Setup(p => p.GetPotentialCollisions(mobileObject, It.IsAny<IEnumerable<MaterialObject>>()))
                .Returns((potentialExistingCollisions, new List<MaterialObject>()));
            existingCollisionsCalculatorMock.Setup(e => e.GetExistingCollisions(mobileObject, potentialExistingCollisions))
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
            var potentialExistingCollisions = new List<MaterialObject> { new MaterialObjectMock() };
            physicsSystem.RegisterMobileObject(mobileObject);
            potentialCollisionsDetectorMock.Setup(p => p.GetPotentialCollisions(mobileObject, It.IsAny<IEnumerable<MaterialObject>>()))
                .Returns((potentialExistingCollisions, new List<MaterialObject>()));
            existingCollisionsCalculatorMock.Setup(e => e.GetExistingCollisions(mobileObject, potentialExistingCollisions))
                .Returns(new List<CollisionInfo> { });

            // Act
            physicsSystem.Run();

            // Assert
            collisionProcessorMock.Verify(c => c.ProcessExistingCollisions(mobileObject, It.IsAny<IList<CollisionInfo>>()), Times.Never);
        }

        [Test]
        public void Run_ShouldNotCheckNextCollisions_WhenObjectIsNotMoving()
        {
            // Arrange
            var mobileObject = new MaterialObjectMock { Velocity = Vector2.Zero, DidRotate = true };
            physicsSystem.RegisterMobileObject(mobileObject);

            // Act
            physicsSystem.Run();

            // Assert
            futureCollisionsCalculatorMock.Verify(p => p.FindNextCollisions(mobileObject, It.IsAny<IEnumerable<MaterialObject>>()), Times.Never);
        }

        [Test]
        public void Run_ShouldCallCollisionProcessor_ProcessNextCollisions_WhenVelocityIsNonZero()
        {
            // Arrange
            var mobileObject = new MaterialObjectMock { Velocity = new Vector2(1, 0) };
            var potentialFutureCollisions = new List<MaterialObject> { new MaterialObjectMock() };
            physicsSystem.RegisterMobileObject(mobileObject);
            potentialCollisionsDetectorMock.Setup(p => p.GetPotentialCollisions(mobileObject, It.IsAny<IEnumerable<MaterialObject>>()))
                .Returns((new List<MaterialObject>(), potentialFutureCollisions));
            futureCollisionsCalculatorMock.Setup(f => f.FindNextCollisions(mobileObject, potentialFutureCollisions))
                .Returns(new List<FutureCollisionInfo> { new FutureCollisionInfo() });

            // Act
            physicsSystem.Run();

            // Assert
            collisionProcessorMock.Verify(c => c.ProcessNextCollisions(mobileObject, It.IsAny<IList<FutureCollisionInfo>>()), Times.Once);
        }
    }
}
