using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Procool.UI
{
    [RequireComponent(typeof(Button))]
    public class SelectionItem : MonoBehaviour
    {

        public Image Icon;
        public Text Title;

        public SelectionPopup.Item Item;

        private TaskCompletionSource<int> promise;
        private int selectionIndex;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }


        public void Select()
        {
            promise?.TrySetResult(selectionIndex);
        }

        public async Task<int> WaitSelected(SelectionPopup.Item item, int idx)
        {
            Item = item;
            Icon.sprite = item.Sprite;
            Title.text = item.Title;
            selectionIndex = idx;
            return await button.WaitForClick(idx);
        }

        public void Reset()
        {
            promise = null;
            selectionIndex = -1;
        }
    }
}