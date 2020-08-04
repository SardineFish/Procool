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