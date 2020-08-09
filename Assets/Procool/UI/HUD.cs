using System.Threading.Tasks;
using Procool.GameSystems;
using UnityEngine;

namespace Procool.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class HUD : Singleton<HUD>
    {
        private CanvasGroup _canvasGroup;
        protected override void Awake()
        {
            base.Awake();

            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public async Task Show(float time = .2f)
        {
            if (gameObject.activeInHierarchy)
                return;
            await Utility.ShowUIAsync(_canvasGroup, time);
        }

        public async Task Hide(float time = .2f)
        {
            if (!gameObject.activeInHierarchy)
                return;
            await Utility.HideUIAsync(_canvasGroup, time, true);
        }
    }
}