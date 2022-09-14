// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel.Dispatcher
{
    public class InstanceBehavior
    {
        private IInstanceProvider _provider;

        internal InstanceBehavior(DispatchRuntime dispatch, ImmutableDispatchRuntime immutableRuntime)
        {
            _provider = dispatch.InstanceProvider;
            InstanceContextProvider = dispatch.InstanceContextProvider;
        }

        internal IInstanceContextProvider InstanceContextProvider { get; }

        internal void AfterReply(ref MessageRpc rpc, ErrorBehavior error)
        {
            InstanceContext context = rpc.InstanceContext;

            if (context != null)
            {
                try
                {
                    context.UnbindRpc(ref rpc);
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    error.HandleError(e);
                }
            }
        }

        internal void EnsureInstanceContext(ref MessageRpc rpc)
        {
            if (rpc.InstanceContext == null)
            {
                throw new ArgumentNullException("rpc.InstanceContext");
            }

            rpc.OperationContext.SetInstanceContext(rpc.InstanceContext);
            rpc.InstanceContext.Behavior = this;

            if (rpc.InstanceContext.State == CommunicationState.Created)
            {
                lock (rpc.InstanceContext.ThisLock)
                {
                    if (rpc.InstanceContext.State == CommunicationState.Created)
                    {
                        rpc.InstanceContext.Open(rpc.Channel.CloseTimeout);
                    }
                }
            }
            rpc.InstanceContext.BindRpc(ref rpc);
        }

        internal object GetInstance(InstanceContext instanceContext)
        {
            if (_provider == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.SFxNoDefaultConstructor));
            }

            return _provider.GetInstance(instanceContext);
        }

        internal object GetInstance(InstanceContext instanceContext, Message request)
        {
            if (_provider == null)
            {
                throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRP.SFxNoDefaultConstructor), request);
            }

            return _provider.GetInstance(instanceContext, request);
        }

        internal void EnsureServiceInstance(ref MessageRpc rpc)
        {
            if (WcfEventSource.Instance.GetServiceInstanceStartIsEnabled())
            {
                WcfEventSource.Instance.GetServiceInstanceStart(rpc.EventTraceActivity);
            }

            rpc.Instance = rpc.InstanceContext.GetServiceInstance(rpc.Request);

            if (WcfEventSource.Instance.GetServiceInstanceStopIsEnabled())
            {
                WcfEventSource.Instance.GetServiceInstanceStop(rpc.EventTraceActivity);
            }
        }
    }
}