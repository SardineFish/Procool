using Procool.Random;
using Procool.Utils;

namespace Procool.GamePlay.Weapon
{
    public interface IWeaponBehaviour
    {
        void Start(DamageEntity entity, WeaponBehaviourData data, DamageStage stage);

        void Update(DamageEntity entity, WeaponBehaviourData data, DamageStage stage);

        void End(DamageEntity entity, WeaponBehaviourData data, DamageStage stage);

        WeaponBehaviourData GenerateBehaviourData(PRNG prng);

        float EvaluateAdditionalDamage(WeaponBehaviourData data);
        float EvaluateDamageMultiply(WeaponBehaviourData data);
    }
    public abstract class WeaponBehaviour<T> : IWeaponBehaviour where T : WeaponBehaviourData
    {
        protected virtual void Start(DamageEntity entity, T data, DamageStage stage)
        {
        }

        protected virtual void Update(DamageEntity entity, T data, DamageStage stage)
        {
        }

        protected virtual void End(DamageEntity entity, T data, DamageStage stage)
        {
        }

        protected virtual float EvaluateAdditionalDamage(T data)
        {
            if (data.NextStage)
            {
                var damage = 0f;
                foreach (var behaviourData in data.NextStage.Behaviours)
                {
                    damage += behaviourData.Behaviour.EvaluateAdditionalDamage(behaviourData);
                }

                return damage;
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


        public virtual WeaponBehaviourData GenerateBehaviourData(PRNG prng)
        {
            return new WeaponBehaviourData(this);
        }

        public void Start(DamageEntity entity, WeaponBehaviourData data, DamageStage stage)
        {
            Start(entity, data as T, stage);
        }

        public void Update(DamageEntity entity, WeaponBehaviourData data, DamageStage stage)
        {
            Update(entity, data as T, stage);
        }

        public void End(DamageEntity entity, WeaponBehaviourData data, DamageStage stage)
        {
            End(entity, data as T, stage);
        }
    }
}