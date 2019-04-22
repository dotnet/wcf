// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Runtime;
using System.Threading;

namespace System.ServiceModel.Dispatcher
{
    internal class ConcurrencyBehavior
    {
        private ConcurrencyMode _concurrencyMode;
        private bool _enforceOrderedReceive;

        internal ConcurrencyBehavior(DispatchRuntime runtime)
        {
            _concurrencyMode = runtime.ConcurrencyMode;
            _enforceOrderedReceive = runtime.EnsureOrderedDispatch;
        }

        internal bool IsConcurrent(ref MessageRpc rpc)
        {
            return IsConcurrent(_concurrencyMode, _enforceOrderedReceive, rpc.Channel.HasSession);
        }

        internal static bool IsConcurrent(ConcurrencyMode concurrencyMode, bool ensureOrderedDispatch, bool hasSession)
        {
            if (concurrencyMode != ConcurrencyMode.Single)
            {
                return true;
            }

            if (hasSession)
            {
                return false;
            }

            if (ensureOrderedDispatch)
            {
                return false;
            }

            return true;
        }

        internal static bool IsConcurrent(ChannelDispatcher runtime, bool hasSession)
        {
            bool isConcurrencyModeSingle = true;

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

        internal void LockInstance(ref MessageRpc rpc)
        {
            if (_concurrencyMode != ConcurrencyMode.Multiple)
            {
                ConcurrencyInstanceContextFacet resource = rpc.InstanceContext.Concurrency;
                lock (rpc.InstanceContext.ThisLock)
                {
                    if (!resource.Locked)
                    {
                        resource.Locked = true;
                    }
                    else
                    {
                        MessageRpcWaiter waiter = new MessageRpcWaiter(rpc.Pause());
                        resource.EnqueueNewMessage(waiter);
                    }
                }

                if (_concurrencyMode == ConcurrencyMode.Reentrant)
                {
                    rpc.OperationContext.IsServiceReentrant = true;
                }
            }
        }

        internal void UnlockInstance(ref MessageRpc rpc)
        {
            if (_concurrencyMode != ConcurrencyMode.Multiple)
            {
                ConcurrencyBehavior.UnlockInstance(rpc.InstanceContext);
            }
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
                return DequeueFrom(_calloutMessageQueue);
            }
            else
            {
                return DequeueFrom(_newMessageQueue);
            }
        }

        internal void EnqueueNewMessage(ConcurrencyBehavior.IWaiter waiter)
        {
            if (_newMessageQueue == null)
            {
                _newMessageQueue = new Queue<ConcurrencyBehavior.IWaiter>();
            }

            _newMessageQueue.Enqueue(waiter);
        }

        internal void EnqueueCalloutMessage(ConcurrencyBehavior.IWaiter waiter)
        {
            if (_calloutMessageQueue == null)
            {
                _calloutMessageQueue = new Queue<ConcurrencyBehavior.IWaiter>();
            }

            _calloutMessageQueue.Enqueue(waiter);
        }
    }
}
