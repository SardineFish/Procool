using System;
using Procool.Utils;
using UnityEngine;

namespace Procool.Misc
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleRecycle : MonoBehaviour
    {
        public GameObject Prefab;
        private void OnParticleSystemStopped()
        {
            GameObjectPool.Release(Prefab, gameObject);
        }
    }
}