using System;

namespace IPromise {
    public static class PromiseFactory {
        public static Promise<T> CreatePending<T>() => 
            new Promise<T>();
    }
}