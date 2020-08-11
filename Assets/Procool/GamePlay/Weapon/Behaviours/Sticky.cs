using System.Collections;

namespace Procool.GamePlay.Weapon
{
    public class Sticky : WeaponBehaviour
    {
        protected override IEnumerator Run(DamageEntity entity, EmptyBehaviourData data, DamageStage stage, Weapon weapon)
        {
            yield break;
        }
    }
}