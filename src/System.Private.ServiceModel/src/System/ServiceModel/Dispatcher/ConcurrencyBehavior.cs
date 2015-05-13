// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime;
using System.ServiceModel;
using System.Threading;

namespace System.ServiceModel.Dispatcher
{
    internal class ConcurrencyBehavior
    {
        private static bool SupportsTransactedBatch(ChannelDispatcher channelDispatcher)
        {
            return channelDispatcher.IsTransactedReceive && (channelDispatcher.MaxTransactedBatchSize > 0);
        }

        internal static bool IsConcurrent(ChannelDispatcher runtime, bool hasSession)
        {
            bool isConcurrencyModeSingle = true;

            if (ConcurrencyBehavior.SupportsTransactedBatch(runtime))
            {
                return false;
            }

            foreach (EndpointDispatcher endpointDispatcher in runtime.Endpoints)
            {
                if (endpointDispatcher.DispatchRuntime.EnsureOrderedDispatch)
                {
                    return false;
                }

                if (endpointDispatcher.DispatchRuntime.ConcurrencyMode != ConcurrencyMode.Single)
                {
                    isConcurrencyModeSingle = false;
                }
            }

            if (!isConcurrencyModeSingle)
            {
                return true;
            }

            if (!hasSession)
            {
                return true;
            }

            return false;
        }

        internal static void UnlockInstanceBeforeCallout(OperationContext operationContext)
        {
            if (operationContext != null && operationContext.IsServiceReentrant)
            {
                ConcurrencyBehavior.UnlockInstance(operationContext.InstanceContext);
            }
        }

        private static void UnlockInstance(InstanceContext instanceContext)
        {
            ConcurrencyInstanceContextFacet resource = instanceContext.Concurrency;

            lock (instanceContext.ThisLock)
            {
                if (resource.HasWaiters)
                {
                    IWaiter nextWaiter = resource.DequeueWaiter();
                    nextWaiter.Signal();
                }
                else
                {
                    //We have no pending Callouts and no new Messages to process
                    resource.Locked = false;
                }
            }
        }

        internal static void LockInstanceAfterCallout(OperationContext operationContext)
        {
            if (operationContext != null)
            {
                InstanceContext instanceContext = operationContext.InstanceContext;

                if (operationContext.IsServiceReentrant)
                {
                    ConcurrencyInstanceContextFacet resource = instanceContext.Concurrency;
                    ThreadWaiter waiter = null;

                    lock (instanceContext.ThisLock)
                    {
                        if (!resource.Locked)
                        {
                            resource.Locked = true;
                        }
                        else
                        {
                            waiter = new ThreadWaiter();
                            resource.EnqueueCalloutMessage(waiter);
                        }
                    }

                    if (waiter != null)
                    {
                        waiter.Wait();
                    }
                }
            }
        }

        internal interface IWaiter
        {
            void Signal();
        }

        internal class MessageRpcWaiter : IWaiter
        {
            private IResumeMessageRpc _resume;

            internal MessageRpcWaiter(IResumeMessageRpc resume)
            {
                _resume = resume;
            }

            void IWaiter.Signal()
            {
                try
                {
                    bool alreadyResumedNoLock;
                    _resume.Resume(out alreadyResumedNoLock);

                    if (alreadyResumedNoLock)
                    {
                        Fx.Assert("ConcurrencyBehavior resumed more than once for same call.");
                    }
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(e);
                }
            }
        }

        internal class ThreadWaiter : IWaiter
        {
            private ManualResetEvent _wait = new ManualResetEvent(false);

            void IWaiter.Signal()
            {
                _wait.Set();
            }

            internal void Wait()
            {
                _wait.WaitOne();
                _wait.Dispose();
            }
        }
    }

    internal class ConcurrencyInstanceContextFacet
    {
        internal bool Locked;
        private Queue<ConcurrencyBehavior.IWaiter> _calloutMessageQueue;
        private Queue<ConcurrencyBehavior.IWaiter> _newMessageQueue;

        internal bool HasWaiters
        {
            get
            {
                return (((_calloutMessageQueue != null) && (_calloutMessageQueue.Count > 0)) ||
                        ((_newMessageQueue != null) && (_newMessageQueue.Count > 0)));
            }
        }

        private ConcurrencyBehavior.IWaiter DequeueFrom(Queue<ConcurrencyBehavior.IWaiter> queue)
        {
            ConcurrencyBehavior.IWaiter waiter = queue.Dequeue();

            if (queue.Count == 0)
            {
                queue.TrimExcess();
            }

            return waiter;
        }

        internal ConcurrencyBehavior.IWaiter DequeueWaiter()
        {
            // Finishing old work takes precedence over new work.
            if ((_calloutMessageQueue != null) && (_calloutMessageQueue.Count > 0))
            {
                return this.DequeueFrom(_calloutMessageQueue);
            }
            else
            {
                return this.DequeueFrom(_newMessageQueue);
            }
        }

        internal void EnqueueNewMessage(ConcurrencyBehavior.IWaiter waiter)
        {
            if (_newMessageQueue == null)
                _newMessageQueue = new Queue<ConcurrencyBehavior.IWaiter>();
            _newMessageQueue.Enqueue(waiter);
        }

        internal void EnqueueCalloutMessage(ConcurrencyBehavior.IWaiter waiter)
        {
            if (_calloutMessageQueue == null)
                _calloutMessageQueue = new Queue<ConcurrencyBehavior.IWaiter>();
            _calloutMessageQueue.Enqueue(waiter);
        }
    }
}
