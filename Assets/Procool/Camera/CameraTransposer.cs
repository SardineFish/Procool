using System;
using Procool.Utils;
using UnityEngine;

namespace Procool.Cinematic
{
    [RequireComponent(typeof(Camera))]
    public class CameraTransposer : MonoBehaviour, ICustomEditorEX
    {
        [Range(0, 5)]
        public float moveXDamping = 0;

        [Range(0, 5)]
        public float moveYDamping = 0;

        [Range(0, 1)]
        public float moveZDamping = 0;

        [Range(0, 1)]
        public float rollDamping = 0;

        [Range(0, 1)]
        public float scaleDamping = 0;

        public float projectionTransitionTime = 1;

        private new Camera camera;
        
        private Transform movingFollow;
        private Transform rollingFollow;
        [SerializeField] private float targetZPos;
        private float targetSize;

        private bool orthographic;
        private float targetFOV;

        [DisplayInInspector]
        private Vector2 velocity;

        private float velocityZ;
        
        private void Awake()
        {
            camera = GetComponent<Camera>();
        }


        private void LateUpdate()
        {
            SmoothMovingXY();
            // SmoothMovingZ();
        }

        public void Follow(Transform transform)
        {
            movingFollow = transform;
        }

        private void SmoothMovingXY()
        {
            // var targetPos = movingFollow ? movingFollow.position.ToVector2() : transform.position.ToVector2();
            // var pos = transform.position.ToVector2();
            // // pos.x = Mathf.Lerp(pos.x, targetPos.x, (1 - moveXDamping));
            // // pos.y = Mathf.Lerp(pos.y, targetPos.y, (1 - moveYDamping));
            //
            // pos.x = Mathf.SmoothDamp(pos.x, targetPos.x, ref velocity.x, moveXDamping);
            // pos.y = Mathf.SmoothDamp(pos.y, targetPos.y, ref velocity.y, moveYDamping);
            var targetPos = movingFollow
                ? movingFollow.position.ToVector2().ToVector3(targetZPos)
                : Vector2.zero.ToVector3(targetZPos);

            var pos = Cinemachine.Utility.Damper.Damp(targetPos, new Vector3(moveXDamping, moveYDamping, moveZDamping),
                Time.deltaTime);

            transform.position = pos;
        }

        private void SmoothMovingZ()
        {
            var z = Mathf.SmoothDamp(transform.position.z, targetZPos, ref velocityZ, moveZDamping);
            transform.position = transform.position.ToVector2().ToVector3(z);
        }
    }
}