using System;
using Cinemachine;
using Procool.GameSystems;
using Procool.Input;
using UnityEngine;

namespace Procool.GamePlay.Controller
{
    public class MapViewController : MonoBehaviour
    {
        public float moveSpeed;
        public float keyboardZoomRate = 0.2f;
        public float gamepadZoomRate = 0.1f;

        private CinemachineVirtualCamera virtualCamera;

        private GameInput Input;

        private void Awake()
        {
            Input = new GameInput();
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }

        private void OnEnable()
        {
            Input.Enable();
        }

        private void OnDisable()
        {
            Input.Disable();
        }

        private void Update()
        {
            if(CameraManager.Instance.State != CameraManager.CameraState.Map)
                return;
            
            var zoom = Input.GamePlay.Zoom.ReadValue<float>();
            
            if (Mathf.Abs(zoom) > 0.01f)
            {
                switch (InputManager.CurrentInputScheme)
                {
                    case InputSchemeType.Keyboard:
                        CameraManager.Instance.Zoom(-zoom * keyboardZoomRate);
                        break;
                    case InputSchemeType.GamePad:
                        CameraManager.Instance.Zoom(-zoom * gamepadZoomRate * Time.deltaTime);
                        break;
                }
            }

            var movement = Input.GamePlay.Move.ReadValue<Vector2>();
            movement = Vector2.ClampMagnitude(movement, 1);
            movement = CameraManager.Camera.transform.localToWorldMatrix.MultiplyVector(movement);
            transform.Translate(movement * moveSpeed * virtualCamera.m_Lens.OrthographicSize * Time.deltaTime, Space.World);
        }
    }
}