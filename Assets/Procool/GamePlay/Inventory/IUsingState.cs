using System.Collections;

namespace Procool.GamePlay.Inventory
{
    public interface IUsingState
    {
        void Terminate();
        IEnumerator Wait();
        bool Tick();
    }
}