using System;
using System.Threading.Tasks;
using Procool.GamePlay.Controller;
using Procool.GameSystems;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Weapon
{
    public class Explosion : MonoBehaviour
    {
        public float LifeTime;
        public GameObject Prefab;

        Collider2D[] involvedColliders = new Collider2D[8];
        public async void Explode(Vector2 position, float radius, float damage, Player owner = null)
        {
            transform.position = position;
            GetComponentsInChildren<ParticleSystem>().ForEach(particleSystem=>particleSystem.Play());

            var count = Physics2D.OverlapCircleNonAlloc(position, radius, involvedColliders);
            for (var i = 0; i < count; i++)
            {
                var target = involvedColliders[i].attachedRigidbody?.GetComponent<IDamageTarget>();
                if (target != null && target != owner)
                {
                    var distance = MathUtility.RangeMapClamped(
                        0, radius,
                        1, 2,
                        Vector2.Distance(involvedColliders[i].transform.position, position));
                    target.ApplyDamage(damage / (distance * distance));
                }
            }
            
            await Task.Delay(TimeSpan.FromSeconds(LifeTime));
            GameObjectPool.Release(Prefab, this);
               
        }
    }
}