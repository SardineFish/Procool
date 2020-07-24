using System;
using System.Collections.Generic;
using Procool.Map.SpacePartition;

namespace Procool.Map
{
    public class City : BlockContent, IDisposable
    {
        private readonly List<Region> regions;
        private readonly List<Edge> edges;
        private readonly List<Vertex> vertices;

        public IReadOnlyList<Region> Regions => regions.AsReadOnly();
        public IReadOnlyList<Edge> Edges => edges.AsReadOnly();
        public IReadOnlyList<Vertex> Vertices => vertices.AsReadOnly();

        public City(IEnumerable<Region> regions, IEnumerable<Edge> edges, IEnumerable<Vertex> vertices)
        {
            this.regions = new List<Region>(regions);
            this.edges = new List<Edge>(edges);
            this.vertices = new List<Vertex>(vertices);
        }

        public void Dispose()
        {
            foreach (var region in regions)
                Region.Release(region, true);
            foreach(var edge in edges)
                Edge.Release(edge);
            foreach(var vertex in vertices)
                Vertex.Release(vertex);
        }
    }
}