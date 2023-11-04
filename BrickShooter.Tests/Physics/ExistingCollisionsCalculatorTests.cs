using BrickShooter.Physics;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Tests.Mocks;
using FluentAssertions;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace BrickShooter.Tests.Physics
{
    [TestFixture]
    public class ExistingCollisionsCalculatorTests
    {
        private IExistingCollisionsCalculator collisionsCalculator;

        [SetUp]
        public void Setup()
        {
            collisionsCalculator = new ExistingCollisionsCalculator();
        }

        [Test]
        public void GetExistingCollisions_NoCollisions_ReturnsEmptyList()
        {
            // Arrange
            var subject = new MaterialObjectMock(new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, 2), new Vector2(0, 2) });
            var potentialCollisions = new List<MaterialObjectMock>
            {
                new MaterialObjectMock(new Vector2[] { new Vector2(3, 3), new Vector2(4, 3), new Vector2(4, 4), new Vector2(3, 4) }),
                new MaterialObjectMock(new Vector2[] { new Vector2(5, 5), new Vector2(6, 5), new Vector2(6, 6), new Vector2(5, 6) })
            };

            // Act
            var result = collisionsCalculator.GetExistingCollisions(subject, potentialCollisions.Select(x => x.Body).ToList());

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public void GetExistingCollisions_CollisionsDetected_ReturnsCollisionList()
        {
            // Arrange
            var subject = new MaterialObjectMock(new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, 2), new Vector2(0, 2) });
            var potentialCollisions = new List<MaterialObjectMock>
            {
                new MaterialObjectMock(new Vector2[] { new Vector2(1, 1), new Vector2(3, 1), new Vector2(3, 3), new Vector2(1, 3) }),
                new MaterialObjectMock(new Vector2[] { new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, 2), new Vector2(1, 2) })
            };

            // Act
            var result = collisionsCalculator.GetExistingCollisions(subject, potentialCollisions.Select(x => x.Body).ToList());

            // Assert
            result.Should().NotBeEmpty();
            result.Should().OnlyContain(collision => collision.IsColliding);
            result.Should().OnlyContain(collision => collision.MinimalTranslationVector != Vector2.Zero);
        }

        [Test]
        public void GetExistingCollisions_OverlappingPoint_ShouldDetectCollisionAndZeroTranslation()
        {
            // Arrange
            var subject = new MaterialObjectMock(new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, 2), new Vector2(0, 2) });
            var potentialCollisions = new List<MaterialObjectMock>
            {
                new MaterialObjectMock(new Vector2[] { new Vector2(2, 2) })  // Overlapping at a point
            };

            // Act
            var result = collisionsCalculator.GetExistingCollisions(subject, potentialCollisions.Select(x => x.Body).ToList());

            // Assert
            result.Should().NotBeEmpty();
            result.First().IsColliding.Should().BeTrue();
            result.First().MinimalTranslationVector.Should().Be(Vector2.Zero);
        }

        [Test]
        public void GetExistingCollisions_TouchingEdge_ShouldDetectCollisionAndZeroTranslation()
        {
            // Arrange
            var subject = new MaterialObjectMock(new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, 2), new Vector2(0, 2) });
            var potentialCollisions = new List<MaterialObjectMock>
            {
                new MaterialObjectMock(new Vector2[] { new Vector2(2, 2), new Vector2(4, 2), new Vector2(4, 4), new Vector2(2, 4) })  // Touching at an edge
            };

            // Act
            var result = collisionsCalculator.GetExistingCollisions(subject, potentialCollisions.Select(x => x.Body).ToList());

            // Assert
            result.Should().NotBeEmpty();
            result.First().IsColliding.Should().BeTrue();
            result.First().MinimalTranslationVector.Should().Be(Vector2.Zero);
        }

        [Test]
        public void GetExistingCollisions_OverlappingEdge_ReturnsCollisionListWithMinimalTranslationVector()
        {
            // Arrange
            var subject = new MaterialObjectMock(new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, 2), new Vector2(0, 2) });
            var potentialCollisions = new List<MaterialObjectMock>
            {
                new MaterialObjectMock(new Vector2[] { new Vector2(1, 1), new Vector2(3, 1), new Vector2(3, 2), new Vector2(1, 2) })  // Overlapping with an edge
            };

            // Act
            var result = collisionsCalculator.GetExistingCollisions(subject, potentialCollisions.Select(x => x.Body).ToList());

            // Assert
            result.Should().NotBeEmpty();
            result.First().IsColliding.Should().BeTrue();
            result.First().MinimalTranslationVector.Should().Be(new Vector2(0, -2));
        }

        [Test]
        public void GetExistingCollisions_ContainedPolygon_ReturnsCollisionListWithMinimalTranslationVector()
        {
            // Arrange
            var subject = new MaterialObjectMock(new Vector2[] { new Vector2(0, 0), new Vector2(4, 0), new Vector2(4, 4), new Vector2(0, 4) });
            var potentialCollisions = new List<MaterialObjectMock>
            {
                new MaterialObjectMock(new Vector2[] { new Vector2(1, 1), new Vector2(3, 1), new Vector2(3, 3), new Vector2(1, 3) })  // Polygon contained within the subject
            };

            // Act
            var result = collisionsCalculator.GetExistingCollisions(subject, potentialCollisions.Select(x => x.Body).ToList());

            // Assert
            result.Should().NotBeEmpty();
            result.First().IsColliding.Should().BeTrue();
            result.First().MinimalTranslationVector.Should().Be(new Vector2(0, 4));
        }

        [Test]
        public void GetExistingCollisions_SeparatedPolygon_ReturnsEmptyList()
        {
            // Arrange
            var subject = new MaterialObjectMock(new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, 2), new Vector2(0, 2) });
            var potentialCollisions = new List<MaterialObjectMock>
            {
                new MaterialObjectMock(new Vector2[] { new Vector2(4, 4), new Vector2(6, 4), new Vector2(6, 6), new Vector2(4, 6) })  // Separated polygons
            };

            // Act
            var result = collisionsCalculator.GetExistingCollisions(subject, potentialCollisions.Select(x => x.Body).ToList());

            // Assert
            result.Should().BeEmpty();
        }
    }
}
