using BrickShooter.Extensions;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrickShooter.Physics
{
    public class CollisionCalculator : ICollisionCalculator
    {
        public Vector2 GetTranslationVectorForExistingCollisions(MaterialObject collisionSubject, IEnumerable<MaterialObject> potentialCollisions)
        {
            var existingCollisions = potentialCollisions
                .Select(x => GetCollisionResult(collisionSubject, x))
                .Where(x => x.isColliding);
            if(existingCollisions.Count() == 0)
            {
                return Vector2.Zero;
            }
            return existingCollisions.MinBy(x => x.minimalTranslationVector.Magnitude()).minimalTranslationVector;
        }

        public IList<CollisionPredictionResult> FindFutureCollisions(MaterialObject collisionSubject, IEnumerable<MaterialObject> potentialCollisions)
        {
            return potentialCollisions.Select(x => CalculateFutureCollisionByVelocity(collisionSubject, x)).Where(x => x.WillCollide).ToList();
        }

        public static (bool isColliding, Vector2 minimalTranslationVector) GetCollisionResult(MaterialObject collisionSubject, MaterialObject collisionObject)
        {
            var subjectEdges = BuildEdges(collisionSubject.GlobalColliderPolygon.Points).ToList();
            var objectEdges = BuildEdges(collisionObject.GlobalColliderPolygon.Points).ToList();

            bool isColliding = true;
            float minIntervalDistance = float.PositiveInfinity;
            Vector2 translationAxis = Vector2.Zero;
            Vector2 edge;

            // Loop through all the edges of both polygons
            for (int edgeIndex = 0; edgeIndex < subjectEdges.Count + objectEdges.Count; edgeIndex++)
            {
                if (edgeIndex < subjectEdges.Count)
                {
                    edge = subjectEdges[edgeIndex];
                }
                else
                {
                    edge = objectEdges[edgeIndex - subjectEdges.Count];
                }

                // ===== 1. Find if the polygons are currently intersecting =====

                // Find the axis perpendicular to the current edge
                Vector2 axis = new Vector2(-edge.Y, edge.X);
                axis.Normalize();

                // Find the projection of the polygon on the current axis
                float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
                ProjectPolygon(axis, collisionSubject.GlobalColliderPolygon, ref minA, ref maxA);
                ProjectPolygon(axis, collisionObject.GlobalColliderPolygon, ref minB, ref maxB);

                // Check if the polygon projections are currentlty intersecting
                float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance > 0)
                {
                    return (false, Vector2.Zero);
                }

                // Check if the current interval distance is the minimum one. If so store
                // the interval distance and the current distance.
                // This will be used to calculate the minimum translation vector
                intervalDistance = Math.Abs(intervalDistance);
                if (intervalDistance < minIntervalDistance)
                {
                    minIntervalDistance = intervalDistance;
                    translationAxis = axis;

                    Vector2 d = collisionSubject.GlobalColliderPolygon.Center - collisionObject.GlobalColliderPolygon.Center;
                    if (d.DotProduct(translationAxis) < 0)
                        translationAxis = -translationAxis;
                }
            }

            return (isColliding, translationAxis * minIntervalDistance);
        }

        private static void ProjectPolygon(Vector2 axis, ColliderPolygon polygon, ref float min, ref float max)
        {
            // To project a point on an axis use the dot product
            float dotProduct = axis.DotProduct(polygon.Points[0]);
            min = dotProduct;
            max = dotProduct;
            for (int i = 0; i < polygon.Points.Count; i++)
            {
                dotProduct = polygon.Points[i].DotProduct(axis);
                if (dotProduct < min)
                {
                    min = dotProduct;
                }
                else
                {
                    if (dotProduct > max)
                    {
                        max = dotProduct;
                    }
                }
            }
        }

        private static float IntervalDistance(float minA, float maxA, float minB, float maxB)
        {
            if (minA < minB)
            {
                return minB - maxA;
            }
            else
            {
                return minA - maxB;
            }
        }

        private static IEnumerable<Vector2> BuildEdges(IList<Vector2> points)
        {
            var result = new List<Vector2>();
            Vector2 p1;
            Vector2 p2;
            for (int i = 0; i < points.Count; i++)
            {
                p1 = points[i];
                if (i + 1 >= points.Count)
                {
                    p2 = points[0];
                }
                else
                {
                    p2 = points[i + 1];
                }
                result.Add(p2 - p1);
            }

            return result;
        }

        public static CollisionPredictionResult CalculateFutureCollisionByVelocity(MaterialObject collisionSubject, MaterialObject collisionObject)
        {
            CollisionPredictionResult result = new()
            {
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
                    var projectionLength = frontFacingPoint.DistanceTo(frontFacingEdge);
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
                    var projectionLength = frontFacingPoint.DistanceTo(frontFacingEdge);
                    if (projectionLength < closestCollision.projectionLength)
                    {
                        closestCollision = (frontFacingPoint, frontFacingEdge, projectionLength);
                    }
                }
            }

            result.ClosestCollisionPoint = closestCollision.point;
            result.CollisionEdge = closestCollision.edge;
            result.CollisionDistance = closestCollision.projectionLength;
            result.WillCollide = Math.Abs(closestCollision.projectionLength) < result.RelativeVelocity.Magnitude();
            return result;
        }

        //selects points of a material object's collider that can cause collision based on provided velocity
        public static IList<Vector2> GetFrontFacingPoints(MaterialObject materialObject, Vector2 velocity)
        {
            if (velocity == Vector2.Zero || materialObject.LocalColliderPolygon.Points.Count <= 2)
            {
                return materialObject.LocalColliderPolygon.Points;
            }

            var localColliderPoints = materialObject.LocalColliderPolygon.Points
                //this makes sure that polygon center is in (0,0) which makes it much easier to operate on
                .Select(x => x - materialObject.LocalColliderPolygon.Center)
                .ToList();

            var perpendicularAxis = new Vector2(velocity.Y, velocity.X);
            char projectionComparisonAxis = Math.Abs(perpendicularAxis.X) > Math.Abs(perpendicularAxis.Y) ? 'x' : 'y';

            //find width of an object relative to its movement direction
            IEnumerable<(Vector2 key, Vector2 value)> perpendicularProjections = localColliderPoints
                //need to add * new Vector2(1, -1) because in Monogame Y axis is inverted
                .Select(x => (x, x.Project(perpendicularAxis)));

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
            var otherLocalColliderPoints = localColliderPoints.Where(x => x != min && x != max);

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
            result = result.Select(x => x + materialObject.LocalColliderPolygon.Center + materialObject.Position).ToList();
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
    }
}
