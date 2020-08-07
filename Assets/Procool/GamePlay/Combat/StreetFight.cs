﻿using System;
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
        public readonly List<EnemyController> Enemies = new List<EnemyController>();
        public Vector2 Location;
        public float Size;
        public float StartDistance = 10f;
        public City City { get; private set; }
        public PRNG prng;
        
        public bool Started { get; private set; }
        public bool Cleared { get; private set; }

        public void PreLoadCombat(City city, Vector2 location, float size, PRNG prng)
        {
            prng = prng is null ? GameRNG.GetPRNG(location) : prng;
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
            Cleared = false;
            Started = true;
            StartCoroutine(ProcessCombat());
        }

        public override void ClearCombat()
        {
            foreach (var enemy in Enemies)
            {
                GameObjectPool.Release(PrefabManager.Instance.EnemyPrefab, enemy);
            }

            Cleared = false;
            Started = false;
            InvolvedBlocks.Clear();
            Enemies.Clear();
            
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
            
            // Spawn enemies
            var enemyCount = prng.GetInRange(10, 30);
            for (var i = 0; i < enemyCount; i++)
            {    
                Enemies.Add(SpawnEnemy());
            }

            while (true)
            {
                if (Enemies.All(enemy=>enemy.Player.Dead))
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