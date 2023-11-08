using Microsoft.Xna.Framework;
using NUnit.Framework;
using FluentAssertions;
using AutoFixture;
using BrickShooter.Physics.Models;
using BrickShooter.Physics.Interfaces;
using Moq;
using BrickShooter.Tests.Mocks;
using BrickShooter.Extensions;
using MonoGame.Extended;

namespace BrickShooter.Tests.Physics
{
    [TestFixture]
    public class ColliderPolygonTests
    {
        private IFixture fixture;

        [SetUp]
        public void Setup()
        {
            fixture = new Fixture();
        }

        [Test]
        public void Center_CalculatesCenterCorrectly()
        {
            // Arrange
            List<Vector2> points = fixture.Create<List<Vector2>>();
            var collider = new ColliderPolygon(new Mock<IMaterialObject>().Object, fixture.Create("CollisionLayer"), points);

            // Act
            Vector2 center = collider.Center;

            // Assert
            center.X.Should().BeApproximately(points.Average(p => p.X), 0.01f);
            center.Y.Should().BeApproximately(points.Average(p => p.Y), 0.01f);
        }

        [Test]
        public void MaterialObjectPositionChanged_UpdatesPointsCorrectly()
        {
            // Arrange
            List<Vector2> points = fixture.Create<List<Vector2>>();
            var materialObject = fixture.Build<MaterialObjectMock>()
                .With(x => x.Position, Vector2.Zero)
                .With(x => x.Rotation, 0f)
                .Create();
            var collider = new ColliderPolygon(materialObject, fixture.Create("CollisionLayer"), points);

            var offset = fixture.Create<Vector2>();

            // Act
            materialObject.Position += offset;

            // Assert
            var expectedPoints = points.Select(p => p + offset);
            collider.Points.Should().ContainInOrder(expectedPoints);
        }

        [Test]
        public void MaterialObjectRotationChanged_UpdatesPointsCorrectly()
        {
            // Arrange
            List<Vector2> points = fixture.Create<List<Vector2>>();
            var materialObject = fixture.Build<MaterialObjectMock>()
                .With(x => x.Position, Vector2.Zero)
                .With(x => x.Rotation, 0f)
                .Create();
            var collider = new ColliderPolygon(materialObject, fixture.Create("CollisionLayer"), points);

            var newRotation = fixture.Create<float>();

            // Act
            materialObject.Rotation = newRotation;

            // Assert
            var expectedPoints = points.Select(p => p.Rotate(materialObject.Position, newRotation));
            collider.Points.Should().ContainInOrder(expectedPoints);
        }

        [Test]
        public void MaterialObjectPositionChanged_UpdatesBoundsCorrectly()
        {
            // Arrange
            List<Vector2> points = fixture.Create<List<Vector2>>();
            var materialObject = fixture.Build<MaterialObjectMock>()
                .With(x => x.Position, Vector2.Zero)
                .With(x => x.Rotation, 0f)
                .Create();
            var collider = new ColliderPolygon(materialObject, fixture.Create("CollisionLayer"), points);
            var offset = fixture.Create<Vector2>();
            var expectedBounds = collider.Bounds;
            expectedBounds.Offset(offset);

            // Act
            materialObject.Position += offset;

            // Assert
            collider.Bounds.Should().Be(expectedBounds);
        }

        [Test]
        public void MaterialObjectRotationChanged_UpdatesBoundsCorrectly()
        {
            // Arrange
            List<Vector2> points = fixture.Create<List<Vector2>>();
            var materialObject = fixture.Build<MaterialObjectMock>()
                .With(x => x.Position, Vector2.Zero)
                .With(x => x.Rotation, 0f)
                .Create();
            var collider = new ColliderPolygon(materialObject, fixture.Create("CollisionLayer"), points);
            var newRotation = fixture.Create<float>();

            // Act
            materialObject.Rotation = newRotation;

            // Assert
            var expectedPoints = points.Select(p => p.Rotate(materialObject.Position, newRotation));
            var maxX = expectedPoints.Max(x => x.X);
            var minX = expectedPoints.Min(x => x.X);
            var maxY = expectedPoints.Max(x => x.Y);
            var minY = expectedPoints.Min(x => x.Y);
            var expectedBounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);
            collider.Bounds.Should().Be(expectedBounds);
        }

        [Test]
        public void MaterialObjectPositionChanged_UpdatesCenterCorrectly()
        {
            // Arrange
            List<Vector2> points = fixture.Create<List<Vector2>>();
            var materialObject = fixture.Build<MaterialObjectMock>()
                .With(x => x.Position, Vector2.Zero)
                .With(x => x.Rotation, 0f)
                .Create();
            var collider = new ColliderPolygon(materialObject, fixture.Create("CollisionLayer"), points);
            var offset = fixture.Create<Vector2>();
            var expectedCenter = collider.Center + offset;

            // Act
            materialObject.Position += offset;

            // Assert
            collider.Center.X.Should().BeApproximately(expectedCenter.X, 0.01f);
            collider.Center.Y.Should().BeApproximately(expectedCenter.Y, 0.01f);
        }

        [Test]
        public void MaterialObjectRotationChanged_UpdatesCenterCorrectly()
        {
            // Arrange
            List<Vector2> points = fixture.Create<List<Vector2>>();
            var materialObject = fixture.Build<MaterialObjectMock>()
                .With(x => x.Position, Vector2.Zero)
                .With(x => x.Rotation, 0f)
                .Create();
            var collider = new ColliderPolygon(materialObject, fixture.Create("CollisionLayer"), points);
            var newRotation = fixture.Create<float>();
            var expectedCenter = collider.Center.Rotate(materialObject.Position, newRotation);

            // Act
            materialObject.Rotation = newRotation;

            // Assert
            collider.Center.X.Should().BeApproximately(expectedCenter.X, 0.01f);
            collider.Center.Y.Should().BeApproximately(expectedCenter.Y, 0.01f);
        }

        [Test]
        public void MaxMinXMaxMinY_CalculatesCorrectly()
        {
            // Arrange
            List<Vector2> points = fixture.Create<List<Vector2>>();

            // Act
            var collider = new ColliderPolygon(new Mock<IMaterialObject>().Object, fixture.Create("CollisionLayer"), points);

            // Assert
            var bounds = collider.Bounds;
            var minX = points.Min(p => p.X);
            var minY = points.Min(p => p.Y);
            var maxX = points.Max(p => p.X);
            var maxY = points.Max(p => p.Y);

            bounds.X.Should().Be(minX);
            bounds.Y.Should().Be(minY);
            bounds.Width.Should().Be(maxX - minX);
            bounds.Height.Should().Be(maxY - minY);
        }
    }
}
