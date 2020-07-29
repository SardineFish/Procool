using Procool.Map.SpacePartition;
using UnityEngine;

namespace Procool.Map
{
    public partial class CityGenerator
    {
        Edge FindSideMostEdge(Edge edge, Vertex vertex, int direction)
        {
            var baseVert = edge.GetAnother(vertex);
            var baseDir = vertex.Pos - baseVert.Pos;
            var baseAngle = Mathf.Atan2(baseDir.y, baseDir.x);

            var largestAngle = direction < 1 ? Mathf.PI * 2 : -Mathf.PI * 2;
            Edge largestAngleEdge = null;
            foreach (var neighborEdge in vertex.Edges)
            {
                if (neighborEdge == edge || neighborEdge.EdgeType < EdgeType.Street)
                    continue;
                var nextVert = neighborEdge.GetAnother(vertex);
                var dir = nextVert.Pos - vertex.Pos;
                var angle = Mathf.Atan2(dir.y, dir.x);
                var delta = baseAngle - angle;
                if (delta < -Mathf.PI)
                    delta += Mathf.PI * 2;
                else if (delta > Mathf.PI)
                    delta -= Mathf.PI * 2;
                if (delta.CompareTo(largestAngle) == direction)
                {
                    largestAngle = delta;
                    largestAngleEdge = neighborEdge;
                }
            }

            return largestAngleEdge;
        }

        float GetRoadWidth(EdgeType type)
        {
            switch (type)
            {
                case EdgeType.TrunkHighway:
                    return RoadParams.highwayWidth;
                case EdgeType.ExpressWay:
                    return RoadParams.expressWayWidth;
                case EdgeType.ArterialRoad:
                    return RoadParams.arterialRoadWidth;
                case EdgeType.Street:
                    return RoadParams.streetWidth;
                default:
                    return 0;
            }
        }

        Edge FindLeftMostEdge(Edge edge, Vertex vertex)
        {
            return FindSideMostEdge(edge, vertex, -1);
        }

        Edge FindRightMostEdge(Edge edge, Vertex vertex)
            => FindSideMostEdge(edge, vertex, 1);

        void GetTangentNormal(Vertex start, Vertex end, out Vector2 tangent, out Vector2 normal)
        {
            tangent = (end.Pos - start.Pos).normalized;
            normal = Vector3.Cross(Vector3.forward, tangent).ToVector2();
        }
        
        // See: https://www.geogebra.org/geometry/hevc6mjk
        Vector2 RoadJoinPos(Edge edgeA, Edge edgeB, Vertex vertex)
        {
            var vertA = edgeA.GetAnother(vertex);
            var vertB = edgeB.GetAnother(vertex);
            GetTangentNormal(vertA, vertex, out var tangentA, out var normalA);
            GetTangentNormal(vertex, vertB, out var tangentB, out var normalB);
            var o1 = vertA.Pos + normalA * GetRoadWidth(edgeA.EdgeType);
            var o2 = vertex.Pos + normalB * GetRoadWidth(edgeB.EdgeType);
            if (MathUtility.LineIntersect(o1, tangentA, o2, tangentB, out var point))
            {
                return point;
            }
            else
            {
                return vertex.Pos + normalA * Mathf.Max(GetRoadWidth(edgeA.EdgeType), GetRoadWidth(edgeB.EdgeType));
            }
        }

        // See: https://www.geogebra.org/geometry/hevc6mjk
        void GenCrossPosition(Edge edge)
        {
            var (a, b) = edge.Points;
            var tangent = (b.Pos - a.Pos).normalized;
            var normal = Vector3.Cross(Vector3.forward, tangent).ToVector2();

            var edge0 = FindRightMostEdge(edge, a);
            var edge1 = FindLeftMostEdge(edge, b);
            var edge2 = FindRightMostEdge(edge, b);
            var edge3 = FindLeftMostEdge(edge, a);
            Vector2 v0, v1, v2, v3;

            if (!edge0)
                v0 = a.Pos + normal * GetRoadWidth(edge.EdgeType);
            else
                v0 = RoadJoinPos(edge0, edge, a);
            if (!edge3)
                v3 = a.Pos - normal * GetRoadWidth(edge.EdgeType);
            else
                v3 = RoadJoinPos(edge, edge3, a);
            if (!edge1)
                v1 = b.Pos + normal * GetRoadWidth(edge.EdgeType);
            else
                v1 = RoadJoinPos(edge, edge1, b);
            if (!edge2)
                v2 = b.Pos - normal * GetRoadWidth(edge.EdgeType);
            else
                v2 = RoadJoinPos(edge2, edge, b);

            var road = edge.GetData<Road>();
            v0 = road.WorldToRoad(v0);
            v1 = road.WorldToRoad(v1);
            v2 = road.WorldToRoad(v2);
            v3 = road.WorldToRoad(v3);
            road.IntersectPoints = (v3, v0, v1, v2);

            var l1 = Vector2.Dot((v1 - a.Pos), tangent);
            var l2 = Vector2.Dot((v2 - a.Pos), tangent);

            var l0 = Vector2.Dot((v0 - b.Pos), -tangent);
            var l3 = Vector2.Dot((v3 - b.Pos), -tangent);
            var l = Mathf.Min(l1, l2);

            road.StopLine = (Mathf.Max(v0.x, v3.x), Mathf.Min(v1.x, v2.x));

            // v1 = a.Pos + normal * GetRoadWidth(edge.EdgeType) + tangent * l;
            // v2 = a.Pos - normal * GetRoadWidth(edge.EdgeType) + tangent * l;

            // l = Mathf.Min(l0, l3);
            // v0 = b.Pos + normal * GetRoadWidth(edge.EdgeType) - tangent * l;
            // v3 = b.Pos - normal * GetRoadWidth(edge.EdgeType) - tangent * l;

            //AddRoad(v0, v1, v2, v3, edge.Length, edge.EdgeType);
        }
    }
}