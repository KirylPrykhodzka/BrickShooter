using Microsoft.Xna.Framework;
using NUnit.Framework;
using BrickShooter.Extensions;

namespace BrickShooter.Tests.Maths
{
    [TestFixture]
    public class PointExtensionsTests
    {
        [Test]
        public void Rotate_ZeroAngle_ReturnsSamePoint()
        {
            // Arrange
            Point point = new Point(2, 3);
            Point origin = new Point(0, 0);
            float angle = 0;

            // Act
            Point result = point.Rotate(origin, angle);

            // Assert
            Assert.That(result, Is.EqualTo(point));
        }

        [Test]
        public void Rotate_90DegreeAngle_ReturnsExpectedPoint()
        {
            // Arrange
            Point point = new Point(2, 3);
            Point origin = new Point(0, 0);
            float angle = (float)Math.PI / 2; // 90 degrees in radians

            // Act
            Point result = point.Rotate(origin, angle);

            // Assert
            Assert.That(result, Is.EqualTo(new Point(-3, 2)));
        }

        [Test]
        public void Rotate_180DegreeAngle_ReturnsExpectedPoint()
        {
            // Arrange
            Point point = new Point(2, 3);
            Point origin = new Point(0, 0);
            float angle = (float)Math.PI; // 180 degrees in radians

            // Act
            Point result = point.Rotate(origin, angle);

            // Assert
            Assert.That(result, Is.EqualTo(new Point(-2, -3)));
        }

        [Test]
        public void Rotate_Negative45DegreeAngle_ReturnsExpectedPoint()
        {
            // Arrange
            Point point = new Point(2, 2);
            Point origin = new Point(0, 0);
            float angle = -(float)(Math.PI / 4); // -45 degrees in radians

            // Act
            Point result = point.Rotate(origin, angle);

            // Assert
            Assert.That(result, Is.EqualTo(new Point(3, 0)));
        }
    }
}
