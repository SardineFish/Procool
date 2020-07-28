﻿using Procool.Random;

namespace Procool.GamePlay.Weapon
{
    public class Throw : WeaponBehaviour <Throw.Data>
    {
        public class Data : WeaponBehaviourData
        {
            public float Distance = 5;
            public float StartSpeed = 10;
            public Data(IWeaponBehaviour behaviour) : base(behaviour)
            {
            }
        }

        public override WeaponBehaviourData GenerateBehaviourData(PRNG prng)
        {
            return new Throw.Data(this)
            {
                Distance = prng.GetInRange(3, 10),
                StartSpeed = prng.GetInRange(5, 20),
            };
        }
    }
}