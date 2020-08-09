using System.Collections.Generic;
using System.Threading.Tasks;
using Procool.GamePlay.Controller;
using Procool.Random;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Combat
{
    public abstract class Combat
    {
        public CombatInstance Instance { get; private set; }
        public Vector2 Location { get; protected set; }
        public abstract void StartCombat();

        public abstract void ClearCombat();

        public CombatInstance CreateInstance()
        {
            Instance = GameObjectPool.Get<CombatInstance>();
            Instance.Init(this);
            Instance.transform.position = Location;
            return Instance;
        }

        public void DestroyInstance()
        {
            if (!Instance)
                return;
            
            Instance.Clear();
            GameObjectPool.Release(Instance);
            Instance = null;
        }
    }
}