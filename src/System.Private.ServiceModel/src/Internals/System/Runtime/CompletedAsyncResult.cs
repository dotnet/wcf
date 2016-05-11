// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Runtime
{
    //An AsyncResult that completes as soon as it is instantiated.
    internal class CompletedAsyncResult : AsyncResult
    {
        public CompletedAsyncResult(AsyncCallback callback, object state)
            : base(callback, state)
        {
            Complete(true);
        }

        [Fx.Tag.GuaranteeNonBlocking]
        public static void End(IAsyncResult result)
        {
            Fx.AssertAndThrowFatal(result.IsCompleted, "CompletedAsyncResult was not completed!");
            AsyncResult.End<CompletedAsyncResult>(result);
        }
    }

    internal class CompletedAsyncResult<T> : AsyncResult
    {
        private T _data;

        public CompletedAsyncResult(T data, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _data = data;
            Complete(true);
        }

        [Fx.Tag.GuaranteeNonBlocking]
        public static T End(IAsyncResult result)
        {
            Fx.AssertAndThrowFatal(result.IsCompleted, "CompletedAsyncResult<T> was not completed!");
            CompletedAsyncResult<T> completedResult = AsyncResult.End<CompletedAsyncResult<T>>(result);
            return completedResult._data;
        }
    }

    internal class CompletedAsyncResult<TResult, TParameter> : AsyncResult
    {
        private TResult _resultData;
        private TParameter _parameter;

        public CompletedAsyncResult(TResult resultData, TParameter parameter, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _resultData = resultData;
            _parameter = parameter;
            Complete(true);
        }

        [Fx.Tag.GuaranteeNonBlocking]
        public static TResult End(IAsyncResult result, out TParameter parameter)
        {
            Fx.AssertAndThrowFatal(result.IsCompleted, "CompletedAsyncResult<T> was not completed!");
            CompletedAsyncResult<TResult, TParameter> completedResult = AsyncResult.End<CompletedAsyncResult<TResult, TParameter>>(result);
            parameter = completedResult._parameter;
            return completedResult._resultData;
        }
    }
}
