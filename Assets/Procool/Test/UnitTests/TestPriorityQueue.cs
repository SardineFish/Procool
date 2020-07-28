using System.Collections;
using System.Linq;
using NUnit.Framework;
using Procool.Random;
using Procool.Utils;
using UnityEngine.TestTools;

namespace Procool.Test.UnitTest
{
    public class TestPriorityQueue
    {
        [Test]
        public void TestPriorityQueueSimplePasses()
        {
            var prng = GameRNG.GetPRNG(UnityEngine.Random.insideUnitCircle);
            var priorityQueue = new PriorityQueue<float, float>();
            for (var i = 0; i < 1000; i++)
            {
                var x = prng.GetScalar();
                priorityQueue.Add(x, x);
            }

            while (priorityQueue.Count > 0)
            {
                Assert.AreEqual(priorityQueue.Peak.Key, priorityQueue.Min(p => p.Key));
                priorityQueue.Pop();
            }
        }

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityTest]
        public IEnumerator TestPriorityQueueWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // yield to skip a frame
            yield return null;
        }
    }
}