using System;
using System.Threading;
using System.Threading.Tasks;

namespace IPromise {

    public sealed class Promise<T> : IPromise<T>
    {
        internal event EventHandler<CompletedEventArgs<T>> Completed;

        /// <summary>
        /// Creates a new pending promise.
        /// </summary>
        internal Promise() {
        }

        internal Promise(T value){
            Fulfill(value);
        }

        public Promise(
            Action<Action<T>, Action<Exception>> promise,
            IPromiseQueue onQueue = null,
            int delay = 0
        )
        {
            var queue = onQueue ?? PromiseQueue.Blocking;
            queue.Run(
                promise,
                Fulfill,
                Reject,
                delay
            );
        }

        public bool Pending { get; private set; } = true;

        private T value;

        public T Value {
            get {
                if (Pending)
                    throw new PendingPromiseException();
                else if (error != null)
                    throw new RejectedPromiseException();
                else 
                    return value;
            }
            set 
            {
                if (Pending){
                    this.value = value;
                    Pending = false;
                }
            }
        }

        private Exception error;

        public Exception Error {
            get => error;
            set 
            {
                if (Pending){
                    error = value;
                    Pending = false;
                }
            }
        }

        public bool Fulfilled => !Pending && Error == null;

        public bool Rejected => !Pending && Error != null;

        public void Fulfill(T promised){
            Value = promised;
            Completed?.Invoke(this, new CompletedEventArgs<T>());
        }

        public void Reject(Exception e) {
            Error = e;
            Completed?.Invoke(this, new CompletedEventArgs<T>());
        }

        public IPromise<U> Then<U>(Func<T, U> thenDo)
        {
            var thenPromise = new Promise<U>();
            if(Pending)
                Completed += RunOnCompletion(thenPromise, thenDo);
            else if(Rejected)
                thenPromise.Reject(Error);
            else
                TryComplete(thenPromise, thenDo, Value);
            return thenPromise;
        }

        internal EventHandler<CompletedEventArgs<T>> RunOnCompletion<U>(
            IPromise<U> thenPromise, Func<T, U> thenDo)
        {
            void HandleCompletedPromise(
                object sender, CompletedEventArgs<T> args)
            {
                var completedProimse = (Promise<T>)sender;
                if (completedProimse.Fulfilled)
                    TryComplete(thenPromise, thenDo, completedProimse.Value);
                else 
                    thenPromise.Reject(completedProimse.Error);
            }
            return HandleCompletedPromise;
        }

        public void TryComplete<U>(
            IPromise<U> promise, 
            Func<T, U> map, 
            T value)
        {
            try {
                promise.Fulfill(map(value));
            } catch (Exception e) {
                promise.Reject(e);
            }
        }

        public IPromise<T> Catch(Action<Exception> catchError)
        {
            var catchPromise = new Promise<T>();
            if (Pending)
                Completed += RunOnCompletion(catchPromise, catchError);
            else if (Fulfilled)
                catchPromise.Fulfill(Value);
            else
            {
                TryCatchRejection(catchPromise, catchError);
            }
            return catchPromise;
        }

        internal EventHandler<CompletedEventArgs<T>> RunOnCompletion(
            IPromise<T> catchPromise,
            Action<Exception> catchError)
        {
            return (object sender, CompletedEventArgs<T> eventArgs) =>
            {
                var completedPromise = (Promise<T>)sender;
                if(completedPromise.Rejected)
                    TryCatchRejection(catchPromise, catchError);
                else
                    catchPromise.Fulfill(completedPromise.Value);
            };
        }

        private void TryCatchRejection(
            IPromise<T> promise,
            Action<Exception> catchError) 
        {
            try
            {
                catchError(this.Error);
                promise.Reject(this.error);
            }
            catch (Exception exception)
            {
                promise.Reject(exception);
            }
        }
    }


    public static class Promise {
        public static Promise<T> Pending<T>() =>
            new Promise<T>();

        public static Promise<T> Of<T>(T value) =>
            new Promise<T>(value);
    }
}