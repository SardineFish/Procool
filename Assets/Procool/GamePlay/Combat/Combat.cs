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
        public abstract void StartCombat();

        public abstract void ClearCombat();

        public CombatInstance CreateInstance()
        {
            var instance = GameObjectPool.Get<CombatInstance>();
            instance.Init(this);
            return instance;
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