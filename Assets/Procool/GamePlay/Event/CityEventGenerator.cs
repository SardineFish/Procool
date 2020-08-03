using System;
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

        private void Update()
        {
            if (Keyboard.current.f4Key.wasPressedThisFrame)
            {
                GenerateEvent();
            }
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