using System;
using Procool.GamePlay.Controller;
using Procool.GamePlay.Weapon;
using Procool.Map;
using Procool.Random;
using Procool.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Procool.GameSystems
{
    public class GameStartup : Singleton<GameStartup>
    {
        public Transform PlayerSpawnPosition;
        private void Start()
        {
            // var worldBlock = WorldGenerator.Instance.GenerateBlock(new Vector2Int(0, 0));
            // WorldRenderer.Instance.LoadBlock(worldBlock);
            var player = GameSystem.SpawnPlayer();
            var weapon = WeaponSystem.Instance.GenerateWeapon(GameRNG.GetPRNG(new Vector2(0, 0)));
            player.transform.position = PlayerSpawnPosition.position;
            
            player.Inventory.Add(weapon);
            
            CameraManager.Instance.Follow(player.transform);
        }
    }
}