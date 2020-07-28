using System.Collections;
using Procool.Random;

namespace Procool.GamePlay.Weapon
{
    public class EmitTick : WeaponBehaviour<EmitTick.Data>
    {
        public class Data : WeaponBehaviourData
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
                Interval = prng.GetInRange(.3f,2)
            };
        }

        protected override IEnumerator Run(DamageEntity entity, Data data, DamageStage stage, Weapon weapon)
        {
            while (true)
            {
                data.NextStage?.Run(weapon);
                
                foreach (var t in Utility.Timer(data.Interval))
                    yield return null;
            }
        }
    }
}