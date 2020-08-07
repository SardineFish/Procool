using System;
using System.Collections;
using System.Collections.Generic;
using Procool.GamePlay.Combat;
using Procool.GamePlay.Mission;
using Procool.GameSystems;
using Procool.Map.Loader;
using Procool.Random;
using Procool.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Procool.GamePlay.Event
{
    [RequireComponent(typeof(CityLoader))]
    public class CityGamePlayGenerator : MonoBehaviour
    {
        [SerializeField] private Vector2 NPCCount = new Vector2(10, 20);
        private readonly List<CombatEvent> Events = new List<CombatEvent>();
        private CityLoader cityLoader;
        
        private readonly List<NPC> NPCs = new List<NPC>();
        private PRNG prng => cityLoader?.prng;

        private void Awake()
        {
            cityLoader = GetComponent<CityLoader>();
        }

        IEnumerator Test()
        {
            while (true)
            {
                if (Keyboard.current.f4Key.isPressed)
                {
                    GenerateMission();
                    while(Keyboard.current.f4Key.isPressed)
                        yield return null;
                }

                yield return null;
            }
        }

        private void OnEnable()
        {
            StartCoroutine(Test());
        }

        private void Update()
        {
        }

        // void GenerateEvent()
        // {
        //     var player = GameSystem.Player;
        //     var location = GameRNG.GetVec2ByVec2(Vector2.zero) * 500;
        //     var combat = CombatSystem.Instance.GenerateStreetFight(cityLoader.City, location);
        //     var combatEvent = new CombatEvent()
        //     {
        //         City = cityLoader.City,
        //         Combat = combat,
        //         Location = location
        //     };
        //     combat.StartCombat();
        //     Events.Add(combatEvent);
        // }

        NPC SpawnNPC(Vector2 position)
        {
            var npc = GameObjectPool.Get<NPC>(PrefabManager.Instance.NPCPrefab);
            npc.transform.position = position;
            return npc;
        }

        void GenerateMission()
        {
            var count = prng.GetInRange(NPCCount);
            for (var i = 0; i < count; i++)
            {
                var building = cityLoader.City.Buildings.RandomTake(prng.GetScalar());
                var edge = building.Region.Edges
                    .RandomTake(prng.GetScalar(), e => e.IsBoundary ? 2 : 1);
                var (a, b) = edge.Points;
                var pos = Vector2.Lerp(a.Pos, b.Pos, prng.GetScalar());
                var npc = SpawnNPC(pos);
                npc.Active(prng.GetPRNG());
            }
        }
    }
}