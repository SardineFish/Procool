using System.Collections.Generic;

namespace Procool.Utils
{
    public class ObjectPool<T> where T : class, new()
    {
        private Stack<T> pool = new Stack<T>();

        public ObjectPool()
        {
            
        }

        protected T Get()
        {
            if (pool.Count > 0)
                return pool.Pop();
            return new T();
        }

        protected void Release(T obj)
        {
            pool.Push(obj);
        }
    }
}