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
        private bool _useSynchronizationContext = true;

        public bool AutomaticSessionShutdown { get; set; } = true;

        public ConcurrencyMode ConcurrencyMode
        {
            get { return _concurrencyMode; }
            set
            {
                if (!ConcurrencyModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
                }

                _concurrencyMode = value;
            }
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
                    SRP.Format(SRP.SFxCallbackBehaviorAttributeOnlyOnDuplex, serviceEndpoint.Contract.Name)));
            }
            DispatchRuntime dispatchRuntime = clientRuntime.DispatchRuntime;

            dispatchRuntime.ConcurrencyMode = _concurrencyMode;
            dispatchRuntime.AutomaticInputSessionShutdown = AutomaticSessionShutdown;

            if (!_useSynchronizationContext)
            {
                dispatchRuntime.SynchronizationContext = null;
            }
            // Desktop: DataContractSerializerServiceBehavior.ApplySerializationSettings(serviceEndpoint, this.ignoreExtensionDataObject, this.maxItemsInObjectGraph);
        }

        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                SRP.Format(SRP.SFXEndpointBehaviorUsedOnWrongSide, typeof(CallbackBehaviorAttribute).Name)));
        }
    }
}
