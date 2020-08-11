using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Procool.GamePlay.Controller;
using Procool.GamePlay.Weapon;
using Procool.GameSystems;
using Procool.Map;
using Procool.Map.SpacePartition;
using Procool.Random;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Combat
{
    public class StreetFight : Combat
    {
        public readonly List<BuildingBlock> InvolvedBlocks = new List<BuildingBlock>();
        public float Size;
        public float StartDistance = 10f;
        public City City { get; private set; }
        public PRNG prng;
        
        public bool Started { get; private set; }
        public bool Cleared { get; private set; }

        public StreetFight(City city, Vector2 location, float size, PRNG prng)
        {
            this.prng = prng is null ? GameRNG.GetPRNG(location) : prng.GetPRNG();
            InvolvedBlocks.Clear();
            InvolvedBlocks.AddRange(city.FindBlocksInDistance(location, size));
            City = city;
            Location = location;
            Size = size;
        }
        
        public override void StartCombat()
        {
            if(Started)
                return;

            var instance = CreateInstance();
            Cleared = false;
            instance.StartCoroutine(ProcessCombat());
        }

        public override void ClearCombat()
        {
            DestroyInstance();
        }

        EnemyController SpawnEnemy()
        {
            var block = InvolvedBlocks.RandomTake(prng.GetScalar());

            return CombatSystem.Instance.SpawnEnemy(City, block, prng);
        }

        IEnumerator ProcessCombat()
        {
            var player = GameSystems.GameSystem.Player;
            
            // Wait player approaching
            while (Vector2.Distance(player.BlockPosition.Position, Location) >= (Size + StartDistance))
                yield return null;

            CreateInstance();
            
            // Spawn enemies
            var enemyCount = prng.GetInRange(5, 10);
            for (var i = 0; i < enemyCount; i++)
            {    
                Instance.AddEnemy(SpawnEnemy());
            }

            Started = true;

            while (true)
            {
                if (Instance.Enemies.All(enemy=>enemy.Player.Dead))
                {
                    Cleared = true;
                    yield break;
                }
                yield return new WaitForSeconds(1);
            }
            
        }

        private void Update()
        {
            foreach (var block in InvolvedBlocks)
            {
                Region.Utils.DrawDebug(block.Region, Color.red);
            }
        }
    }
}