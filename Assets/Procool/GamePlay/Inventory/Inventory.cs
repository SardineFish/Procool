using System;
using System.Collections.Generic;
using Procool.GamePlay.Controller;
using Procool.GameSystems;
using Procool.Utils;

namespace Procool.GamePlay.Inventory
{
    public class Inventory
    {
        public Player Owner { get; private set; }
        private readonly List<Item> items = new List<Item>();
        public IReadOnlyList<Item> Items => items.AsReadOnly();
        public int Capacity = 32;

        public int ActiveSlot { get; private set; } = 0;
        public Item ActiveItem => items.Count <= ActiveSlot ? null : items[ActiveSlot];

        public bool IsFull => Items.Count >= Capacity;

        public Inventory(Player player)
        {
            Owner = player;
        }

        public void Add(Item item)
        {
            if(item.Owner)
                throw new Exception("Cannot take other player's item.");
            item.Owner = Owner;
            items.Add(item);
            UpdateActiveSlot();
        }

        public Item Take(Item item)
        {
            var idx = items.IndexOf(item);
            if(item.Owner != Owner || idx < 0)
                throw new Exception("Item not belongs to owner.");
            item.Owner = null;
            items.RemoveAt(idx);
            return item;
        }

        public Item Take(int idx)
        {
            var item = items[idx];
            items.RemoveAt(idx);
            item.Owner = null;
            return item;
        }

        public Item GetItem(int idx)
        {
            if (idx >= 0 && idx < items.Count)
                return items[idx];
            return null;
        }

        public DropedItem Drop(Item item)
        {
            var idx = items.IndexOf(item);
            if (item.Owner != Owner || idx < 0)
                throw new Exception("Item not belongs to owner.");

            item.Owner = null;
            items.RemoveAt(idx);

            var dropedItem = GameObjectPool.Get<DropedItem>(PrefabManager.Instance.DropedItemPrefab);
            dropedItem.Init(item, Owner.transform.position);
            return dropedItem;
        }

        void UpdateActiveSlot()
        {
            if (ActiveItem is null)
            {
                for (var i = ActiveSlot + 1; i != ActiveSlot; i = (i + 1) % items.Count)
                {
                    if (items[i] != null)
                    {
                        ActiveSlot = i;
                        break;
                    }
                    
                }
            }
        }

        public Item NextItem()
        {
            for (var i = ActiveSlot + 1; i != ActiveSlot; i = (i + 1) % items.Count)
            {
                if (items[i] != null)
                {
                    ActiveSlot = i;
                    break;
                }

            }

            return ActiveItem;
        }

        public Item PreviousItem()
        {
            for (var i = (ActiveSlot - 1 + items.Count) % items.Count; i != ActiveSlot; i = (i - 1 + items.Count) % items.Count)
            {
                if (items[i] != null)
                {
                    ActiveSlot = i;
                    break;
                }

            }

            return ActiveItem;
        }
    }
}