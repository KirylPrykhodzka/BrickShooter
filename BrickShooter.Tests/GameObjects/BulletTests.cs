using AutoFixture;
using BrickShooter.Constants;
using BrickShooter.GameObjects;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using BrickShooter.Tests.Mocks;
using FluentAssertions;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Moq;
using NUnit.Framework;

namespace BrickShooter.Tests.GameObjects
{
    [TestFixture]
    public class BulletTests
    {
        private Fixture fixture;

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
        }

        [Test]
        public void Bullet_Constructor_Should_SetBounciness()
        {
            // Arrange
            float expectedBounciness = BulletConstants.BOUNCINESS;

            // Act
            var bullet = new Bullet();

            // Assert
            bullet.Bounciness.Should().Be(expectedBounciness);
        }

        [Test]
        public void Bullet_Move_Should_SetPositionVelocityAndRotation()
        {
            // Arrange
            var bullet = new Bullet();
            var expectedPosition = fixture.Create<Vector2>();
            var expectedRotation = fixture.Create<float>();

            // Act
            bullet.Move(expectedPosition, expectedRotation);

            // Assert
            bullet.Position.Should().Be(expectedPosition);
            bullet.Rotation.Should().Be(expectedRotation);
            bullet.Velocity.Should().BeEquivalentTo(new Vector2((float)Math.Cos(expectedRotation), (float)Math.Sin(expectedRotation)) * BulletConstants.VELOCITY);
        }

        [Test]
        public void Bullet_Reset_Should_ResetPositionRotationAndVelocity()
        {
            // Arrange
            var bullet = new Bullet
            {
                Position = fixture.Create<Vector2>(),
                Rotation = fixture.Create<float>(),
                Velocity = fixture.Create<Vector2>()
            };

            // Act
            bullet.Reset();

            // Assert
            bullet.Position.Should().Be(Vector2.Zero);
            bullet.Rotation.Should().Be(0);
            bullet.Velocity.Should().Be(Vector2.Zero);
        }

        [Test]
        public void OnCollisionTest_ShouldInvoke_OnPlayerHit_UponCollisionWithPlayer()
        {
            // Arrange
            var bullet = new Bullet();
            bool onPlayerHitInvoked = false;
            bullet.OnPlayerHit = (sender) =>
            {
                onPlayerHitInvoked = true;
            };
            var colliderPolygonMock = new Mock<IColliderPolygon>();
            colliderPolygonMock.SetupGet(x => x.CollisionLayer).Returns(nameof(Player));

            // Act
            bullet.OnVelocityCollision(new VelocityCollisionInfo
            {
                CollisionPair = new CollisionPair
                {
                    CollisionObject = colliderPolygonMock.Object,
                }
            });

            // Assert
            onPlayerHitInvoked.Should().BeTrue();
        }

        [Test]
        public void OnCollisionTest_ShouldNotInvoke_OnPlayerHit_UponCollisionWithNonPlayerObject()
        {
            // Arrange
            var bullet = new Bullet();
            bool onPlayerHitInvoked = false;
            bullet.OnPlayerHit = (sender) =>
            {
                onPlayerHitInvoked = true;
            };
            var colliderPolygonMock = new Mock<IColliderPolygon>();
            colliderPolygonMock.SetupGet(x => x.CollisionLayer).Returns(fixture.Create("CollisionLayer"));

            // Act
            bullet.OnVelocityCollision(new VelocityCollisionInfo
            {
                CollisionPair = new CollisionPair
                {
                    CollisionObject = colliderPolygonMock.Object,
                }
            });

            // Assert
            onPlayerHitInvoked.Should().BeFalse();
        }

        [Test]
        public void OnCollision_ShouldProperlyUpdateRotationUponCollisionWithWall()
        {
            // Arrange
            var bullet = new Bullet
            {
                Velocity = fixture.Create<Vector2>()
            };
            var wallCollider = new ColliderPolygon(fixture.Create<MaterialObjectMock>(), nameof(Wall), fixture.Create<List<Vector2>>());
            var velocityCollisionInfo = new VelocityCollisionInfo
            {
                CollisionPair = new CollisionPair
                {
                    CollisionObject = wallCollider,
                },
                Normal = fixture.Create<Vector2>()
            };

            // Act
            bullet.OnVelocityCollision(velocityCollisionInfo);

            // Assert
            var expectedRotation = Vector2.Reflect(bullet.Velocity, velocityCollisionInfo.Normal).ToAngle() + MathF.PI / 2;
            bullet.Rotation.Should().Be(expectedRotation);
        }
    }
}
