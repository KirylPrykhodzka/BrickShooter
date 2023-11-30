using AutoFixture;
using BrickShooter.Physics;
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
            GlobalObjects.AbsoluteDeltaTime = 2f;
        }

        [Test]
        public void FindNextCollisions_NoCollision_ReturnsEmptyList()
        {
            // Arrange
            GlobalObjects.AbsoluteDeltaTime = 1f;
            var subject = new MaterialObjectMock();
            var collisionObject = new MaterialObjectMock();
            subject.Velocity = new Vector2(1, 0);
            collisionObject.Velocity = new Vector2(-1, 0);
            subject.Colliders.Add(new ColliderPolygon(subject, fixture.Create("CollisionLayer"), new List<Vector2> { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) }));
            collisionObject.Colliders.Add(new ColliderPolygon(collisionObject, fixture.Create("CollisionLayer"), new List<Vector2> { new Vector2(4, 0), new Vector2(5, 0), new Vector2(5, 1), new Vector2(4, 1) }));

            // Act
            var result = collisionsCalculator.FindNextCollisions(new List<CollisionPair> { new CollisionPair(subject.Colliders.First(), collisionObject.Colliders.First())});

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public void FindNextCollisions_CollisionDetected_ReturnsCollisionInfo()
        {
            // Arrange
            var subject = new MaterialObjectMock(new List<Vector2> { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, 2), new Vector2(0, 2) });
            var collisionObject = new MaterialObjectMock(new List<Vector2> { new Vector2(3, 0), new Vector2(5, 0), new Vector2(5, 2), new Vector2(3, 2) });
            subject.Velocity = new Vector2(2, 0);
            collisionObject.Velocity = new Vector2(-1, 0);

            // Act
            var result = collisionsCalculator.FindNextCollisions(new List<CollisionPair> { new CollisionPair(subject.Colliders.First(), collisionObject.Colliders.First()) });

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
            subject.Colliders.Add(new ColliderPolygon(subject, fixture.Create("CollisionLayer"), new List<Vector2> { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, 2), new Vector2(0, 2) }));
            collisionObject.Colliders.Add(new ColliderPolygon(collisionObject, fixture.Create("CollisionLayer"), new List<Vector2> { new Vector2(3, 0), new Vector2(5, 0), new Vector2(5, 2), new Vector2(3, 2) }));

            // Act
            var result = collisionsCalculator.CalculateFutureCollisionResult(new CollisionPair(subject.Colliders.First(), collisionObject.Colliders.First()));

            // Assert
            result.CollisionObject.Should().Be(collisionObject.Colliders.First());
        }

        [Test]
        public void CalculateFutureCollisionResult_ShouldCorrectlyCalculateRelativeVelocity()
        {
            // Arrange
            var subject = new MaterialObjectMock();
            var collisionObject = new MaterialObjectMock();
            subject.Velocity = fixture.Create<Vector2>();
            collisionObject.Velocity = fixture.Create<Vector2>();
            subject.Colliders.Add(new ColliderPolygon(subject, fixture.Create("CollisionLayer"), new List<Vector2> { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, 2), new Vector2(0, 2) }));
            collisionObject.Colliders.Add(new ColliderPolygon(collisionObject, fixture.Create("CollisionLayer"), new List<Vector2> { new Vector2(3, 0), new Vector2(5, 0), new Vector2(5, 2), new Vector2(3, 2) }));

            // Act
            var result = collisionsCalculator.CalculateFutureCollisionResult(new CollisionPair(subject.Colliders.First(), collisionObject.Colliders.First()));

            // Assert
            result.RelativeVelocity.Should().Be((subject.Velocity - collisionObject.Velocity) * GlobalObjects.ScaledDeltaTime);
        }

        [TestCaseSource(nameof(FutureCollisionCases))]
        public void CalculateFutureCollisionResult_ShouldCorrectlyDetermineThatObjectsWillCollide(Vector2[] subjectPoints, Vector2 subjectVelocity, Vector2[] objectPoints, Vector2 objectVelocity)
        {
            // Arrange
            var subject = new MaterialObjectMock(subjectPoints)
            {
                Velocity = subjectVelocity
            };

            var collisionObject = new MaterialObjectMock(objectPoints)
            {
                Velocity = objectVelocity
            };

            // Act
            var result = collisionsCalculator.CalculateFutureCollisionResult(new CollisionPair(subject.Colliders.First(), collisionObject.Colliders.First()));

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
            subject.Colliders.Add(new ColliderPolygon(subject, fixture.Create("CollisionLayer"), subjectPoints));

            var collisionObject = new MaterialObjectMock
            {
                Velocity = objectVelocity
            };
            collisionObject.Colliders.Add(new ColliderPolygon(collisionObject, fixture.Create("CollisionLayer"), objectPoints));

            // Act
            var result = collisionsCalculator.CalculateFutureCollisionResult(new CollisionPair(subject.Colliders.First(), collisionObject.Colliders.First()));

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
            GlobalObjects.AbsoluteDeltaTime = 1f;
            var subject = new MaterialObjectMock(subjectPoints)
            {
                Velocity = subjectVelocity
            };


            var collisionObject = new MaterialObjectMock(objectPoints)
            {
                Velocity = objectVelocity
            };

            // Act
            var result = collisionsCalculator.CalculateFutureCollisionResult(new CollisionPair(subject.Colliders.First(), collisionObject.Colliders.First()));

            // Assert
            result.WillCollide.Should().BeTrue();
            result.CollisionPoint.Should().Be(closestCollisionPoint);
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
        public void CalculateFutureCollisionResult_ShouldCorrectlyCalculateCollisionEdge(Vector2[] subjectPoints, Vector2 subjectVelocity, Vector2[] objectPoints, Vector2 objectVelocity, Vector2 collisionEdge)
        {
            // Arrange
            var subject = new MaterialObjectMock(subjectPoints)
            {
                Velocity = subjectVelocity
            };

            var collisionObject = new MaterialObjectMock(objectPoints)
            {
                Velocity = objectVelocity
            };

            // Act
            var result = collisionsCalculator.CalculateFutureCollisionResult(new CollisionPair(subject.Colliders.First(), collisionObject.Colliders.First()));

            // Assert
            result.WillCollide.Should().BeTrue();
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
                    new Vector2(0, 4)
                );
                yield return new TestCaseData(
                    new Vector2[] { new Vector2(0, 0), new Vector2(-4, 0), new Vector2(2, -2) },
                    new Vector2(0, -5),
                    new Vector2[] { new Vector2(1, -6), new Vector2(3, 0), new Vector2(3, -7) },
                    new Vector2(0, 0),
                    new Vector2(-2, -6)
                );
            }
        }

        [TestCaseSource(nameof(DistanceToCollisionCases))]
        public void CalculateFutureCollisionResult_ShouldCorrectlyCalculateDistanceToCollision(Vector2[] subjectPoints, Vector2 subjectVelocity, Vector2[] objectPoints, Vector2 objectVelocity, float distanceToCollision)
        {
            // Arrange
            var subject = new MaterialObjectMock(subjectPoints)
            {
                Velocity = subjectVelocity
            };

            var collisionObject = new MaterialObjectMock(objectPoints)
            {
                Velocity = objectVelocity
            };

            // Act
            var result = collisionsCalculator.CalculateFutureCollisionResult(new CollisionPair(subject.Colliders.First(), collisionObject.Colliders.First()));

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
