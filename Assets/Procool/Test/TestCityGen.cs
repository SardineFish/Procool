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

            generator = new CityGenerator(new Block(new Vector2Int(0, 0), BlockLevel), Count);
            generator.MinRoadDistance = MinRoadDistance;
            generator.MaxRoadDistance = MaxRoadDistance;
            generator.RloadRandomOffsetRatio = RoadOffset;
            
            yield return generator.RunProgressive();

            while (true)
            {
                foreach (var edge in generator.Edges)
                {
                    var (a, b) = edge.Points;
                    Debug.DrawLine(a.Pos, b.Pos, Color.cyan);
                }

                yield return null;
            }
        }
    }
}