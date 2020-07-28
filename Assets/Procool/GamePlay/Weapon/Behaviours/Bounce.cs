using System.Collections;
using Procool.Random;
using UnityEngine;

namespace Procool.GamePlay.Weapon
{
    public class Bounce : WeaponBehaviour
    {

        protected override float EvaluateDamageMultiply(EmptyBehaviourData data) => 2;

        protected override IEnumerator Run(DamageEntity entity, EmptyBehaviourData data, DamageStage stage, Weapon weapon)
        {
            while (true)
            {
                if (entity.GetMostContact(out var contact))
                {
                    var normal = contact.normal;
                    var direction = entity.transform.up;
                    var reflect = Vector2.Reflect(direction.ToVector2(), normal);
                    entity.transform.rotation *= Quaternion.FromToRotation(direction, reflect.ToVector3());
                }
                yield return null;
            }
        }
    }
}