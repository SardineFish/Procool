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
        /// Random float in (0, 1)
        /// </summary>
        /// <returns></returns>
        public float GetScalar()
        {
            return (float)random.NextDouble();
        }

        public Vector2 GetVec2InUnitCircle()
        {
            var r = GetScalar();
            var rad = GetScalar();
            r = Mathf.Sqrt(r);
            rad *= Mathf.PI * 2;
            return new Vector2(r * Mathf.Cos(rad), r * Mathf.Sin(rad));
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