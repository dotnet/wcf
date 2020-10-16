// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime;
using System.Threading;

namespace System.ServiceModel
{
    internal class CloseCollectionAsyncResult : AsyncResult
    {
        private bool _completedSynchronously;
        private Exception _exception;
        private static AsyncCallback s_nestedCallback = Fx.ThunkCallback(new AsyncCallback(Callback));
        private int _count;

        public CloseCollectionAsyncResult(TimeSpan timeout, AsyncCallback otherCallback, object state, IList<ICommunicationObject> collection)
            : base(otherCallback, state)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            _completedSynchronously = true;

            _count = collection.Count;
            if (_count == 0)
            {
                Complete(true);
                return;
            }

            for (int index = 0; index < collection.Count; index++)
            {
                CallbackState callbackState = new CallbackState(this, collection[index]);
                IAsyncResult result;
                try
                {
                    result = collection[index].BeginClose(timeoutHelper.RemainingTime(), s_nestedCallback, callbackState);
                }
#pragma warning disable 56500 // covered by FxCOP
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }

                    Decrement(true, e);
                    collection[index].Abort();
                    continue;
                }

                if (result.CompletedSynchronously)
                {
                    CompleteClose(collection[index], result);
                }
            }
        }

        private void CompleteClose(ICommunicationObject communicationObject, IAsyncResult result)
        {
            Exception closeException = null;
            try
            {
                communicationObject.EndClose(result);
            }
#pragma warning disable 56500 // covered by FxCOP
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                closeException = e;
                communicationObject.Abort();
            }

            Decrement(result.CompletedSynchronously, closeException);
        }

        private static void Callback(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
            {
                return;
            }

            CallbackState callbackState = (CallbackState)result.AsyncState;
            callbackState.Result.CompleteClose(callbackState.Instance, result);
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
            AsyncResult.End<CloseCollectionAsyncResult>(result);
        }

        internal class CallbackState
        {
            private ICommunicationObject _instance;
            private CloseCollectionAsyncResult _result;

            public CallbackState(CloseCollectionAsyncResult result, ICommunicationObject instance)
            {
                _result = result;
                _instance = instance;
            }

            public ICommunicationObject Instance
            {
                get { return _instance; }
            }

            public CloseCollectionAsyncResult Result
            {
                get { return _result; }
            }
        }
    }
}
