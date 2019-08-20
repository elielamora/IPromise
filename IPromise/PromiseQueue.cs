using System;

namespace IPromise
{
    public abstract class PromiseQueue
    {
        public static IPromiseQueue ThreadPool { get; } = 
            new ThreadPoolPromiseQueue();
    }
}