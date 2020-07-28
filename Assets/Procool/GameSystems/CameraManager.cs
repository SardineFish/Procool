using Cinemachine;
using Procool.GamePlay.Controller;
using UnityEngine;

namespace Procool.GameSystems
{
    [RequireComponent(typeof(Camera))]
    public class CameraManager : Singleton<CameraManager>
    {
        public static Camera Camera { get; private set; }
        public CinemachineVirtualCamera playerCamera;
        protected override void Awake()
        {
            base.Awake();

            Camera = GetComponent<Camera>();
        }

        public void Follow(Player player)
        {
            playerCamera.Follow = player.transform;
        }
    }
}