using System;
using UnityEngine;

namespace Procool.VFX
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Flicker : MonoBehaviour
    {
        public float frequence = 1;

        public new SpriteRenderer renderer;
        private bool lightUp = false;
        private float previousTime = 0;

        private void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            var t = (Time.time - previousTime) / frequence;
            if (t >= 1)
            {
                t = 0;
                previousTime = Time.time;
                lightUp = !lightUp;
            }

            var color = renderer.color;
            if (lightUp)
                color.a = t;
            else
                color.a = 1 - t;
            renderer.color = color;

        }
    }
}