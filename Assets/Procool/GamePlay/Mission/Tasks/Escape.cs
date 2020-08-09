using System;
using System.Collections;
using System.Linq;
using Procool.GamePlay.Combat;
using Procool.GamePlay.Controller;
using Procool.GameSystems;
using Procool.Map;
using Procool.Random;
using UnityEngine;

namespace Procool.GamePlay.Mission
{
    public class Escape : Task
    {
        private StreetFight _combat;
        public Escape(StreetFight combat, PRNG prng) : base(combat.City, combat.Location, prng)
        {
            this._combat = combat;
        }

        public override async System.Threading.Tasks.Task<MissionState> Start(Player player)
        {
            TaskState = MissionState.Active;
            
            while (true)
            {
                if (_combat.Instance.Enemies.All(enemy =>
                    Vector2.Distance(enemy.transform.position, player.transform.position) > 300))
                    break;

                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1));
            }

            TaskState = MissionState.Completed;

            return MissionState.Completed;
        }

        private Func<Escape, PRNG, Task>[] Genereators = new Func<Escape, PRNG, Task>[]
        {
            (current, prng) =>
                new Assassination(current.City, CombatSystem.Instance.RandomLocation(current.City, prng), prng),
            (current, prng) =>
                new SuppressEnemies(current.City, CombatSystem.Instance.RandomLocation(current.City, prng), prng),
            (current, prng) => null,
        };

        public override Task GenerateNextTask(PRNG prng)
        {
            return Genereators.RandomTake(prng.GetScalar()).Invoke(this, prng);
        }

        public override string ToString()
        {
            return "Escape this area.";
        }
    }
}