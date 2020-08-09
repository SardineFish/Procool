using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Procool.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TodoItem : MonoBehaviour
    {
        public float transitionTime;
        public RectTransform wrapper;
        public Image border;
        public Image icon;
        public Text title;
        public Text detail;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public async Task EaseIn()
        {
            var pos = wrapper.anchoredPosition;
            foreach (var t in Utility.TimerNormalized(transitionTime))
            {
                pos.x = Mathf.Lerp(-300, 0, t);
                wrapper.anchoredPosition = pos;
                _canvasGroup.alpha = t;
                await Task.Yield();
            }
        }

        public async Task EaseOut()
        {
            var pos = wrapper.anchoredPosition;
            foreach (var t in Utility.TimerNormalized(transitionTime))
            {
                pos.x = Mathf.Lerp(0, 300, t);
                wrapper.anchoredPosition = pos;
                _canvasGroup.alpha = t;
                await Task.Yield();
            }
        }
    }
}