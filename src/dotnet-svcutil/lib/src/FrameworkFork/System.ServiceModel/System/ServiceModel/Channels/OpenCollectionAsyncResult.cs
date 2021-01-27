// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime;
using System.Threading;

namespace System.ServiceModel
{
    internal class OpenCollectionAsyncResult : AsyncResult
    {
        private bool _completedSynchronously;
        private Exception _exception;
        private static AsyncCallback s_nestedCallback = Fx.ThunkCallback(new AsyncCallback(Callback));
        private int _count;
        private TimeoutHelper _timeoutHelper;

        public OpenCollectionAsyncResult(TimeSpan timeout, AsyncCallback otherCallback, object state, IList<ICommunicationObject> collection)
            : base(otherCallback, state)
        {
            _timeoutHelper = new TimeoutHelper(timeout);
            _completedSynchronously = true;

            _count = collection.Count;
            if (_count == 0)
            {
                Complete(true);
                return;
            }

            for (int index = 0; index < collection.Count; index++)
            {
                // Throw exception if there was a failure calling EndOpen in the callback (skips remaining items)
                if (_exception != null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(_exception);
                CallbackState callbackState = new CallbackState(this, collection[index]);
                IAsyncResult result = collection[index].BeginOpen(_timeoutHelper.RemainingTime(), s_nestedCallback, callbackState);
                if (result.CompletedSynchronously)
                {
                    collection[index].EndOpen(result);
                    Decrement(true);
                }
            }
        }

        private static void Callback(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
                return;
            CallbackState callbackState = (CallbackState)result.AsyncState;
            try
            {
                callbackState.Instance.EndOpen(result);
                callbackState.Result.Decrement(false);
            }
#pragma warning disable 56500 // covered by FxCOP
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                    throw;

                callbackState.Result.Decrement(false, e);
            }
        }

        private void Decrement(bool completedSynchronously)
        {
            if (completedSynchronously == false)
                _completedSynchronously = false;
            if (Interlocked.Decrement(ref _count) == 0)
            {
                if (_exception != null)
                    Complete(_completedSynchronously, _exception);
                else
                    Complete(_completedSynchronously);
            }
        }

        private void Decrement(bool completedSynchronously, Exception exception)
        {
            _exception = exception;
            this.Decrement(completedSynchronously);
        }

        public static void End(IAsyncResult result)
        {
            AsyncResult.End<OpenCollectionAsyncResult>(result);
        }

        internal class CallbackState
        {
            private ICommunicationObject _instance;
            private OpenCollectionAsyncResult _result;

            public CallbackState(OpenCollectionAsyncResult result, ICommunicationObject instance)
            {
                _result = result;
                _instance = instance;
            }

            public ICommunicationObject Instance
            {
                get { return _instance; }
            }

            public OpenCollectionAsyncResult Result
            {
                get { return _result; }
            }
        }
    }
}
