// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Samples.MessageInterceptor
{
    internal delegate IAsyncResult ChainedBeginHandler(TimeSpan timeout, AsyncCallback asyncCallback, object state);
    internal delegate void ChainedEndHandler(IAsyncResult result);

    internal class ChainedAsyncResult : AsyncResult
    {
        private ChainedBeginHandler _begin2;
        private ChainedEndHandler _end1;
        private ChainedEndHandler _end2;
        private TimeoutHelper _timeoutHelper;
        private static AsyncCallback s_begin1Callback = new AsyncCallback(Begin1Callback);
        private static AsyncCallback s_begin2Callback = new AsyncCallback(Begin2Callback);

        protected ChainedAsyncResult(TimeSpan timeout, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _timeoutHelper = new TimeoutHelper(timeout);
        }

        public ChainedAsyncResult(TimeSpan timeout, AsyncCallback callback, object state, ChainedBeginHandler begin1, ChainedEndHandler end1, ChainedBeginHandler begin2, ChainedEndHandler end2)
            : base(callback, state)
        {
            _timeoutHelper = new TimeoutHelper(timeout);
            Begin(begin1, end1, begin2, end2);
        }

        protected void Begin(ChainedBeginHandler begin1, ChainedEndHandler end1, ChainedBeginHandler begin2, ChainedEndHandler end2)
        {
            _end1 = end1;
            _begin2 = begin2;
            _end2 = end2;

            IAsyncResult result = begin1(_timeoutHelper.RemainingTime(), s_begin1Callback, this);
            if (!result.CompletedSynchronously)
                return;

            if (Begin1Completed(result))
            {
                this.Complete(true);
            }
        }

        private static void Begin1Callback(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
                return;

            ChainedAsyncResult thisPtr = (ChainedAsyncResult)result.AsyncState;

            bool completeSelf = false;
            Exception completeException = null;

            try
            {
                completeSelf = thisPtr.Begin1Completed(result);
            }
            catch (Exception exception)
            {
                completeSelf = true;
                completeException = exception;
            }

            if (completeSelf)
            {
                thisPtr.Complete(false, completeException);
            }
        }

        private bool Begin1Completed(IAsyncResult result)
        {
            _end1(result);

            result = _begin2(_timeoutHelper.RemainingTime(), s_begin2Callback, this);
            if (!result.CompletedSynchronously)
            {
                return false;
            }

            _end2(result);
            return true;
        }

        private static void Begin2Callback(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
                return;

            ChainedAsyncResult thisPtr = (ChainedAsyncResult)result.AsyncState;

            Exception completeException = null;

            try
            {
                thisPtr._end2(result);
            }
            catch (Exception exception)
            {
                completeException = exception;
            }

            thisPtr.Complete(false, completeException);
        }

        public static void End(IAsyncResult result)
        {
            AsyncResult.End<ChainedAsyncResult>(result);
        }
    }
}
