using Procool.GamePlay.Controller;

namespace Procool.GameSystems
{
    public class GameSystem : Singleton<GameSystem>
    {
        public static Player Player { get; private set; }

        public static Player SpawnPlayer()
        {
            Player = Instantiate(PrefabManager.Instance.PlayerPrefab).GetComponent<Player>();
            return Player;
        }
    }
}