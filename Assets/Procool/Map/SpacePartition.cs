using System;
using System.Collections.Generic;
using System.Linq;
using Procool.Utils;
using UnityEngine;

namespace Procool.Map
{
    public class SpaceParticion
    {
        public class Vertex : ObjectWithPool<Vertex>
        {
            public UInt32 ID { get; private set; }
            public Vector2 Pos;
            public object Data;

            public static Vertex Get(Vector2 pos)
            {
                var vert = GetInternal();
                vert.Pos = pos;
                vert.ID = UniqueID32.Get();
                return vert;
            }

            public static void Release(Vertex vert) => ReleaseInternal(vert);
        }

        public class Edge : ObjectWithPool<Edge>
        {
            public UInt64 ID { get; private set; }
            public (Vertex, Vertex) Points = (null, null);
            public (Region, Region) Regions = (null, null);

            public void AddRegion(Region region)
            {
                if (Regions.Item1 is null)
                    Regions.Item1 = region;
                else if (Regions.Item2 is null)
                    Regions.Item2 = region;
                else
                    throw new Exception("Regions overflow on edge.");
            }

            public Region GetAnother(Region region)
            {
                if (Regions.Item1 == region)
                    return Regions.Item2;
                else if (Regions.Item2 == region)
                    return Regions.Item1;
                else
                    throw new Exception("Edge not belongs to region.");
            }

            public static implicit operator bool(Edge edge)
                => !(edge is null);

            public static Edge Get(Vertex a, Vertex b)
            {
                var edge = GetInternal();
                edge.Points = (a, b);
                edge.ID = IDFromVerts(a, b);
                edge.Regions = (null, null);
                return edge;
            }
            
            public static UInt64 IDFromVerts(Vertex a, Vertex b)
                => a.ID > b.ID
                    ? (((ulong) a.ID << 32) | b.ID)
                    : (((ulong) b.ID << 32) | a.ID);

            public static void Release(Edge edge)
            {
                ReleaseInternal(edge);
            }
        }

        public class Region : ObjectWithPool<Region>
        {
            public UInt32 ID { get; private set; }
            public SpaceParticion ParentSpace = null;
            public SpaceParticion SubSpace = null;
            public List<SpaceParticion.Edge> Edges = new List<SpaceParticion.Edge>();
            public List<SpaceParticion.Vertex> Vertices = new List<SpaceParticion.Vertex>();

            public static Region Get()
            {
                var region = GetInternal();
                region.ID = UniqueID32.Get();
                region.ParentSpace = null;
                region.SubSpace = null;
                region.Edges.Clear();
                region.Vertices.Clear();

                return region;
            }

            public static void Release(Region region) => ReleaseInternal(region);
        }

        public HashSet<Region> Regions;
        public Dictionary<UInt64, Edge> EdgesById;
        public Dictionary<Vector2, Vertex> VerticesByVector;

        public IEnumerable<Edge> Edges => EdgesById.Values;

        public IEnumerable<Vertex> Vertices => VerticesByVector.Values;

        public SpaceParticion(int count)
        {
            Regions = new HashSet<Region>();
            VerticesByVector = new Dictionary<Vector2, Vertex>();
            EdgesById = new Dictionary<ulong, Edge>();
        }

        public Vertex GetVertex(Vector2 pos)
        {
            if (VerticesByVector.ContainsKey(pos))
                return VerticesByVector[pos];
            var vert = Vertex.Get(pos);
            VerticesByVector[pos] = vert;
            return vert;
        }

        public Edge GetEdge(Vertex a, Vertex b)
        {
            var edgeID = Edge.IDFromVerts(a, b);
            Edge edge;
            if (EdgesById.ContainsKey(edgeID))
                edge = EdgesById[edgeID];
            else
            {
                edge = Edge.Get(a, b);
                EdgesById[edge.ID] = edge;
            }

            return edge;
        }

        public Region CreateRegion(List<Vertex> vertices)
        {
            var region = Region.Get();
            region.Vertices.AddRange(vertices);

            Regions.Add(region);

            for (var i = 0; i < vertices.Count; i++)
            {
                var a = vertices[i];
                var b = vertices[(i + 1) % vertices.Count];

                var edge = GetEdge(a, b);
                region.Edges.Add(edge);
            }

            return region;
        }
    }
}