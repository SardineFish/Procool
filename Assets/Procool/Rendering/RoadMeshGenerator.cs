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
            Mesh.RecalculateBounds();
            return Mesh;
        }

        void GenerateRoadMesh(Edge edge)
        {
            var road = edge.GetData<Road>();
            var (v0, v1, v2, v3) = road.IntersectPoints;
            AddRoad(v1, v2, v3, v0, edge.Length, edge.EdgeType);
        }

        void GenerateCrossRoadMesh(Vertex vertex)
        {
            var crossroad = vertex.GetData<CrossRoad>();
            var offset = verts.Count;
            
            foreach(var road in crossroad.Roads)
            {
                var (a, b) = road.CrossRoads;
                Vector2 v0;
                if (a.Vertex == vertex)
                {
                    v0 = road.IntersectPoints.Item1;
                }
                else
                {
                    v0 = road.IntersectPoints.Item3;
                }

                verts.Add(new VertexData()
                {
                    pos = v0,
                    data = Vector2.zero,
                    uv = Vector2.zero,
                });
            }

            for (var i = offset + 2; i < verts.Count; i++)
            {
                triangles.Add(offset + 0);
                triangles.Add(i);
                triangles.Add(i - 1);
            }
        }

        public void GenerateMesh()
        {
            foreach (var edge in City.Edges)
            {
                if(edge.EdgeType < EdgeType.Street)
                    continue;
                
                GenerateRoadMesh(edge);
            }

            foreach (var vert in City.Vertices)
            {
                if(vert.Edges.Count <3)
                    continue;
                GenerateCrossRoadMesh(vert);
            }

            SetupMesh();
        }

        public void Dispose()
        {
            Mesh.Clear();
        }
    }
}