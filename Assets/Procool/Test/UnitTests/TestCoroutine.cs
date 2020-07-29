using System.Collections;
using NUnit.Framework;
using Procool.Utils;

namespace Procool.Test.UnitTest
{
    public class TestCoroutine
    {
        [NUnit.Framework.Test]
        public void TestCoroutineSimplePasses()
        {
            // Use the Assert class to test conditions.
            
        }

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityEngine.TestTools.UnityTest]
        public IEnumerator TestCoroutineRunnerStatic()
        {
            bool coroutine1Done = false;
            bool coroutine2Done = false;
            bool coroutine3Done = false;
            bool coroutine4Done = false;
            bool coroutine5Done = false;

            IEnumerator Coroutine1()
            {
                yield return Coroutine2();
                coroutine1Done = true;
            }

            IEnumerator Coroutine2()
            {
                for (var i = 0; i < 10; i++)
                {
                    yield return null;
                }

                coroutine2Done = true;
            }

            IEnumerator Coroutine3()
            {
                for (var i = 0; i < 5; i++)
                {
                    yield return null;
                }

                coroutine3Done = true;
            }

            IEnumerator Coroutine4()
            {
                coroutine4Done = true;
                yield break;
            }

            IEnumerator Coroutine5()
            {
                coroutine5Done = true;
                yield return Coroutine4();
            }

            yield return CoroutineRunner.All(new[] {Coroutine3(), Coroutine2()});

            Assert.IsTrue(coroutine3Done);
            Assert.IsTrue(coroutine2Done);
            coroutine2Done = coroutine3Done = false;

            yield return CoroutineRunner.All(new[] {Coroutine1(), Coroutine3()});
            Assert.IsTrue(coroutine1Done);
            Assert.IsTrue(coroutine2Done);
            Assert.IsTrue(coroutine3Done);

            yield return CoroutineRunner.All(new[] {Coroutine5()});
            Assert.IsTrue(coroutine4Done);
            Assert.IsTrue(coroutine5Done);
            
        }

        [UnityEngine.TestTools.UnityTest]
        public IEnumerator TestCoroutineRunnerInstance()
        {
            bool coroutine1Done = false;
            bool coroutine2Done = false;

            IEnumerator Coroutine1()
            {
                yield return Coroutine2();
                coroutine1Done = true;
            }

            IEnumerator Coroutine2()
            {
                for (var i = 0; i < 10; i++)
                {
                    yield return null;
                }

                coroutine2Done = true;
            }

            var runner = new CoroutineRunner(Coroutine2());
            while (runner.Tick())
                yield return null;
            Assert.IsTrue(coroutine2Done);

            coroutine2Done = false;
            
            runner = new CoroutineRunner(Coroutine1());
            while (runner.Tick())
                yield return null;
            
            Assert.IsTrue(coroutine1Done);
            Assert.IsTrue(coroutine2Done);
        }
    }
}