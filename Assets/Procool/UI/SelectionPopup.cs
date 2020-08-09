using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Procool.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Procool.UI
{
    public class SelectionPopup : PopupUISingleton<SelectionPopup>
    {
        public struct Item
        {
            public Sprite Sprite;
            public string Title;
        }

        public GameObject SelectionItemPrefab;
        public RectTransform ItemsContainer;

        private List<Item> items;

        public async Task<int> Popup(List<Item> items)
        {
            List<SelectionItem> selectionItems = items
                .Select(item => GameObjectPool.Get<SelectionItem>(SelectionItemPrefab))
                .ToList();

            selectionItems.ForEach(item => item.transform.SetParent(ItemsContainer));
            
            Show();
            
            var task = await Task<int>.WhenAny(selectionItems.Select(async (selectionItem, idx) =>
            {
                return await selectionItem.WaitSelected(items[idx], idx);
            }));
            
            await Hide();
            
            foreach (var selectionItem in selectionItems)
            {
                selectionItem.Reset();
                GameObjectPool.Release(SelectionItemPrefab, selectionItem);
            }
            selectionItems.Clear();


            return task.Result;
        }
        
    }
}