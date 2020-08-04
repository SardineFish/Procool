using System;
using System.Collections;
using System.Collections.Generic;
using Procool.GamePlay.Combat;
using Procool.GameSystems;
using Procool.Map.Loader;
using Procool.Random;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Procool.GamePlay.Event
{
    [RequireComponent(typeof(CityLoader))]
    public class CityEventGenerator : MonoBehaviour
    {
        private readonly List<CombatEvent> Events = new List<CombatEvent>();
        private CityLoader cityLoader;

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
                    GenerateEvent();
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

        void GenerateEvent()
        {
            var player = GameSystem.Player;
            var location = GameRNG.GetVec2ByVec2(Vector2.zero) * 500;
            var combat = CombatSystem.Instance.GenerateStreetFight(cityLoader.City, location);
            var combatEvent = new CombatEvent()
            {
                City = cityLoader.City,
                Combat = combat,
                Location = location
            };
            combat.StartCombat();
            Events.Add(combatEvent);
        }
    }
}