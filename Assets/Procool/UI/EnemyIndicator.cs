using System;
using System.Collections.Generic;
using Procool.GamePlay.Controller;
using Procool.GameSystems;
using Procool.Utils;
using UnityEngine;

namespace Procool.UI
{
    public class EnemyIndicator : Singleton<EnemyIndicator>
    {
        public GameObject IndicatorPrefab;
        private readonly List<GameObject> indicators = new List<GameObject>(16);

        private void Update()
        {
            var i = 0;
            foreach (var enemy in Enemy.AssetsManager.Assets)
            {
                if (enemy && enemy.isActiveAndEnabled)
                {
                    if (i >= indicators.Count)
                    {
                        var indicator = GameObjectPool.Get(IndicatorPrefab);
                        indicator.transform.SetParent(transform);
                        indicators.Add(indicator);
                    }
                    UpdateIndicator(indicators[i], enemy.Player);
                    i++;
                }
            }

            for (var j = i; j < indicators.Count; j++)
            {
                GameObjectPool.Release(IndicatorPrefab, indicators[j]);
            }
            indicators.RemoveRange(i, indicators.Count - i);
            
        }

        void UpdateIndicator(GameObject indicator, Player target)
        {
            var pos = CameraManager.Camera.WorldToViewportPoint(target.transform.position);
            var dir = (new Vector2(.5f, .5f) - pos.ToVector2()).normalized;
            var (hit, distance, normal) =
                MathUtility.BoxRaycast(new Rect(0, 0, 1, 1), pos, dir);
            var point = pos.ToVector2() + dir * distance;
            var screenPoint = CameraManager.Camera.ViewportToScreenPoint(point);
            var transform = indicator.transform as RectTransform;
            // screenPoint.y = -screenPoint.y;
            screenPoint.z = 0;
            transform.localPosition = Vector3.zero;
            transform.anchoredPosition = screenPoint;
            indicator.transform.rotation = Quaternion.FromToRotation(Vector3.up,
                (target.transform.position - CameraManager.Camera.transform.position).ToVector2());
            indicator.transform.localScale = Vector3.one;
        }
    }
}