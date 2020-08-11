using System;
using Procool.GamePlay.Weapon;
using Procool.GameSystems;
using UnityEngine;
using UnityEngine.UI;

namespace Procool.UI
{
    public class WeaponCoolDownUI : MonoBehaviour
    {
        public Image cooldownProgress;
        
        private void Update()
        {
            if(!GameSystem.Player)
                return;

            var weapon = GameSystem.Player.Inventory.ActiveItem as Weapon;
            var color = cooldownProgress.color;
            color.a = weapon.CoolDownRate;
            cooldownProgress.color = color;
        }
    }
}