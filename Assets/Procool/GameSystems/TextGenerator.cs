using Procool.Random;

namespace Procool.GameSystems
{
    public class TextGenerator : Singleton<TextGenerator>
    {

        private static string[] greeting = new[]
        {
            "Hey~"
        };
        public static string GenerateGreeting(PRNG prng = null)
        {
            var p = prng is null ? GameRNG.GetScalar() : prng.GetScalar();
            return greeting.RandomTake(p);
        }
    }
}