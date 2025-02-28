﻿using System.Collections;
using Procool.Random;
using UnityEngine;

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
                Distance = prng.GetInRange(5, 10),
                StartSpeed = prng.GetInRange(5, 16),
            };
        }

        protected override IEnumerator Run(DamageEntity entity, Data data, DamageStage stage, Weapon weapon)
        {
            var acceleration = data.StartSpeed * data.StartSpeed / (2 * data.Distance);
            var speed = data.StartSpeed;
            while (true)
            {   
                entity.transform.Translate(new Vector3(0, speed * Time.deltaTime, 0), Space.Self);
                
                // var move = entity.transform.localToWorldMatrix.MultiplyVector(new Vector3(0,
                //     speed * Time.deltaTime, 0));
                // entity.Move(move);

                var v = speed - acceleration * Time.deltaTime;
                if (MathUtility.SignInt(v) != MathUtility.SignInt(speed))
                    break;
                speed = v;
                
                yield return null;
            }
            
            if(!data.NextStage)
                entity.Terminate();
            else 
                data.NextStage?.Run(weapon, entity);
        }
    }
}