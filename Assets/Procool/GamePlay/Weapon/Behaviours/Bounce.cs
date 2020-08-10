using System.Collections;
using Procool.Random;
using UnityEngine;

namespace Procool.GamePlay.Weapon
{
    public class Bounce : WeaponBehaviour
    {

        protected override float EvaluateDamageMultiply(EmptyBehaviourData data) => 1.5f;

        protected override IEnumerator Run(DamageEntity entity, EmptyBehaviourData data, DamageStage stage, Weapon weapon)
        {
            while (true)
            {
                if (entity.GetMostContact(out var hit))
                {
                    Debug.DrawLine(hit.point, hit.point + hit.normal, Color.red);
                    if (Vector2.Dot(entity.transform.up, hit.normal) < 0)
                    {
                        entity.ContactedDamageTargets.Clear();
                        var normal = hit.normal;
                        var direction = entity.transform.up;
                        var reflect = Vector2.Reflect(direction.ToVector2(), normal);
                        entity.transform.rotation *= Quaternion.FromToRotation(direction, reflect.ToVector3());
                    }
                }
                yield return null;
            }
        }
    }
}