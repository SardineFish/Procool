using System.Collections;

namespace Procool.GamePlay.Weapon
{
    public class EmitOnce : WeaponBehaviour<EmptyBehaviourData>
    {
        protected override IEnumerator Run(DamageEntity entity, EmptyBehaviourData data, DamageStage stage, Weapon weapon)
        {
            data.NextStage?.Run(weapon);
            entity.Terminate();
            yield break;
        }
    }
}