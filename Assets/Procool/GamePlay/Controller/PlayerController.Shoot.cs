using System.Collections;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Controller
{
    public partial class PlayerController
    {
        class PlayerShoot : PlayerAction
        {
            private CoroutineRunner weaponRunner;
            public override bool CanEnter(PlayerController player)
            {
                return player.Input.GamePlay.Fire.ReadValue<bool>();
            }

            public override void Enter(PlayerController player)
            {
                var weapon = player.Player.Inventory.GetItem(0) as Weapon.Weapon;
                if (weapon != null)
                {
                }
            }

            // public override bool Update(PlayerController player)
            // {
            //     if (weaponRunner)
            //     {
            //         if (!weaponRunner.Tick())
            //         {
            //             weaponRunner = null;
            //             return false;
            //         }
            //         if(weaponRunner.)
            //     }
            //     if (!weaponRunner.Tick())
            //     {
            //         weaponRunner = null;
            //         return false;
            //     }
            //     return player.Input.GamePlay.Fire.ReadValue<bool>();
            // }
        }
    }
}