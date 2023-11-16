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
        //Type overloads
        public DuplexChannelFactory(Type callbackInstanceType)
            : this((object)callbackInstanceType)
        { }
        public DuplexChannelFactory(Type callbackInstanceType, Binding binding, String remoteAddress)
            : this((object)callbackInstanceType, binding, new EndpointAddress(remoteAddress))
        { }
        public DuplexChannelFactory(Type callbackInstanceType, Binding binding, EndpointAddress remoteAddress)
            : this((object)callbackInstanceType, binding, remoteAddress)
        { }
        public DuplexChannelFactory(Type callbackInstanceType, Binding binding)
            : this((object)callbackInstanceType, binding)
        { }
        public DuplexChannelFactory(Type callbackInstanceType, ServiceEndpoint endpoint)
            : this((object)callbackInstanceType, endpoint)
        { }

        //InstanceContext overloads
        public DuplexChannelFactory(InstanceContext callbackInstance)
            : this((object)callbackInstance)
        { }
        public DuplexChannelFactory(InstanceContext callbackInstance, Binding binding, String remoteAddress)
            : this((object)callbackInstance, binding, new EndpointAddress(remoteAddress))
        { }
        public DuplexChannelFactory(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress)
            : this((object)callbackInstance, binding, remoteAddress)
        { }
        public DuplexChannelFactory(InstanceContext callbackInstance, Binding binding)
            : this((object)callbackInstance, binding)
        { }
        public DuplexChannelFactory(InstanceContext callbackInstance, ServiceEndpoint endpoint)
            : this((object)callbackInstance, endpoint)
        { }

        // TChannel provides ContractDescription
        public DuplexChannelFactory(object callbackObject)
            : base(typeof(TChannel))
        {
            using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.CreateBoundedActivity() : null)
            {
                if (DiagnosticUtility.ShouldUseActivity)
                {
                    ServiceModelActivity.Start(activity, SRP.Format(SRP.ActivityConstructChannelFactory, TraceUtility.CreateSourceString(this)), ActivityType.Construct);
                }
                if (callbackObject == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(callbackObject));
                }

                CheckAndAssignCallbackInstance(callbackObject);
                InitializeEndpoint((EndpointAddress)null);
            }
        }

        // TChannel provides ContractDescription, attr/config [TChannel,name] provides Address,Binding
        public DuplexChannelFactory(object callbackObject, Binding binding)
            : this(callbackObject, binding, (EndpointAddress)null)
        {
        }

        // TChannel provides ContractDescription, provide Address,Binding explicitly
        public DuplexChannelFactory(object callbackObject, Binding binding, String remoteAddress)
            : this(callbackObject, binding, new EndpointAddress(remoteAddress))
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
                    ServiceModelActivity.Start(activity, SRP.Format(SRP.ActivityConstructChannelFactory, TraceUtility.CreateSourceString(this)), ActivityType.Construct);
                }
                if (callbackObject == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(callbackObject));
                }

                if (binding == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(binding));
                }

                CheckAndAssignCallbackInstance(callbackObject);
                InitializeEndpoint(binding, remoteAddress);
            }
        }

        // provide ContractDescription,Address,Binding explicitly
        public DuplexChannelFactory(object callbackObject, ServiceEndpoint endpoint)
            : base(typeof(TChannel))
        {
            using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.CreateBoundedActivity() : null)
            {
                if (DiagnosticUtility.ShouldUseActivity)
                {
                    ServiceModelActivity.Start(activity, SRP.Format(SRP.ActivityConstructChannelFactory, TraceUtility.CreateSourceString(this)), ActivityType.Construct);
                }
                if (callbackObject == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(callbackObject));
                }

                if (endpoint == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(endpoint));
                }

                CheckAndAssignCallbackInstance(callbackObject);
                InitializeEndpoint(endpoint);
            }
        }

        internal void CheckAndAssignCallbackInstance(object callbackInstance)
        {
            if (callbackInstance is Type)
            {
                CallbackType = (Type)callbackInstance;
            }
            else if (callbackInstance is InstanceContext)
            {
                CallbackInstance = (InstanceContext)callbackInstance;
            }
            else
            {
                CallbackInstance = new InstanceContext(callbackInstance);
            }
        }

        public TChannel CreateChannel(InstanceContext callbackInstance)
        {
            return CreateChannel(callbackInstance, CreateEndpointAddress(Endpoint), null);
        }

        public TChannel CreateChannel(InstanceContext callbackInstance, EndpointAddress address)
        {
            if (address == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(address));
            }

            return CreateChannel(callbackInstance, address, address.Uri);
        }

        public override TChannel CreateChannel(EndpointAddress address, Uri via)
        {
            return CreateChannel(CallbackInstance, address, via);
        }

        public virtual TChannel CreateChannel(InstanceContext callbackInstance, EndpointAddress address, Uri via)
        {
            if (address == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(address));
            }

            if (CallbackType != null && callbackInstance == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.SFxCreateDuplexChannelNoCallback1));
            }
            if (callbackInstance == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.SFxCreateDuplexChannelNoCallback));
            }

            if (callbackInstance.UserObject == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.SFxCreateDuplexChannelNoCallbackUserObject));
            }

            if (!HasDuplexOperations())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.SFxCreateDuplexChannel1, Endpoint.Contract.Name)));
            }

            Type userObjectType = callbackInstance.UserObject.GetType();
            Type callbackType = Endpoint.Contract.CallbackContractType;
            if (callbackType != null && !callbackType.IsAssignableFrom(userObjectType))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(
                    SRP.SFxCreateDuplexChannelBadCallbackUserObject, callbackType)));
            }

            EnsureOpened();
            TChannel result = ServiceChannelFactory.CreateChannel<TChannel>(address, via);
            // Desktop: this.ServiceChannelFactory.CreateChannel(typeof(TChannel), address, via);

            IDuplexContextChannel duplexChannel = result as IDuplexContextChannel;
            if (duplexChannel != null)
            {
                duplexChannel.CallbackInstance = callbackInstance;
            }
            return result;
        }

        //Static functions to create channels
        private static InstanceContext GetInstanceContextForObject(object callbackObject)
        {
            if (callbackObject is InstanceContext)
            {
                return (InstanceContext)callbackObject;
            }

            return new InstanceContext(callbackObject);
        }

        public static TChannel CreateChannel(object callbackObject, Binding binding, EndpointAddress endpointAddress)
        {
            return CreateChannel(GetInstanceContextForObject(callbackObject), binding, endpointAddress);
        }

        public static TChannel CreateChannel(object callbackObject, Binding binding, EndpointAddress endpointAddress, Uri via)
        {
            return CreateChannel(GetInstanceContextForObject(callbackObject), binding, endpointAddress, via);
        }

        public static TChannel CreateChannel(InstanceContext callbackInstance, Binding binding, EndpointAddress endpointAddress)
        {
            DuplexChannelFactory<TChannel> channelFactory = new DuplexChannelFactory<TChannel>(callbackInstance, binding, endpointAddress);
            TChannel channel = channelFactory.CreateChannel();
            SetFactoryToAutoClose(channel);
            return channel;
        }

        public static TChannel CreateChannel(InstanceContext callbackInstance, Binding binding, EndpointAddress endpointAddress, Uri via)
        {
            DuplexChannelFactory<TChannel> channelFactory = new DuplexChannelFactory<TChannel>(callbackInstance, binding);
            TChannel channel = channelFactory.CreateChannel(endpointAddress, via);
            SetFactoryToAutoClose(channel);
            return channel;
        }
    }
}
