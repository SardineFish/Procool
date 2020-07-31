using Procool.Utils;
using UnityEngine;

namespace Procool.GameSystems
{
    public class PrefabManager : Singleton<PrefabManager>
    {
        public GameObject DropedItemPrefab;
        public GameObject PlayerPrefab;
        public GameObject DamageEntityPrefab;
        public GameObject CityLoaderPrefab;

        protected override void Awake()
        {
            base.Awake();
            GameObjectPool.PreAlloc(DropedItemPrefab, 16);
            GameObjectPool.PreAlloc(DamageEntityPrefab, 16);
        }
    }
}