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
        public void CatchCompletedPromise()
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
        public void ReCatchInstantlyFailedPromise()
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
    }
}