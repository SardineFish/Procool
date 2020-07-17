using System.Collections.Generic;
using UnityEngine;

namespace Procool.Utils
{
    public class ShaderPool
    {
        private static readonly Dictionary<string, Material> Pool = new Dictionary<string, Material>();

        public static Material Get(string name)
        {
            if (!Pool.ContainsKey(name) || !Pool[name])
                Pool[name] = new Material(Shader.Find(name));
            return Pool[name];
        }
    }
}