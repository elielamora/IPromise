using System;
using System.Threading;

namespace IPromise
{
    public class BlockingPromiseQueue : IPromiseQueue
    {
        void IPromiseQueue.Run<T>(
            Action<T> action,
            T args,
            int delay
        )
        {
            if (0 < delay)
                Thread.Sleep(delay);
            action(args);
        }
    }
}