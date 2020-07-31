using System;
using System.Collections.Generic;
using Procool.Map;
using Procool.Map.SpacePartition;
using UnityEngine;
using UnityEngine.Rendering;

namespace Procool.Rendering
{
    public partial class CityMeshGenerator
    {
        struct VertexData
        {
            public Vector3 pos;
            public Vector2 uv;
            public Vector4 data; // (width, length, type)
        }

        private static VertexAttributeDescriptor[] RoadVertexDataLayout = new[]
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

        private readonly List<VertexData> roadVertices = new List<VertexData>(4096);
        private readonly List<int> roadTriangles = new List<int>(4096);


        void AddRoad(Road road)
        {
            var width = road.RoadWidth;
            var type = road.Type;
            var (v0, v1, v2, v3) = road.IntersectPoints;
            var (stopLineA, stopLineB) = road.StopLine;
            var length = stopLineB - stopLineA;
            
            var offset = roadVertices.Count;
            roadVertices.Add(new VertexData()
            {
                pos = road.RoadToWorld(v0),
                uv = new Vector2((v0.x - stopLineA)/length, 0),
                data = new Vector4(width, length, (int) type)
            });
            roadVertices.Add(new VertexData()
            {
                pos = road.RoadToWorld(v1),
                uv = new Vector2((v1.x - stopLineA)/length, 1),
                data = new Vector4(width, length, (int) type)
            });
            roadVertices.Add(new VertexData()
            {
                pos = road.RoadToWorld(v2),
                uv = new Vector2((v2.x - stopLineA)/length, 1),
                data = new Vector4(width, length, (int) type)
            });
            roadVertices.Add(new VertexData()
            {
                pos = road.RoadToWorld(v3),
                uv = new Vector2((v3.x - stopLineA)/length, 0),
                data = new Vector4(width, length, (int) type)
            });
            roadTriangles.Add(offset + 0);
            roadTriangles.Add(offset + 1);
            roadTriangles.Add(offset + 2);
            roadTriangles.Add(offset + 0);
            roadTriangles.Add(offset + 2);
            roadTriangles.Add(offset + 3);
        }

        Mesh SetupRoadMesh()
        {
            RoadMesh.SetVertexBufferParams(roadVertices.Count, RoadVertexDataLayout);
            RoadMesh.SetVertexBufferData(roadVertices, 0, 0, roadVertices.Count);
            RoadMesh.SetTriangles(roadTriangles, 0);
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
            var offset = roadVertices.Count;
            
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

                roadVertices.Add(new VertexData()
                {
                    pos = v0,
                    data = Vector2.zero,
                    uv = Vector2.zero,
                });
            }

            for (var i = offset + 2; i < roadVertices.Count; i++)
            {
                roadTriangles.Add(offset + 0);
                roadTriangles.Add(i);
                roadTriangles.Add(i - 1);
            }
        }

        void ClearRoadMesh()
        {
            RoadMesh.Clear();
            roadVertices.Clear();
            roadTriangles.Clear();
        }
    }
}