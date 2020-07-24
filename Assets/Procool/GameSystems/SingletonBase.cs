using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Procool.GameSystems
{
    public class SingletonBase : MonoBehaviour
    {
        
    }
    public class Singleton<T> : SingletonBase where T : Singleton<T>
    {
        public static T Instance { get; private set; }

        [SerializeField] private bool dontDestroy = false;

        protected virtual void Awake()
        {
            if (!Instance)
                Instance = this as T;
            if (dontDestroy)
            {
                transform.parent = null;
                GameObject.DontDestroyOnLoad(gameObject);
            }
        }
    }
}