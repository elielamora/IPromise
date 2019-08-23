using System;
using System.Threading;

namespace IPromise
{
    internal class ThreadPoolPromiseQueue : IPromiseQueue
    {
        void IPromiseQueue.Run<T>(
            Action<T> action,
            T args,
            int delay)
        {
            ThreadPool.QueueUserWorkItem((state) => {
                if (0 < delay)
                    Thread.Sleep(delay);
                action(args);
            });
        }
    }
}