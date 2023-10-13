using Microsoft.Xna.Framework;
using NUnit.Framework;
using BrickShooter.Extensions;
using FluentAssertions;

namespace BrickShooter.Tests.Maths
{
    [TestFixture]
    public class Vector2ExtensionsTests
    {
        [Test]
        public void Rotate_PointZeroDegrees_ShouldReturnOriginalPoint()
        {
            // Arrange
            var point = new Vector2(2, 3);
            var origin = new Vector2(0, 0);
            double angleInRadians = 0;

            // Act
            var result = point.Rotate(origin, angleInRadians);

            // Assert
            result.Should().Be(point);
        }

        [Test]
        public void Rotate_90DegreesCounterClockwise_ShouldReturnExpectedPoint()
        {
            // Arrange
            var point = new Vector2(2, 3);
            var origin = new Vector2(0, 0);
            double angleInRadians = Math.PI / 2; // 90 degrees in radians

            // Act
            var result = point.Rotate(origin, angleInRadians);

            // Assert
            result.Should().Be(new Vector2(-3, 2));
        }

        [Test]
        public void Rotate_45DegreesClockwise_ShouldReturnExpectedPoint()
        {
            // Arrange
            var point = new Vector2(2, 3);
            var origin = new Vector2(0, 0);
            double angleInRadians = -Math.PI / 4; // -45 degrees in radians

            // Act
            var result = point.Rotate(origin, angleInRadians);

            // Assert
            result.X.Should().BeApproximately(3.5355f, 0.0001f);
            result.Y.Should().BeApproximately(0.7071f, 0.0001f);
        }

        [Test]
        public void Rotate_90DegreesCounterClockwiseWithOrigin_ShouldReturnExpectedPoint()
        {
            // Arrange
            var point = new Vector2(2, 2);
            var origin = new Vector2(1, 1); // Non-zero origin
            double angleInRadians = -Math.PI / 2; // 90 degrees in radians

            // Act
            var result = point.Rotate(origin, angleInRadians);

            // Assert
            result.X.Should().BeApproximately(2, 0.0001f);
            result.Y.Should().BeApproximately(0, 0.0001f);
        }

        [Test]
        public void Rotate_180DegreesWithOrigin_ShouldReturnFlippedPoint()
        {
            // Arrange
            var point = new Vector2(3, 2);
            var origin = new Vector2(2, 2); // Non-zero origin
            double angleInRadians = Math.PI; // 180 degrees in radians

            // Act
            var result = point.Rotate(origin, angleInRadians);

            // Assert
            result.X.Should().BeApproximately(1, 0.0001f);
            result.Y.Should().BeApproximately(2, 0.0001f);
        }

        [Test]
        public void Rotate_60DegreesClockwiseWithOrigin_ShouldReturnExpectedPoint()
        {
            // Arrange
            var point = new Vector2(2, 2);
            var origin = new Vector2(1, 1); // Non-zero origin
            double angleInRadians = -Math.PI / 3; // -60 degrees in radians

            // Act
            var result = point.Rotate(origin, angleInRadians);

            // Assert
            result.X.Should().BeApproximately(2.3660f, 0.0001f);
            result.Y.Should().BeApproximately(0.6340f, 0.0001f);
        }

        [Test]
        public void Project_VectorOntoItself_ShouldReturnSameVector()
        {
            // Arrange
            var vector = new Vector2(2, 3);

            // Act
            var projected = vector.Project(vector);

            // Assert
            projected.Should().Be(vector);
        }

        [Test]
        public void Project_VectorOntoHorizontalAxis_ShouldReturnHorizontalVector()
        {
            // Arrange
            var vector = new Vector2(3, 4);
            var axis = new Vector2(1, 0); // Horizontal axis

            // Act
            var projected = vector.Project(axis);

            // Assert
            projected.X.Should().Be(vector.X);
            projected.Y.Should().Be(0);
        }

        [Test]
        public void Project_VectorOntoVerticalAxis_ShouldReturnVerticalVector()
        {
            // Arrange
            var vector = new Vector2(3, 4);
            var axis = new Vector2(0, 1); // Vertical axis

            // Act
            var projected = vector.Project(axis);

            // Assert
            projected.X.Should().Be(0);
            projected.Y.Should().Be(vector.Y);
        }

        [Test]
        public void Project_VectorOnto45DegreesDiagonalAxis_ShouldReturnDiagonalVector()
        {
            // Arrange
            var vector = new Vector2(3, 4);
            var axis = new Vector2(1, 1); // 45 degrees diagonal axis

            // Act
            var projected = vector.Project(axis);

            // Assert
            projected.X.Should().BeApproximately(3.5f, 0.0001f);
            projected.Y.Should().BeApproximately(3.5f, 0.0001f);
        }
    }
}
