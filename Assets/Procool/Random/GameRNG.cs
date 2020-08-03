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
            return new PRNG((int)FloatToIntBits(GetScalarByVec2(p)));
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

        public static Vector2 GetVec2ByVec2(Vector2 pos)
        {
            float2 p = pos;
            float3 p3 = math.frac(new float3(p.xyx) * new float3(.1031f, .1030f, .0973f));
            p3 += math.dot(p3, p3.yzx + 33.33f);
            return math.frac((p3.xx + p3.yz) * p3.zy);
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

        public static float GetScalarByInt2(Vector2Int v)
            => Int32ToFloat01(HashVecInt2(v));

        public static float GetScalarByInt3(Vector3Int v)
            => Int32ToFloat01(HashVecInt3(v));
        
        #endregion

        #region Hash

        // Reference: https://stackoverflow.com/questions/664014/what-integer-hash-function-are-good-that-accepts-an-integer-hash-key
        public static uint HashInt32(uint x)
        {
            x = ((x >> 16) ^ x) * 0x45d9f3b;
            x = ((x >> 16) ^ x) * 0x45d9f3b;
            x = (x >> 16) ^ x;
            return x;
        }

        public static int HashInt32(int x) => (int) HashInt32((uint) x);

        public static uint UnhashInt32(uint x)
        {
            x = ((x >> 16) ^ x) * 0x119de1f3;
            x = ((x >> 16) ^ x) * 0x119de1f3;
            x = (x >> 16) ^ x;
            return x;
        }

        public static ulong HashInt64(ulong x)
        {
            x = (x ^ (x >> 30)) * 0xbf58476d1ce4e5b9L;
            x = (x ^ (x >> 27)) * 0x94d049bb133111ebL;
            x = x ^ (x >> 31);
            return x;
        }

        public static ulong UnhashInt64(ulong x)
        {
            x = (x ^ (x >> 31) ^ (x >> 62)) * 0x319642b2d24d8ec3L;
            x = (x ^ (x >> 27) ^ (x >> 54)) * 0x96de1b173f119089L;
            x = x ^ (x >> 30) ^ (x >> 60);
            return x;
        }

        static uint CantorPairing(uint x, uint y)
            => (x + y) * (x + y + 1) / 2 + y;

        public static uint HashVecInt2(Vector2Int v)
        {
            return CantorPairing((uint)HashInt32(v.x), (uint)HashInt32(v.y));
        }

        public static uint HashVecInt3(Vector3Int v)
            => CantorPairing(CantorPairing((uint) HashInt32(v.x), (uint) HashInt32(v.y)), (uint) HashInt32(v.z));

        #endregion
        
        #region Utils

        /// <summary>
        /// Directly convert 32 bits float into 32 bits integer
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        static unsafe uint FloatToIntBits(float f)
        {
            return *(uint*) &f;
        }
        
        static float Int32ToFloat01(uint n)
        {
            return (float) n / UInt32.MaxValue;
        }

        #endregion
    }
}