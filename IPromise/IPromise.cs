using System;

namespace IPromise
{
    public interface IPromise<T>
    {
        IPromiseQueue Queue { get; }

        bool Pending { get; }

        bool Fulfilled { get; }

        bool Rejected { get; }

        T Value { get; }

        Exception Error { get; }

        void Fulfill(T promised);

        void Reject(Exception e);

        IPromise<U> Then<U>(
            Func<T, U> thenDo,
            IPromiseQueue queue = null);

        IPromise<T> Catch(
            Action<Exception> catchError,
            IPromiseQueue queue = null);
    }
}
