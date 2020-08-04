using System;
using System.Collections;
using System.Collections.Generic;
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

        public void PreLoadCombat(City city, Vector2 location, float size)
        {
            InvolvedBlocks.Clear();
            InvolvedBlocks.AddRange(city.FindBlocksInDistance(location, size));
            City = city;
            Location = location;
            Size = size;
            prng = GameRNG.GetPRNG(location);
        }
        
        public override void StartCombat()
        {
            StartCoroutine(ProcessCombat());
        }

        public override void StopCombat()
        {
            throw new System.NotImplementedException();
        }

        EnemyController SpawnEnemy()
        {
            var block = InvolvedBlocks.RandomTake(prng.GetScalar());
            var edge = block.Region.Edges.RandomTake(prng.GetScalar());
            var t = prng.GetScalar();
            var (a, b) = edge.Points;
            var pos = Vector2.Lerp(a.Pos, b.Pos, t);
            var enemy = GameObjectPool.Get<EnemyController>(PrefabManager.Instance.EnemyPrefab);
            var weapon = WeaponSystem.Instance.GenerateWeapon(prng);
            enemy.Player.Inventory.Add(weapon);
            enemy.Weapon = weapon;
            enemy.Active(City, block, pos);
            return enemy;
        }

        IEnumerator ProcessCombat()
        {
            var player = GameSystems.GameSystem.Player;
            
            // Wait player approaching
            while (Vector2.Distance(player.BlockPosition.Position, Location) >= (Size + StartDistance))
                yield return null;
            
            // Spawn enemies
            var enemyCount = 1;// prng.GetInRange()
            for (var i = 0; i < enemyCount; i++)
            {    
                Enemies.Add(SpawnEnemy());
                Enemies.Add(SpawnEnemy());
                Enemies.Add(SpawnEnemy());
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