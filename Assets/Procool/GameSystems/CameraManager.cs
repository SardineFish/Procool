using UnityEngine;

namespace Procool.GameSystems
{
    [RequireComponent(typeof(Camera))]
    public class CameraManager : Singleton<CameraManager>
    {
        public static Camera Camera { get; private set; }
        protected override void Awake()
        {
            base.Awake();

            Camera = GetComponent<Camera>();
        }
    }
}