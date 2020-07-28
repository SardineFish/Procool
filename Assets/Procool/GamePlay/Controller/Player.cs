using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Controller
{
    public class Player : ManagedMonobehaviour<Player>
    {
        public float HP = 100;
        public float MaxHP = 100;
        public float Armour = 0;

        public void ApplyDamage(float damage)
        {
            HP -= damage;
            if (HP <= 0)
                HP = 0;
        }
    }
}