using System.Collections;
using Procool.GamePlay.Inventory;

namespace Procool.GamePlay.Weapon
{
    public class Weapon : Item
    {
        public DamageStage FirstStage;
        public int Quality;
        public float Damage = 1;

        public override IEnumerator Activate()
        {
            yield return FirstStage.Run(this);
        }
    }
}