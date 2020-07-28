using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Procool.Utils
{
    public static class CoroutineRunner
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

        public static IEnumerator RunProgressive(IEnumerator coroutine)
        {
            Stack<IEnumerator> runStack = new Stack<IEnumerator>();
            runStack.Push(coroutine);
            while (runStack.Count > 0)
            {
                var iterator = runStack.Pop();
                for (var state = iterator.MoveNext(); state; state = iterator.MoveNext())
                {
                    if (iterator.Current is null)
                    {
                        yield return null;
                    }
                    else if (iterator.Current is IEnumerator next)
                    {
                        runStack.Push(iterator);
                        runStack.Push(next);
                        break;
                    }
                }
            }
        }
        
        public static IEnumerator All(IEnumerable<IEnumerator> coroutines)    
        {
            
            var list = ListPool<IEnumerator>.Get();
            list.Clear();
            list.AddRange(coroutines.Select(RunProgressive));

            bool keepRunning = true;
            while (keepRunning)
            {
                keepRunning = false;
                foreach (var coroutine in list)
                {
                    keepRunning |= coroutine.MoveNext();
                }

                if (!keepRunning)
                    break;
                yield return null;
            }
            
            ListPool<IEnumerator>.Release(list);
        }
    }
}