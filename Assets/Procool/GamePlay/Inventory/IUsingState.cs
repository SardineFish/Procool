using System;
using System.Collections;

namespace Procool.GamePlay.Inventory
{
    public interface IUsingState : IDisposable
    {
        void Terminate();
        IEnumerator Wait();
        bool Tick();
    }
}