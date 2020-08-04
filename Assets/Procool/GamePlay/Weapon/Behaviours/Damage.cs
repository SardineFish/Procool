using System.Collections;

namespace Procool.GamePlay.Weapon
{
    public class Damage : WeaponBehaviour<EmptyBehaviourData>
    {
        protected override float EvaluateAdditionalDamage(EmptyBehaviourData data) => 1;

        protected override IEnumerator Run(DamageEntity entity, EmptyBehaviourData data, DamageStage stage, Weapon weapon)
        {
            while (true)
            {
                foreach (var player in entity.HitPlayer())
                {
                    if (!entity.DamageRecord.Contains(player))
                    {
                        entity.DamageRecord.Add(player);
                        player.ApplyDamage(weapon.Damage);
                    }
                }

                yield return null;
            }
        }
    }
}