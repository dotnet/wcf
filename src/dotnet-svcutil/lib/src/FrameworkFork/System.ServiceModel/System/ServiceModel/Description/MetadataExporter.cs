// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Description
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime;
    using System.ServiceModel.Channels;

    //For export we provide a builder that allows the gradual construction of a set of MetadataDocuments
    public abstract class MetadataExporter
    {
        private PolicyVersion _policyVersion = PolicyVersion.Policy12;
        private readonly Collection<MetadataConversionError> _errors = new Collection<MetadataConversionError>();
        private readonly Dictionary<object, object> _state = new Dictionary<object, object>();

        //prevent inheritance until we are ready to allow it.
        internal MetadataExporter()
        {
        }

        public PolicyVersion PolicyVersion
        {
            get
            {
                return _policyVersion;
            }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                _policyVersion = value;
            }
        }

        public Collection<MetadataConversionError> Errors
        {
            get { return _errors; }
        }
        public Dictionary<object, object> State
        {
            get { return _state; }
        }

        public abstract void ExportContract(ContractDescription contract);
        public abstract void ExportEndpoint(ServiceEndpoint endpoint);

        public abstract MetadataSet GetGeneratedMetadata();

        internal PolicyConversionContext ExportPolicy(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            PolicyConversionContext policyContext = new ExportedPolicyConversionContext(endpoint, bindingParameters);

            foreach (IPolicyExportExtension exporter in endpoint.Binding.CreateBindingElements().FindAll<IPolicyExportExtension>())
                try
                {
                    exporter.ExportPolicy(this, policyContext);
                }
#pragma warning disable 56500 // covered by FxCOP
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                        throw;
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateExtensionException(exporter, e));
                }

            return policyContext;
        }

        protected internal PolicyConversionContext ExportPolicy(ServiceEndpoint endpoint)
        {
            return this.ExportPolicy(endpoint, null);
        }

        private sealed class ExportedPolicyConversionContext : PolicyConversionContext
        {
            private readonly BindingElementCollection _bindingElements;
            private PolicyAssertionCollection _bindingAssertions;
            private Dictionary<OperationDescription, PolicyAssertionCollection> _operationBindingAssertions;
            private Dictionary<MessageDescription, PolicyAssertionCollection> _messageBindingAssertions;
            private Dictionary<FaultDescription, PolicyAssertionCollection> _faultBindingAssertions;
            private BindingParameterCollection _bindingParameters;

            internal ExportedPolicyConversionContext(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
                : base(endpoint)
            {
                _bindingElements = endpoint.Binding.CreateBindingElements();
                _bindingAssertions = new PolicyAssertionCollection();
                _operationBindingAssertions = new Dictionary<OperationDescription, PolicyAssertionCollection>();
                _messageBindingAssertions = new Dictionary<MessageDescription, PolicyAssertionCollection>();
                _faultBindingAssertions = new Dictionary<FaultDescription, PolicyAssertionCollection>();
                _bindingParameters = bindingParameters;
            }

            public override BindingElementCollection BindingElements
            {
                get { return _bindingElements; }
            }

            internal override BindingParameterCollection BindingParameters
            {
                get { return _bindingParameters; }
            }

            public override PolicyAssertionCollection GetBindingAssertions()
            {
                return _bindingAssertions;
            }

            public override PolicyAssertionCollection GetOperationBindingAssertions(OperationDescription operation)
            {
                lock (_operationBindingAssertions)
                {
                    if (!_operationBindingAssertions.ContainsKey(operation))
                        _operationBindingAssertions.Add(operation, new PolicyAssertionCollection());
                }

                return _operationBindingAssertions[operation];
            }

            public override PolicyAssertionCollection GetMessageBindingAssertions(MessageDescription message)
            {
                lock (_messageBindingAssertions)
                {
                    if (!_messageBindingAssertions.ContainsKey(message))
                        _messageBindingAssertions.Add(message, new PolicyAssertionCollection());
                }
                return _messageBindingAssertions[message];
            }

            public override PolicyAssertionCollection GetFaultBindingAssertions(FaultDescription fault)
            {
                lock (_faultBindingAssertions)
                {
                    if (!_faultBindingAssertions.ContainsKey(fault))
                        _faultBindingAssertions.Add(fault, new PolicyAssertionCollection());
                }
                return _faultBindingAssertions[fault];
            }
        }

        private Exception CreateExtensionException(IPolicyExportExtension exporter, Exception e)
        {
            string errorMessage = string.Format(SRServiceModel.PolicyExtensionExportError, exporter.GetType(), e.Message);
            return new InvalidOperationException(errorMessage, e);
        }
    }
}
