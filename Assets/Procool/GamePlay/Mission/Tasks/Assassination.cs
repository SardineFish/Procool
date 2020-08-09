using System;
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
        private readonly StreetFight combat;

        private string description = "Assassinate the target.";
        
        public Assassination(City city, Vector2 location, PRNG prng) : base(city, location, prng)
        {
            combat = CombatSystem.Instance.GenerateStreetFight(city, location, prng);
        }

        public Assassination(StreetFight streetFight, PRNG prng) : base(streetFight.City, streetFight.Location, prng)
        {
            combat = streetFight;
        }

        EnemyController SpawnTarget()
        {
            var block = combat.InvolvedBlocks.RandomTake(prng.GetScalar());
            var enemy = CombatSystem.Instance.SpawnEnemy(City, block, prng);

            description =
                $"Assassinate the target at {TextGenerator.GenerateAddress(City, block, enemy.transform.position)}";

            return enemy;
        }

        public override async System.Threading.Tasks.Task<MissionState> Start(Player player)
        {
            TaskState = MissionState.Active;

            combat.StartCombat();

            target = combat.Instance.Enemies.RandomTake(prng.GetScalar());
            description =
                $"Assassinate the target at {TextGenerator.GenerateAddress(City, target.transform.position)}";

            while (!target.Player.Dead)
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1));

            TaskState = MissionState.Completed;
            
            return MissionState.Completed;
        }

        private Func<Assassination, PRNG, Task>[] Generators = new Func<Assassination, PRNG, Task>[]
        {
            (ass, prng) => new Assassination(ass.City, CombatSystem.Instance.RandomLocation(ass.City, prng), prng),
            (ass, prng) => new SuppressEnemies(ass.combat, prng),
            (ass, prng) => new Escape(ass.combat, prng),
            (assassin, prng) => null,
        };

        public override Task GenerateNextTask(PRNG prng)
        {
            return Generators.RandomTake(prng.GetScalar()).Invoke(this, prng);
        }

        public override string ToString()
        {
            return description;
        }
    }
}