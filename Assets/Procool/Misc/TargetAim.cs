using System;
using UnityEngine;

namespace Procool.Misc
{
    [ExecuteInEditMode]
    public class TargetAim : MonoBehaviour
    {
        public Transform target;
        public Vector3 forward = Vector3.forward;

        private void Update()
        {
            if (target)
            {
                var dir = this.transform.worldToLocalMatrix.MultiplyVector(
                    (target.transform.position - transform.position)).normalized;

                var rotation = transform.rotation;
                rotation *= Quaternion.FromToRotation(forward, dir);
                transform.rotation *= Quaternion.FromToRotation(forward, dir);
                
            }
        }
    }
}