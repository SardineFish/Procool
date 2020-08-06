using System;
using Procool.GameSystems;
using UnityEngine;
using UnityEngine.Serialization;

namespace Procool.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class WorldPositionUI : MonoBehaviour
    {
        public Transform positionTarget;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            if(!positionTarget)
                return;

            var screenPos = CameraManager.Camera.WorldToScreenPoint(positionTarget.transform.position);
            _rectTransform.position = screenPos;
        }
    }
}