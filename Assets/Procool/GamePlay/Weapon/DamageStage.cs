using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public BulletVFX BulletVFX;

        public IEnumerator Run(Weapon weapon, Transform attachTo = null)
        {
            return CreateEntity(weapon, attachTo).Run();
        }

        public DamageEntity CreateEntity(Weapon weapon, Transform attachTo = null)
        {
            var entity = GameObjectPool.Get<DamageEntity>();
            entity.Init(weapon.Owner, weapon, this);
            entity.transform.parent = attachTo;
            return entity;
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