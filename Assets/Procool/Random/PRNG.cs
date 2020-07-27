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
        /// x,y ∈ (0, 1)
        /// </summary>
        /// <returns></returns>
        public Vector2 GetVec2()
        {
            return new Vector2(GetScalar(), GetScalar());
        }
        
    }
}