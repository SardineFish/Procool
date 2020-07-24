using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Procool.Map.SpacePartition;
using Procool.Random;
using Procool.Utils;
using UnityEngine;
using Space = Procool.Map.SpacePartition.Space;

namespace Procool.Map
{
    public partial class CityGenerator : IDisposable
    {
        public const int LargeCityBlocks = 30;
        public const int TinyCityBlocks = 10;

        #region Options

        public RoadParameters RoadParams = new RoadParameters()
        {
            streetDistanceRange = new Vector2(10, 15),
            alleyDistanceRange = new Vector2(1, 3),
            randomOffsetRatio = .3f,
            streetCrossMergeThreshold = 1,
            streetCrossMergePass = 2,
            alleyCrossMergeThreshold = 1,
            alleyCrossMergePass = 1,
        };

        public ExpressWayParameters ExpressWayParams = new ExpressWayParameters()
        {
            acceptableBendAngle = 60,
            straightRoadWeight = .2f,
            mergeWeight = .2f,
            roadStraighten = true,
        };

        public float ActualSizeRatio = 0.6f;
        public float BoundaryExtendRatio = 0.3f;
        public int BoundaryEdges = 6;


        public bool[] RoadConnection = new bool[6];

        public int CityBlocks { get; private set; }
        public Block Block { get; private set; }
        
        #endregion
        
        public City City { get; private set; }
        
        private VoronoiGenerator voronoiGenerator;
        private Space Space => voronoiGenerator?.Space;
        private List<Edge> isolatedEdges = new List<Edge>();
        private List<Vertex> isolatedVertices = new List<Vertex>();
        private readonly HashSet<Edge> regionsEdges = new HashSet<Edge>();
        private readonly HashSet<Vertex> regionsVertices = new HashSet<Vertex>();
        private PathFinder pathFinder;
        

        private IEnumerable<Edge> Edges => regionsEdges.Concat(isolatedEdges);
        private IEnumerable<Vertex> Vertices => regionsVertices.Concat(isolatedVertices);
        
        private bool dirty = false;

        private PRNG prng;
        
        public CityGenerator(Block block, int citiyBlocks)
        {
            CityBlocks = citiyBlocks;
            this.Block = block;
            prng = GameRNG.GetPRNG(Block.Position);
        }

        IEnumerator SplitRoads(EdgeType type, Vector2 roadDistance, float randomOffset)
        {
            var count = Space.Regions.Count;

            for (var regionIdx = 0; regionIdx < count; regionIdx++)
            {
                var region = Space.Regions[regionIdx];
                var obb = region.ComputeOMBB();

                var roadCounts =
                    Mathf.Floor(obb.HalfSize.x * 2 / prng.GetInRange(roadDistance.x, roadDistance.y));
                var gap = obb.HalfSize.x * 2 / (roadCounts + 1);

                Region regionA = region;
                Region regionB = null;

                for (int i = 1; i <= roadCounts; i++)
                {
                    if (!regionA)
                        break;
                    var x = -obb.HalfSize.x + gap * i;
                    x += prng.GetInRange(-1, 1) * (gap / 2 * randomOffset);
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

                yield return null;
            }

            dirty = true;
        }

        #warning Debug Code
        void DrawDebugEdges()
        {
            foreach (var edge in Edges)
            {
                Color color = Color.cyan;
                color.a = ((int) edge.EdgeType + 1) / 6.0f;
                color.a = Mathf.Pow(color.a, 4);
                Debug.DrawLine(edge.Points.Item1.Pos, edge.Points.Item2.Pos, color);
            }
        }

        IEnumerator GenerateFramework()
        {
            var points = new List<Vector2>(CityBlocks);
            var size = Block.Size * ActualSizeRatio;
            for (var i = 0; i < CityBlocks; i++)
            {
                points.Add(prng.GetVec2InsideUnitCircle() * size);
            }

            voronoiGenerator = new VoronoiGenerator(points);
            voronoiGenerator.BoundaryEdges = BoundaryEdges;
            voronoiGenerator.BoundaryExtend = size * BoundaryExtendRatio;
            yield return voronoiGenerator.RunProgressive();
            
            foreach (var edge in voronoiGenerator.Edges)
            {
                edge.EdgeType = EdgeType.ArterialRoad;
            }
            dirty = true;
        }

        Vertex CreateIsolatedVertex(Vector2 pos)
        {
            var vert = Vertex.Get(pos);
            isolatedVertices.Add(vert);
            return vert;
        }
        
        Edge CreateIsolatedEdge(Vertex a, Vertex b)
        {
            var edge = Edge.Get(a, b);
            isolatedEdges.Add(edge);
            a.AddEdge(edge);
            b.AddEdge(edge);
            return edge;
        }

        void GenerateEntrance()
        {
            UpdateEdgesAndVerts();
            for (var i = 0; i < 6; i++)
            {
                if (!RoadConnection[i])
                    continue;

                var connectPoint = MapGenerator.ConnectRoad(Block,
                    Block + MathH.GetAxialDirection(i));

                Vertex nearstVert = regionsVertices
                    .Where(vert => vert.IsBoundary)
                    .MinOf(vert => (vert.Pos - connectPoint).magnitude);

                if (nearstVert)
                {
                    var vert = CreateIsolatedVertex(connectPoint);
                    vert.VertexType = VertexType.Entrance;
                    //nearstVert.VertexType = VertexType.Anchor;
                    var edge = CreateIsolatedEdge(vert, nearstVert);
                }
            }
        }

        void GenerateExpressWay()
        {
            UpdateEdgesAndVerts();
            pathFinder = new PathFinder(Vertices, Edges);


            float cosAcceptableAngle = Mathf.Cos(Mathf.Deg2Rad * ExpressWayParams.acceptableBendAngle);
            float maxTurnScale = 1 + (1 + cosAcceptableAngle) * ExpressWayParams.straightRoadWeight / (1 - cosAcceptableAngle);
            float minTurnScale = 1 - ExpressWayParams.straightRoadWeight;
            pathFinder.CostEvaluator = (vertex, nextEdge, prevEdge) =>
            {
                if (!prevEdge)
                    return nextEdge.Length;
                var cost = nextEdge.Length;
                var prevVert = prevEdge.GetAnother(vertex);
                var nextVert = nextEdge.GetAnother(vertex);
                var cosAngle = Vector2.Dot((vertex.Pos - prevVert.Pos).normalized, (nextVert.Pos - vertex.Pos).normalized);

                cost *= MathUtility.RangeMapClamped(-1, 1, maxTurnScale, minTurnScale, cosAngle);

                if (nextEdge.EdgeType > EdgeType.ArterialRoad)
                    cost *= (1 - ExpressWayParams.mergeWeight);

                return cost;
            };
            for (var i = 0; i < isolatedVertices.Count; i++)
            {
                var start = isolatedVertices[i];
                if(start.VertexType != VertexType.Entrance)
                    continue;
                for (var j = i + 1; j < isolatedVertices.Count; j++)
                {
                    var end = isolatedVertices[j];
                    if (end.VertexType != VertexType.Entrance)
                        continue;

                    var path = pathFinder.Find(start, end);
                    foreach (var edge in path)
                    {
                        edge.EdgeType = EdgeType.ExpressWay;
                    }

                    if (ExpressWayParams.roadStraighten)
                        StraightenRoads(start, path);
                }
            }

        }

        void StraightenRoads(Vertex startVert, IEnumerable<Edge> path)
        {
            var vertex = startVert;
            Edge prevEdge = null;
            var i = 0;
            foreach (var edge in path)
            {
                if (i == 0)
                {
                    prevEdge = edge;
                    vertex = edge.GetAnother(vertex);
                    i++;
                    continue;
                }

                var prevVert = prevEdge.GetAnother(vertex);
                var nextVert = edge.GetAnother(vertex);

                if (vertex.VertexType != VertexType.Anchor)
                {
                    var t = Vector2.Dot((vertex.Pos - prevVert.Pos), (nextVert.Pos - prevVert.Pos).normalized) /
                            (nextVert.Pos - prevVert.Pos).magnitude;
                    var p = MathUtility.QuadraticBezierCurve(prevVert.Pos, vertex.Pos, nextVert.Pos, t);
                    vertex.Pos = p;
                    vertex.VertexType = VertexType.Anchor;
                }

                prevEdge = edge;
                vertex = nextVert;
                
                DrawDebugEdges();
            }
        }

        
        void MergeCrossing(float threshold, float mergePass)
        {
            for (var pass = 0; pass < mergePass; pass++)
            {
                UpdateEdgesAndVerts();
                
                foreach (var edge in regionsEdges)
                {
                    if(!edge.Valid)
                        continue;
                    if(edge.Length > threshold)
                        continue;

                    var (a, b) = edge.Points;
                    Vector2 newPos = Vector2.zero;
                    var typeA = a.Edges.Max(e => e.EdgeType);
                    var typeB = b.Edges.Max(e => e.EdgeType);
                    if (typeA > typeB)
                        newPos = a.Pos;
                    else if (typeA < typeB)
                        newPos = b.Pos;
                    else
                        newPos = (a.Pos + b.Pos) / 2;
                    
                    edge.Collapse(newPos);
                    Edge.Release(edge);
                }

                dirty = true;
            }
        }

        void UpdateEdgesAndVerts()
        {
            if(!dirty)
               return;
            dirty = false;
            regionsEdges.Clear();
            regionsVertices.Clear();
            foreach (var region in Space.Regions)
            {
                foreach (var edge in region.Edges)
                {
                    regionsEdges.Add(edge);
                    var (a, b) = edge.Points;
                    regionsVertices.Add(a);
                    regionsVertices.Add(b);
                }
            }
        }

        void GenAlley(BuildingBlock buildingBlock)
        {
            var obb = buildingBlock.Region.ComputeOMBB();

            var roadCounts =
                new Vector2Int(
                    Mathf.FloorToInt(obb.HalfSize.x * 2 / prng.GetInRange(RoadParams.alleyDistanceRange.x,
                        RoadParams.alleyDistanceRange.y)),
                    Mathf.FloorToInt(obb.HalfSize.x * 2 / prng.GetInRange(RoadParams.alleyDistanceRange.x,
                        RoadParams.alleyDistanceRange.y)));
            
            var gap = obb.HalfSize * 2 / (roadCounts + Vector2.one);
            for (var i = 1; i < roadCounts.x; i++)
            {
                var x = -obb.HalfSize.x + gap.x * i;
                x += prng.GetInRange(-1, 1) * (gap.x / 2 * RoadParams.randomOffsetRatio);
                var pos = obb.Center + obb.AxisX * x;
                buildingBlock.SubSpace.SplitByLine(pos, obb.AxisY);
            }

            for (var i = 1; i < roadCounts.y; i++)
            {
                var y = -obb.HalfSize.y + gap.y * i;
                y += prng.GetInRange(-1, 1) * (gap.y / 2 * RoadParams.randomOffsetRatio);
                var pos = obb.Center + obb.AxisY * y;
                buildingBlock.SubSpace.SplitByLine(pos, obb.AxisX);
            }
        }

        IEnumerator GenerateCityData()
        {
            UpdateEdgesAndVerts();
            foreach (var edge in Edges)
            {
                var road = Road.Get(edge);
                edge.SetData(road);
                road.RoadWidth = GetRoadWidth(edge.EdgeType);
            }

            foreach (var vertex in Vertices)
            {
                var crossRoad = CrossRoad.Get(vertex);
                vertex.SetData(crossRoad);
                crossRoad.AddRoads(vertex.Edges.Select(edge => edge.GetData<Road>()));
            }
            foreach(var vertex in Vertices)
                vertex.GetData<CrossRoad>().LinkCrossRoads();
            foreach (var edge in Edges)
                GenCrossPosition(edge);
            foreach (var region in Space.Regions)
            {
                region.ReOrderVertices();
                
                var buildingBlock = BuildingBlock.Get(region);
                region.SetData(buildingBlock);
                buildingBlock.SetupSubspace();
                GenAlley(buildingBlock);

                foreach (var subRegion in buildingBlock.SubSpace.Regions)
                {
                    subRegion.ReOrderVertices();
                }
                yield return null;
            }
        }

        public IEnumerator RunProgressive()
        {
            yield return GenerateFramework();
            
            MergeCrossing(RoadParams.streetCrossMergeThreshold, RoadParams.streetCrossMergePass);

            GenerateEntrance();
            
            GenerateExpressWay();

            yield return SplitRoads(EdgeType.Street, RoadParams.streetDistanceRange, RoadParams.randomOffsetRatio);
            yield return SplitRoads(EdgeType.Street, RoadParams.streetDistanceRange, RoadParams.randomOffsetRatio);

            MergeCrossing(RoadParams.streetCrossMergeThreshold, RoadParams.streetCrossMergePass);
            
            yield return GenerateCityData();

            // yield return SplitRoads(EdgeType.Alley, RoadParams.alleyDistanceRange, RoadParams.randomOffsetRatio);
            // yield return SplitRoads(EdgeType.Alley, RoadParams.alleyDistanceRange, RoadParams.randomOffsetRatio);
            //
            // MergeCrossing(RoadParams.alleyCrossMergeThreshold, RoadParams.alleyCrossMergePass);

            UpdateEdgesAndVerts();
            
            City = new City(Space.Regions, Edges, Vertices);
        }

        public void Dispose()
        {
            voronoiGenerator?.Dispose();
            isolatedEdges.Clear();
            isolatedVertices.Clear();
            // foreach (var isolatedEdge in isolatedEdges)
            // {
            //     Edge.Release(isolatedEdge);
            // }
            // foreach (var isolatedVertex in isolatedVertices)
            // {
            //     Vertex.Release(isolatedVertex);
            // }
        }
    }
}