﻿using System;
using System.Collections.Generic;
using Procool.GamePlay.Inventory;
using Procool.GameSystems;
using Procool.Misc;
using Procool.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Procool.GamePlay.Controller
{
    public class Player : ManagedMonobehaviour<Player>, IBlockPositionEntity, IDamageTarget
    {
        public float HP = 100;
        public float MaxHP = 100;
        public float Armour = 0;
        public float MaxArmour = 100;
        public bool Dead => HP <= 0;
        public UnityEvent OnDamage;
        public UnityEvent OnDead;
        public UnityEvent OnLoaded;
        public Inventory.Inventory Inventory { get; private set; }
        
        public BlockPosition BlockPosition { get; set; }

        public readonly HashSet<InteractiveObject> AvailableInteractiveObjects = new HashSet<InteractiveObject>();

        private void Awake()
        {
            Inventory = new Inventory.Inventory(this);
        }

        private void Update()
        {
            BlockPosition = new BlockPosition(new Block(0, 0, 5), transform.position);
        }

        public void ApplyDamage(float damage)
        {
            HP -= damage;
            if (HP <= 0)
            {
                HP = 0;
                OnDead.Invoke();
                var vfx = GameObjectPool.Get<ParticleSystem>(PrefabManager.Instance.DeadVFXPrefab);
                vfx.transform.position = transform.position;
                vfx.Play();
            }
            OnDamage.Invoke();
        }

        public void Heal(float deltaHP)
        {
            HP = Mathf.Clamp(HP + deltaHP, 0, MaxHP);
        }
    }
}