﻿using System.Collections;

namespace Procool.GamePlay.Inventory
{
    public class FailedUsing : IUsingState
    {
        public static FailedUsing Instance { get; } = new FailedUsing();
        public void Terminate()
        {
        }

        public IEnumerator Wait()
        {
            yield break;
        }
    }
}