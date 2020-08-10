using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Procool.GamePlay.Controller;
using Procool.GameSystems;
using Procool.Map;
using Procool.Map.Loader;
using Procool.Random;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Event
{
    [RequireComponent(typeof(CityLoader))]
    public class CityVehicleSpawner : LazyLoadComponent
    {
        public float spawnGap = 5;
        public readonly HashSet<Vehicle> Vehicles = new HashSet<Vehicle>();
        private CityLoader _cityLoader;
        private PRNG prng;

        private void Awake()
        {
            _cityLoader = GetComponent<CityLoader>();
        }

        public override void Load()
        {
            prng = _cityLoader.prng.GetPRNG();
            StartCoroutine(SpawnVehicles());
        }

        void TrySpawnAt(Vector2 pos, Lane lane)
        {
            if (Vehicles.All(vehicle => Vector2.Distance(pos, vehicle.transform.position) > spawnGap))
            {
                var prefab = PrefabManager.Instance.VehiclePrefabs.RandomTake(prng.GetScalar());
                var vehicle = GameObjectPool.Get<Vehicle>(prefab);
                Vehicles.Add(vehicle);
                vehicle.GetComponent<AIDriver>().SpawnAt(ref lane, pos);
                vehicle.GetComponent<AIDriver>().StartDrive();
            }
        }

        IEnumerator SpawnVehicles()
        {
            while (true)
            {
                if (_cityLoader.LoadedBlocksCount > 0)
                {
                    var block = _cityLoader.LoadedBlocks.RandomTake(prng.GetScalar());
                    foreach (var road in block.Region.Edges.Select(edge => edge.GetData<Road>()))
                    {
                        foreach (var lane in road.Lanes)
                        {
                            TrySpawnAt(lane.WorldEntry, lane);

                        }

                        // yield return new WaitForSeconds(0.2f);
                    }
                }
                yield return new WaitForSeconds(1f);
            }
        }
    }
}