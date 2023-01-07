using UnityEngine;

namespace Asterodis.Entities.Portals
{
    public readonly struct Edge
    {
        public readonly Vector3 PointA;
        public readonly Vector3 PointB;
        public readonly Vector3 Normal;
        public readonly float EdgeRadius;
        public readonly Vector3 Origin;
        public Edge(Vector3 pointA, Vector3 pointB, Vector3 normal, float edgeRadius)
        {
            PointA = pointA;
            PointB = pointB;
            Normal = normal;
            Origin = Vector3.Lerp(pointA, pointB, 0.5f);
            EdgeRadius = edgeRadius;
        }
    }
}