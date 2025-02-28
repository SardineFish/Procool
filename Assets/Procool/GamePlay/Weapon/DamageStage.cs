﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Procool.GameSystems;
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
    public class DamageStage
    {
        public ColliderType ColliderType;
        public List<WeaponBehaviourData> Behaviours = new List<WeaponBehaviourData>();
        // public BulletVFX BulletVFX;
        public bool Detach = false;
        public bool IsFirstStage = false;
        public float EmitRate { get; private set; } = 0;

        // public IEnumerator Run(Weapon weapon, Transform attachTo = null)
        // {
        //     return CreateEntity(weapon, attachTo).Run();
        // }
        //
        // public DamageEntity CreateEntity(Weapon weapon, Transform attachTo = null)
        // {
        //     var entity = GameObjectPool.Get<DamageEntity>(PrefabManager.Instance.DamageEntityPrefab);
        //     entity.Init(weapon.Owner, weapon, this);
        //     entity.transform.parent = attachTo;
        //     return entity;
        // }
        //

        public void Run(Weapon weapon, DamageEntity entity)
        {
            var runner =
                CoroutineRunner.All(Behaviours.Select(behaviour =>
                    behaviour.Behaviour.Run(entity, behaviour, this, weapon)));
            entity.AppendCoroutine(runner);
        }

        public bool RequestEmit(Weapon weapon, DamageEntity entity)
        {
            var interval = EmitInterval(weapon);
            if (Time.time - entity.PreviousEmitTime >= interval)
            {
                entity.PreviousEmitTime = Time.time;
                return true;
            }

            return false;
        }

        public DamageEntity CreateDetached(Weapon weapon, Transform inheritTransform)
            => CreateDetached(weapon, inheritTransform.position, inheritTransform.rotation);

        public DamageEntity CreateDetached(Weapon weapon, Vector2 position, Quaternion rotation)
        {
            var entity = GameObjectPool.Get<DamageEntity>(PrefabManager.Instance.DamageEntityPrefab);
            entity.Init(weapon.Owner, weapon, position, rotation);
            entity.StageInfo = this.ToString();
            Run(weapon, entity);
            return entity;
        }

        public float EmitInterval(Weapon weapon)
        {
            return EmitRate / weapon.EmitRateLimit;
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

        public float EvaluateEmitRate()
        {
            float emitRate = 0;
            foreach (var behaviour in Behaviours)
            {
                emitRate += behaviour.Behaviour.EvaluateEmitRate(behaviour);
            }

            EmitRate = emitRate;

            return emitRate;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var behaviour in Behaviours)
                sb.AppendLine(behaviour.ToString());
            return sb.ToString();
        }

        public static implicit operator bool(DamageStage stage) => !(stage is null);
    }
}