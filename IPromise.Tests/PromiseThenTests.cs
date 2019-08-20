using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace IPromise.Tests {
    [TestFixture]
    public class PromiseThenTests 
    {
        Func<int, int> addOne = x => x + 1;

        [Test]
        public void ThenOnCompletedPromise(){
            var promise = Promise.Of(1);

            Assert.That(promise.Value, Is.EqualTo(1));
            Assert.That(promise.Fulfilled, Is.True);

            var addThree = promise
                .Then(addOne)
                .Then(addOne)
                .Then(addOne);

            Assert.That(promise, Is.Not.EqualTo(addThree));
            Assert.That(addThree.Fulfilled, Is.True);
            Assert.That(addThree.Value, Is.EqualTo(4));
        }

        [Test]
        public void ThenOnPendingPromise(){
            var promise = PromiseFactory.CreatePending<int>();

            var thenAddOne = promise.Then(addOne);
            Assert.That(thenAddOne.Pending);

            promise.Fulfill(1);

            Assert.That(thenAddOne.Pending, Is.False);
            Assert.That(thenAddOne.Fulfilled, Is.True);
            Assert.That(thenAddOne.Rejected, Is.False);
            Assert.That(thenAddOne.Value, Is.EqualTo(2));
        }

        [Test]
        public void ThenSkippedOnRejection()
        {
            var i = 0;
            Func<object, object> identityAndCount = 
                obj => { i++; return obj; };
            var promise = new Promise<object>((_, reject) =>
            {
                reject(new Exception());
            });
            var thenCountThrice = promise
                .Then(identityAndCount)
                .Then(identityAndCount)
                .Then(identityAndCount);
            
            Assert.That(promise.Rejected, Is.True);
            Assert.That(promise.Error, Is.Not.Null);
            Assert.That(thenCountThrice.Rejected);
            Assert.That(thenCountThrice.Error, Is.Not.Null);
            Assert.That(i, Is.EqualTo(0));
        }

        [Test]
        public void ThenSkippedOnRejectionAfterAttaching()
        {
            var i = 0;
            Func<object, object> identityAndCount = 
                obj => { i++; return obj; };
            var promise = Promise.Pending<object>();
            var thenCountThrice = promise
                .Then(identityAndCount)
                .Then(identityAndCount)
                .Then(identityAndCount);
            
            promise.Reject(new Exception());

            Assert.That(promise.Rejected, Is.True);
            Assert.That(promise.Error, Is.Not.Null);
            Assert.That(thenCountThrice.Rejected);
            Assert.That(thenCountThrice.Error, Is.Not.Null);
            Assert.That(i, Is.EqualTo(0));
        }

        [Test]
        public void ThenSkippedOnRejectionAsync()
        {
            var i = 0;
            Func<object, object> identityAndCount = 
                obj => { i++; return obj; };
            var promise = new Promise<object>((_, reject) =>
            {
                Task.Run(() => {
                    Task.Delay(100);
                    reject(new Exception());
                });
            });
            var thenCountThrice = promise
                .Then(identityAndCount)
                .Then(identityAndCount)
                .Then(identityAndCount);

            Thread.Sleep(200);

            Assert.That(promise.Rejected, Is.True);
            Assert.That(promise.Error, Is.Not.Null);
            Assert.That(thenCountThrice.Rejected);
            Assert.That(thenCountThrice.Error, Is.Not.Null);
            Assert.That(i, Is.EqualTo(0));
        }
    }
}
