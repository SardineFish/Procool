using System;
using System.Collections.Generic;
using System.Linq;
using Procool.Map.SpacePartition;
using UnityEngine;

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
        
        public IReadOnlyList<BuildingBlock> BuildingBlocks { get; }
        public IReadOnlyList<Road> Roads { get; }
        public IReadOnlyList<CrossRoad> CrossRoads { get; }
        public IReadOnlyList<Building> Buildings { get; }

        public City(IEnumerable<Region> regions, IEnumerable<Edge> edges, IEnumerable<Vertex> vertices)
        {
            this.regions = new List<Region>(regions);
            this.edges = new List<Edge>(edges);
            this.vertices = new List<Vertex>(vertices);
            BuildingBlocks = regions
                .Select(region => region.GetData<BuildingBlock>())
                .ToList()
                .AsReadOnly();
            Roads = edges
                .Select(edge => edge.GetData<Road>())
                .ToList()
                .AsReadOnly();
            CrossRoads = vertices
                .Select(vert => vert.GetData<CrossRoad>())
                .ToList()
                .AsReadOnly();
            Buildings = regions
                .SelectMany(region =>
                    region.GetData<BuildingBlock>().SubSpace.Regions
                        .Select(r => r.GetData<Building>()))
                .ToList()
                .AsReadOnly();

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

        // TODO: Optimise with k-d tree
        public IEnumerable<BuildingBlock> FindBlocksInDistance(Vector2 center, float distance)
        {
            return BuildingBlocks.Where(block => Vector2.Distance(center, block.Center) < distance);
        }

        // TODO: Optimise with k-d tree
        public IEnumerable<Building> FindBuildingsInDistance(Vector2 center, float distance)
        {
            return Buildings.Where(building => Vector2.Distance(center, building.Center) < distance);
        }

        public BuildingBlock FindBlockAt(Vector2 pos)
        {
            return BuildingBlocks
                .Where(block => block.OBB.IsOverlap(pos))
                .FirstOrDefault(block => block.Region.OverlapPoint(pos));
        }
    }
}