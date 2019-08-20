using System;

namespace IPromise
{
    public interface IPromiseQueue
    {
        void Run<T>(
            Action<Action<T>, Action<Exception>> promiseCallback,
            Action<T> fulfill,
            Action<Exception> reject,
            int delay = 0
        );
    }
}