using System;

namespace Procool.Utils
{
    public static class UniqueID32
    {
        private static UInt32 nextId = 0;
        public static UInt32 Get() => ++nextId;
    }
}