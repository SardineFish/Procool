using Procool.Random;

namespace Procool.GamePlay.Weapon
{
    public class Timeout : WeaponBehaviour<Timeout.Data>
    {
        public class Data : WeaponBehaviourData
        {
            public float Timeout;
            public Data(IWeaponBehaviour behaviour) : base(behaviour)
            {
            }
        }

        public override WeaponBehaviourData GenerateBehaviourData(PRNG prng)
        {   
            return new Timeout.Data(this)
            {
                Timeout = prng.GetInRange(3, 5)
            };
        }
    }
}