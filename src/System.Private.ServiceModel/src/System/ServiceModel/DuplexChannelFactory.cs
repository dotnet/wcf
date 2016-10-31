// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel
{
    public class DuplexChannelFactory<TChannel> : ChannelFactory<TChannel>
    {
        //InstanceContext overloads

        public DuplexChannelFactory(InstanceContext callbackInstance, Binding binding, String remoteAddress)
            : this((object)callbackInstance, binding, new EndpointAddress(remoteAddress))
        { }
        public DuplexChannelFactory(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress)
            : this((object)callbackInstance, binding, remoteAddress)
        { }
        public DuplexChannelFactory(InstanceContext callbackInstance, Binding binding)
            : this((object)callbackInstance, binding)
        { }
        public DuplexChannelFactory(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress)
            : this((object)callbackInstance, endpointConfigurationName, remoteAddress)
        { }
        public DuplexChannelFactory(InstanceContext callbackInstance, string endpointConfigurationName)
            : this((object)callbackInstance, endpointConfigurationName)
        { }


        // TChannel provides ContractDescription, attr/config [TChannel,name] provides Address,Binding
        public DuplexChannelFactory(object callbackObject, string endpointConfigurationName)
            : this(callbackObject, endpointConfigurationName, null)
        {
        }

        // TChannel provides ContractDescription, attr/config [TChannel,name] provides Binding, provide Address explicitly
        public DuplexChannelFactory(object callbackObject, string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(typeof(TChannel))
        {
            using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.CreateBoundedActivity() : null)
            {
                if (DiagnosticUtility.ShouldUseActivity)
                {
                    ServiceModelActivity.Start(activity, SR.Format(SR.ActivityConstructChannelFactory, TraceUtility.CreateSourceString(this)), ActivityType.Construct);
                }
                if (callbackObject == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("callbackObject");
                }

                if (endpointConfigurationName == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpointConfigurationName");
                }

                this.CheckAndAssignCallbackInstance(callbackObject);
                this.InitializeEndpoint(endpointConfigurationName, remoteAddress);
            }
        }

        // TChannel provides ContractDescription, attr/config [TChannel,name] provides Address,Binding
        public DuplexChannelFactory(object callbackObject, Binding binding)
            : this(callbackObject, binding, (EndpointAddress)null)
        {
        }

        // TChannel provides ContractDescription, provide Address,Binding explicitly
        public DuplexChannelFactory(object callbackObject, Binding binding, EndpointAddress remoteAddress)
            : base(typeof(TChannel))
        {
            using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.CreateBoundedActivity() : null)
            {
                if (DiagnosticUtility.ShouldUseActivity)
                {
                    ServiceModelActivity.Start(activity, SR.Format(SR.ActivityConstructChannelFactory, TraceUtility.CreateSourceString(this)), ActivityType.Construct);
                }
                if (callbackObject == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("callbackObject");
                }

                if (binding == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("binding");
                }

                this.CheckAndAssignCallbackInstance(callbackObject);
                this.InitializeEndpoint(binding, remoteAddress);
            }
        }

        internal void CheckAndAssignCallbackInstance(object callbackInstance)
        {
            if (callbackInstance is Type)
            {
                this.CallbackType = (Type)callbackInstance;
            }
            else if (callbackInstance is InstanceContext)
            {
                this.CallbackInstance = (InstanceContext)callbackInstance;
            }
            else
            {
                this.CallbackInstance = new InstanceContext(callbackInstance);
            }
        }

        public TChannel CreateChannel(InstanceContext callbackInstance)
        {
            return CreateChannel(callbackInstance, CreateEndpointAddress(this.Endpoint), null);
        }

        public TChannel CreateChannel(InstanceContext callbackInstance, EndpointAddress address)
        {
            if (address == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("address");
            }

            return CreateChannel(callbackInstance, address, address.Uri);
        }

        public override TChannel CreateChannel(EndpointAddress address, Uri via)
        {
            return CreateChannel(this.CallbackInstance, address, via);
        }

        public virtual TChannel CreateChannel(InstanceContext callbackInstance, EndpointAddress address, Uri via)
        {
            if (address == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("address");
            }

            if (this.CallbackType != null && callbackInstance == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.SFxCreateDuplexChannelNoCallback1));
            }
            if (callbackInstance == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.SFxCreateDuplexChannelNoCallback));
            }

            if (callbackInstance.UserObject == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.SFxCreateDuplexChannelNoCallbackUserObject));
            }

            if (!this.HasDuplexOperations())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SFxCreateDuplexChannel1, this.Endpoint.Contract.Name)));
            }

            Type userObjectType = callbackInstance.UserObject.GetType();
            Type callbackType = this.Endpoint.Contract.CallbackContractType;
            if (callbackType != null && !callbackType.IsAssignableFrom(userObjectType))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(
                    SR.SFxCreateDuplexChannelBadCallbackUserObject, callbackType)));
            }

            EnsureOpened();
            TChannel result = this.ServiceChannelFactory.CreateChannel<TChannel>(address, via);

            IDuplexContextChannel duplexChannel = result as IDuplexContextChannel;
            if (duplexChannel != null)
            {
                duplexChannel.CallbackInstance = callbackInstance;
            }
            return result;
        }
    }
}
