﻿using System.Collections;
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
                Timeout = prng.GetInRange(1.5f, 3)
            };
        }

        protected override IEnumerator Run(DamageEntity entity, Data data, DamageStage stage, Weapon weapon)
        {
            foreach (var t in Utility.Timer(data.Timeout))
                yield return null;

            data.NextStage?.Run(weapon, entity);
        }
    }
}