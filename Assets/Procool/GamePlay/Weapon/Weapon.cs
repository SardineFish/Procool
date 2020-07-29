using System.Collections;
using Procool.GamePlay.Inventory;
using Procool.Utils;

namespace Procool.GamePlay.Weapon
{
    public class Weapon : Item
    {
        public DamageStage FirstStage;
        public int Quality;
        public float Damage = 1;

        private DamageEntity damageEntity = null;

        // public override CoroutineRunner Activate()
        // {
        //     damageEntity = FirstStage.RunDetach()
        //     var runner = new CoroutineRunner(Run());
        //     return runner;
        // }

        public override IUsingState Activate()
        {
            return FirstStage.CreateDetached(this, Owner.transform);
        }

        public override void Abort()
        {
            if(damageEntity)
                damageEntity.Terminate();
        }
    }
}