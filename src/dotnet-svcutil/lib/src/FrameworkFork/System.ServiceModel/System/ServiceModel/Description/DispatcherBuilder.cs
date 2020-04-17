// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;

namespace System.ServiceModel.Description
{
    internal class DispatcherBuilder
    {
        internal static ClientRuntime BuildProxyBehavior(ServiceEndpoint serviceEndpoint, out BindingParameterCollection parameters)
        {
            parameters = new BindingParameterCollection();
            SecurityContractInformationEndpointBehavior.ClientInstance.AddBindingParameters(serviceEndpoint, parameters);

            AddBindingParameters(serviceEndpoint, parameters);

            ContractDescription contractDescription = serviceEndpoint.Contract;
            ClientRuntime clientRuntime = new ClientRuntime(contractDescription.Name, contractDescription.Namespace);
            clientRuntime.ContractClientType = contractDescription.ContractType;

            IdentityVerifier identityVerifier = serviceEndpoint.Binding.GetProperty<IdentityVerifier>(parameters);
            if (identityVerifier != null)
            {
                clientRuntime.IdentityVerifier = identityVerifier;
            }

            for (int i = 0; i < contractDescription.Operations.Count; i++)
            {
                OperationDescription operation = contractDescription.Operations[i];

                if (!operation.IsServerInitiated())
                {
                    DispatcherBuilder.BuildProxyOperation(operation, clientRuntime);
                }
                else
                {
                    DispatcherBuilder.BuildDispatchOperation(operation, clientRuntime.CallbackDispatchRuntime);
                }
            }

            DispatcherBuilder.ApplyClientBehavior(serviceEndpoint, clientRuntime);
            return clientRuntime;
        }

        internal class ListenUriInfo
        {
            private Uri _listenUri;
            private ListenUriMode _listenUriMode;

            public ListenUriInfo(Uri listenUri, ListenUriMode listenUriMode)
            {
                _listenUri = listenUri;
                _listenUriMode = listenUriMode;
            }

            public Uri ListenUri
            {
                get { return _listenUri; }
            }

            public ListenUriMode ListenUriMode
            {
                get { return _listenUriMode; }
            }

            // implement Equals and GetHashCode so that we can use this as a key in a dictionary
            public override bool Equals(Object other)
            {
                return this.Equals(other as ListenUriInfo);
            }

            public bool Equals(ListenUriInfo other)
            {
                if (other == null)
                {
                    return false;
                }

                if (object.ReferenceEquals(this, other))
                {
                    return true;
                }

                return (_listenUriMode == other._listenUriMode)
                    && EndpointAddress.UriEquals(_listenUri, other._listenUri, true /* ignoreCase */, true /* includeHost */);
            }

            public override int GetHashCode()
            {
                return EndpointAddress.UriGetHashCode(_listenUri, true /* includeHost */);
            }
        }

        private static void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection parameters)
        {
            foreach (IContractBehavior icb in endpoint.Contract.Behaviors)
            {
                icb.AddBindingParameters(endpoint.Contract, endpoint, parameters);
            }
            foreach (IEndpointBehavior ieb in endpoint.Behaviors)
            {
                ieb.AddBindingParameters(endpoint, parameters);
            }
            foreach (OperationDescription op in endpoint.Contract.Operations)
            {
                foreach (IOperationBehavior iob in op.Behaviors)
                {
                    iob.AddBindingParameters(op, parameters);
                }
            }
        }

        private static void BuildProxyOperation(OperationDescription operation, ClientRuntime parent)
        {
            ClientOperation child;
            if (operation.Messages.Count == 1)
            {
                child = new ClientOperation(parent, operation.Name, operation.Messages[0].Action);
            }
            else
            {
                child = new ClientOperation(parent, operation.Name, operation.Messages[0].Action,
                                            operation.Messages[1].Action);
            }
            child.TaskMethod = operation.TaskMethod;
            child.TaskTResult = operation.TaskTResult;
            child.SyncMethod = operation.SyncMethod;
            child.BeginMethod = operation.BeginMethod;
            child.EndMethod = operation.EndMethod;
            child.IsOneWay = operation.IsOneWay;
            child.IsInitiating = operation.IsInitiating;
            child.IsSessionOpenNotificationEnabled = operation.IsSessionOpenNotificationEnabled;
            for (int i = 0; i < operation.Faults.Count; i++)
            {
                FaultDescription fault = operation.Faults[i];
                child.FaultContractInfos.Add(new FaultContractInfo(fault.Action, fault.DetailType, fault.ElementName, fault.Namespace, operation.KnownTypes));
            }

            parent.Operations.Add(child);
        }

        private static void BuildDispatchOperation(OperationDescription operation, DispatchRuntime parent)
        {
            string requestAction = operation.Messages[0].Action;
            DispatchOperation child = null;
            if (operation.IsOneWay)
            {
                child = new DispatchOperation(parent, operation.Name, requestAction);
            }
            else
            {
                string replyAction = operation.Messages[1].Action;
                child = new DispatchOperation(parent, operation.Name, requestAction, replyAction);
            }

            child.HasNoDisposableParameters = operation.HasNoDisposableParameters;

            child.IsSessionOpenNotificationEnabled = operation.IsSessionOpenNotificationEnabled;
            for (int i = 0; i < operation.Faults.Count; i++)
            {
                FaultDescription fault = operation.Faults[i];
                child.FaultContractInfos.Add(new FaultContractInfo(fault.Action, fault.DetailType, fault.ElementName, fault.Namespace, operation.KnownTypes));
            }

            if (requestAction != MessageHeaders.WildcardAction)
            {
                parent.Operations.Add(child);
            }
            else
            {
                if (parent.HasMatchAllOperation)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.SFxMultipleContractStarOperations0));
                }

                parent.UnhandledDispatchOperation = child;
            }
        }

        private static void ApplyClientBehavior(ServiceEndpoint serviceEndpoint, ClientRuntime clientRuntime)
        {
            // contract behaviors
            ContractDescription contractDescription = serviceEndpoint.Contract;
            for (int i = 0; i < contractDescription.Behaviors.Count; i++)
            {
                IContractBehavior behavior = contractDescription.Behaviors[i];
                behavior.ApplyClientBehavior(contractDescription, serviceEndpoint, clientRuntime);
            }
            // endpoint behaviors
            BindingInformationEndpointBehavior.Instance.ApplyClientBehavior(serviceEndpoint, clientRuntime);
            for (int i = 0; i < serviceEndpoint.Behaviors.Count; i++)
            {
                IEndpointBehavior behavior = serviceEndpoint.Behaviors[i];
                behavior.ApplyClientBehavior(serviceEndpoint, clientRuntime);
            }
            // operation behaviors
            DispatcherBuilder.BindOperations(contractDescription, clientRuntime, null);
        }

        private static void BindOperations(ContractDescription contract, ClientRuntime proxy, DispatchRuntime dispatch)
        {
            if (!(((proxy == null) != (dispatch == null))))
            {
                throw Fx.AssertAndThrowFatal("DispatcherBuilder.BindOperations: ((proxy == null) != (dispatch == null))");
            }

            MessageDirection local = (proxy == null) ? MessageDirection.Input : MessageDirection.Output;

            for (int i = 0; i < contract.Operations.Count; i++)
            {
                OperationDescription operation = contract.Operations[i];
                MessageDescription first = operation.Messages[0];

                if (first.Direction != local)
                {
                    if (proxy == null)
                    {
                        proxy = dispatch.CallbackClientRuntime;
                    }

                    ClientOperation proxyOperation = proxy.Operations[operation.Name];
                    Fx.Assert(proxyOperation != null, "");

                    for (int j = 0; j < operation.Behaviors.Count; j++)
                    {
                        IOperationBehavior behavior = operation.Behaviors[j];
                        behavior.ApplyClientBehavior(operation, proxyOperation);
                    }
                }
                else
                {
                    if (dispatch == null)
                    {
                        dispatch = proxy.CallbackDispatchRuntime;
                    }

                    DispatchOperation dispatchOperation = null;
                    if (dispatch.Operations.Contains(operation.Name))
                    {
                        dispatchOperation = dispatch.Operations[operation.Name];
                    }
                    if (dispatchOperation == null && dispatch.UnhandledDispatchOperation != null && dispatch.UnhandledDispatchOperation.Name == operation.Name)
                    {
                        dispatchOperation = dispatch.UnhandledDispatchOperation;
                    }

                    if (dispatchOperation != null)
                    {
                        for (int j = 0; j < operation.Behaviors.Count; j++)
                        {
                            IOperationBehavior behavior = operation.Behaviors[j];
                            behavior.ApplyDispatchBehavior(operation, dispatchOperation);
                        }
                    }
                }
            }
        }

        internal class BindingInformationEndpointBehavior : IEndpointBehavior
        {
            private static BindingInformationEndpointBehavior s_instance;
            public static BindingInformationEndpointBehavior Instance
            {
                get
                {
                    if (s_instance == null)
                    {
                        s_instance = new BindingInformationEndpointBehavior();
                    }
                    return s_instance;
                }
            }
            public void Validate(ServiceEndpoint serviceEndpoint) { }
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection parameters) { }
            public void ApplyClientBehavior(ServiceEndpoint serviceEndpoint, ClientRuntime behavior)
            {
                behavior.ManualAddressing = this.IsManualAddressing(serviceEndpoint.Binding);
                behavior.EnableFaults = !this.IsMulticast(serviceEndpoint.Binding);
                if (serviceEndpoint.Contract.IsDuplex())
                {
                    behavior.CallbackDispatchRuntime.ChannelDispatcher.MessageVersion = serviceEndpoint.Binding.MessageVersion;
                }
            }
            public void ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher)
            {
                IBindingRuntimePreferences runtimePreferences = serviceEndpoint.Binding as IBindingRuntimePreferences;
                if (runtimePreferences != null)
                {
                    // it is ok to go up to the ChannelDispatcher here, since
                    // all endpoints that share a ChannelDispatcher also share the same binding
                    endpointDispatcher.ChannelDispatcher.ReceiveSynchronously = runtimePreferences.ReceiveSynchronously;
                }

                endpointDispatcher.ChannelDispatcher.ManualAddressing = this.IsManualAddressing(serviceEndpoint.Binding);
                endpointDispatcher.ChannelDispatcher.EnableFaults = !this.IsMulticast(serviceEndpoint.Binding);
                endpointDispatcher.ChannelDispatcher.MessageVersion = serviceEndpoint.Binding.MessageVersion;
            }

            private bool IsManualAddressing(Binding binding)
            {
                TransportBindingElement transport = binding.CreateBindingElements().Find<TransportBindingElement>();
                if (transport == null)
                {
                    string text = string.Format(SRServiceModel.SFxBindingMustContainTransport2, binding.Name, binding.Namespace);
                    Exception error = new InvalidOperationException(text);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(error);
                }
                return transport.ManualAddressing;
            }

            private bool IsMulticast(Binding binding)
            {
                IBindingMulticastCapabilities multicast = binding.GetProperty<IBindingMulticastCapabilities>(new BindingParameterCollection());
                return (multicast != null) && multicast.IsMulticast;
            }
        }

        internal class SecurityContractInformationEndpointBehavior : IEndpointBehavior
        {
            private bool _isForClient;
            private SecurityContractInformationEndpointBehavior(bool isForClient)
            {
                _isForClient = isForClient;
            }
            private static SecurityContractInformationEndpointBehavior s_serverInstance;
            public static SecurityContractInformationEndpointBehavior ServerInstance
            {
                get
                {
                    if (s_serverInstance == null)
                    {
                        s_serverInstance = new SecurityContractInformationEndpointBehavior(false);
                    }
                    return s_serverInstance;
                }
            }
            private static SecurityContractInformationEndpointBehavior s_clientInstance;
            public static SecurityContractInformationEndpointBehavior ClientInstance
            {
                get
                {
                    if (s_clientInstance == null)
                    {
                        s_clientInstance = new SecurityContractInformationEndpointBehavior(true);
                    }
                    return s_clientInstance;
                }
            }
            public void Validate(ServiceEndpoint serviceEndpoint) { }
            public void ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher) { }
            public void ApplyClientBehavior(ServiceEndpoint serviceEndpoint, ClientRuntime behavior) { }
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection parameters)
            {
                // get Contract info security needs, and put in BindingParameterCollection
                ISecurityCapabilities isc = null;
                BindingElementCollection elements = endpoint.Binding.CreateBindingElements();
                if (isc != null)
                {
                    // ensure existence of binding parameter
                    ChannelProtectionRequirements requirements = parameters.Find<ChannelProtectionRequirements>();
                    if (requirements == null)
                    {
                        requirements = new ChannelProtectionRequirements();
                        parameters.Add(requirements);
                    }

                    MessageEncodingBindingElement encoding = elements.Find<MessageEncodingBindingElement>();
                    // use endpoint.Binding.Version
                    if (encoding != null && encoding.MessageVersion.Addressing == AddressingVersion.None)
                    {
                        // This binding does not support response actions, so...
                        requirements.Add(ChannelProtectionRequirements.CreateFromContractAndUnionResponseProtectionRequirements(endpoint.Contract, isc, _isForClient));
                    }
                    else
                    {
                        requirements.Add(ChannelProtectionRequirements.CreateFromContract(endpoint.Contract, isc, _isForClient));
                    }
                }
            }
        }
    }
}
