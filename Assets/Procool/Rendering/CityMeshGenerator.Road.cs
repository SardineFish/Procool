using System;
using System.Collections.Generic;
using Procool.Map;
using Procool.Map.SpacePartition;
using UnityEngine;
using UnityEngine.Rendering;

namespace Procool.Rendering
{
    public partial class CityMeshGenerator : IDisposable
    {
        struct VertexData
        {
            public Vector3 pos;
            public Vector2 uv;
            public Vector4 data; // (width, length, type)
        }

        private static VertexAttributeDescriptor[] VertexDataLayout = new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord1, VertexAttributeFormat.Float32, 4),
        };

        public float StreetWidth = 1;
        public float ArterialRoadWidth = 1.5f;
        public float ExpressWayWidth = 2;
        public float HighwayWidth = 2;
        
        public Mesh RoadMesh { get; private set; } = new Mesh();

        private readonly List<VertexData> verts = new List<VertexData>(4096);
        private readonly List<int> triangles = new List<int>(4096);
        private readonly List<Vector2> vertData = new List<Vector2>(4096);


        void AddRoad(Road road)
        {
            var width = road.RoadWidth;
            var type = road.Type;
            var (v0, v1, v2, v3) = road.IntersectPoints;
            var (stopLineA, stopLineB) = road.StopLine;
            var length = stopLineB - stopLineA;
            
            var offset = verts.Count;
            verts.Add(new VertexData()
            {
                pos = road.RoadToWorld(v0),
                uv = new Vector2((v0.x - stopLineA)/length, 0),
                data = new Vector4(width, length, (int) type)
            });
            verts.Add(new VertexData()
            {
                pos = road.RoadToWorld(v1),
                uv = new Vector2((v1.x - stopLineA)/length, 1),
                data = new Vector4(width, length, (int) type)
            });
            verts.Add(new VertexData()
            {
                pos = road.RoadToWorld(v2),
                uv = new Vector2((v2.x - stopLineA)/length, 1),
                data = new Vector4(width, length, (int) type)
            });
            verts.Add(new VertexData()
            {
                pos = road.RoadToWorld(v3),
                uv = new Vector2((v3.x - stopLineA)/length, 0),
                data = new Vector4(width, length, (int) type)
            });
            triangles.Add(offset + 0);
            triangles.Add(offset + 1);
            triangles.Add(offset + 2);
            triangles.Add(offset + 0);
            triangles.Add(offset + 2);
            triangles.Add(offset + 3);
        }

        Mesh SetupRoadMesh()
        {
            RoadMesh.SetVertexBufferParams(verts.Count, VertexDataLayout);
            RoadMesh.SetVertexBufferData(verts, 0, 0, verts.Count);
            RoadMesh.SetTriangles(triangles, 0);
            RoadMesh.RecalculateBounds();
            return RoadMesh;
        }

        void GenerateRoadMesh(Edge edge)
        {
            var road = edge.GetData<Road>();
            AddRoad(road);
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
                    v0 = road.RoadToWorld(road.IntersectPoints.Item1);
                }
                else
                {
                    v0 = road.RoadToWorld(road.IntersectPoints.Item3);
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
    }
}