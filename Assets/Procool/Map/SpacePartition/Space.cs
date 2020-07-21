using System;
using System.Collections.Generic;
using Procool.Utils;
using UnityEditor.U2D.Animation;
using UnityEngine;

namespace Procool.Map.SpacePartition
{
    public partial class Space : ObjectWithPool<Space>
    {
        public HashSet<Region> Regions;
        public Dictionary<UInt64, Edge> EdgesById;
        public Dictionary<Vector2, Vertex> VerticesByVector;
        public List<Vertex> Boundary;

        public IEnumerable<Edge> Edges => EdgesById.Values;

        public IEnumerable<Vertex> Vertices => VerticesByVector.Values;

        public Space()
        {
            Regions = new HashSet<Region>();
            VerticesByVector = new Dictionary<Vector2, Vertex>();
            EdgesById = new Dictionary<ulong, Edge>();
            Boundary = new List<Vertex>();
        }

        public static Space Get()
        {
            var space = GetInternal();
            return space;
        }

        public static void Release(Space space)
        {
            foreach (var region in space.Regions)
            {
                Region.Release(region);
            }

            foreach (var vertex in space.VerticesByVector)
            {
                Vertex.Release(vertex.Value);
            }

            foreach (var edge in space.EdgesById)
            {
                Edge.Release(edge.Value);
            }
            
            space.Regions.Clear();
            space.VerticesByVector.Clear();
            space.EdgesById.Clear();
            space.Boundary.Clear();
            
            ReleaseInternal(space);
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