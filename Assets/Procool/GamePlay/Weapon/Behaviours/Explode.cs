using System.Collections;
using Procool.Random;

namespace Procool.GamePlay.Weapon
{
    public class Explode : WeaponBehaviour<Explode.Data>
    {
        public class Data : WeaponBehaviourData
        {
            public float Radius;
            public Data(IWeaponBehaviour behaviour) : base(behaviour)
            {
            }
        }

        public override WeaponBehaviourData GenerateBehaviourData(PRNG prng)
        {
            return new Data(this)
            {
                Radius = prng.GetInRange(2, 5),
            };
        }

        protected override float EvaluateDamageMultiply(Data data)
        {
            return data.Radius * 2;
        }

        protected override IEnumerator Run(DamageEntity entity, Data data, DamageStage stage, Weapon weapon)
        {
            WeaponSystem.Instance.GenerateExplosion(entity.transform.position, data.Radius, weapon.Damage, entity.Owner);
            entity.Terminate();
            yield break;
        }
    }
}