﻿using AutoFixture;
using BrickShooter.Physics;
using BrickShooter.Physics.Interfaces;
using Microsoft.Xna.Framework;
using Moq;
using NUnit.Framework;

namespace BrickShooter.Tests.Physics
{
    [TestFixture]
    public class MaterialObjectMoverTests
    {
        private IFixture fixture;
        private IMaterialObjectMover mover;

        [SetUp]
        public void Setup()
        {
            fixture = new Fixture();
            mover = new MaterialObjectMover();
        }

        [Test]
        public void MoveObject_Should_Update_MaterialObject_Position()
        {
            // Arrange
            var initialPosition = fixture.Create<Vector2>();
            var materialObject = new Mock<IMaterialObject>();
            materialObject.SetupGet(x => x.Position).Returns(initialPosition);
            var movement = fixture.Create<Vector2>();

            // Act
            mover.MoveObject(materialObject.Object, movement);

            // Assert
            materialObject.VerifySet(x => x.Position = initialPosition + movement, Times.Once);
        }
    }
}
