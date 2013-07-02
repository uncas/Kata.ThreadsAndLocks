using System;
using System.Threading;
using NUnit.Framework;

namespace Kata.ThreadsAndLocks
{
    /// <summary>
    ///     Testing double-checked lock.
    ///     See http://blog.decarufel.net/2009/05/how-to-test-your-multi-threaded-code_11.html
    ///     and http://stackoverflow.com/a/9468889/41094
    /// </summary>
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Get_WhenTwoThreadsCompete_OnlyOneHeavyWeight()
        {
            int numberOfHeavyWeightCalls = 0;
            string cache = null;
            const int numberOfThreads = 2;
            int numberOfWaitingThreads = 0;
            var sut = new DoubleCheckLockedGetter<string>();
            Func<string> cachedGetter = () =>
                {
                    string cachedResult = cache;
                    numberOfWaitingThreads++;
                    while (numberOfWaitingThreads < numberOfThreads)
                        Thread.Sleep(1);
                    return cachedResult;
                };
            Func<string> heavyWeightGetter = () =>
                {
                    numberOfHeavyWeightCalls++;
                    cache = "bla";
                    return cache;
                };
            var thread =
                new Thread(
                    a =>
                    ((DoubleCheckLockedGetter<string>) a).Get(cachedGetter,
                                                              heavyWeightGetter));
            thread.Start(sut);
            sut.Get(cachedGetter, heavyWeightGetter);
            thread.Join();

            Assert.AreEqual(1, numberOfHeavyWeightCalls);
        }
    }
}