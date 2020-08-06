using System;
using System.Collections.Generic;
using Procool.GameSystems;
using UnityEngine;

namespace Procool.Utils
{
    public class GameObjectPool : Singleton<GameObjectPool>
    {
        private string defaultObjectName = "[GameObject]";
        public Func<GameObject> Allocator;
        Stack<GameObject> objectPool = new Stack<GameObject>();
        private static Dictionary<GameObject, GameObjectPool> prefabPools = new Dictionary<GameObject, GameObjectPool>();
        private static HashSet<GameObjectPool> componentPools = new HashSet<GameObjectPool>();

        static class PerComponentPool<T> where T : Component
        {
            private static GameObjectPool pool;

            public static bool Ready => pool;

            static GameObject Allocator()
            {
                var obj = new GameObject();
                var component = obj.AddComponent<T>();
                return obj;
            }

            public static GameObjectPool GetOrCreatePool()
            {
                if (!pool)
                {
                    var obj = new GameObject();
                    pool = obj.AddComponent<GameObjectPool>();
                    pool.Allocator = Allocator;
                }

                return pool;
            }
        }

        public static GameObject Get(GameObject prefab) => GetOrCreatePrefabPool(prefab).Get();

        public static GameObject Get(GameObject prefab, string name) => GetOrCreatePrefabPool(prefab).Get(name);

        public static T Get<T>() where T : Component => GetOrCreateComponentPool<T>().Get().GetComponent<T>();

        public static T Get<T>(string name) where T : Component =>
            GetOrCreateComponentPool<T>().Get(name).GetComponent<T>();

        public static T Get<T>(GameObject prefab) where T : Component
            => Get(prefab)?.GetComponent<T>();

        public static T Get<T>(GameObject prefab, string name) where T : Component
            => Get(prefab, name).GetComponent<T>();

        public static void Release<T>(GameObject prefab, T component) where T : Component
        {
            if(component && component.gameObject)
                Release(prefab, component.gameObject);
        }

        public static void Release(GameObject prefab, GameObject obj)
        {
            GetOrCreatePrefabPool(prefab).Release(obj);
        }

        public static void Release<T>(T component) where T : Component
        {
            if (component && component.gameObject)
                GetOrCreateComponentPool<T>().Release(component.gameObject);
        }

        public static void Release<T>(GameObject obj) where T : Component
            => GetOrCreateComponentPool<T>().Release(obj);

        public static void PreAlloc(GameObject prefab, int count)
        {
            GetOrCreatePrefabPool(prefab).PreAlloc(count);
        }

        public static void PreAlloc<T>(int count) where T : Component
        {
            var pool = GetOrCreateComponentPool<T>();
            pool.PreAlloc(count);
        }

        static GameObjectPool GetOrCreatePrefabPool(GameObject prefab)
        {
            if (prefabPools.ContainsKey(prefab))
                return prefabPools[prefab];
            var pool = CreatePrefabPool(prefab);
            prefabPools[prefab] = pool;
            return pool;
        }

        static GameObjectPool CreatePrefabPool(GameObject prefab)
        {
            var obj = new GameObject();
            var pool = obj.AddComponent<GameObjectPool>();
            pool.transform.parent = Instance.transform;
            pool.Allocator = () => Instantiate(prefab);
            pool.defaultObjectName = prefab.name;
            pool.name = "[Pool]" + prefab.name;
            return pool;
        }

        static GameObjectPool GetOrCreateComponentPool<T>() where T : Component
        {
            if (!PerComponentPool<T>.Ready)
            {
                var pool = PerComponentPool<T>.GetOrCreatePool();
                pool.transform.parent = Instance.transform;
                pool.defaultObjectName = typeof(T).Name;
                pool.name = "[Pool]" + pool.defaultObjectName;
                componentPools.Add(pool);
            }

            return PerComponentPool<T>.GetOrCreatePool();
        }

        GameObject CreateObject()
        {
            var newObj = Allocator?.Invoke();
            newObj.name = defaultObjectName;
            return newObj;
        }

        public GameObject Get()
        {
            if (objectPool.Count > 0)
            {
                var obj = objectPool.Pop();
                obj.SetActive(true);
                // obj.transform.parent = null;
                obj.transform.SetParent(null);
                return obj;
            }

            return CreateObject();
        }

        public GameObject Get(string name)
        {
            var obj = Get();
            obj.name = name;
            return obj;
        }

        public void Release(GameObject obj)
        {
            if (!obj)
                return;
            // obj.transform.parent = transform;
            obj.transform.SetParent(transform);
            obj.layer = 0;
            obj.SetActive(false);
            objectPool.Push(obj);
        }

        public void PreAlloc(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var obj = CreateObject();
                Release(obj);
            }
        }
    }
}