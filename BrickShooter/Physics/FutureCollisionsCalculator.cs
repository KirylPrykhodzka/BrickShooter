using BrickShooter.Extensions;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using BrickShooter.Resources;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrickShooter.Physics
{
    public class FutureCollisionsCalculator : IFutureCollisionsCalculator
    {
        private readonly MemoryCache<(IList<Vector2> colliderPoints, Vector2 relativeVelocity), IList<Vector2>> frontFacingPointsCache = new(1000);

        public IList<FutureCollisionInfo> FindNextCollisions(IMaterialObject collisionSubject, IEnumerable<IMaterialObject> potentialCollisions)
        {
            return potentialCollisions.Select(x => CalculateFutureCollisionResult(collisionSubject, x))
                .Where(x => x.WillCollide)
                .ToList();
        }

        /// <summary>
        /// finds all points and edges of both polygons that can potentially collide based on relative velocity
        /// checks which point will collide first with which edge
        /// determines whether the collision will happen within this update
        /// </summary>
        /// <param name="collisionSubject"></param>
        /// <param name="collisionObject"></param>
        /// <returns></returns>
        public FutureCollisionInfo CalculateFutureCollisionResult(IMaterialObject collisionSubject, IMaterialObject collisionObject)
        {
            var result = new FutureCollisionInfo
            {
                CollisionObject = collisionObject,
                RelativeVelocity = (collisionSubject.Velocity - collisionObject.Velocity) * GlobalObjects.DeltaTime
            };

            var subjectFrontFacingPoints = GetFrontFacingPoints(collisionSubject.Body, result.RelativeVelocity);
            var objectFrontFacingPoints = GetFrontFacingPoints(collisionObject.Body, -result.RelativeVelocity);
            var subjectFrontFacingEdges = GetFrontFacingEdges(collisionSubject.Body.Points, subjectFrontFacingPoints);
            var objectFrontFacingEdges = GetFrontFacingEdges(collisionObject.Body.Points, objectFrontFacingPoints);

            (Vector2 point, (Vector2 point1, Vector2 point2) edge, float projectionLength) closestCollision = (default, default, float.PositiveInfinity);
            foreach (var frontFacingPoint in subjectFrontFacingPoints)
            {
                foreach (var frontFacingEdge in objectFrontFacingEdges)
                {
                    var intersectionResult = FindIntersection((frontFacingPoint, frontFacingPoint + result.RelativeVelocity), frontFacingEdge);
                    if (!intersectionResult.intersect)
                    {
                        continue;
                    }
                    //check if edge intersects with the line formed by point and point + fixedVelocity
                    //if yes, projectionLength = distance between point and point of intersection
                    var projectionLength = (intersectionResult.pointOfIntersection - frontFacingPoint).Length();
                    if (projectionLength < closestCollision.projectionLength)
                    {
                        closestCollision = (frontFacingPoint, frontFacingEdge, projectionLength);
                    }
                }
            }
            foreach (var frontFacingPoint in objectFrontFacingPoints)
            {
                foreach (var frontFacingEdge in subjectFrontFacingEdges)
                {
                    var intersectionResult = FindIntersection((frontFacingPoint, frontFacingPoint - result.RelativeVelocity), frontFacingEdge);
                    if (!intersectionResult.intersect)
                    {
                        continue;
                    }
                    //check if edge intersects with the line formed by point and point + fixedVelocity
                    //if yes, projectionLength = distance between point and point of intersection
                    var projectionLength = (intersectionResult.pointOfIntersection - frontFacingPoint).Length();
                    if (projectionLength < closestCollision.projectionLength)
                    {
                        closestCollision = (frontFacingPoint, frontFacingEdge, projectionLength);
                    }
                }
            }

            result.ClosestCollisionPoint = closestCollision.point;
            result.CollisionEdge = closestCollision.edge;
            result.DistanceToCollision = closestCollision.projectionLength;
            result.WillCollide = Math.Abs(closestCollision.projectionLength) < result.RelativeVelocity.Length();
            return result;
        }

        //selects points of a material object's collider that can cause collision based on provided velocity
        private IList<Vector2> GetFrontFacingPoints(IColliderPolygon polygon, Vector2 velocity)
        {
            var points = polygon.Points;
            if (velocity == Vector2.Zero || points.Count <= 2)
            {
                return points.ToList();
            }

            if (frontFacingPointsCache.TryGetValue((points, velocity), out IList<Vector2> cachedValue))
            {
                return cachedValue;
            }

            var perpendicularVelocity = new Vector2(velocity.Y, -velocity.X);
            char projectionComparisonAxis = Math.Abs(perpendicularVelocity.X) > Math.Abs(perpendicularVelocity.Y) ? 'x' : 'y';

            //find width of an object relative to its movement direction
            IEnumerable<(Vector2 key, Vector2 value)> perpendicularProjections = points
                .Select(x => (x, x.Project(perpendicularVelocity)));

            //left- and right-most points of the polygon relative to its velocity
            Vector2 min = Vector2.Zero;
            Vector2 max = Vector2.Zero;

            if (projectionComparisonAxis == 'x')
            {
                perpendicularProjections = perpendicularProjections.OrderBy(x => x.value.X);
                var minX = perpendicularProjections.First().value.X;
                var maxX = perpendicularProjections.Last().value.X;
                var minGroup = perpendicularProjections.TakeWhile(x => x.value.X == minX);
                var maxGroup = perpendicularProjections.SkipWhile(x => x.value.X != maxX);
                if (velocity.Y > 0)
                {
                    min = minGroup.MaxBy(x => x.key.Y).key;
                    max = maxGroup.MaxBy(x => x.key.Y).key;
                }
                else
                {
                    min = minGroup.MinBy(x => x.key.Y).key;
                    max = maxGroup.MinBy(x => x.key.Y).key;
                }
            }
            else
            {
                perpendicularProjections = perpendicularProjections.OrderBy(x => x.value.Y);
                var minY = perpendicularProjections.First().value.Y;
                var maxY = perpendicularProjections.Last().value.Y;
                var minGroup = perpendicularProjections.TakeWhile(x => x.value.Y == minY);
                var maxGroup = perpendicularProjections.SkipWhile(x => x.value.Y != maxY);
                if (velocity.X > 0)
                {
                    min = minGroup.MaxBy(x => x.key.X).key;
                    max = maxGroup.MaxBy(x => x.key.X).key;
                }
                else
                {
                    min = minGroup.MinBy(x => x.key.X).key;
                    max = maxGroup.MinBy(x => x.key.X).key;
                }
            }

            //find all points closer to the front then min and max
            var result = new List<Vector2> { min, max };

            if (projectionComparisonAxis == 'x')
            {
                var center = polygon.Center.X;
                var leftOfX = points.Where(x => x.X < center);
                var rightOfX = points.Where(x => x.X >= center);
                if (velocity.Y > 0)
                {
                    result.AddRange(leftOfX.Where(x => x.Y > min.Y));
                    result.AddRange(rightOfX.Where(x => x.Y > max.Y));
                }
                else
                {
                    result.AddRange(leftOfX.Where(x => x.Y < min.Y));
                    result.AddRange(rightOfX.Where(x => x.Y < max.Y));
                }
            }
            else
            {
                var center = polygon.Center.Y;
                var aboveY = points.Where(x => x.Y < center);
                var belowY = points.Where(x => x.Y >= center);
                if (velocity.X > 0)
                {
                    result.AddRange(aboveY.Where(x => x.X > min.X));
                    result.AddRange(belowY.Where(x => x.X > max.X));
                }
                else
                {
                    result.AddRange(aboveY.Where(x => x.X < min.X));
                    result.AddRange(belowY.Where(x => x.X < max.X));
                }
            }

            frontFacingPointsCache.Add((points, velocity), result);
            return result;
        }

        //get all edges that front facing points belong to (edges are described by two points delimiting them)
        private static IList<(Vector2 point1, Vector2 point2)> GetFrontFacingEdges(IList<Vector2> allPoints, IList<Vector2> frontFacingPoints)
        {
            List<(Vector2 point1, Vector2 point2)> result = new();
            var firstPoint = allPoints.First();
            var lastPoint = allPoints.Last();
            if (frontFacingPoints.Contains(firstPoint) && frontFacingPoints.Contains(lastPoint))
            {
                result.Add((firstPoint, lastPoint));
            }
            for (int i = 1; i < allPoints.Count; i++)
            {
                var currentPoint = allPoints[i];
                var previousPoint = allPoints[i - 1];
                if (frontFacingPoints.Contains(currentPoint) && frontFacingPoints.Contains(previousPoint))
                {
                    result.Add((currentPoint, previousPoint));
                }
            }

            return result;
        }

        //http://www.csharphelper.com/howtos/howto_segment_intersection.html
        private static (bool intersect, Vector2 pointOfIntersection) FindIntersection((Vector2 point1, Vector2 point2) segment1, (Vector2 point1, Vector2 point2) segment2)
        {
            // Get the segments' parameters.
            float dx12 = segment1.point2.X - segment1.point1.X;
            float dy12 = segment1.point2.Y - segment1.point1.Y;
            float dx34 = segment2.point2.X - segment2.point1.X;
            float dy34 = segment2.point2.Y - segment2.point1.Y;

            // Solve for t1 and t2
            float denominator = dy12 * dx34 - dx12 * dy34;

            float t1 =
                ((segment1.point1.X - segment2.point1.X) * dy34 + (segment2.point1.Y - segment1.point1.Y) * dx34)
                    / denominator;
            if (float.IsInfinity(t1))
            {
                return (false, Vector2.Zero);
            }

            float t2 =
                ((segment2.point1.X - segment1.point1.X) * dy12 + (segment1.point1.Y - segment2.point1.Y) * dx12)
                    / -denominator;

            // Find the point of intersection.
            var pointOfIntersection = new Vector2(segment1.point1.X + dx12 * t1, segment1.point1.Y + dy12 * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            var intersect =
                t1 >= 0 && t1 <= 1 &&
                 t2 >= 0 && t2 <= 1;

            return (intersect, pointOfIntersection);
        }
    }
}
