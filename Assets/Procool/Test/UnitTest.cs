using System;
using System.Linq;
using Procool.Random;
using Procool.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace Procool.Test
{
    public class UnitTest : MonoBehaviour, ICustomEditorEX
    {
        [EditorButton("Run Test")]
        private void Awake()
        {
            RunTest("Priority Queue", TestPriorityQueue);
        }

        public void RunTest(string name, Action TestFunc)
        {
            try
            {
                TestFunc?.Invoke();
                Debug.Log($"Test '{name}' passed.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Test '{name}' failed: {ex.Message}");
                Debug.LogException(ex);
            }
        }

        public void TestPriorityQueue()
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

            //Assert.AreEqual(priorityQueue.Peak.Key, priorityQueue.Min(p => p.Key));
        }
    }
}