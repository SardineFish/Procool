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
        public float EmitRateLimit = 10;

        private DamageEntity damageEntity = null;
        private float previousActiveTime = -100;

        public float CoolDown
        {
            get
            {
                var cd = Mathf.Max(FirstStage.Behaviours[0].NextStage.EmitInterval(this), 0.2f);
                if (FirstStage.Behaviours[0].Behaviour is EmitScatter)
                    cd = FirstStage.EmitInterval(this);
                cd = Mathf.Min(cd, 5);
                return cd;
            }
        }

        public float CoolDownRate => (Time.time - previousActiveTime) / CoolDown;

        // public override CoroutineRunner Activate()
        // {
        //     damageEntity = FirstStage.RunDetach()
        //     var runner = new CoroutineRunner(Run());
        //     return runner;
        // }

        public override IUsingState Activate()
        {
            if(Time.time < previousActiveTime + CoolDown)
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