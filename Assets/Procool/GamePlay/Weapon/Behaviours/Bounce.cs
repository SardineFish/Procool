using System.Collections;
using Procool.Random;

namespace Procool.GamePlay.Weapon
{
    public class Bounce : WeaponBehaviour
    {

        protected override float EvaluateDamageMultiply(EmptyBehaviourData data) => 2;

        protected override IEnumerator Run(DamageEntity entity, EmptyBehaviourData data, DamageStage stage)
        {
            while (true)
            {
                yield return null;
            }
        }
    }
}