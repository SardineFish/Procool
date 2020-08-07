using System.Collections;
using Procool.GamePlay.Combat;
using Procool.GameSystems;
using Procool.Map;
using Procool.Random;
using UnityEngine;

namespace Procool.GamePlay.Mission
{
    public class SuppressEnemies : Task
    {
        public StreetFight combat;
        public override IEnumerator Start()
        {
            TaskState = MissionState.Active;
            
            if (!combat)
                combat = CombatSystem.Instance.GenerateStreetFight(City, Location, prng);
            combat.StartCombat();
            while (!combat.Cleared)
                yield return null;

            TaskState = MissionState.Completed;
        }

        public SuppressEnemies(City city, Vector2 location, PRNG prng) : base(city, location, prng)
        {
        }

        public SuppressEnemies(StreetFight streetFight, PRNG prng) : base(streetFight.City, streetFight.Location, prng)
        {
            combat = streetFight;
        }

        public override string ToString()
        {
            return "Suppress the enemies.";
        }
    }
}