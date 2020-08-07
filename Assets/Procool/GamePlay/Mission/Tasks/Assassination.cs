using System.Collections;
using Procool.GamePlay.Combat;
using Procool.GamePlay.Controller;
using Procool.GameSystems;
using Procool.Map;
using Procool.Random;
using UnityEngine;

namespace Procool.GamePlay.Mission
{
    public class Assassination : Task
    {
        private EnemyController target;
        private StreetFight combat;
        
        public Assassination(City city, Vector2 location, PRNG prng) : base(city, location, prng)
        {
        }

        EnemyController SpawnTarget()
        {
            var block = combat.InvolvedBlocks.RandomTake(prng.GetScalar());
            return CombatSystem.Instance.SpawnEnemy(City, block, prng);
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
    }
}