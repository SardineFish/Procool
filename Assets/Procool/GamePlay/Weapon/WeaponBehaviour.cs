using System.Collections;
using Procool.Random;
using Procool.Utils;

namespace Procool.GamePlay.Weapon
{
    public interface IWeaponBehaviour
    {
        IEnumerator Run(DamageEntity entity, WeaponBehaviourData data, DamageStage stage);

        WeaponBehaviourData GenerateBehaviourData(PRNG prng);

        float EvaluateAdditionalDamage(WeaponBehaviourData data);
        float EvaluateDamageMultiply(WeaponBehaviourData data);
    }
    public abstract class WeaponBehaviour<T> : IWeaponBehaviour where T : WeaponBehaviourData
    {
        protected virtual IEnumerator Run(DamageEntity entity, T data, DamageStage stage)
        {
            yield break;
        }

        protected virtual float EvaluateAdditionalDamage(T data)
        {
            if (data.NextStage)
            {
                var damage = 0f;
                return data.NextStage.EvaluateDamage();
            }

            return 0;
        }

        protected virtual float EvaluateDamageMultiply(T data)
        {
            return 1;
        }

        public float EvaluateAdditionalDamage(WeaponBehaviourData data)
        {
            return EvaluateAdditionalDamage(data as T);
        }

        public float EvaluateDamageMultiply(WeaponBehaviourData data)
        {
            return EvaluateDamageMultiply((data as T));
        }


        public IEnumerator Run(DamageEntity entity, WeaponBehaviourData data, DamageStage stage)
        {
            return Run(entity, data as T, stage);
        }

        public virtual WeaponBehaviourData GenerateBehaviourData(PRNG prng)
        {
            return new EmptyBehaviourData(this);
        }
        
    }

    public class WeaponBehaviour : WeaponBehaviour<EmptyBehaviourData>
    {
        
    }
}