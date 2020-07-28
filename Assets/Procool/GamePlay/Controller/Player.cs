using System;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Controller
{
    public class Player : ManagedMonobehaviour<Player>
    {
        public float HP = 100;
        public float MaxHP = 100;
        public float Armour = 0;
        public Inventory.Inventory Inventory { get; private set; }

        private void Awake()
        {
            Inventory = new Inventory.Inventory(this);
        }

        public void ApplyDamage(float damage)
        {
            HP -= damage;
            if (HP <= 0)
                HP = 0;
        }
    }
}