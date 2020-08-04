using Procool.GamePlay.Controller;
using UnityEngine;

namespace Procool.GameSystems
{
    public class GameSystem : Singleton<GameSystem>
    {
        [SerializeField] private Player player;
        public static Player Player => Instance.player;

        public static Player SpawnPlayer()
        {
            Instance.player = Instantiate(PrefabManager.Instance.PlayerPrefab).GetComponent<Player>();
            return Player;
        }
    }
}