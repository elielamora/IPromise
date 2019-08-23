using System;

namespace IPromise
{
    public interface IPromiseQueue
    {
        void Run<T>(Action<T> action, T args, int delay = 0);
    }
}