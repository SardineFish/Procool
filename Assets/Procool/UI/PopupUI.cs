using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Procool.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PopupUI : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            // gameObject.SetActive(false);
        }

        public virtual async Task Show(float time = .2f)
        {
            await Utility.ShowUIAsync(_canvasGroup, time);
        }

        public virtual async Task Hide(float time = .2f)
        {
            await Utility.HideUIAsync(_canvasGroup, time, true);
        }
    }

    public class PopupUISingleton<T> : PopupUI where T : PopupUISingleton<T>
    {
        public static T Instance { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Instance = this as T;
        }
    }
    
}