using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Procool.UI
{
    public class SelectionItem : MonoBehaviour
    {
        public class SelectEvent : UnityEvent<SelectionPopup.Item>
        {
        }

        public Image Icon;
        public Text Title;

        public SelectionPopup.Item Item;

        public SelectEvent OnSelected;

        private TaskCompletionSource<int> promise;
        private int selectionIndex;
        

        public void Select()
        {
            promise?.TrySetResult(selectionIndex);
        }

        public async Task<int> WaitSelected(SelectionPopup.Item item, int idx)
        {
            Icon.sprite = item.Sprite;
            Title.text = item.Title;
            selectionIndex = idx;
            promise = new TaskCompletionSource<int>();
            return await promise.Task;
        }

        public void Reset()
        {
            promise = null;
            selectionIndex = -1;
        }
    }
}