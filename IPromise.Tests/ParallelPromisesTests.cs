using System;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

namespace IPromise.Tests
{
    [TestFixture]
    public class ParallelPromisesTests 
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
        public void TestPromisesInParallel()
        {
            var firstPromise = new Promise<int>((fulfill, reject) => {
                ThreadPool.QueueUserWorkItem((state) => {
                    Thread.Sleep(100);
                    fulfill(1);
                });
            });

            var secondPromise = new Promise<int>((fulfill, reject) => {
                ThreadPool.QueueUserWorkItem((state) => {
                    Thread.Sleep(100);
                    fulfill(1);
                });
            });

            Thread.Sleep(150);
            Assert.That(firstPromise.Fulfilled);
            Assert.That(secondPromise.Fulfilled);
        }
    }
}