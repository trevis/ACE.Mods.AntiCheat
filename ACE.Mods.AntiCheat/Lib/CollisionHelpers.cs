using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace ACE.Mods.AntiCheat.Lib
{
    internal static class CollisionHelpers
    {
        public static Vector3? GetDoorCollisionPoint(Position start, Position end, WorldObject door)
        {
            // TODO: this only currently works in the same landblock...
            if (start.Landblock != end.Landblock)
            {
                return null;
            }

            var doorPos = door.PhysicsObj.Position.ACEPosition();
            var pathStart = new Vector2(start.PositionX, start.PositionY);
            var pathEnd = new Vector2(end.PositionX, end.PositionY);

            var doorLine = GetDoorLine(doorPos);
            var intersection = GetLineIntersection(pathStart, pathEnd, doorLine.Item1, doorLine.Item2);

            if (intersection.HasValue)
            {
                // Convert back to 3D, using the door's Z position
                return new Vector3(intersection.Value.X, intersection.Value.Y, doorPos.PositionZ);
            }

            return null;
        }

        /// <summary>
        /// Creates a door line segment based on the door's position and rotation
        /// </summary>
        private static (Vector2, Vector2) GetDoorLine(Position doorPos)
        {
            float doorWidth = 3.0f;
            float halfWidth = doorWidth * 0.5f;

            var doorForward = Vector3.Normalize(Vector3.Transform(Vector3.UnitY, doorPos.Rotation));
            var perpendicular = new Vector2(-doorForward.Y, doorForward.X);

            var doorCenter = new Vector2(doorPos.PositionX, doorPos.PositionY);
            var doorStart = doorCenter - perpendicular * halfWidth;
            var doorEnd = doorCenter + perpendicular * halfWidth;

            return (doorStart, doorEnd);
        }

        /// <summary>
        /// Calculates the intersection point between two line segments
        /// </summary>
        /// <param name="p1">Start point of line 1</param>
        /// <param name="p2">End point of line 1</param>
        /// <param name="p3">Start point of line 2</param>
        /// <param name="p4">End point of line 2</param>
        /// <returns>Intersection point, or null if no intersection</returns>
        private static Vector2? GetLineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            float x1 = p1.X, y1 = p1.Y;
            float x2 = p2.X, y2 = p2.Y;
            float x3 = p3.X, y3 = p3.Y;
            float x4 = p4.X, y4 = p4.Y;

            // Calculate the direction vectors
            float dx1 = x2 - x1;
            float dy1 = y2 - y1;
            float dx2 = x4 - x3;
            float dy2 = y4 - y3;

            float denominator = dx1 * dy2 - dy1 * dx2;

            // Lines are parallel
            if (Math.Abs(denominator) < 1e-10)
                return null;

            // Calculate parameters
            float t = ((x3 - x1) * dy2 - (y3 - y1) * dx2) / denominator;
            float u = ((x3 - x1) * dy1 - (y3 - y1) * dx1) / denominator;

            // Check if intersection is within both line segments
            if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
            {
                // Calculate intersection point
                float intersectionX = x1 + t * dx1;
                float intersectionY = y1 + t * dy1;
                return new Vector2(intersectionX, intersectionY);
            }

            return null;
        }
    }
}