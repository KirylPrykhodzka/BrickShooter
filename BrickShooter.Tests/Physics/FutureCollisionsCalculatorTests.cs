using AutoFixture;
using BrickShooter.Physics;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using BrickShooter.Tests.Mocks;
using FluentAssertions;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using System.Collections;

namespace BrickShooter.Tests.Physics
{
    [TestFixture]
    public class FutureCollisionsCalculatorTests
    {
        private FutureCollisionsCalculator collisionsCalculator;
        private IFixture fixture;

        [SetUp]
        public void Setup()
        {
            fixture = new Fixture();
            collisionsCalculator = new FutureCollisionsCalculator();
            GlobalObjects.DeltaTime = 2f;
        }

        [Test]
        public void FindNextCollisions_NoCollision_ReturnsEmptyList()
        {
            // Arrange
            GlobalObjects.DeltaTime = 1f;
            var subject = new MaterialObjectMock();
            var collisionObject = new MaterialObjectMock();
            subject.Velocity = new Vector2(1, 0);
            collisionObject.Velocity = new Vector2(-1, 0);
            subject.ColliderPolygon.SetPoints(new List<Vector2> { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) });
            collisionObject.ColliderPolygon.SetPoints(new List<Vector2> { new Vector2(4, 0), new Vector2(5, 0), new Vector2(5, 1), new Vector2(4, 1) });

            // Act
            var result = collisionsCalculator.FindNextCollisions(subject, new List<MaterialObject> { collisionObject });

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public void FindNextCollisions_CollisionDetected_ReturnsCollisionInfo()
        {
            // Arrange
            var subject = new MaterialObjectMock();
            var collisionObject = new MaterialObjectMock();
            subject.Velocity = new Vector2(2, 0);
            collisionObject.Velocity = new Vector2(-1, 0);
            subject.ColliderPolygon.SetPoints(new List<Vector2> { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, 2), new Vector2(0, 2) });
            collisionObject.ColliderPolygon.SetPoints(new List<Vector2> { new Vector2(3, 0), new Vector2(5, 0), new Vector2(5, 2), new Vector2(3, 2) });

            // Act
            var result = collisionsCalculator.FindNextCollisions(subject, new List<MaterialObject> { collisionObject });

            // Assert
            result.Count.Should().Be(1);
            result.First().WillCollide.Should().BeTrue();
        }

        [Test]
        public void CalculateFutureCollisionResult_ShouldCorrectlySetCollisionObject()
        {
            // Arrange
            var subject = new MaterialObjectMock();
            var collisionObject = new MaterialObjectMock();
            subject.Velocity = fixture.Create<Vector2>();
            collisionObject.Velocity = fixture.Create<Vector2>();
            subject.ColliderPolygon.SetPoints(new List<Vector2> { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, 2), new Vector2(0, 2) });
            collisionObject.ColliderPolygon.SetPoints(new List<Vector2> { new Vector2(3, 0), new Vector2(5, 0), new Vector2(5, 2), new Vector2(3, 2) });

            // Act
            var result = collisionsCalculator.CalculateFutureCollisionResult(subject, collisionObject);

            // Assert
            result.CollisionObject.Should().Be(collisionObject);
        }

        [Test]
        public void CalculateFutureCollisionResult_ShouldCorrectlyCalculateRelativeVelocity()
        {
            // Arrange
            var subject = new MaterialObjectMock();
            var collisionObject = new MaterialObjectMock();
            subject.Velocity = fixture.Create<Vector2>();
            collisionObject.Velocity = fixture.Create<Vector2>();
            subject.ColliderPolygon.SetPoints(new List<Vector2> { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, 2), new Vector2(0, 2) });
            collisionObject.ColliderPolygon.SetPoints(new List<Vector2> { new Vector2(3, 0), new Vector2(5, 0), new Vector2(5, 2), new Vector2(3, 2) });

            // Act
            var result = collisionsCalculator.CalculateFutureCollisionResult(subject, collisionObject);

            // Assert
            result.RelativeVelocity.Should().Be((subject.Velocity - collisionObject.Velocity) * GlobalObjects.DeltaTime);
        }

        [TestCaseSource(nameof(FutureCollisionCases))]
        public void CalculateFutureCollisionResult_ShouldCorrectlyDetermineThatObjectsWillCollide(Vector2[] subjectPoints, Vector2 subjectVelocity, Vector2[] objectPoints, Vector2 objectVelocity)
        {
            // Arrange
            var subject = new MaterialObjectMock
            {
                Velocity = subjectVelocity
            };
            subject.ColliderPolygon.SetPoints(new List<Vector2>(subjectPoints));

            var collisionObject = new MaterialObjectMock
            {
                Velocity = objectVelocity
            };
            collisionObject.ColliderPolygon.SetPoints(new List<Vector2>(objectPoints));

            // Act
            var result = collisionsCalculator.CalculateFutureCollisionResult(subject, collisionObject);

            // Assert
            result.WillCollide.Should().BeTrue();
        }

        public static IEnumerable FutureCollisionCases
        {
            get
            {
                yield return new TestCaseData(
                    new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, 2) },
                    new Vector2(2, 0),
                    new Vector2[] { new Vector2(3, 0), new Vector2(5, 0), new Vector2(5, 2) },
                    new Vector2(-1, 0)
                );

                yield return new TestCaseData(
                    new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, 2) },
                    new Vector2(2, 0),
                    new Vector2[] { new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, 2) },
                    new Vector2(-1, 0)
                );

                yield return new TestCaseData(
                    new Vector2[] { new Vector2(0, 0), new Vector2(2, 2), new Vector2(2, 0) },
                    new Vector2(2, 2),
                    new Vector2[] { new Vector2(3, 0), new Vector2(5, 2), new Vector2(5, 0) },
                    new Vector2(-2, 2)
                );

                yield return new TestCaseData(
                    new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, 2) },
                    new Vector2(2, 0),
                    new Vector2[] { new Vector2(4, 0), new Vector2(6, 0), new Vector2(6, 2) },
                    new Vector2(-1, 0)
                );

                yield return new TestCaseData(
                    new Vector2[] { new Vector2(0, 0), new Vector2(2, 2), new Vector2(2, 0) },
                    new Vector2(2, 2),
                    new Vector2[] { new Vector2(4, 0), new Vector2(6, 2), new Vector2(6, 0) },
                    new Vector2(-2, 2)
                );
            }
        }

        [TestCaseSource(nameof(FutureNonCollisionCases))]
        public void CalculateFutureCollisionResult_ShouldCorrectlyDetermineThatObjectsWillNotCollide(Vector2[] subjectPoints, Vector2 subjectVelocity, Vector2[] objectPoints, Vector2 objectVelocity)
        {
            // Arrange
            var subject = new MaterialObjectMock
            {
                Velocity = subjectVelocity
            };
            subject.ColliderPolygon.SetPoints(new List<Vector2>(subjectPoints));

            var collisionObject = new MaterialObjectMock
            {
                Velocity = objectVelocity
            };
            collisionObject.ColliderPolygon.SetPoints(new List<Vector2>(objectPoints));

            // Act
            var result = collisionsCalculator.CalculateFutureCollisionResult(subject, collisionObject);

            // Assert
            result.WillCollide.Should().BeFalse();
        }

        public static IEnumerable FutureNonCollisionCases
        {
            get
            {
                yield return new TestCaseData(
                    new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, 2) },
                    new Vector2(1, 0),
                    new Vector2[] { new Vector2(3, 0), new Vector2(5, 0), new Vector2(5, 2) },
                    new Vector2(1, 0)
                );

                yield return new TestCaseData(
                    new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, 2) },
                    new Vector2(2, 0),
                    new Vector2[] { new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, 2) },
                    new Vector2(2, 0)
                );

                yield return new TestCaseData(
                    new Vector2[] { new Vector2(0, 0), new Vector2(2, 2), new Vector2(2, 0) },
                    new Vector2(2, 2),
                    new Vector2[] { new Vector2(3, 0), new Vector2(5, 2), new Vector2(5, 0) },
                    new Vector2(2, 2)
                );

                yield return new TestCaseData(
                    new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, 2) },
                    new Vector2(1, 0),
                    new Vector2[] { new Vector2(4, 0), new Vector2(6, 0), new Vector2(6, 2) },
                    new Vector2(1, 0)
                );

                yield return new TestCaseData(
                    new Vector2[] { new Vector2(0, 0), new Vector2(2, 2), new Vector2(2, 0) },
                    new Vector2(2, 2),
                    new Vector2[] { new Vector2(4, 0), new Vector2(6, 2), new Vector2(6, 0) },
                    new Vector2(2, 2)
                );
            }
        }

        [TestCaseSource(nameof(ClosestCollisionPointCases))]
        public void CalculateFutureCollisionResult_ShouldCorrectlyCalculateClosestCollisionPoint(Vector2[] subjectPoints, Vector2 subjectVelocity, Vector2[] objectPoints, Vector2 objectVelocity, Vector2 closestCollisionPoint)
        {
            // Arrange
            var subject = new MaterialObjectMock
            {
                Velocity = subjectVelocity
            };
            subject.ColliderPolygon.SetPoints(new List<Vector2>(subjectPoints));

            var collisionObject = new MaterialObjectMock
            {
                Velocity = objectVelocity
            };
            collisionObject.ColliderPolygon.SetPoints(new List<Vector2>(objectPoints));

            // Act
            var result = collisionsCalculator.CalculateFutureCollisionResult(subject, collisionObject);

            // Assert
            result.ClosestCollisionPoint.Should().Be(closestCollisionPoint);
        }

        public static IEnumerable ClosestCollisionPointCases
        {
            get
            {
                yield return new TestCaseData(
                    new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(3, -2) },
                    new Vector2(5, -5),
                    new Vector2[] { new Vector2(4, -2), new Vector2(4, -6), new Vector2(6, -4) },
                    new Vector2(-1, 2),
                    new Vector2(3, -2)
                );
                yield return new TestCaseData(
                    new Vector2[] { new Vector2(0, 0), new Vector2(-4, 0), new Vector2(2, -2) },
                    new Vector2(0, -5),
                    new Vector2[] { new Vector2(1, -6), new Vector2(3, 0), new Vector2(3, -7) },
                    new Vector2(0, 0),
                    new Vector2(2, -2)
                );
            }
        }

        [TestCaseSource(nameof(CollisionEdgeTestCases))]
        public void CalculateFutureCollisionResult_ShouldCorrectlyCalculateCollisionEdge(Vector2[] subjectPoints, Vector2 subjectVelocity, Vector2[] objectPoints, Vector2 objectVelocity, (Vector2, Vector2) collisionEdge)
        {
            // Arrange
            var subject = new MaterialObjectMock
            {
                Velocity = subjectVelocity
            };
            subject.ColliderPolygon.SetPoints(new List<Vector2>(subjectPoints));

            var collisionObject = new MaterialObjectMock
            {
                Velocity = objectVelocity
            };
            collisionObject.ColliderPolygon.SetPoints(new List<Vector2>(objectPoints));

            // Act
            var result = collisionsCalculator.CalculateFutureCollisionResult(subject, collisionObject);

            // Assert
            result.CollisionEdge.Should().Be(collisionEdge);
        }

        public static IEnumerable CollisionEdgeTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(3, -2) },
                    new Vector2(5, -5),
                    new Vector2[] { new Vector2(4, -2), new Vector2(4, -6), new Vector2(6, -4) },
                    new Vector2(-1, 2),
                    (new Vector2(4, -6), new Vector2(4, -2))
                );
                yield return new TestCaseData(
                    new Vector2[] { new Vector2(0, 0), new Vector2(-4, 0), new Vector2(2, -2) },
                    new Vector2(0, -5),
                    new Vector2[] { new Vector2(1, -6), new Vector2(3, 0), new Vector2(3, -7) },
                    new Vector2(0, 0),
                    (new Vector2(3, 0), new Vector2(1, -6))
                );
            }
        }

        [TestCaseSource(nameof(DistanceToCollisionCases))]
        public void CalculateFutureCollisionResult_ShouldCorrectlyCalculateDistanceToCollision(Vector2[] subjectPoints, Vector2 subjectVelocity, Vector2[] objectPoints, Vector2 objectVelocity, float distanceToCollision)
        {
            // Arrange
            var subject = new MaterialObjectMock
            {
                Velocity = subjectVelocity
            };
            subject.ColliderPolygon.SetPoints(new List<Vector2>(subjectPoints));

            var collisionObject = new MaterialObjectMock
            {
                Velocity = objectVelocity
            };
            collisionObject.ColliderPolygon.SetPoints(new List<Vector2>(objectPoints));

            // Act
            var result = collisionsCalculator.CalculateFutureCollisionResult(subject, collisionObject);

            // Assert
            result.DistanceToCollision.Should().BeApproximately(distanceToCollision, 0.01f);
        }

        public static IEnumerable DistanceToCollisionCases
        {
            get
            {
                yield return new TestCaseData(
                    new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(3, -2) },
                    new Vector2(5, -5),
                    new Vector2[] { new Vector2(4, -2), new Vector2(4, -6), new Vector2(6, -4) },
                    new Vector2(-1, 2),
                    1.54f
                );
                yield return new TestCaseData(
                    new Vector2[] { new Vector2(0, 0), new Vector2(-4, 0), new Vector2(2, -2) },
                    new Vector2(0, -5),
                    new Vector2[] { new Vector2(1, -6), new Vector2(3, 0), new Vector2(3, -7) },
                    new Vector2(0, 0),
                    1f
                );
            }
        }
    }
}
