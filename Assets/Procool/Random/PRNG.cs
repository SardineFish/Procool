using UnityEngine;

namespace Procool.Random
{
    public class PRNG
    {
        private System.Random random;

        public PRNG(int seed)
        {
            random = new System.Random(seed);
        }


        /// <summary>
        /// Random float in [0, 1)
        /// </summary>
        /// <returns></returns>
        public float GetScalar()
        {
            return (float)random.NextDouble();
        }

        /// <summary>
        /// Random int in [0, int.MaxValue)
        /// </summary>
        /// <returns></returns>
        public int GetInt()
        {
            return random.Next();
        }

        public Vector2 GetVec2InsideUnitCircle()
        {
            var r = GetScalar();
            var rad = GetScalar();
            r = Mathf.Sqrt(r);
            rad *= Mathf.PI * 2;
            return new Vector2(r * Mathf.Cos(rad), r * Mathf.Sin(rad));
        }

        public Vector2 GetVec2OnUnitCircle()
        {
            var rad = GetScalar();
            rad *= Mathf.PI * 2;
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        }

        /// <summary>
        /// Random float in [min, max)
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public float GetInRange(float min, float max)
        {
            return GetScalar() * (max - min) + min;
        }

        /// <summary>
        /// Random float in [x, y)
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public float GetInRange(Vector2 range)
            => GetScalar() * (range.y - range.x) + range.x;

        /// <summary>
        /// Random integer in [min, max)
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public int GetInRange(int min, int max)
        {
            return (int) (GetScalar() * (max - min) + min);
        }

        /// <summary>
        /// Random interger in [x, y)
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public int GetInRange(Vector2Int range)
            => GetInRange(range.x, range.y);

        /// <summary>
        /// x,y ∈ (0, 1)
        /// </summary>
        /// <returns></returns>
        public Vector2 GetVec2()
        {
            return new Vector2(GetScalar(), GetScalar());
        }

        public Vector2 GetVec2InBox(Vector2 halfSize)
        {
            return new Vector2(GetInRange(-halfSize.x, halfSize.x), GetInRange(-halfSize.y, halfSize.y));
        }


        public PRNG GetPRNG()
        {
            return new PRNG(GetInt());
        }
        
    }
}