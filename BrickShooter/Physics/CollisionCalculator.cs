using BrickShooter.Extensions;
using BrickShooter.Helpers;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace BrickShooter.Physics
{
    public class CollisionCalculator : ICollisionCalculator
    {
        public IList<CollisionData> GetExistingCollisions(MaterialObject collisionSubject, IEnumerable<MaterialObject> potentialCollisions)
        {
            //if an object collides with multiple objects at the same time, we remove 1 collision per update to make it simpler
            return potentialCollisions
                .Select(x => SATCollisionCalculator.GetCollisionResult(collisionSubject, x))
                .Where(x => x.isColliding)
                .Select(x => new CollisionData
                {
                    MinimalTranslationVector = x.minimalTranslationVector
                })
                .ToList();
        }

        public IList<CollisionPredictionResult> FindNextCollisions(MaterialObject collisionSubject, IEnumerable<MaterialObject> potentialCollisions)
        {
            return potentialCollisions
                .Select(x => CalculateFutureCollisionByVelocity(collisionSubject, x))
                .Where(x => x.WillCollide)
                .ToList();
        }

        public static CollisionPredictionResult CalculateFutureCollisionByVelocity(MaterialObject collisionSubject, MaterialObject collisionObject)
        {
            CollisionPredictionResult result = new()
            {
                CollisionSubject = collisionSubject,
                CollisionObject = collisionObject,
                RelativeVelocity = (collisionSubject.Velocity - collisionObject.Velocity) * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds,
            };

            var subjectFrontFacingPoints = GetFrontFacingPoints(collisionSubject, result.RelativeVelocity);
            var objectFrontFacingPoints = GetFrontFacingPoints(collisionObject, -result.RelativeVelocity);
            var subjectFrontFacingEdges = GetFrontFacingEdges(collisionSubject, subjectFrontFacingPoints);
            var objectFrontFacingEdges = GetFrontFacingEdges(collisionObject, objectFrontFacingPoints);
            //for every subject front facing point, 
            //the shortest projection will tell the point and edge of collision

            (Vector2 point, (Vector2 point1, Vector2 point2) edge, float projectionLength) closestCollision = (default, default, float.PositiveInfinity);
            foreach(var frontFacingPoint in subjectFrontFacingPoints)
            {
                foreach(var frontFacingEdge in objectFrontFacingEdges)
                {
                    var intersectionResult = FindIntersection((frontFacingPoint, frontFacingPoint + result.RelativeVelocity), frontFacingEdge);
                    if(!intersectionResult.intersect)
                    {
                        continue;
                    }
                    //check if edge intersects with the line formed by point and point + fixedVelocity
                    //if yes, projectionLength = distance between point and point of intersection
                    var projectionLength = (intersectionResult.pointOfIntersection - frontFacingPoint).Magnitude();
                    if(projectionLength < closestCollision.projectionLength)
                    {
                        closestCollision = (frontFacingPoint, frontFacingEdge, projectionLength);
                    }
                }
            }
            foreach (var frontFacingPoint in objectFrontFacingPoints)
            {
                foreach (var frontFacingEdge in subjectFrontFacingEdges)
                {
                    var intersectionResult = FindIntersection((frontFacingPoint, frontFacingPoint + result.RelativeVelocity), frontFacingEdge);
                    if (!intersectionResult.intersect)
                    {
                        continue;
                    }
                    //check if edge intersects with the line formed by point and point + fixedVelocity
                    //if yes, projectionLength = distance between point and point of intersection
                    var projectionLength = (intersectionResult.pointOfIntersection - frontFacingPoint).Magnitude();
                    if (projectionLength < closestCollision.projectionLength)
                    {
                        closestCollision = (frontFacingPoint, frontFacingEdge, projectionLength);
                    }
                }
            }

            result.ClosestCollisionPoint = closestCollision.point;
            result.CollisionEdge = closestCollision.edge;
            result.DistanceToCollision = closestCollision.projectionLength;
            result.WillCollide = Math.Abs(closestCollision.projectionLength) < result.RelativeVelocity.Magnitude();
            return result;
        }

        //selects points of a material object's collider that can cause collision based on provided velocity
        public static IList<Vector2> GetFrontFacingPoints(MaterialObject materialObject, Vector2 velocity)
         {
            if (velocity == Vector2.Zero || materialObject.GlobalColliderPolygon.Points.Count <= 2)
            {
                return materialObject.GlobalColliderPolygon.Points;
            }

            var localizedColliderPoints = materialObject.GlobalColliderPolygon.Points
                //this makes sure that center of localizedColliderPoints is (0,0) which makes finding front facing points much handier
                .Select(x => x - materialObject.GlobalColliderPolygon.Center)
                .ToList();

            var perpendicularVelocity = new Vector2(velocity.Y, -velocity.X);
            char projectionComparisonAxis = Math.Abs(perpendicularVelocity.X) > Math.Abs(perpendicularVelocity.Y) ? 'x' : 'y';

            //find width of an object relative to its movement direction
            IEnumerable<(Vector2 key, Vector2 value)> perpendicularProjections = localizedColliderPoints
                .Select(x => (x, x.Project(perpendicularVelocity)));

            //left- and right-most points of the polygon relative to its velocity
            Vector2 min = Vector2.Zero;
            Vector2 max = Vector2.Zero;

            if (projectionComparisonAxis == 'x')
            {
                var minX = perpendicularProjections.Min(x => x.value.X);
                var maxX = perpendicularProjections.Max(x => x.value.X);
                var minGroup = perpendicularProjections.Where(x => x.value.X == minX);
                var maxGroup = perpendicularProjections.Where(x => x.value.X == maxX);
                if(velocity.Y > 0)
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
                var minY = perpendicularProjections.Min(x => x.value.Y);
                var maxY = perpendicularProjections.Max(x => x.value.Y);
                var minGroup = perpendicularProjections.Where(x => x.value.Y == minY);
                var maxGroup = perpendicularProjections.Where(x => x.value.Y == maxY);
                if(velocity.X > 0)
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
            var otherLocalColliderPoints = localizedColliderPoints.Where(x => x != min && x != max);

            if (projectionComparisonAxis == 'y')
            {
                var aboveY = otherLocalColliderPoints.Where(x => x.Y < 0);
                var belowY = otherLocalColliderPoints.Where(x => x.Y >= 0);
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
            else
            {
                var leftOfX = otherLocalColliderPoints.Where(x => x.X < 0);
                var rightToX = otherLocalColliderPoints.Where(x => x.X >= 0);
                if(velocity.Y > 0)
                {
                    result.AddRange(leftOfX.Where(x => x.Y > min.Y));
                    result.AddRange(rightToX.Where(x => x.Y > max.Y));
                }
                else
                {
                    result.AddRange(leftOfX.Where(x => x.Y < min.Y));
                    result.AddRange(rightToX.Where(x => x.Y < max.Y));
                }
            }

            //need to "move" polygon back to its original position relative to object's position before returning
            //and then we also need to understand front facing points position in global space
            result = result.Select(x => x + materialObject.GlobalColliderPolygon.Center).ToList();
            return result;
        }

        //get all edges that front facing points belong to (edges are described by two points delimiting them)
        public static IList<(Vector2 point1, Vector2 point2)> GetFrontFacingEdges(MaterialObject materialObject, IList<Vector2> frontFacingPoints)
        {
            List<(Vector2 point1, Vector2 point2)> result = new();
            var firstPoint = materialObject.GlobalColliderPolygon.Points.First();
            var lastPoint = materialObject.GlobalColliderPolygon.Points.Last();
            if (frontFacingPoints.Contains(firstPoint) && frontFacingPoints.Contains(lastPoint))
            {
                result.Add((firstPoint, lastPoint));
            }
            for(int i = 1; i < materialObject.GlobalColliderPolygon.Points.Count; i++)
            {
                var currentPoint = materialObject.GlobalColliderPolygon.Points[i];
                var previousPoint = materialObject.GlobalColliderPolygon.Points[i - 1];
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
            float denominator = (dy12 * dx34 - dx12 * dy34);

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
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));

            return (intersect, pointOfIntersection);
        }
    }
}
