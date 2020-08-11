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
                    if (data.NextStage)
                        data.NextStage.Run(weapon, entity);
                    else
                        entity.Terminate();
                    // entity.Terminate();
                    while (true)
                    {
                        entity.transform.position = contact.point + contact.normal * entity.BulletVfx.BulletSize;
                        yield return null;
                    }
                    
                    yield break;
                }

                yield return null;
            }
        }
    }
}