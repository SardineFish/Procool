﻿using System;
using System.Collections.Generic;

namespace Procool.Utils
{
    public class ObjectPoolBase<T> where T : class, new()
    {
        private Stack<T> pool = new Stack<T>();

        public ObjectPoolBase()
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

    public class ObjectWithPool<T> where T: ObjectWithPool<T>, new()
    {
        public static Stack<T> pool = new Stack<T>();
        public bool Valid { get; private set; }

        public ObjectWithPool()
        {
            Valid = false;
        }

        ~ObjectWithPool()
        {
            if(Valid)
                throw new Exception("Unexpected GC");
        }
        

        protected static T GetInternal()
        {
            T obj;
            if (pool.Count > 0)
                obj = pool.Pop();
            else
                obj = new T();
            obj.Valid = true;
            return obj;
        }

        protected static void ReleaseInternal(T obj)
        {
            if (!obj.Valid)
                return;
            obj.Valid = false;
            pool.Push(obj);
        }

        public static void PreAlloc(int count)
        {
            var newPool = new Stack<T>(pool.Count + count);
            foreach (var obj in pool)
            {
                newPool.Push(obj);
            }

            pool.Clear();
            pool = newPool;

            for (var i = 0; i < count; i++)
            {
                pool.Push(new T());
            }
        }

        public static implicit operator bool(ObjectWithPool<T> obj)
        {
            return !(obj is null) && obj.Valid;
        }
        
    }
}