﻿using Procool.Random;

namespace Procool.GamePlay.Weapon
{
    public class Laser : WeaponBehaviour<Laser.Data>
    {
        public class Data : WeaponBehaviourData
        {
            public float DPSMultiply = 1;
            public float Width = .2f;
            public Data(IWeaponBehaviour behaviour) : base(behaviour)
            {
            }
        }

        protected override float EvaluateDamageMultiply(Data data) => data.DPSMultiply;

        public override WeaponBehaviourData GenerateBehaviourData(PRNG prng)
        {
            return new Laser.Data(this)
            {
                DPSMultiply = prng.GetInRange(.5f, 2f),
                Width = prng.GetInRange(.1f,.5f)
            };
        }
    }
}