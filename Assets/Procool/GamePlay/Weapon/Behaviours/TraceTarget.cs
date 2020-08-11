using System.Collections;
using System.Linq;
using Procool.GamePlay.Controller;
using Procool.Random;
using UnityEngine;

namespace Procool.GamePlay.Weapon
{
    public class TraceTarget : WeaponBehaviour<TraceTarget.TraceData>
    {
        public class TraceData : WeaponBehaviourData
        {
            public float Radius;
            public float MaxAngularVelocity;
            public TraceData(IWeaponBehaviour behaviour) : base(behaviour)
            {
            }
        }
        protected override IEnumerator Run(DamageEntity entity, TraceData data, DamageStage stage, Weapon weapon)
        {
            var minDist = float.MaxValue;
            Player target = null;
            foreach (var player in Player.AssetsManager.Assets.Where(asset=>asset.isActiveAndEnabled))
            {
                var distance = Vector2.Distance(player.transform.position, entity.transform.position);
                if (distance < data.Radius && distance < minDist && player != entity.Owner)
                {
                    minDist = distance;
                    target = player;
                }
            }

            while (target)
            {
                var dir = (target.transform.position - entity.transform.position).normalized;
                var deltaAng = Vector2.SignedAngle(entity.transform.up, dir);
                if (Mathf.Abs(deltaAng)> data.MaxAngularVelocity * Time.deltaTime)
                {
                    deltaAng = MathUtility.SignInt(deltaAng) * data.MaxAngularVelocity * Time.deltaTime;
                }
                entity.transform.Rotate(Vector3.forward, deltaAng);

                yield return null;
            }
        }

        public override WeaponBehaviourData GenerateBehaviourData(PRNG prng)
        {
            return new TraceData(this)
            {
                Radius = prng.GetInRange(10, 20),
                MaxAngularVelocity = prng.GetInRange(60, 60)
            };   
        }
    }
}