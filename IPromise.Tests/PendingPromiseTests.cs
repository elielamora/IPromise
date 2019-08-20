using System;
using NUnit.Framework;

namespace IPromise.Tests
{
    public class PendingPromiseTests
    {
        private IPromise<int> promise;
        [SetUp]
        public void Setup(){
            promise = PromiseFactory.CreatePending<int>();
        }

        [Test]
        public void CreateNewPromise()
        {
            Assert.That(promise.Pending);
            Assert.Throws<PendingPromiseException>(
                () => { _ = promise.Value; }
            );
            Assert.That(promise.Error, Is.Null);
        }

        [Test]
        public void FullfilPromise(){
            promise.Fulfill(8);
            Assert.That(promise.Pending, Is.False);
            Assert.That(promise.Fulfilled, Is.True);
            Assert.That(promise.Rejected, Is.False);
            Assert.That(promise.Value, Is.EqualTo(8));
            Assert.That(promise.Error, Is.Null);
        }

        [Test]
        public void RejectedPromise(){
            promise.Reject(new Exception());
            Assert.That(promise.Pending, Is.False);
            Assert.That(promise.Fulfilled, Is.False);
            Assert.That(promise.Rejected, Is.True);
            Assert.Throws<RejectedPromiseException>(
                () => { _ = promise.Value; }
            );
            Assert.That(promise.Error, Is.Not.Null);
            Assert.That(promise.Error, Is.TypeOf<Exception>());
        }

        [Test]
        public void FulfilledOnlyOnce(){
            promise.Fulfill(3);
            promise.Fulfill(4);

            Assert.That(promise.Value, Is.EqualTo(3));
        }

        [Test]
        public void RejectedOnlyOnce(){
            promise.Reject(new Exception("Reason 1."));
            promise.Reject(new Exception("Reason 2."));
            
            Assert.That(promise.Error, 
                Has.Property("Message").EqualTo("Reason 1."));
        }

        [Test]
        public void CannotRejectAfterFulfilling(){
            promise.Fulfill(10);
            promise.Reject(new Exception("Reason."));

            Assert.That(promise.Pending, Is.False);
            Assert.That(promise.Fulfilled, Is.True);
            Assert.That(promise.Rejected, Is.False);
            Assert.That(promise.Value, Is.EqualTo(10));
            Assert.That(promise.Error, Is.Null);
        }

        [Test]
        public void CannotFulfillAfterRejection(){
            promise.Reject(new Exception("Reason."));
            promise.Fulfill(10);

            Assert.That(promise.Pending, Is.False);
            Assert.That(promise.Fulfilled, Is.False);
            Assert.That(promise.Rejected, Is.True);
            Assert.Throws<RejectedPromiseException>(
                () => { _ = promise.Value; }
            );
            Assert.That(promise.Error, Has.Property("Message")
                .EqualTo("Reason."));
        }
    }
}