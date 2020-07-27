using System.Collections.Generic;
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

        public void Start(DamageEntity entity)
        {
            
        }
        public void Update(DamageEntity entity)
        {
            
        }

        public void End(DamageEntity entity)
        {
            
        }

        public void Terminate()
        {
            
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