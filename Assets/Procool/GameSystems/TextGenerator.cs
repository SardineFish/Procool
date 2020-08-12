using Procool.Map;
using Procool.Random;
using UnityEngine;

namespace Procool.GameSystems
{
    public class TextGenerator : Singleton<TextGenerator>
    {

        private static string[] greeting = new[]
        {
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus vehicula nisl justo, sollicitudin auctor metus dapibus vitae. Quisque sodales maximus porta. Nullam laoreet, nunc a commodo sodales, dui nisi euismod.",
        };
        public static string GenerateGreeting(PRNG prng = null)
        {
            var p = prng is null ? GameRNG.GetScalar() : prng.GetScalar();
            return greeting.RandomTake(p);
        }

        public static string GenerateRoadName(City city, Road road)
        {
            var idx = city.Roads.IndexOf(road);
            return idx.ToOrdinal() + " Street";
        }

        public static string GenerateCityName(City city)
        {
            return "XXX City";
        }

        public static string GenerateAddress(City city, BuildingBlock buildingBlock, Vector2 position)
        {
            var edge = buildingBlock.Region.Edges
                .MinOf(e => MathUtility.DistanceToSegment(e.Points.Item1.Pos, e.Points.Item2.Pos, position));

            return $"{GenerateRoadName(city, edge.GetData<Road>())}, {GenerateCityName(city)}";
        }

        public static string GenerateAddress(City city, Vector2 position)
        {
            var block = city.FindBlockAt(position);
            return GenerateAddress(city, block, position);
        }
    }
}