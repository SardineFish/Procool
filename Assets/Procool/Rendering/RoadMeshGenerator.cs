using System;
using System.Collections.Generic;
using Procool.Map;
using Procool.Map.SpacePartition;
using UnityEngine;
using UnityEngine.Rendering;

namespace Procool.Rendering
{
    public class RoadMeshGenerator : IDisposable
    {
        struct VertexData
        {
            public Vector3 pos;
            public Vector2 uv;
            public Vector2 data; // (length, type)
        }

        private static VertexAttributeDescriptor[] VertexDataLayout = new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord1, VertexAttributeFormat.Float32, 2),
        };

        public float StreetWidth = 1;
        public float ArterialRoadWidth = 1.5f;
        public float ExpressWayWidth = 2;
        public float HighwayWidth = 2;
        
        public Mesh Mesh { get; private set; }
        public City City { get; private set; }

        private readonly List<VertexData> verts = new List<VertexData>(4096);
        private readonly List<int> triangles = new List<int>(4096);
        private readonly List<Vector2> vertData = new List<Vector2>(4096);

        public RoadMeshGenerator(City city)
        {
            City = city;
            Mesh = new Mesh();
        }
        

        void AddRoad(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3, float length, EdgeType type)
        {
            var offset = verts.Count;
            verts.Add(new VertexData()
            {
                pos = v0,
                uv = new Vector2(0, 1),
                data = new Vector2(length, (int) type)
            });
            verts.Add(new VertexData()
            {
                pos = v1,
                uv = new Vector2(1, 1),
                data = new Vector2(length, (int) type)
            });
            verts.Add(new VertexData()
            {
                pos = v2,
                uv = new Vector2(1, 0),
                data = new Vector2(length, (int) type)
            });
            verts.Add(new VertexData()
            {
                pos = v3,
                uv = new Vector2(0, 0),
                data = new Vector2(length, (int) type)
            });
            triangles.Add(offset + 0);
            triangles.Add(offset + 1);
            triangles.Add(offset + 2);
            triangles.Add(offset + 0);
            triangles.Add(offset + 2);
            triangles.Add(offset + 3);
        }

        Mesh SetupMesh()
        {
            Mesh.SetVertexBufferParams(verts.Count, VertexDataLayout);
            Mesh.SetVertexBufferData(verts, 0, 0, verts.Count);
            Mesh.SetTriangles(triangles, 0);
            return Mesh;
        }

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
                    return HighwayWidth;
                case EdgeType.ExpressWay:
                    return ExpressWayWidth;
                case EdgeType.ArterialRoad:
                    return ArterialRoadWidth;
                case EdgeType.Street:
                    return StreetWidth;
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
        void GenerateMesh(Edge edge)
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

            AddRoad(v0, v1, v2, v3, edge.Length, edge.EdgeType);
        }

        public void GenerateMesh()
        {
            foreach (var edge in City.Edges)
            {
                if(edge.EdgeType < EdgeType.Street)
                    continue;
                
                GenerateMesh(edge);
            }

            SetupMesh();
        }

        public void Dispose()
        {
            Mesh.Clear();
        }
    }
}