using AutoFixture;
using BrickShooter.Configuration;
using BrickShooter.Extensions;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Tests.Mocks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Moq;
using NUnit.Framework;

namespace BrickShooter.Tests.Physics
{
    [TestFixture]
    public class MaterialObjectTests
    {
        private Fixture fixture;
        private Mock<IPhysicsSystem> physicsSystemMock;

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
            physicsSystemMock = new Mock<IPhysicsSystem>();
            ServiceProviderFactory.ServiceProvider = new ServiceCollection().AddSingleton(physicsSystemMock.Object).BuildServiceProvider();
        }

        [TearDown]
        public void TearDown()
        {
            ServiceProviderFactory.ServiceProvider = null;
        }

        [Test]
        public void GetGlobalColliderPolygon_PositionAndRotationDidNotChange_ReturnsSamePoints()
        {
            //Arrange
            var materialObject = new MaterialObjectMock(fixture.CreateMany<Vector2>(4).ToArray());

            //Act
            var colliderPolygon = materialObject.SingleCollider;
            materialObject.Position = materialObject.Position;
            materialObject.Rotation = materialObject.Rotation;
            var sameColliderPolygon = materialObject.SingleCollider;

            //Assert
            colliderPolygon.Points.Should().BeEquivalentTo(sameColliderPolygon.Points);
        }

        [Test]
        public void ShouldCalculateColliderPolygonBasedOn_InitialPoints_Position_And_Rotation()
        {
            //Arrange
            var initialColliderPoints = fixture.CreateMany<Vector2>(4).ToArray();
            var materialObject = new MaterialObjectMock(initialColliderPoints);
            materialObject.Rotation = fixture.Create<float>();
            materialObject.Position = fixture.Create<Vector2>();
            var expectedColliderPoints = initialColliderPoints.Select(x => x.Rotate(Vector2.Zero, materialObject.Rotation) + materialObject.Position).ToArray();

            //Act
            var colliderPolygon = materialObject.SingleCollider;

            //Assert
            colliderPolygon.Points.Should().BeEquivalentTo(expectedColliderPoints);
        }

        [Test]
        public void ShouldRecalculateColliderPolygonWhenPositionChanges()
        {
            //Arrange
            var initialColliderPoints = fixture.CreateMany<Vector2>(4).ToArray();
            var materialObject = new MaterialObjectMock(initialColliderPoints);
            materialObject.Rotation = fixture.Create<float>();
            materialObject.Position = fixture.Create<Vector2>();

            //Act
            var colliderPointsBefore = materialObject.SingleCollider.Points;
            materialObject.Position = fixture.Create<Vector2>();
            var colliderPointsAfter = materialObject.SingleCollider.Points;

            //Assert
            colliderPointsAfter.Should().NotBeEquivalentTo(colliderPointsBefore);
        }
    }
}