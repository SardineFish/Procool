using System.Collections.Generic;
using Procool.GamePlay.Controller;
using Procool.Random;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Mission
{
    [RequireComponent(typeof(Player), typeof(InteractiveObject))]
    public class NPC : MonoBehaviour
    {
        public readonly List<Mission> AvailableMissions = new List<Mission>();
        public PRNG prng { get; private set; }

        public void Active(PRNG prng)
        {
            this.prng = prng;
            foreach (var lazyLoadComponent in GetComponents<LazyLoadComponent>())
            {
                lazyLoadComponent.Load();
            }
        }
        
        
    }
}