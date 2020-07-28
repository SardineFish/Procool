using System;
using UnityEngine;

namespace Procool.GamePlay.Inventory
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DropedItem : MonoBehaviour
    {
        public Item Item { get; private set; }
        private new SpriteRenderer renderer;

        private void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
        }

        public void Init(Item item, Vector2 position)
        {
            Item = item;
            renderer.sprite = item.Sprite;
            transform.position = position;
        }
    }
}