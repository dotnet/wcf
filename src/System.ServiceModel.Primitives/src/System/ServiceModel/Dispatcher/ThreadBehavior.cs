// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;
using System.Threading;

namespace System.ServiceModel.Dispatcher
{
    internal class ThreadBehavior
    {
        private SendOrPostCallback _threadAffinityStartCallback;
        private SendOrPostCallback _threadAffinityEndCallback;
        private static Action<object> s_cleanThreadCallback;
        private readonly SynchronizationContext _context;

        internal ThreadBehavior(DispatchRuntime dispatch)
        {
            _context = dispatch.SynchronizationContext;
        }

        private SendOrPostCallback ThreadAffinityStartCallbackDelegate
        {
            get
            {
                if (_threadAffinityStartCallback == null)
                {
                    _threadAffinityStartCallback = new SendOrPostCallback(SynchronizationContextStartCallback);
                }
                return _threadAffinityStartCallback;
            }
        }
        private SendOrPostCallback ThreadAffinityEndCallbackDelegate
        {
            get
            {
                if (_threadAffinityEndCallback == null)
                {
                    _threadAffinityEndCallback = new SendOrPostCallback(SynchronizationContextEndCallback);
                }
                return _threadAffinityEndCallback;
            }
        }

        private static Action<object> CleanThreadCallbackDelegate
        {
            get
            {
                if (ThreadBehavior.s_cleanThreadCallback == null)
                {
                    ThreadBehavior.s_cleanThreadCallback = new Action<object>(ThreadBehavior.CleanThreadCallback);
                }
                return ThreadBehavior.s_cleanThreadCallback;
            }
        }

        internal void BindThread(ref MessageRpc rpc)
        {
            BindCore(ref rpc, true);
        }

        internal void BindEndThread(ref MessageRpc rpc)
        {
            BindCore(ref rpc, false);
        }

        private void BindCore(ref MessageRpc rpc, bool startOperation)
        {
            SynchronizationContext syncContext = GetSyncContext(rpc.InstanceContext);

            if (syncContext != null)
            {
                IResumeMessageRpc resume = rpc.Pause();
                if (startOperation)
                {
                    syncContext.OperationStarted();
                    syncContext.Post(ThreadAffinityStartCallbackDelegate, resume);
                }
                else
                {
                    syncContext.Post(ThreadAffinityEndCallbackDelegate, resume);
                }
            }
            else if (rpc.SwitchedThreads)
            {
                IResumeMessageRpc resume = rpc.Pause();
                ActionItem.Schedule(ThreadBehavior.CleanThreadCallbackDelegate, resume);
            }
        }

        private SynchronizationContext GetSyncContext(InstanceContext instanceContext)
        {
            Fx.Assert(instanceContext != null, "instanceContext is null !");
            SynchronizationContext syncContext = instanceContext.SynchronizationContext ?? _context;
            return syncContext;
        }

        private void SynchronizationContextStartCallback(object state)
        {
            ResumeProcessing((IResumeMessageRpc)state);
        }
        private void SynchronizationContextEndCallback(object state)
        {
            IResumeMessageRpc resume = (IResumeMessageRpc)state;

            ResumeProcessing(resume);

            SynchronizationContext syncContext = GetSyncContext(resume.GetMessageInstanceContext());
            Fx.Assert(syncContext != null, "syncContext is null !?");
            syncContext.OperationCompleted();
        }
        private void ResumeProcessing(IResumeMessageRpc resume)
        {
            bool alreadyResumedNoLock;
            resume.Resume(out alreadyResumedNoLock);

            if (alreadyResumedNoLock)
            {
                string text = SRP.Format(SRP.SFxMultipleCallbackFromSynchronizationContext, _context.GetType().ToString());
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(text));
            }
        }

        private static void CleanThreadCallback(object state)
        {
            bool alreadyResumedNoLock;
            ((IResumeMessageRpc)state).Resume(out alreadyResumedNoLock);

            if (alreadyResumedNoLock)
            {
                Fx.Assert("IOThreadScheduler called back twice");
            }
        }

        internal static SynchronizationContext GetCurrentSynchronizationContext()
        {
            return SynchronizationContext.Current;
        }
    }
}
