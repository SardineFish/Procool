using System.Collections.Generic;
using Procool.Map.SpacePartition;
using Procool.Utils;
using UnityEngine;

namespace Procool.Map
{
    public class Road : ObjectWithPool<Road>
    {
        public Edge Edge { get; private set; }
        public float RoadWidth = 0;
        public (float, float) StopLine;
        public (Vector2, Vector2, Vector2, Vector2) IntersectPoints;

        public EdgeType Type => Edge.EdgeType;

        private Vector2 tangent;
        private Vector2 normal;

        public (CrossRoad, CrossRoad) CrossRoads => (Edge.Points.Item1.GetData<CrossRoad>(),
            Edge.Points.Item2.GetData<CrossRoad>());
        
        public static Road Get(Edge edge)
        {
            var data = GetInternal();
            data.Edge = edge;
            data.RoadWidth = 0;
            data.StopLine = (0, 0);
            data.IntersectPoints = (Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
            var (a, b) = edge.Points;
            GetTangentNormal(a, b, out data.tangent, out data.normal);
            return data;
        }

        static void GetTangentNormal(Vertex start, Vertex end, out Vector2 tangent, out Vector2 normal)
        {
            tangent = (end.Pos - start.Pos).normalized;
            normal = Vector3.Cross(Vector3.forward, tangent).ToVector2();
        }

        public static void Release(Road data)
            => ReleaseInternal(data);

        public Vector2 RoadToWorld(Vector2 pos)
        {
            return Edge.Points.Item1.Pos + tangent * pos.x + normal * pos.y;
        }

        public Vector2 WorldToRoad(Vector2 pos)
        {
            var (a, b) = Edge.Points;
            return new Vector2(
                Vector2.Dot(pos - a.Pos, tangent),
                Vector2.Dot(pos - a.Pos, normal)
            );
        }
    }
}