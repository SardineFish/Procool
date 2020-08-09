using System;
using Procool.GameSystems;
using UnityEngine;
using UnityEngine.UI;

namespace Procool.UI
{
    public class PlayerStat : MonoBehaviour
    {
        [SerializeField] private Slider hpBar;
        [SerializeField] private Slider armorBar;

        private void Update()
        {
            if(!GameSystem.Player)
                return;

            hpBar.value = GameSystem.Player.HP / GameSystem.Player.MaxHP;
            armorBar.value = GameSystem.Player.Armour/ GameSystem.Player.MaxArmour;
        }
    }
}