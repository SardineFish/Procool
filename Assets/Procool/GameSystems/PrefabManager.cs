using System.Collections.Generic;
using Procool.GamePlay.Controller;
using Procool.Misc;
using Procool.Random;
using Procool.Utils;
using UnityEngine;

namespace Procool.GameSystems
{
    public class PrefabManager : Singleton<PrefabManager>
    {
        public GameObject DropedItemPrefab;
        public GameObject PlayerPrefab;
        public GameObject EnemyPrefab;
        public GameObject DamageEntityPrefab;
        public GameObject CityLoaderPrefab;
        public GameObject InputHintUIPrefab;
        public GameObject NPCPrefab;
        public List<GameObject> VehiclePrefabs = new List<GameObject>();
        public GameObject LargeExplosionPrefab;
        public GameObject SmallExplosionPrefab;
        public GameObject FireVFXPrefab;
        public GameObject MissionIndicator;
        public GameObject NPCIndicator;

        protected override void Awake()
        {
            base.Awake();
            GameObjectPool.PreAlloc(DropedItemPrefab, 16);
            GameObjectPool.PreAlloc(DamageEntityPrefab, 16);
            GameObjectPool.PreAlloc(InputHintUIPrefab, 4);
        }

        public Vehicle SpawnVehicle(PRNG prng = null)
        {
            prng = prng ?? GameRNG.GetPRNG();

            var prefab = VehiclePrefabs.RandomTake(prng.GetScalar());
            var vehicle = GameObjectPool.Get<Vehicle>(prefab);
            var instance = vehicle.GetComponent<InstanceGameObject>();
            if (!instance)
                instance = vehicle.gameObject.AddComponent<InstanceGameObject>();
            instance.Prefab = prefab;
            vehicle.Load();
            return vehicle;
        }

        public void ReleaseVehicle(Vehicle vehicle)
        {
            vehicle.Unload();
            var instance = vehicle.GetComponent<InstanceGameObject>();
            if (instance)
                GameObjectPool.Release(instance.Prefab, vehicle);
            else
                Destroy(vehicle.gameObject);
        }
    }
}