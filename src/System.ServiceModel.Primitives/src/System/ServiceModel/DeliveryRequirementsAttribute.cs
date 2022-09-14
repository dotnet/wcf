// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;

namespace System.ServiceModel
{
    // This attribute specifies what the service implementation 
    // requires from the binding that dispatches messages.
    [AttributeUsage(ServiceModelAttributeTargets.ContractBehavior, AllowMultiple = true)]
    public sealed class DeliveryRequirementsAttribute : Attribute, IContractBehavior
    {
        private QueuedDeliveryRequirementsMode queuedDeliveryRequirements = QueuedDeliveryRequirementsMode.Allowed;

        // RequireQueuedDelivery: Validates that any binding associated
        // with the service/channel supports Queued
        // delivery.
        //
        // DisallowQueuedDelivery: Validates that no binding associated
        // with the service/channel supports Queued     
        // delivery.
        //
        // Ignore: Agnostic
        public QueuedDeliveryRequirementsMode QueuedDeliveryRequirements
        {
            get { return queuedDeliveryRequirements; }
            set
            {
                if (QueuedDeliveryRequirementsModeHelper.IsDefined(value))
                {
                    queuedDeliveryRequirements = value;
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
                }
            }
        }

        // True: Validates that any binding associated
        // with the service/channel supports Ordered 
        // delivery. 
        //
        // False: Does no validation.
        public bool RequireOrderedDelivery { get; set; } = false;

        void IContractBehavior.Validate(ContractDescription description, ServiceEndpoint endpoint)
        {
            if (description == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(description));
            if (endpoint == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(endpoint));

            ValidateEndpoint(endpoint);
        }

        void IContractBehavior.AddBindingParameters(ContractDescription description, ServiceEndpoint endpoint, BindingParameterCollection parameters)
        {
        }

        void IContractBehavior.ApplyClientBehavior(ContractDescription description, ServiceEndpoint endpoint, ClientRuntime proxy)
        {
        }

        void IContractBehavior.ApplyDispatchBehavior(ContractDescription description, ServiceEndpoint endpoint, DispatchRuntime dispatch)
        {
        }

        private void ValidateEndpoint(ServiceEndpoint endpoint)
        {
            string name = endpoint.Contract.ContractType.Name;
            EnsureQueuedDeliveryRequirements(name, endpoint.Binding);
            EnsureOrderedDeliveryRequirements(name, endpoint.Binding);
        }

        private void EnsureQueuedDeliveryRequirements(string name, Binding binding)
        {
            if (QueuedDeliveryRequirements == QueuedDeliveryRequirementsMode.Required
                || QueuedDeliveryRequirements == QueuedDeliveryRequirementsMode.NotAllowed)
            {
                IBindingDeliveryCapabilities caps = binding.GetProperty<IBindingDeliveryCapabilities>(new BindingParameterCollection());
                if (caps == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        SRP.Format(SRP.SinceTheBindingForDoesnTSupportIBindingCapabilities2_1, name)));
                }
                else
                {
                    bool queuedTransport = caps.QueuedDelivery;
                    if (QueuedDeliveryRequirements == QueuedDeliveryRequirementsMode.Required && !queuedTransport)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                            SRP.Format(SRP.BindingRequirementsAttributeRequiresQueuedDelivery1, name)));
                    }
                    else if (QueuedDeliveryRequirements == QueuedDeliveryRequirementsMode.NotAllowed && queuedTransport)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                            SRP.Format(SRP.BindingRequirementsAttributeDisallowsQueuedDelivery1, name)));
                    }
                }
            }
        }

        private void EnsureOrderedDeliveryRequirements(string name, Binding binding)
        {
            if (RequireOrderedDelivery)
            {
                IBindingDeliveryCapabilities caps = binding.GetProperty<IBindingDeliveryCapabilities>(new BindingParameterCollection());
                if (caps == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        SRP.Format(SRP.SinceTheBindingForDoesnTSupportIBindingCapabilities1_1, name)));
                }
                else
                {
                    if (!caps.AssuresOrderedDelivery)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                            SRP.Format(SRP.TheBindingForDoesnTSupportOrderedDelivery1, name)));
                    }
                }
            }
        }
    }
}

