using System.Collections;
using Procool.GamePlay.Inventory;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Weapon
{
    public class Weapon : Item
    {
        public DamageStage FirstStage;
        public int Quality;
        public float Damage = 1;
        public float CoolDown = 0.2f;
        public float EmitRateLimit = 10;

        private DamageEntity damageEntity = null;
        private float previousActiveTime = 0;

        // public override CoroutineRunner Activate()
        // {
        //     damageEntity = FirstStage.RunDetach()
        //     var runner = new CoroutineRunner(Run());
        //     return runner;
        // }

        public override IUsingState Activate()
        {
            var cd = Mathf.Max(FirstStage.Behaviours[0].NextStage.EmitInterval(this), CoolDown);
            if (FirstStage.Behaviours[0].Behaviour is EmitScatter)
                cd = FirstStage.EmitRate;
            cd = Mathf.Min(cd, 5);
            if(Time.time < previousActiveTime + cd)
                return FailedUsing.Instance;
            previousActiveTime = Time.time;
            var entity = FirstStage.CreateDetached(this, Owner.transform);
            entity.transform.parent = Owner.transform;
            return entity;
        }

        public override void Abort()
        {
            if(damageEntity)
                damageEntity.Terminate();
        }
    }
}