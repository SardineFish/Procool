using System;
using System.Collections;
using Procool.GamePlay.Combat;
using Procool.GamePlay.Controller;
using Procool.GameSystems;
using Procool.Map;
using Procool.Random;
using UnityEngine;

namespace Procool.GamePlay.Mission
{
    public class SuppressEnemies : Task
    {
        public readonly StreetFight combat;
        public override async System.Threading.Tasks.Task<MissionState> Start(Player player)
        {
            TaskState = MissionState.Active;

            combat.StartCombat();
            while (!combat.Cleared)
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1));

            TaskState = MissionState.Completed;
            return MissionState.Completed;
        }

        public SuppressEnemies(City city, Vector2 location, PRNG prng) : base(city, location, prng)
        {
            combat = CombatSystem.Instance.GenerateStreetFight(city, location, prng);
        }

        public SuppressEnemies(StreetFight streetFight, PRNG prng) : base(streetFight.City, streetFight.Location, prng)
        {
            combat = streetFight;
        }
        
        private Func<SuppressEnemies, PRNG, Task>[] Generators = new Func<SuppressEnemies, PRNG, Task>[]
        {
            (current, prng) =>
                new Assassination(current.City, CombatSystem.Instance.RandomLocation(current.City, prng), prng),
            (current, prng) =>
                new SuppressEnemies(current.City, CombatSystem.Instance.RandomLocation(current.City, prng), prng),
            (current, prng) => null,
        }; 

        public override Task GenerateNextTask(PRNG prng)
        {
            return Generators.RandomTake(prng.GetScalar()).Invoke(this, prng);
        }

        public override string ToString()
        {
            return "Suppress the enemies.";
        }
    }
}