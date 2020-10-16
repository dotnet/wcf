// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Description
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime;
    using System.Security;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using Microsoft.Xml;

    public abstract partial class MetadataImporter
    {
        private readonly KeyedByTypeCollection<IPolicyImportExtension> _policyExtensions;
        private readonly Dictionary<XmlQualifiedName, ContractDescription> _knownContracts = new Dictionary<XmlQualifiedName, ContractDescription>();
        private readonly Collection<MetadataConversionError> _errors = new Collection<MetadataConversionError>();
        private readonly Dictionary<object, object> _state = new Dictionary<object, object>();

        //prevent inheritance until we are ready to allow it.
        internal MetadataImporter()
            : this(null, MetadataImporterQuotas.Defaults)
        {
        }

        internal MetadataImporter(IEnumerable<IPolicyImportExtension> policyImportExtensions)
            : this(policyImportExtensions, MetadataImporterQuotas.Defaults)
        {
        }

        internal MetadataImporter(IEnumerable<IPolicyImportExtension> policyImportExtensions,
            MetadataImporterQuotas quotas)
        {
            if (quotas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("quotas");
            }

            if (policyImportExtensions == null)
            {
                policyImportExtensions = LoadPolicyExtensionsFromConfig();
            }

            this.Quotas = quotas;
            _policyExtensions = new KeyedByTypeCollection<IPolicyImportExtension>(policyImportExtensions);
        }

        public KeyedByTypeCollection<IPolicyImportExtension> PolicyImportExtensions
        {
            get { return _policyExtensions; }
        }

        public Collection<MetadataConversionError> Errors
        {
            get { return _errors; }
        }

        public Dictionary<object, object> State
        {
            get { return _state; }
        }

        public Dictionary<XmlQualifiedName, ContractDescription> KnownContracts
        {
            get { return _knownContracts; }
        }

        // Abstract Building Methods
        public abstract Collection<ContractDescription> ImportAllContracts();
        public abstract ServiceEndpointCollection ImportAllEndpoints();

        internal virtual XmlElement ResolvePolicyReference(string policyReference, XmlElement contextAssertion)
        {
            return null;
        }
        internal BindingElementCollection ImportPolicy(ServiceEndpoint endpoint, Collection<Collection<XmlElement>> policyAlternatives)
        {
            foreach (Collection<XmlElement> selectedPolicy in policyAlternatives)
            {
                BindingOnlyPolicyConversionContext policyConversionContext = new BindingOnlyPolicyConversionContext(endpoint, selectedPolicy);

                if (TryImportPolicy(policyConversionContext))
                {
                    return policyConversionContext.BindingElements;
                }
            }
            return null;
        }

        internal bool TryImportPolicy(PolicyConversionContext policyContext)
        {
            foreach (IPolicyImportExtension policyImporter in _policyExtensions)
            {
                try
                {
                    policyImporter.ImportPolicy(this, policyContext);
                }
#pragma warning disable 56500 // covered by FxCOP
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                        throw;
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateExtensionException(policyImporter, e));
                }
            }

            if (policyContext.GetBindingAssertions().Count != 0)
            {
                return false;
            }

            foreach (OperationDescription operation in policyContext.Contract.Operations)
            {
                if (policyContext.GetOperationBindingAssertions(operation).Count != 0)
                {
                    return false;
                }

                foreach (MessageDescription message in operation.Messages)
                {
                    if (policyContext.GetMessageBindingAssertions(message).Count != 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        [SecuritySafeCritical]
        private static Collection<IPolicyImportExtension> LoadPolicyExtensionsFromConfig()
        {
            throw new NotImplementedException();
        }

        private Exception CreateExtensionException(IPolicyImportExtension importer, Exception e)
        {
            string errorMessage = string.Format(SRServiceModel.PolicyExtensionImportError, importer.GetType(), e.Message);
            return new InvalidOperationException(errorMessage, e);
        }

        internal class BindingOnlyPolicyConversionContext : PolicyConversionContext
        {
            private static readonly PolicyAssertionCollection s_noPolicy = new PolicyAssertionCollection();
            private readonly BindingElementCollection _bindingElements = new BindingElementCollection();
            private readonly PolicyAssertionCollection _bindingPolicy;

            internal BindingOnlyPolicyConversionContext(ServiceEndpoint endpoint, IEnumerable<XmlElement> bindingPolicy)
                : base(endpoint)
            {
                _bindingPolicy = new PolicyAssertionCollection(bindingPolicy);
            }

            public override BindingElementCollection BindingElements { get { return _bindingElements; } }

            public override PolicyAssertionCollection GetBindingAssertions()
            {
                return _bindingPolicy;
            }

            public override PolicyAssertionCollection GetOperationBindingAssertions(OperationDescription operation)
            {
                return s_noPolicy;
            }

            public override PolicyAssertionCollection GetMessageBindingAssertions(MessageDescription message)
            {
                return s_noPolicy;
            }

            public override PolicyAssertionCollection GetFaultBindingAssertions(FaultDescription fault)
            {
                return s_noPolicy;
            }
        }
    }
}
