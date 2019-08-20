using System;
using System.Threading;

namespace IPromise
{
    public class BlockingPromiseQueue : IPromiseQueue
    {
        public void Run<T>(
            Action<Action<T>, Action<Exception>> promiseCallback,
            Action<T> fulfill, 
            Action<Exception> reject, 
            int delay = 0
        )
        {
            if (0 < delay)
                Thread.Sleep(delay);
            promiseCallback(fulfill, reject);
        }
    }
}