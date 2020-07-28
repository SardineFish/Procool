using Procool.Utils;
using UnityEngine;

namespace Procool.GameSystems
{
    public class PrefabManager : Singleton<PrefabManager>
    {
        public GameObject DropedItemPrefab;
        public GameObject PlayerPrefab;

        protected override void Awake()
        {
            base.Awake();
            GameObjectPool.PreAlloc(DropedItemPrefab, 16);
        }
    }
}