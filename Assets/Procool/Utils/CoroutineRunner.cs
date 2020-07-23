using System.Collections;
using System.Collections.Generic;

namespace Procool.Utils
{
    public class CoroutineRunner
    {
        public static void Run(IEnumerator coroutine)
        {
            Stack<IEnumerator> runStack = new Stack<IEnumerator>();
            runStack.Push(coroutine);
            while (runStack.Count > 0)
            {
                var iterator = runStack.Pop();
                for (var state = iterator.MoveNext(); state; state = iterator.MoveNext())
                {
                    if(iterator.Current is null)
                        continue;
                    else if (iterator.Current is IEnumerator next)
                    {
                        runStack.Push(iterator);
                        runStack.Push(next);
                        break;
                    }
                }
            }
        }
    }
}