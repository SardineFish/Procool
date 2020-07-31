using System;
using Procool.Map;
using Procool.Random;
using Procool.Utils;
using UnityEngine;

namespace Procool.GameSystems
{
    public class WorldGenerator : Singleton<WorldGenerator>
    {
        public int seed = -543004387;
        public float CityBlockPropotion = 0.3f;

        public CityGenerator.ExpressWayParameters expressWayParameters = new CityGenerator.ExpressWayParameters()
        {
            mergeWeight = .2f,
            acceptableBendAngle = 60,
            straightRoadWeight = .8f,
        };

        public CityGenerator.RoadParameters roadParameters = new CityGenerator.RoadParameters()
        {
            streetDistanceRange = new Vector2(10, 15),
            alleyDistanceRange = new Vector2(2, 3),
            randomOffsetRatio = .3f,
            streetCrossMergePass = 2,
            streetCrossMergeThreshold = 1,
            alleyCrossMergeThreshold = 1,
            alleyCrossMergePass = 1,
        };

        public CityGenerator.CityParameters cityParameters = new CityGenerator.CityParameters()
        {
            size = .6f,
            blocksCount = 30,
            boundaryEdges = 12,
            boundaryExtend = .6f
        };

        protected override void Awake()
        {
            base.Awake();
            GameRNG.SetSeed(seed);
        }

        CityGenerator CreateCityGenerator(Block block)
        {   
            var cityGenerator = new CityGenerator(block);
            cityGenerator.CityParams = cityParameters;
            cityGenerator.RoadParams = roadParameters;
            cityGenerator.ExpressWayParams = expressWayParameters;
            
            var prng = GameRNG.GetPRNG(block.Position);
            for (var i = 0; i < 6; i++)
            {
                if (prng.GetScalar() < .5f)
                {
                    cityGenerator.RoadConnection[i] = true;
                }
            }
            
            // CoroutineRunner.Run(cityGenerator.RunProgressive());

            return cityGenerator;
        }

        public static BlockType GenerateBlockType(Block block)
        {
            if (block.Position == Vector2Int.zero)
                return BlockType.City;
            var p = GameRNG.GetScalarByInt3(new Vector3Int(block.Position.x, block.Position.y, block.Level));
            if (p < Instance.CityBlockPropotion)
                return BlockType.City;
            return BlockType.Wild;
        }

        public BlockGenerator GenerateBlock(Block block)
        {
            var blockType = GenerateBlockType(block);
            var generator = new BlockGenerator(block, blockType);
            
            switch (blockType)
            {
                case BlockType.City:
                    generator.AddContent(CreateCityGenerator(block));
                    break;
            }

            return generator;
        }
    }
}