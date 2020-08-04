using System.Collections;
using Procool.Random;

namespace Procool.GamePlay.Weapon
{
    public class EmitTick : WeaponBehaviour<EmitTick.Data>
    {
        public class Data : EmitterBehaviourData
        {
            public float Interval = 1;
            public Data(IWeaponBehaviour behaviour) : base(behaviour)
            {
                
            }
        }

        protected override float EvaluateDamageMultiply(Data data) => 1 / data.Interval;

        public override WeaponBehaviourData GenerateBehaviourData(PRNG prng)
        {
            return new EmitTick.Data(this)
            {
                Interval = prng.GetInRange(.1f, .2f),
            };
        }

        protected override IEnumerator Run(DamageEntity entity, Data data, DamageStage stage, Weapon weapon)
        {
            while (true)
            {
                if (data.NextStage)
                {
                    var emittedEntity = data.NextStage.CreateDetached(weapon, entity.transform);
                    emittedEntity.SetVFX(ref data.BulletVFX);
                    emittedEntity.RunDetach();
                }
                
                foreach (var t in Utility.Timer(data.Interval))
                    yield return null;
            }
        }
    }
}