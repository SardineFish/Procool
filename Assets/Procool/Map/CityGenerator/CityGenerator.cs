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
    public partial class CityGenerator : IBlockContentGenerator
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

        public CityParameters CityParams = new CityParameters()
        {
            size = 0.6f,
            blocksCount = 30,
            boundaryEdges = 12,
            boundaryExtend = .6f
        };


        public bool[] RoadConnection = new bool[6];

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
        
        public CityGenerator(Block block)
        {
            this.Block = block;
            prng = GameRNG.GetPRNG(Block.Position);
        }

        public BlockContent GetContent()
        {
            return City;
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
            var points = new List<Vector2>(CityParams.blocksCount);
            var size = Block.Size * CityParams.size;
            for (var i = 0; i < CityParams.blocksCount; i++)
            {
                points.Add(prng.GetVec2InsideUnitCircle() * size);
            }

            voronoiGenerator = new VoronoiGenerator(points);
            voronoiGenerator.BoundaryEdges = CityParams.boundaryEdges;
            voronoiGenerator.BoundaryExtend = size * CityParams.boundaryExtend;
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