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
    }
}