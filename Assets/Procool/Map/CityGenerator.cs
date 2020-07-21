using System.Collections;
using System.Collections.Generic;
using Procool.Random;
using Procool.Utils;
using UnityEngine;
using Space = Procool.Map.SpacePartition.Space;

namespace Procool.Map
{
    public class CityGenerator
    {
        public const int LargeCityBlocks = 30;
        public const int TinyCityBlocks = 10;

        private const float CityPointsRange = 0.6f;

        private Space space;

        public int CityBlocks { get; private set; }
        public Block Block { get; private set; }
        private VoronoiGenerator voronoiGenerator;
        
        public CityGenerator(Block block, int citiyBlocks)
        {
            CityBlocks = citiyBlocks;
            this.Block = block;
        }

        public IEnumerator RunProgressive()
        {
            var points = new List<Vector2>(CityBlocks);
            var prng = GameRNG.GetPRNG(Block.Position);
            var size = Block.Size * CityPointsRange;
            for (var i = 0; i < CityBlocks; i++)
            {
                points.Add(prng.GetVec2InUnitCircle() * size);
            }
            
            voronoiGenerator = new VoronoiGenerator(points);
            yield return voronoiGenerator.RunProgressive();
            
            
        }
        
    }
}