﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Procool.Utils;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Procool.GamePlay.Weapon
{
    public enum ColliderType
    {
        None,
        Box,
        Circle,
    }
    public class DamageStage : ObjectWithPool<DamageStage>
    {
        public ColliderType ColliderType;
        public Sprite Sprite;
        public List<WeaponBehaviourData> Behaviours = new List<WeaponBehaviourData>();

        public IEnumerator Run()
        {
            var entity = GameObjectPool.Get<DamageEntity>();
            var runner = CoroutineRunner.All(Behaviours.Select(behaviour =>
                behaviour.Behaviour.Run(entity, behaviour, this)));
            var coroutine = entity.StartCoroutine(runner);
            yield return coroutine;
            
            GameObjectPool.Release(entity);
        }

        public void Terminate(DamageEntity entity)
        {
            entity.StopAllCoroutines();
        }

        public float EvaluateDamage()
        {
            var damage = 0f;
            var multiply = 1f;
            foreach (var behaviour in Behaviours)
            {
                damage += behaviour.Behaviour.EvaluateAdditionalDamage(behaviour);
                multiply *= behaviour.Behaviour.EvaluateDamageMultiply(behaviour);
            }

            return damage * multiply;
        }
    }
}