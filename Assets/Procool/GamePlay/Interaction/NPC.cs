using System.Collections.Generic;
using Procool.GamePlay.Controller;
using Procool.Random;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Interaction
{
    [RequireComponent(typeof(Player), typeof(InteractiveObject))]
    public class NPC : MonoBehaviour
    {
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