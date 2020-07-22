using System;
using System.Collections;
using Procool.Map;
using Procool.Random;
using Procool.Utils;
using UnityEngine;

namespace Procool.Test
{
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

        private CityGenerator generator;

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
            if(generator != null)
                generator.Dispose();

            var prng = GameRNG.GetPRNG(new Vector2(3, 7));
            generator = new CityGenerator(new Block(new Vector2Int(0, 0), BlockLevel), Count);
            generator.MinRoadDistance = MinRoadDistance;
            generator.MaxRoadDistance = MaxRoadDistance;
            generator.RloadRandomOffsetRatio = RoadOffset;
            generator.BoundaryEdges = BoundEdges;
            generator.BoundaryExtendRatio = BoundExtendRatio;
            for (var i = 0; i < 6; i++)
            {
                if (prng.GetScalar() < .5f)
                {
                    generator.RoadConnection[i] = true;
                }
            }
            
            yield return generator.RunProgressive();

            while (true)
            {
                foreach (var edge in generator.Edges)
                {
                    var (a, b) = edge.Points;
                    Color color = Color.cyan;
                    color.a = ((int) edge.EdgeType + 1) / 6.0f;  
                    Debug.DrawLine(a.Pos, b.Pos, color);
                }

                yield return null;
            }
        }
    }
}