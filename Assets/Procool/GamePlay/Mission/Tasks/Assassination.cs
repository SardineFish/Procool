using System.Collections;
using Procool.GamePlay.Combat;
using Procool.GamePlay.Controller;
using Procool.GameSystems;
using Procool.Map;
using Procool.Random;
using UnityEngine;
using TextGenerator = Procool.GameSystems.TextGenerator;

namespace Procool.GamePlay.Mission
{
    public class Assassination : Task
    {
        private EnemyController target;
        private StreetFight combat;

        private string description = "Assassinate the target.";
        
        public Assassination(City city, Vector2 location, PRNG prng) : base(city, location, prng)
        {
        }

        EnemyController SpawnTarget()
        {
            var block = combat.InvolvedBlocks.RandomTake(prng.GetScalar());
            var enemy = CombatSystem.Instance.SpawnEnemy(City, block, prng);

            description =
                $"Assassinate the target at {TextGenerator.GenerateAddress(City, block, enemy.transform.position)}";

            return enemy;
        }

        public override IEnumerator Start()
        {
            TaskState = MissionState.Active;
            
            if (!combat)
                combat = CombatSystem.Instance.GenerateStreetFight(City, Location, prng);

            target = SpawnTarget();

            while (!target.Player.Dead)
                yield return null;

            TaskState = MissionState.Completed;
        }

        public override string ToString()
        {
            return description;
        }
    }
}