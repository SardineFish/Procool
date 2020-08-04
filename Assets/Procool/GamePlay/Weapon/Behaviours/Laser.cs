using System.Collections;
using Procool.Random;
using UnityEngine;

namespace Procool.GamePlay.Weapon
{
    public class Laser : WeaponBehaviour<Laser.Data>
    {
        public class Data : WeaponBehaviourData
        {
            public float DPSMultiply = 1;
            public float Width = .2f;
            public Data(IWeaponBehaviour behaviour) : base(behaviour)
            {
            }
        }

        protected override float EvaluateDamageMultiply(Data data) => data.DPSMultiply;

        public override WeaponBehaviourData GenerateBehaviourData(PRNG prng)
        {
            return new Laser.Data(this)
            {
                DPSMultiply = prng.GetInRange(.5f, 2f),
                Width = prng.GetInRange(.1f,.5f)
            };
        }

        private RaycastHit2D[] resultArray = new RaycastHit2D[16];

        protected override IEnumerator Run(DamageEntity entity, Data data, DamageStage stage, Weapon weapon)
        {
            var count = Physics2D.RaycastNonAlloc(entity.transform.position, entity.transform.up, resultArray, 64);
            Vector2 endPoint = entity.transform.position;
            var distance = 0f;
            for (var i = 0; i < count; i++)
            {
                if (resultArray[i].rigidbody.gameObject != entity.Owner.gameObject)
                {
                    endPoint = resultArray[i].point;
                    distance = resultArray[i].distance;
                    break;
                }
            }

            // entity.SpriteRenderer.tileMode = SpriteTileMode.Continuous;
            // entity.SpriteRenderer.size =
            //     new Vector2(data.Width, distance);

            entity.GetComponent<BoxCollider2D>().offset = new Vector2(0, distance / 2);
            entity.GetComponent<BoxCollider2D>().size = new Vector2(data.Width, distance);

            while (true)
                yield return null;
        }
    }
}