using System;
using UnityEngine;

namespace Procool.Test
{
    public class TestRaycast : MonoBehaviour
    {
        public Transform target;

        private void Update()
        {
            if (!target)
                return;
            var hit = Physics2D.CircleCast(transform.position, 0.2f, target.transform.position - transform.position);
            Debug.DrawLine(transform.position, hit.point, Color.white);
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.red);
        }
    }
}