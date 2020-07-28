using System.Collections;

namespace Procool.GamePlay.Weapon
{
    public class Damage : WeaponBehaviour<EmptyBehaviourData>
    {
        protected override float EvaluateAdditionalDamage(EmptyBehaviourData data) => 1;

        protected override IEnumerator Run(DamageEntity entity, EmptyBehaviourData data, DamageStage stage, Weapon weapon)
        {
            switch (stage.ColliderType)
            {
                case ColliderType.Box:
                    entity.BoxCollider.enabled = true;
                    break;
                case ColliderType.Circle:
                    entity.CircleCollider.enabled = true;
                    break;
            }
            
            while (true)
            {
                foreach (var player in entity.ContactedPlayers)
                {
                    if (!entity.DamageRecord.Contains(player))
                    {
                        entity.DamageRecord.Add(player);
                        player.ApplyDamage(weapon.Damage);
                    }
                }
            }
        }
    }
}