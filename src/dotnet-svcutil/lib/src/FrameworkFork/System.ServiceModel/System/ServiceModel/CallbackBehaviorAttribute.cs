// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace System.ServiceModel
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CallbackBehaviorAttribute : Attribute, IEndpointBehavior
    {
        private ConcurrencyMode _concurrencyMode = ConcurrencyMode.Single;
        private bool _automaticSessionShutdown = true;
        private bool _useSynchronizationContext = true;

        public bool AutomaticSessionShutdown
        {
            get { return _automaticSessionShutdown; }
            set { _automaticSessionShutdown = value; }
        }


        public bool UseSynchronizationContext
        {
            get { return _useSynchronizationContext; }
            set { _useSynchronizationContext = value; }
        }

        void IEndpointBehavior.Validate(ServiceEndpoint serviceEndpoint)
        {
        }

        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint serviceEndpoint, BindingParameterCollection parameters)
        {
        }

        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint serviceEndpoint, ClientRuntime clientRuntime)
        {
            if (!serviceEndpoint.Contract.IsDuplex())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    string.Format(SRServiceModel.SFxCallbackBehaviorAttributeOnlyOnDuplex, serviceEndpoint.Contract.Name)));
            }
            DispatchRuntime dispatchRuntime = clientRuntime.DispatchRuntime;

            dispatchRuntime.ConcurrencyMode = _concurrencyMode;
            dispatchRuntime.AutomaticInputSessionShutdown = _automaticSessionShutdown;

            if (!_useSynchronizationContext)
            {
                dispatchRuntime.SynchronizationContext = null;
            }
            // Desktop: DataContractSerializerServiceBehavior.ApplySerializationSettings(serviceEndpoint, this.ignoreExtensionDataObject, this.maxItemsInObjectGraph);
        }

        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                string.Format(SRServiceModel.SFXEndpointBehaviorUsedOnWrongSide, typeof(CallbackBehaviorAttribute).Name)));
        }
    }
}
