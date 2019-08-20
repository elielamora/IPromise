using System;
using System.Threading;

using NUnit.Framework;

namespace IPromise.Tests
{
    [TestFixture]
    public class PromiseQueueTests
    {
        [OneTimeSetUp]
        public void SetupTestFixture()
        {
            ThreadPool.GetMinThreads(
                out int minThreads, 
                out int completionPortThreads);
            ThreadPool.SetMinThreads(
                Math.Max(minThreads, 2),
                completionPortThreads);
        }

        [Test]
        public void ThreadPoolPromiseQueue()
        {
            var promise = new Promise<int>(
                (fulfill, reject) => {
                    fulfill(2);
                },
                onQueue: PromiseQueue.ThreadPool,
                delay: 100
            );

            Thread.Sleep(75);
            Assert.That(promise.Pending);

            Thread.Sleep(75);
            Assert.That(promise.Fulfilled);
        }
    }
}