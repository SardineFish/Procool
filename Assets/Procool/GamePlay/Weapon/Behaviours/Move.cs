using Procool.Random;

namespace Procool.GamePlay.Weapon
{
    public class Move : WeaponBehaviour<Move.Data>
    {
        public class Data : WeaponBehaviourData
        {
            public float Speed = 10;
            public Data(IWeaponBehaviour behaviour) : base(behaviour)
            {
                
            }
        }

        public override WeaponBehaviourData GenerateBehaviourData(PRNG prng)
        {
            return new Move.Data(this)
            {
                Speed = prng.GetInRange(5, 20)
            };
        }
    }
}