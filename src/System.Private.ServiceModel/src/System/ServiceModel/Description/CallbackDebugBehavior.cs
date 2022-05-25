// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace System.ServiceModel.Description
{
    public class CallbackDebugBehavior : IEndpointBehavior
    {
        bool includeExceptionDetailInFaults = false;

        public CallbackDebugBehavior(bool includeExceptionDetailInFaults)
        {
            this.includeExceptionDetailInFaults = includeExceptionDetailInFaults;
        }

        public bool IncludeExceptionDetailInFaults
        {
            get { return this.includeExceptionDetailInFaults; }
            set { this.includeExceptionDetailInFaults = value; }
        }

        void IEndpointBehavior.Validate(ServiceEndpoint serviceEndpoint)
        {
        }
        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint serviceEndpoint, BindingParameterCollection bindingParameters)
        {
        }
        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                SR.Format(SR.SFXEndpointBehaviorUsedOnWrongSide, typeof(CallbackDebugBehavior).Name)));
        }
        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint serviceEndpoint, ClientRuntime behavior)
        {
            ChannelDispatcher channelDispatcher = behavior.CallbackDispatchRuntime.ChannelDispatcher;
            if (channelDispatcher != null && this.includeExceptionDetailInFaults)
            {
                channelDispatcher.IncludeExceptionDetailInFaults = true;
            }
        }
    }
}
