using Procool.GamePlay.Combat;
using Procool.GamePlay.Controller;
using Procool.GamePlay.Weapon;
using Procool.Map;
using Procool.Random;
using Procool.Utils;
using UnityEngine;

namespace Procool.GameSystems
{
    public class CombatSystem : Singleton<CombatSystem>
    {
        public Vector2 StreetFightRange = new Vector2(80, 200);
        public float OpenSpaceSpawnRate = 0.7f; 
        
        public StreetFight GenerateStreetFight(City city, Vector2 location, PRNG prng)
        {
            var size = Mathf.Lerp(StreetFightRange.x, StreetFightRange.y, GameRNG.GetScalarByVec2(location));
            var streetFight = new StreetFight(city, location, size, prng);
            return streetFight;
        }

        public Vector2 RandomLocation(City city, PRNG prng = null)
        {
            prng = CreatePRNGIfNull(prng);

            var block = city.BuildingBlocks.RandomTake(prng.GetScalar());
            return RandomLocation(city, block, prng);
        }

        public Vector2 RandomLocation(City city, BuildingBlock block, PRNG prng = null)
        {
            prng = CreatePRNGIfNull(prng);

            if (!block.OpenSpaces.Empty() && prng.GetScalar() < OpenSpaceSpawnRate)
            {
                var region = block.OpenSpaces.RandomTake(prng.GetScalar());
                var obb = region.ComputeOMBB();
                var pos = prng.GetVec2InBox(obb.HalfSize);
                pos = obb.ObbToWorld(pos);
                return pos;
                return pos;
            }
            else
            {
                var region = block.SubSpace.Regions.RandomTake(prng.GetScalar());
                var edge = region.Edges.RandomTake(prng.GetScalar());
                var (a, b) = edge.Points;
                var pos = Vector2.Lerp(a.Pos, b.Pos, prng.GetScalar());
                return pos;
            }
        }

        public EnemyController SpawnEnemy(City city, BuildingBlock block, Vector2 location, PRNG prng = null)
        {
            prng = CreatePRNGIfNull(prng);
            
            var enemy = GameObjectPool.Get<EnemyController>(PrefabManager.Instance.EnemyPrefab);
            var weapon = WeaponSystem.Instance.GenerateWeapon(prng);
            enemy.Player.Inventory.Add(weapon);
            enemy.Weapon = weapon;
            enemy.Active(city, block, location);
            enemy.Player.HP = enemy.Player.MaxHP;
            enemy.transform.position = location;
            return enemy;
        }

        public EnemyController SpawnEnemy(City city, BuildingBlock block, PRNG prng = null)
        {
            prng = CreatePRNGIfNull(prng);

            var pos = RandomLocation(city, block, prng);
            
            return SpawnEnemy(city, block, pos, prng);
        }

        public EnemyController SpawnEnemy(City city, PRNG prng = null)
        {
            prng = CreatePRNGIfNull(prng);

            var block = city.BuildingBlocks.RandomTake(prng.GetScalar());
            return SpawnEnemy(city, block, prng);
        }

        PRNG CreatePRNGIfNull(PRNG prng)
            => prng ?? GameRNG.GetPRNG();
    }
}