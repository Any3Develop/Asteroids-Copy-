using System.Linq;
using UnityEngine;

namespace Services.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 Clamp(this Vector3 source, float min, float max)
        {
            source.x = Mathf.Clamp(source.x, min, max);
            source.y = Mathf.Clamp(source.y, min, max);
            source.z = Mathf.Clamp(source.z, min, max);
            return source;
        }
        
        public static Vector3 Max(this Vector3 source, float value)
        {
            source.x = Mathf.Max(source.x, value);
            source.y = Mathf.Max(source.y, value);
            source.z = Mathf.Max(source.z, value);
            return source;
        }
        
        public static Vector2 Min(this Vector2 source, float value)
        {
            source.x = Mathf.Min(source.x, value);
            source.y = Mathf.Min(source.y, value);
            return source;
        }
        
        public static Vector2 Max(this Vector2 source, float value)
        {
            source.x = Mathf.Max(source.x, value);
            source.y = Mathf.Max(source.y, value);
            return source;
        }
        
        public static float AverageXYZ(this Vector3 source)
        {
            return (source.x + source.y + source.z) / 3f;
        }
        
        public static float AverageXY(this Vector3 source)
        {
            return (source.x + source.y) / 2f;
        }

        public static float Max(this Vector3 source)
        {
            return Mathf.Max(source.x, source.y, source.z);
        }
        
        public static float Sum(this Vector3 source)
        {
            return source.x + source.y + source.z;
        }
        
        public static float Sum(this Vector2 source)
        {
            return source.x + source.y;
        }
        
        public static Vector3 Abs(this Vector3 source)
        {
            source.x = Mathf.Abs(source.x);
            source.y = Mathf.Abs(source.y);
            source.z = Mathf.Abs(source.z);
            return source;
        }
        
        public static Vector3 AbsXY(this Vector3 source)
        {
            source.x = Mathf.Abs(source.x);
            source.y = Mathf.Abs(source.y);
            return source;
        }
        
        public static Vector3 SetZ(this Vector3 source, float value)
        {
            source.z = value;
            return source;
        }
        
        public static Vector3 SetX(this Vector3 source, float value)
        {
            source.x = value;
            return source;
        }
        
        public static Vector3 SetY(this Vector3 source, float value)
        {
            source.y = value;
            return source;
        }
        
        public static Vector2 SetX(this Vector2 source, float value)
        {
            source.x = value;
            return source;
        }
        
        public static Vector2 SetY(this Vector2 source, float value)
        {
            source.y = value;
            return source;
        }
        
        /// <summary>
        /// Calculate point on 2D bounding box
        /// </summary>
        /// <param name="t">Value 0-1 space, progress or line position</param>
        /// <param name="bounds">Target bounds to unwrapping and calculate points</param>
        /// <returns></returns>
        public static Vector3 CalculatePointOnBoundingEdges(float t, Bounds bounds)
        {
            var heightPos = bounds.max.y;
            var widthPos = bounds.max.x;
            var sides = new (Vector3 A, Vector3 B)[]
            {
                (bounds.min, bounds.min.SetY(heightPos)), // left
                (bounds.min.SetY(heightPos), bounds.max), // top
                (bounds.max, bounds.min.SetX(widthPos)),  // right
                (bounds.min.SetX(widthPos), bounds.min),  // bottom
            };

            var baseLineLength = bounds.size.Sum() * 2f;
            var wantedPosition = baseLineLength * t;
            var currentPosition = 0f;

            foreach (var (a, b) in sides)
            {
                var segmentLength = Vector3.Distance(a, b);
                currentPosition += segmentLength;
                if (wantedPosition > currentPosition)
                    continue;
                var offset = currentPosition - segmentLength;
                currentPosition = wantedPosition - offset;
                return Vector3.Lerp(a, b, currentPosition / segmentLength);
            }

            return sides.LastOrDefault().B;
        }
    }
}