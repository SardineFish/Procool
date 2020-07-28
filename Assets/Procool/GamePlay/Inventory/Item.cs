using System.Collections;
using Procool.GamePlay.Controller;

namespace Procool.GamePlay.Inventory
{
    public abstract class Item
    {
        public Player Owner;
        public abstract IEnumerator Activate();
    }
}