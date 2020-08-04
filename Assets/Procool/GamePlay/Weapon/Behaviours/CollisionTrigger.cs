using System.Collections;

namespace Procool.GamePlay.Weapon
{
    public class CollisionTrigger : WeaponBehaviour
    {
        protected override IEnumerator Run(DamageEntity entity, EmptyBehaviourData data, DamageStage stage, Weapon weapon)
        {
            while (true)
            {
                if (entity.GetMostContact(out var contact))
                {
                    data.NextStage?.Run(weapon, entity);
                    entity.Terminate();
                    yield break;
                }

                yield return null;
            }
        }
    }
}