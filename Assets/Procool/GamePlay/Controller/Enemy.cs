using System;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Controller
{
    [RequireComponent(typeof(Player))]
    public class Enemy : ManagedMonobehaviour<Enemy>
    {
        public Player Player { get; private set; }

        private void Awake()
        {
            Player = GetComponent<Player>();
        }
    }
}