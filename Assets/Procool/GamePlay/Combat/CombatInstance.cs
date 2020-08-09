using System.Collections.Generic;
using Procool.GamePlay.Controller;
using Procool.GameSystems;
using Procool.Random;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Combat
{
    public class CombatInstance : MonoBehaviour
    {
        public Combat Combat { get; private set; }
        public readonly List<EnemyController> Enemies = new List<EnemyController>();

        public void Init(Combat combat)
        {
            this.Combat = combat;
            this.Enemies.Clear();
        }

        public void Clear()
        {
            foreach (var enemy in Enemies)
            {
                enemy.Deactivate();
                GameObjectPool.Release(PrefabManager.Instance.EnemyPrefab, enemy);
            }
            Enemies.Clear();
            Combat = null;
        }

        public void AddEnemy(EnemyController enemy)
        {
            Enemies.Add(enemy);
        }
    }
}