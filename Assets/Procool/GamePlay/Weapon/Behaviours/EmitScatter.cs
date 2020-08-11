using System.Collections;
using Procool.Random;
using UnityEngine;

namespace Procool.GamePlay.Weapon
{
    public class EmitScatter : WeaponBehaviour<EmitScatter.Data>
    {
        public class Data : EmitterBehaviourData
        {
            public bool RandomDirection;
            public float Angle;
            public int Count;
            public Data(IWeaponBehaviour behaviour) : base(behaviour)
            {
                
            }
        }

        public override WeaponBehaviourData GenerateBehaviourData(PRNG prng)
        {
            return new Data(this)
            {
                RandomDirection = prng.GetBool(),
                Angle = prng.GetInRange(60, 360),
                Count = prng.GetInRange(3, 8)
            };
        }

        protected override float EvaluateDamageMultiply(Data data)
        {
            return data.Count;
        }

        protected override float EvaluateEmitRate(Data data)
        {
            if (!data.NextStage)
                return 0;
            var next = data.NextStage ? data.NextStage.EvaluateEmitRate() : 1;
            next = Mathf.Max(next, 1);
            return data.Count * next;
        }

        protected override IEnumerator Run(DamageEntity entity, Data data, DamageStage stage, Weapon weapon)
        {
            if (!data.NextStage || !stage.RequestEmit(weapon, entity))
            {
                entity.Terminate();
                yield break;
            }
            
            var angle = data.Angle;
            var randomDir = data.RandomDirection;
            if (stage.IsFirstStage)
            {
                randomDir = false;
                angle = MathUtility.RangeMapClamped(60, 360, 30, 90, angle);
            }

            var rotation = entity.transform.rotation;
            if (randomDir)
                rotation = Quaternion.FromToRotation(Vector3.up, GameRNG.GetVec2OnCircle());
            var dAng = angle / (data.Count - 1);
            for (var i = 0; i < data.Count; i++)
            {
                var ang = -angle / 2 + dAng * i;
                var emittedEntity = data.NextStage.CreateDetached(weapon, entity.transform.position,
                    rotation * Quaternion.AngleAxis(ang, Vector3.forward));
                emittedEntity.SetVFX(ref data.BulletVFX);
                emittedEntity.RunDetach();
            }
            
            entity.Terminate();
            yield break;
        }
    }
}