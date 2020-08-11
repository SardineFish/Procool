using System;
using Procool.GamePlay.Controller;
using UnityEngine;

namespace Procool.Misc
{
    [RequireComponent(typeof(Player))]
    public class AutoHealing : MonoBehaviour
    {
        public float healTime = 1f;
        public float healSpeed = 10;

        private float previousStopTime = 0;

        private void Update()
        {
            if (Time.time - previousStopTime >= healTime)
            {
                GetComponent<Player>().Heal(healSpeed * Time.deltaTime);
            }
            
        }

        public void Stop()
        {
            previousStopTime = Time.time;
        }
    }
}