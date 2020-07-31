using System;
using System.Collections;

namespace Procool.Map
{
    public interface IBlockContentGenerator : IDisposable
    {
        IEnumerator RunProgressive();
        BlockContent GetContent();
    }
}