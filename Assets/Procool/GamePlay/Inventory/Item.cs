using System.Collections;
using Procool.GamePlay.Controller;
using UnityEngine;

namespace Procool.GamePlay.Inventory
{
    public abstract class Item
    {
        public Player Owner;
        public Sprite Sprite;
        public abstract IEnumerator Activate();

    }
}