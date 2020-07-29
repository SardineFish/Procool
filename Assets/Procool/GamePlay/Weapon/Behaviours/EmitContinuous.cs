using System;
using System.Collections;

namespace Procool.GamePlay.Weapon
{
    public class EmitContinuous : WeaponBehaviour<EmptyBehaviourData>
    {
        protected override IEnumerator Run(DamageEntity entity, EmptyBehaviourData data, DamageStage stage, Weapon weapon)
        {
            if (data.NextStage)
            {
                data.NextStage.Run(weapon, entity);
                // var newEntity = data.NextStage.CreateEntity(weapon, entity.transform);
                // entity.OnTerminated += OnTerminated;
                //
                // yield return data.NextStage.Run(weapon);
                //
                // void OnTerminated(DamageEntity _)
                // {
                //     newEntity.Terminate();
                //     entity.OnTerminated -= OnTerminated;
                // }
            }
            yield break;
        }
    }
}