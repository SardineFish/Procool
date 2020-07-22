using System;
using System.Collections;
using System.Collections.Generic;
using Procool.Map.SpacePartition;
using Procool.Random;
using Procool.Utils;
using UnityEngine;
using Space = Procool.Map.SpacePartition.Space;

namespace Procool.Map
{
    public class CityGenerator : IDisposable
    {
        public const int LargeCityBlocks = 30;
        public const int TinyCityBlocks = 10;

        public float MinRoadDistance = 10f;
        public float MaxRoadDistance = 20f;
        public float RloadRandomOffsetRatio = 0.3f;

        public float ActualSizeRatio = 0.6f;

        public int CityBlocks { get; private set; }
        public Block Block { get; private set; }
        private VoronoiGenerator voronoiGenerator;
        public Space Space => voronoiGenerator?.Space;
        
        public HashSet<Edge> Edges { get; private set; } = new HashSet<Edge>();
        public HashSet<Vertex> Vertices { get; private set; } = new HashSet<Vertex>();
        private bool dirty = false;

        private PRNG prng;
        
        public CityGenerator(Block block, int citiyBlocks)
        {
            CityBlocks = citiyBlocks;
            this.Block = block;
            prng = GameRNG.GetPRNG(Block.Position);
        }

        IEnumerator SplitRoads(EdgeType type)
        {
            var count = Space.Regions.Count;

            for (var regionIdx = 0; regionIdx < count; regionIdx++)
            {
                var region = Space.Regions[regionIdx];
                // if (region.IsBoundary)
                //     continue;
                var obb = region.ComputeOMBB();

                var roadCounts = new Vector2(
                    Mathf.Floor(obb.HalfSize.x * 2 / prng.GetInRange(MinRoadDistance, MaxRoadDistance)),
                    Mathf.Floor(obb.HalfSize.y * 2 / prng.GetInRange(MinRoadDistance, MaxRoadDistance)));

                var gap = obb.HalfSize * 2 / (roadCounts + Vector2.one);

                Region regionA = region;
                Region regionB = null;

                for (int i = 1; i <= roadCounts.x; i++)
                {
                    if (!regionA)
                        break;
                    var x = -obb.HalfSize.x + gap.x * i;
                    x += prng.GetInRange(-1, 1) * (gap.x / 2 * RloadRandomOffsetRatio);
                    var pos = obb.Center + obb.AxisX * x;

                    var (nextA, nextB, newEdge) = Space.SplitRegionByLine(regionA, pos, obb.AxisY);
                    if (!nextA && regionB)
                    {
                        (nextA, nextB, newEdge) = Space.SplitRegionByLine(regionB, pos, obb.AxisY);
                    }

                    if (newEdge)
                        newEdge.EdgeType = type;

                    regionA = nextA;
                    regionB = nextB;
                }

                DrawDebugRegions();
                yield return null;

                var _ = 0;
            }

            dirty = true;
        }

        #warning Debug Code
        void DrawDebugRegions()
        {
            foreach (var region in Space.Regions)
            {
                foreach (var edge in region.Edges)
                {
                    Debug.DrawLine(edge.Points.Item1.Pos, edge.Points.Item2.Pos);
                }
            }
        }

        void GenerateFramework()
        {
            var points = new List<Vector2>(CityBlocks);
            var size = Block.Size * ActualSizeRatio;
            for (var i = 0; i < CityBlocks; i++)
            {
                points.Add(prng.GetVec2InUnitCircle() * size);
            }

            voronoiGenerator = new VoronoiGenerator(points);
            voronoiGenerator.RunProgressive();
            foreach (var edge in voronoiGenerator.Edges)
            {
                edge.EdgeType = EdgeType.ArterialRoad;
            }
            dirty = true;
        }

        void GenerateEntrance()
        {
            
        }

        void GenerateExpressWay()
        {
            
        }

        void UpdateEdges()
        {
            if(!dirty)
               return;
            dirty = false;
            Edges.Clear();
            foreach (var region in Space.Regions)
            {
                foreach (var edge in region.Edges)
                    Edges.Add(edge);
            }
        }

        public IEnumerator RunProgressive()
        {
            GenerateFramework();

            yield return SplitRoads(EdgeType.Street);

            DrawDebugRegions();

            yield return null;
            
            yield return SplitRoads(EdgeType.Street);

            UpdateEdges();
        }

        public void Dispose()
        {
            voronoiGenerator?.Dispose();
        }
    }
}