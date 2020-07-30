using System;
using System.Collections;
using Cinemachine;
using Procool.GamePlay.Controller;
using UnityEngine;

namespace Procool.GameSystems
{
    public class CameraManager : Singleton<CameraManager>
    {
        public enum CameraState
        {
            Player,
            Transition,
            Map,
        }
        
        public static Camera Camera { get; private set; }
        public Camera mapCamera;
        public Camera gameCamera;
        public CinemachineVirtualCamera playerVirtualCamera;
        public CinemachineVirtualCamera mapTransitionVirtualCamera;
        public CinemachineVirtualCamera mapVirtualCamera;
        public float cameraTransitionTime = 1f;
        public CameraState State = CameraState.Player;

        public AnimationCurve gameCameraZoomSpeed = new AnimationCurve(new Keyframe(0, 10), new Keyframe(700, 50));
        public float mapViewDistanceThreshold = 500;
        public float gameViewHalfSizeThreshold = 260;
        
        protected override void Awake()
        {
            base.Awake();

            Camera = gameCamera;
            mapCamera.gameObject.SetActive(false);
            gameCamera.gameObject.SetActive(true);
        }

        public void Follow(Player player)
        {
            playerVirtualCamera.Follow = player.transform;
        }

        public void Zoom(float speedMultiplier)
        {
            if (State == CameraState.Player)
            {
                var transposer = playerVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                var speed = gameCameraZoomSpeed.Evaluate(transposer.m_CameraDistance);
                speed *= speedMultiplier;
                transposer.m_CameraDistance += speed;
                if (transposer.m_CameraDistance >= mapViewDistanceThreshold)
                {
                    transposer.m_CameraDistance = mapViewDistanceThreshold;
                    SwitchToMapCamera();
                }
            }
            else if (State == CameraState.Map)
            {
                var scaleRate = speedMultiplier > 0
                    ? 1 + (speedMultiplier)
                    : 1 / (1 - speedMultiplier);
                mapVirtualCamera.m_Lens.OrthographicSize *= scaleRate;
                if (mapVirtualCamera.m_Lens.OrthographicSize <= gameViewHalfSizeThreshold)
                {
                    mapTransitionVirtualCamera.m_Lens.OrthographicSize = gameViewHalfSizeThreshold;
                    SwitchToPlayerCamera();
                    
                }
            }
        }
        

        public void SwitchToMapCamera()
        {
            if (State != CameraState.Player)
                return;
            State = CameraState.Transition;
            StartCoroutine(SwitchToMapCameraCoroutine());
        }

        IEnumerator SwitchToMapCameraCoroutine()
        {
            mapTransitionVirtualCamera.transform.position = playerVirtualCamera.transform.position;
            mapTransitionVirtualCamera.m_Lens = playerVirtualCamera.m_Lens;

            mapTransitionVirtualCamera.enabled = true;
            playerVirtualCamera.enabled = false;
            
            var halfHeight = Mathf.Abs(playerVirtualCamera.m_Lens.FarClipPlane) * 
                             Mathf.Tan(Mathf.Deg2Rad * playerVirtualCamera.m_Lens.FieldOfView / 2);

            var farZ = playerVirtualCamera.m_Lens.FarClipPlane + playerVirtualCamera.transform.position.z;
            var startPos = playerVirtualCamera.transform.position + playerVirtualCamera.transform.forward * playerVirtualCamera.m_Lens.FarClipPlane;
            var zDir = playerVirtualCamera.transform.forward;
            
            foreach (var t in Utility.TimerNormalized(cameraTransitionTime))
            {
                var smoothT = Mathf.Pow(t, 1f);
                var fov = Mathf.Lerp(playerVirtualCamera.m_Lens.FieldOfView, 1, smoothT);
                var tan = Mathf.Tan(Mathf.Deg2Rad * fov / 2);
                var far = halfHeight / tan;
                mapTransitionVirtualCamera.transform.position = mapTransitionVirtualCamera.transform.position.Set(z:farZ - far);
                mapTransitionVirtualCamera.m_Lens.FieldOfView = fov;
                mapTransitionVirtualCamera.m_Lens.FarClipPlane = far;
                yield return null;
            }

            mapCamera.orthographic = true;
            mapCamera.transform.position = mapTransitionVirtualCamera.transform.position;
            mapCamera.orthographicSize = halfHeight;
            mapVirtualCamera.transform.position = mapTransitionVirtualCamera.transform.position.Set(z: -200);
            mapVirtualCamera.m_Lens.OrthographicSize = halfHeight;

            mapCamera.gameObject.SetActive(true);
            gameCamera.gameObject.SetActive(false);
            mapTransitionVirtualCamera.enabled = false;
            mapVirtualCamera.enabled = true;

            State = CameraState.Map;
        }

        public void SwitchToPlayerCamera()
        {
            if (State != CameraState.Map)
                return;
            State = CameraState.Transition;
            StartCoroutine(SwitchToPlayerCameraCoroutine());
        }

        IEnumerator SwitchToPlayerCameraCoroutine()
        {
            mapTransitionVirtualCamera.m_Lens.FieldOfView = 1;
            var startSize = mapVirtualCamera.m_Lens.OrthographicSize;
            var targetFOV = playerVirtualCamera.m_Lens.FieldOfView;
            var startDistance =
                startSize / Mathf.Tan(Mathf.Deg2Rad * mapTransitionVirtualCamera.m_Lens.FieldOfView / 2);

            var farZ = startSize / Mathf.Tan(Mathf.Deg2Rad * playerVirtualCamera.m_Lens.FieldOfView / 2) -
                       Mathf.Abs(playerVirtualCamera.transform.position.z);

            mapTransitionVirtualCamera.transform.position = new Vector3(mapVirtualCamera.transform.position.x,
                mapVirtualCamera.transform.position.y, -startDistance);
            
            mapTransitionVirtualCamera.enabled = true;
            mapVirtualCamera.enabled = false;
            mapCamera.gameObject.SetActive(false);
            gameCamera.gameObject.SetActive(true);
            
            foreach (var t in Utility.TimerNormalized(cameraTransitionTime))
            {
                var fov = Mathf.Lerp(1, targetFOV, t);
                var far = startSize / Mathf.Tan(Mathf.Deg2Rad * fov / 2);
                
                mapTransitionVirtualCamera.m_Lens.FieldOfView = fov;
                mapTransitionVirtualCamera.m_Lens.FarClipPlane = far;

                var xyPos = Vector2.Lerp(mapTransitionVirtualCamera.transform.position.ToVector2(),
                    playerVirtualCamera.transform.position.ToVector2(), .1f);
                mapTransitionVirtualCamera.transform.position = xyPos.ToVector3(farZ - far);
                

                yield return null;
            }

            playerVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance =
                Mathf.Abs(playerVirtualCamera.transform.position.z);
            mapTransitionVirtualCamera.enabled = false;
            playerVirtualCamera.enabled = true;
            State = CameraState.Player;

        }
        
        
    }
}