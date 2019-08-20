using System;
using System.Threading;

namespace IPromise
{
    internal class ThreadPoolPromiseQueue : IPromiseQueue
    {
        void IPromiseQueue.Run<T>(
            Action<Action<T>, Action<Exception>> promiseCallback,
            Action<T> fulfill,
            Action<Exception> reject,
            int delay)
        {
            ThreadPool.QueueUserWorkItem((state) => {
                if (0 < delay)
                    Thread.Sleep(delay);
                promiseCallback(fulfill, reject);
            });
        }
    }
}