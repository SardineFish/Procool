using Procool.Random;

namespace Procool.GamePlay.Weapon
{
    public class Bounce : WeaponBehaviour<Bounce.Data>
    {
        public class Data : WeaponBehaviourData
        {
            public Data(IWeaponBehaviour behaviour) : base(behaviour)
            {
            }
        }

        protected override float EvaluateAdditionalDamage(Data data)
        {
            return 2 * base.EvaluateAdditionalDamage(data);
        }

        public override WeaponBehaviourData GenerateBehaviourData(PRNG prng)
        {
            var data = new Data(this);
            // data.NextStage = stage;
            return data;
        }
    }
}