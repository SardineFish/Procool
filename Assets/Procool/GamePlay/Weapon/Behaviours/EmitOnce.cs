using System.Collections;
using Procool.Random;

namespace Procool.GamePlay.Weapon
{
    public class EmitOnce : WeaponBehaviour<EmitterBehaviourData>
    {
        protected override IEnumerator Run(DamageEntity entity, EmitterBehaviourData data, DamageStage stage, Weapon weapon)
        {
            if (data.NextStage)
            {
                var emittedEntity = data.NextStage.CreateDetached(weapon, entity.transform);
                emittedEntity.SetVFX(ref data.BulletVFX);
                emittedEntity.RunDetach();
            }
            entity.Terminate();
            yield break;
        }

        public override WeaponBehaviourData GenerateBehaviourData(PRNG prng)
        {
            return new EmitterBehaviourData(this);
        }
    }
}