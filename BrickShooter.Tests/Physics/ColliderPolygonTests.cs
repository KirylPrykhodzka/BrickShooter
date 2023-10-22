using Microsoft.Xna.Framework;
using NUnit.Framework;
using FluentAssertions;
using AutoFixture;
using BrickShooter.Physics.Models;

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
            var collider = new ColliderPolygon();
            collider.SetPoints(points);

            // Act
            Vector2 center = collider.Center;

            // Assert
            center.X.Should().BeApproximately(points.Average(p => p.X), 0.01f);
            center.Y.Should().BeApproximately(points.Average(p => p.Y), 0.01f);
        }

        [Test]
        public void Offset_UpdatesPointsCorrectly()
        {
            // Arrange
            List<Vector2> points = fixture.Create<List<Vector2>>();
            var collider = new ColliderPolygon();
            collider.SetPoints(points);

            float offsetX = fixture.Create<float>();
            float offsetY = fixture.Create<float>();

            // Act
            collider.Offset(offsetX, offsetY);

            // Assert
            var expectedPoints = points.Select(p => new Vector2(p.X + offsetX, p.Y + offsetY));
            collider.Points.Should().ContainInOrder(expectedPoints);
        }

        [Test]
        public void MaxMinXMaxMinY_CalculatesCorrectly()
        {
            // Arrange
            List<Vector2> points = fixture.Create<List<Vector2>>();
            var collider = new ColliderPolygon();
            collider.SetPoints(points);

            // Act and Assert
            collider.MaxX.Should().Be(points.Max(p => p.X));
            collider.MinX.Should().Be(points.Min(p => p.X));
            collider.MaxY.Should().Be(points.Max(p => p.Y));
            collider.MinY.Should().Be(points.Min(p => p.Y));
        }
    }
}
