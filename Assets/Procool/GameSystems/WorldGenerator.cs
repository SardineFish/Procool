using Procool.Map;
using Procool.Random;
using Procool.Utils;
using UnityEngine;

namespace Procool.GameSystems
{
    public class WorldGenerator : Singleton<WorldGenerator>
    {
        public int seed = -543004387;
        public int cityGenerationLevel = 4;
        private CityGenerator cityGenerator;

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

        City GenerateCity(Block block)
        {
            if(cityGenerator != null)
                cityGenerator.Dispose();
            
            cityGenerator = new CityGenerator(block);
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
            
            CoroutineRunner.Run(cityGenerator.RunProgressive());

            return cityGenerator.City;
        }

        public WorldBlock GenerateBlock(Vector2Int blockPos)
        {
            var block = new Block(blockPos, cityGenerationLevel);
            
            var worldBlock = new WorldBlock(block);
            worldBlock.AddContent(GenerateCity(block));

            return worldBlock;
        }
    }
}