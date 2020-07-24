using System;
using System.Collections;
using Procool.Map;
using Procool.Map.SpacePartition;
using Procool.Random;
using Procool.Rendering;
using Procool.Utils;
using UnityEngine;

namespace Procool.Test
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RoadRenderer))]
    public class TestCityGen : MonoBehaviour, ICustomEditorEX
    {
        public int Seed = 0;
        public int Count = 30;
        public int BlockLevel = 3;

        public float MinRoadDistance = 1;
        public float MaxRoadDistance = 3;
        [Range(0, 1)]
        public float RoadOffset = 0.3f;

        public int BoundEdges = 6;
        public float BoundExtendRatio = 0.3f;

        public CityGenerator.ExpressWayParameters ExpressWayParameters = new CityGenerator.ExpressWayParameters()
        {
            mergeWeight = .2f,
            acceptableBendAngle = 60,
            straightRoadWeight = .8f,
        };

        public CityGenerator.RoadParameters RoadParameters = new CityGenerator.RoadParameters()
        {
            streetDistanceRange = new Vector2(10, 15),
            alleyDistanceRange = new Vector2(2, 3),
            randomOffsetRatio = .3f,
            streetCrossMergePass = 2,
            streetCrossMergeThreshold = 1,
            alleyCrossMergeThreshold = 1,
            alleyCrossMergePass = 1,
        };

        private CityGenerator generator;

        private void Awake()
        {
            Vertex.PreAlloc(8192);
            Edge.PreAlloc(4096);
            Region.PreAlloc(4096);
        }

        [EditorButton]
        void RandomSeed()
        {
            Seed = (int) DateTime.Now.Ticks;
            GameRNG.SetSeed(Seed);
        }

        [EditorButton]
        void Generate()
        {
            GameRNG.SetSeed(Seed);
            StopAllCoroutines();
            StartCoroutine(RunGenerator());
        }

        IEnumerator RunGenerator()
        {
            if (generator != null)
            {
                generator.City.Dispose();
                generator.Dispose();
            }

            var prng = GameRNG.GetPRNG(new Vector2(3, 7));
            generator = new CityGenerator(new Block(new Vector2Int(0, 0), BlockLevel), Count);
            generator.RoadParams = RoadParameters;
            generator.BoundaryEdges = BoundEdges;
            generator.BoundaryExtendRatio = BoundExtendRatio;
            generator.ExpressWayParams = ExpressWayParameters;
            for (var i = 0; i < 6; i++)
            {
                if (prng.GetScalar() < .5f)
                {
                    generator.RoadConnection[i] = true;
                }
            }
            
            CoroutineRunner.Run(generator.RunProgressive());
            // yield return generator.RunProgressive();
            
            GetComponent<RoadRenderer>().Render(generator.City);

            while (true)
            {
                foreach (var edge in generator.City.Edges)
                {
                    var (a, b) = edge.Points;
                    Color color = Color.cyan;
                    color.a = ((int) edge.EdgeType + 1) / 6.0f;
                    color.a = Mathf.Pow(color.a, 2.5f);
                    Debug.DrawLine(a.Pos, b.Pos, color);
                }

                foreach (var region in generator.City.Regions)
                {
                    var buildingBlock = region.GetData<BuildingBlock>();
                    foreach (var subRegion in buildingBlock.SubSpace.Regions)
                    {
                        foreach (var edge in subRegion.Edges)
                        {
                            var (a, b) = edge.Points;
                            Color color = Color.cyan;
                            color.a = ((int) edge.EdgeType + 1) / 6.0f;
                            color.a = Mathf.Pow(color.a, 2.5f);
                            color.a += 0.1f;
                            Debug.DrawLine(a.Pos, b.Pos, color);
                        }
                    }
                }

                yield return null;
            }
        }
    }
}