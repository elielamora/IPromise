using System;

namespace IPromise
{
    public static class PromiseQueue
    {
        public static IPromiseQueue ThreadPool { get; } = 
            new ThreadPoolPromiseQueue();
        
        public static IPromiseQueue Blocking { get; } =
            new BlockingPromiseQueue();
    }
}