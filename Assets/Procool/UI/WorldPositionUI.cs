﻿using System;
using Procool.GameSystems;
using Procool.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Procool.UI
{
    public class WorldPositionUI : MonoBehaviour
    {
        public GameObject prefab;
        public Vector2 worldSpaceOffset;
        private RectTransform _instanceTransform;
        public bool showOnLoad = false;

        private void Start()
        {
        }

        private void OnEnable()
        {
            if (showOnLoad)
                Show();
        }

        private void OnDisable()
        {
            if (showOnLoad)
                Hide();
        }

        public void Show()
        {
            Show<RectTransform>();
        }

        public T Show<T>() where  T : Component
        {
            if (_instanceTransform)
                return _instanceTransform.GetComponent<T>();
            var component = GameObjectPool.Get<T>(prefab);
            _instanceTransform = component.transform as RectTransform;
            GameUIManager.Instance.AddHUDUI(component.gameObject);
            return component;
        }

        public void Hide()
        {
            if(!_instanceTransform)
                return;
            
            GameObjectPool.Release(prefab, _instanceTransform.gameObject);
            _instanceTransform = null;
        }

        private void LateUpdate()
        {
            if(!_instanceTransform)
                return;
            
            var screenPos = CameraManager.Camera.WorldToScreenPoint(transform.localToWorldMatrix.MultiplyPoint(worldSpaceOffset.ToVector3()));
            _instanceTransform.position = screenPos;
        }
    }
}