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
        public int Capacity = 16;

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
    }
}