using System.Collections;
using Procool.Random;

namespace Procool.GamePlay.Weapon
{
    public class Destructor : WeaponBehaviour<Destructor.Data>
    {
        public class Data : WeaponBehaviourData
        {
            public float Timeout = 5;
            public Data(IWeaponBehaviour behaviour) : base(behaviour)
            {
            }
        }

        public override WeaponBehaviourData GenerateBehaviourData(PRNG prng)
        {
            return new Destructor.Data(this)
            {
                Timeout = prng.GetInRange(3, 8)
            };
        }

        protected override IEnumerator Run(DamageEntity entity, Data data, DamageStage stage, Weapon weapon)
        {
            foreach (var t in Utility.Timer(data.Timeout))
            {
                yield return null;
            }
            entity.Terminate();
        }
    }
}