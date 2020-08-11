using System.Collections;
using System.Linq;
using Procool.GamePlay.Controller;
using Procool.Random;
using Unity.Mathematics;
using UnityEngine;

namespace Procool.GamePlay.Weapon
{
    public class EmitTick : WeaponBehaviour<EmitTick.Data>
    {
        public enum EmitType : int
        {
            Default = 0,
            Spin,
            Random,
            Lockon,
        }
        public class Data : EmitterBehaviourData
        {
            public float Interval = 1;
            public EmitType EmitType;
            public float SpinVelocity;
            public Data(IWeaponBehaviour behaviour) : base(behaviour)
            {
                
            }
        }

        protected override float EvaluateDamageMultiply(Data data) => 1 / data.Interval;

        protected override float EvaluateEmitRate(Data data)
        {
            if (!data.NextStage)
                return 0;
            var next = data.NextStage ? data.NextStage.EvaluateEmitRate() : 1;
            next = Mathf.Max(next, 1);
            return 1 / data.Interval * next;
        }

        public override WeaponBehaviourData GenerateBehaviourData(PRNG prng)
        {
            return new EmitTick.Data(this)
            {
                Interval = prng.GetInRange(.1f, .2f),
                EmitType = (EmitType)prng.GetInRange(0, 4),
                SpinVelocity = prng.GetInRange(20, 60)
            };
        }

        protected override IEnumerator Run(DamageEntity entity, Data data, DamageStage stage, Weapon weapon)
        {
            if (!data.NextStage)
                yield break;

            Player lockTarget;
            lockTarget = Player.AssetsManager.Assets
                .Where(target => target.isActiveAndEnabled)
                .Where(target => Vector2.Distance(target.transform.position, entity.transform.position) < 20)
                .MinOf(target => Vector2.Distance(target.transform.position, entity.transform.position));

            var dir = entity.transform.up;
            while (true)
            {
                if (!data.NextStage.RequestEmit(weapon, entity))
                {
                    yield return null;
                    continue;
                }
                if (!stage.IsFirstStage)
                {
                    if (data.EmitType == EmitType.Lockon && lockTarget)
                        dir = (lockTarget.transform.position - entity.transform.position).normalized;
                    else if (data.EmitType == EmitType.Random)
                        dir = GameRNG.GetVec2OnCircle();
                    else if (data.EmitType == EmitType.Spin)
                        dir = MathUtility.Rotate(dir, data.SpinVelocity / 180 * Mathf.PI);
                    else
                        dir = entity.transform.up;
                        
                }
                else
                    dir = entity.transform.up;

                var rotation = Quaternion.FromToRotation(Vector3.up, dir);
                var emittedEntity = data.NextStage.CreateDetached(weapon, entity.transform.position, rotation);
                emittedEntity.SetVFX(ref data.BulletVFX);
                emittedEntity.RunDetach();
                
                foreach (var t in Utility.Timer(data.Interval))
                    yield return null;
            }
        }
    }
}