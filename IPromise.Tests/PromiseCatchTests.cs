using System;

using NUnit.Framework;

namespace IPromise.Tests
{
    [TestFixture]
    public class PromiseCatchTests
    {
        Exception error;

        [SetUp]
        public void Setup(){
            error = new Exception("Failed.");
        }

        [Test]
        public void CatchRejectedPromise()
        {
            bool failed = false;
            var promise = new Promise<int>((fulfill, reject) => {
                reject(error);
            });
            var catchPromise = promise.Catch(e => {
                failed = true;
            });
            Assert.That(failed);
            Assert.That(promise.Rejected);
            Assert.That(catchPromise.Rejected);
            Assert.That(catchPromise.Error, Is.EqualTo(error));
            Assert.That(catchPromise.Error, Is.EqualTo(promise.Error));
        }

        [Test]
        public void DoubleCatchRejectedPromise()
        {
            var recaught = false;
            var differentError = new Exception("Different Failure.");
            var promise = new Promise<int>((fulfill, reject) => {
                reject(error);
            });
            var catchPromise = promise.Catch(e => {
                throw differentError;
            });
            var recaughtPromise = catchPromise.Catch(e => {
                recaught = true;
            });
            
            Assert.That(promise.Rejected, Is.True);
            Assert.That(catchPromise.Rejected, Is.True);
            Assert.That(promise.Error, Is.EqualTo(error));
            Assert.That(catchPromise.Error, Is.EqualTo(differentError));
            Assert.That(recaughtPromise.Rejected, Is.True);
            Assert.That(recaughtPromise.Error, Is.EqualTo(differentError));
            Assert.That(recaught, Is.True);
        }

        [Test]
        public void CatchFulfilledPromise()
        {
            var completed = Promise.Of(3);
            bool caught = false;
            var catchPromise = completed.Catch((e) =>
            {
                caught = true;
            });
            
            Assert.That(caught, Is.False);
            Assert.That(catchPromise.Fulfilled);
            Assert.That(catchPromise.Value, Is.EqualTo(3));
        }

        [Test]
        public void AttachCatchOnPendingPromiseLaterRejected()
        {
            bool caught = false;
            var promise = Promise.Pending<int>();
            var catchPromise = promise.Catch(e => {
                caught = true;
            });

            Assert.That(promise.Pending);
            Assert.That(catchPromise.Pending);

            promise.Reject(error);

            Assert.That(caught, Is.True);
            Assert.That(promise.Rejected);
            Assert.That(catchPromise.Rejected, Is.True);
            Assert.That(catchPromise.Error, Is.EqualTo(error));
        }

        [Test]
        public void AttachCatchOnPendingPromiseLaterFulfilled()
        {
            bool caught = false;
            var promise = Promise.Pending<int>();
            var catchPromise = promise.Catch(e => {
                caught = true;
            });

            promise.Fulfill(84);

            Assert.That(caught, Is.False);
            Assert.That(catchPromise.Fulfilled);
            Assert.That(catchPromise.Value, Is.EqualTo(84));
        }
    }
}