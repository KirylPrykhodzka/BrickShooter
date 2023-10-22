using AutoFixture;
using BrickShooter.Physics;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using BrickShooter.Tests.Mocks;
using FluentAssertions;
using Microsoft.Xna.Framework;
using Moq;
using NUnit.Framework;

namespace BrickShooter.Tests
{
    [TestFixture]
    public class MaterialObjectMoverTests
    {
        private IFixture fixture;
        private MaterialObjectMover _materialObjectMover;

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
            _materialObjectMover = new MaterialObjectMover();
            GlobalObjects.DeltaTime = 2f;
        }

        [Test]
        public void MoveWithoutObstruction_Should_UpdateMaterialObjectPosition()
        {
            // Arrange
            var position = fixture.Create<Vector2>();
            var velocity = fixture.Create<Vector2>();
            var materialObject = new MaterialObjectMock { Position = position, Velocity = velocity };

            // Act
            _materialObjectMover.MoveWithoutObstruction(materialObject);

            // Assert
            materialObject.Position.X.Should().Be(position.X + velocity.X * GlobalObjects.DeltaTime);
            materialObject.Position.Y.Should().Be(position.Y + velocity.Y * GlobalObjects.DeltaTime);
        }
    }
}
