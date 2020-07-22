using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Procool.Random
{
    public static class GameRNG
    {
        private static int seed;
        
        private static System.Random random = new System.Random();

        public static void SetSeed(int seed)
        {
            GameRNG.seed = seed;
            random = new System.Random(seed);
            randOffset = 0;
            randOffset = GetVec4ByScalar((float) random.NextDouble());
        }
        
        #region RandSequence


        // TODO: Rewrite to use object pool & custom PRNG
        public static PRNG GetPRNG(Vector2 p)
        {
            return new PRNG(FloatToIntBits(GetScalarByVec2(p)));
        }

        public static void ReleasePRNG(PRNG prng)
        {
            
        }
        
        #endregion
        
        // Reference: https://www.shadertoy.com/view/4djSRW 
        #region WhiteNoise

        private static float4 randOffset = 0;
        public static float GetScalarByVec2(Vector2 pos)
        {
            var p = new float2(pos) + randOffset.xy;
            var p3 = math.frac(new float3(p.xyx) * .1031f);
            p3 += math.dot(p3, p3.yzx + 33.33f);
            return math.frac((p3.x + p3.y) * p3.z);
        }

        public static Vector4 GetVec4ByScalar(float p)
        {
            p += randOffset.x;
            float4 p4 = math.frac(new float4(p) * new float4(.1031f, .1030f, .0973f, .1099f));
            p4 += math.dot(p4, p4.wzxy + 33.33f);
            return math.frac((p4.xxyz + p4.yzzw) * p4.zywx);
        }

        public static Vector4 GetVec4ByVec4(Vector4 pos)
        {
            var p4 = new float4(pos);
            p4 = math.frac(p4 * new float4(.1031f, .1030f, .0973f, .1099f));
            p4 += math.dot(p4, p4.wzxy + 33.33f);
            return math.frac((p4.xxyz + p4.yzzw) * p4.zywx);
        }

        public static float GetScalarByVec4(Vector4 pos)
        {
            var p4 = new float4(pos);
            p4 = math.frac(p4 * new float4(.1031f, .1030f, .0973f, .1099f));
            p4 += math.dot(p4, p4.wxzy + 33.33f);
            return math.frac((p4.xxyz + p4.yzzw) * p4.zywx).x;
        }

        public static float GetScalarByVec2Pair(Vector2 a, Vector2 b)
        {
            Vector4 p;
            if (a.x < b.x)
                p = new Vector4(a.x, a.y, b.x, b.y);
            else if (b.x < a.x)
                p = new Vector4(b.x, b.y, a.x, a.y);
            else if (a.y < b.y)
                p = new Vector4(a.x, a.y, b.x, b.y);
            else
                p = new Vector4(b.x, b.y, a.x, a.y);
            return GetScalarByVec4(p);
        }
        
        #endregion

        #region Utils

        static int FloatToIntBits(float f)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(f), 0);
        }

        #endregion
    }
}