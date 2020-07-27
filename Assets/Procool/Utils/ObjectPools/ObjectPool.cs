using System.Collections.Generic;

namespace Procool.Utils
{
    public static class ObjectPool<T> where T : new()
    {
        private static Stack<T> pool;

        public static T Get()
        {
            if (pool.Count > 0)
                return pool.Pop();
            return new T();
        }

        public static void Release(T obj)
            => pool.Push(obj);
    }
}