// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime
{
    internal class OperationWithTimeoutAsyncResult : AsyncResult
    {
        private static readonly Action<object> s_scheduledCallback = new Action<object>(OnScheduled);
        private TimeoutHelper _timeoutHelper;
        private Action<TimeSpan> _operationWithTimeout;

        public OperationWithTimeoutAsyncResult(Action<TimeSpan> operationWithTimeout, TimeSpan timeout, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _operationWithTimeout = operationWithTimeout;
            _timeoutHelper = new TimeoutHelper(timeout);
            _ = Task.Factory.StartNew(s_scheduledCallback, this, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        private static void OnScheduled(object state)
        {
            OperationWithTimeoutAsyncResult thisResult = (OperationWithTimeoutAsyncResult)state;
            Exception completionException = null;
            try
            {
                thisResult._operationWithTimeout(thisResult._timeoutHelper.RemainingTime());
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                completionException = e;
            }

            thisResult.Complete(false, completionException);
        }

        public static void End(IAsyncResult result)
        {
            End<OperationWithTimeoutAsyncResult>(result);
        }
    }

    internal abstract class AsyncResult : IAsyncResult
    {
        private static AsyncCallback s_asyncCompletionWrapperCallback;
        private AsyncCallback _callback;
        private bool _endCalled;
        private Exception _exception;
        private AsyncCompletion _nextAsyncCompletion;
        private Action _beforePrepareAsyncCompletionAction;
        private Func<IAsyncResult, bool> _checkSyncValidationFunc;
        private ManualResetEvent _manualResetEvent;
        private object _thisLock;


        protected AsyncResult(AsyncCallback callback, object state)
        {
            _callback = callback;
            AsyncState = state;
            _thisLock = new object();
        }

        public object AsyncState { get; }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (_manualResetEvent != null)
                {
                    return _manualResetEvent;
                }

                lock (ThisLock)
                {
                    if (_manualResetEvent == null)
                    {
                        _manualResetEvent = new ManualResetEvent(IsCompleted);
                    }
                }

                return _manualResetEvent;
            }
        }

        public bool CompletedSynchronously { get; private set; }

        public bool HasCallback
        {
            get
            {
                return _callback != null;
            }
        }

        public bool IsCompleted { get; private set; }

        // used in conjunction with PrepareAsyncCompletion to allow for finally blocks
        protected Action<AsyncResult, Exception> OnCompleting { get; set; }

        private object ThisLock
        {
            get
            {
                return _thisLock;
            }
        }

        // subclasses like TraceAsyncResult can use this to wrap the callback functionality in a scope
        protected Action<AsyncCallback, IAsyncResult> VirtualCallback
        {
            get;
            set;
        }

        protected void Complete(bool completedSynchronously)
        {
            if (IsCompleted)
            {
                throw new InvalidOperationException(SR.Format(SR.AsyncResultCompletedTwice, GetType()));
            }



            CompletedSynchronously = completedSynchronously;
            if (OnCompleting != null)
            {
                // Allow exception replacement, like a catch/throw pattern.
                try
                {
                    OnCompleting(this, _exception);
                }
                catch (Exception exception)
                {
                    if (Fx.IsFatal(exception))
                    {
                        throw;
                    }
                    _exception = exception;
                }
            }

            if (completedSynchronously)
            {
                // If we completedSynchronously, then there's no chance that the manualResetEvent was created so
                // we don't need to worry about a race condition.
                Debug.Assert(_manualResetEvent == null, "No ManualResetEvent should be created for a synchronous AsyncResult.");
                IsCompleted = true;
            }
            else
            {
                lock (ThisLock)
                {
                    IsCompleted = true;
                    if (_manualResetEvent != null)
                    {
                        _manualResetEvent.Set();
                    }
                }
            }

            if (_callback != null)
            {
                try
                {
                    if (VirtualCallback != null)
                    {
                        VirtualCallback(_callback, this);
                    }
                    else
                    {
                        _callback(this);
                    }
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }

                    throw new CommunicationException(InternalSR.AsyncCallbackThrewException, e);
                }
            }
        }

        protected void Complete(bool completedSynchronously, Exception exception)
        {
            _exception = exception;
            Complete(completedSynchronously);
        }

        private static void AsyncCompletionWrapperCallback(IAsyncResult result)
        {
            if (result == null)
            {
                throw new InvalidOperationException(InternalSR.InvalidNullAsyncResult);
            }
            if (result.CompletedSynchronously)
            {
                return;
            }

            AsyncResult thisPtr = (AsyncResult)result.AsyncState;
            if (!thisPtr.OnContinueAsyncCompletion(result))
            {
                return;
            }

            AsyncCompletion callback = thisPtr.GetNextCompletion();
            if (callback == null)
            {
                ThrowInvalidAsyncResult(result);
            }

            bool completeSelf = false;
            Exception completionException = null;
            try
            {
                completeSelf = callback(result);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                completeSelf = true;
                completionException = e;
            }

            if (completeSelf)
            {
                thisPtr.Complete(false, completionException);
            }
        }

        // Note: this should be only derived by the TransactedAsyncResult
        protected virtual bool OnContinueAsyncCompletion(IAsyncResult result)
        {
            return true;
        }

        // Note: this should be used only by the TransactedAsyncResult
        protected void SetBeforePrepareAsyncCompletionAction(Action beforePrepareAsyncCompletionAction)
        {
            _beforePrepareAsyncCompletionAction = beforePrepareAsyncCompletionAction;
        }

        // Note: this should be used only by the TransactedAsyncResult
        protected void SetCheckSyncValidationFunc(Func<IAsyncResult, bool> checkSyncValidationFunc)
        {
            _checkSyncValidationFunc = checkSyncValidationFunc;
        }

        protected AsyncCallback PrepareAsyncCompletion(AsyncCompletion callback)
        {
            if (_beforePrepareAsyncCompletionAction != null)
            {
                _beforePrepareAsyncCompletionAction();
            }

            _nextAsyncCompletion = callback;
            if (AsyncResult.s_asyncCompletionWrapperCallback == null)
            {
                AsyncResult.s_asyncCompletionWrapperCallback = new AsyncCallback(AsyncCompletionWrapperCallback);
            }
            return AsyncResult.s_asyncCompletionWrapperCallback;
        }

        protected bool CheckSyncContinue(IAsyncResult result)
        {
            AsyncCompletion dummy;
            return TryContinueHelper(result, out dummy);
        }

        protected bool SyncContinue(IAsyncResult result)
        {
            AsyncCompletion callback;
            if (TryContinueHelper(result, out callback))
            {
                return callback(result);
            }
            else
            {
                return false;
            }
        }

        private bool TryContinueHelper(IAsyncResult result, out AsyncCompletion callback)
        {
            if (result == null)
            {
                throw new InvalidOperationException(InternalSR.InvalidNullAsyncResult);
            }

            callback = null;
            if (_checkSyncValidationFunc != null)
            {
                if (!_checkSyncValidationFunc(result))
                {
                    return false;
                }
            }
            else if (!result.CompletedSynchronously)
            {
                return false;
            }

            callback = GetNextCompletion();
            if (callback == null)
            {
                ThrowInvalidAsyncResult("Only call Check/SyncContinue once per async operation (once per PrepareAsyncCompletion).");
            }
            return true;
        }

        private AsyncCompletion GetNextCompletion()
        {
            AsyncCompletion result = _nextAsyncCompletion;
            _nextAsyncCompletion = null;
            return result;
        }

        protected static void ThrowInvalidAsyncResult(IAsyncResult result)
        {
            throw new InvalidOperationException(InternalSR.InvalidAsyncResultImplementation(result.GetType()));
        }

        protected static void ThrowInvalidAsyncResult(string debugText)
        {
            string message = InternalSR.InvalidAsyncResultImplementationGeneric;
            if (debugText != null)
            {
#if DEBUG
                message += " " + debugText;
#endif
            }
            throw new InvalidOperationException(message);
        }

        protected static TAsyncResult End<TAsyncResult>(IAsyncResult result)
            where TAsyncResult : AsyncResult
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            TAsyncResult asyncResult = result as TAsyncResult;

            if (asyncResult == null)
            {
                throw new ArgumentException(InternalSR.InvalidAsyncResult, nameof(result));
            }

            if (asyncResult._endCalled)
            {
                throw new InvalidOperationException(InternalSR.AsyncResultAlreadyEnded);
            }


            asyncResult._endCalled = true;

            if (!asyncResult.IsCompleted)
            {
                asyncResult.AsyncWaitHandle.WaitOne();
            }

            if (asyncResult._manualResetEvent != null)
            {
                asyncResult._manualResetEvent.Dispose();
            }

            if (asyncResult._exception != null)
            {
                throw asyncResult._exception;
            }

            return asyncResult;
        }

        // can be utilized by subclasses to write core completion code for both the sync and async paths
        // in one location, signalling chainable synchronous completion with the boolean result,
        // and leveraging PrepareAsyncCompletion for conversion to an AsyncCallback.
        // NOTE: requires that "this" is passed in as the state object to the asynchronous sub-call being used with a completion routine.
        protected delegate bool AsyncCompletion(IAsyncResult result);
    }
}
