using System.Collections;
using Procool.Random;
using UnityEngine;

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
                Speed = prng.GetInRange(10, 20)
            };
        }

        protected override IEnumerator Run(DamageEntity entity, Data data, DamageStage stage, Weapon weapon)
        {
            while (true)
            {
                entity.transform.Translate(new Vector3(0, data.Speed * Time.deltaTime, 0), Space.Self);
                yield return null;
            }
        }
    }
}