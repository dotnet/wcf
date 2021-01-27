// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Description
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.CodeDom;
    using Microsoft.CodeDom.Compiler;

    public class ServiceContractGenerationContext
    {
        private readonly ServiceContractGenerator _serviceContractGenerator;
        private readonly ContractDescription _contract;
        private readonly CodeTypeDeclaration _contractType;
        private readonly CodeTypeDeclaration _duplexCallbackType;
        private readonly Collection<OperationContractGenerationContext> _operations = new Collection<OperationContractGenerationContext>();

        private CodeNamespace _codeNamespace;
        private CodeTypeDeclaration _channelType;
        private CodeTypeReference _channelTypeReference;
        private CodeTypeDeclaration _clientType;
        private CodeTypeReference _clientTypeReference;
        private CodeTypeReference _contractTypeReference;
        private CodeTypeReference _duplexCallbackTypeReference;

        private ServiceContractGenerator.CodeTypeFactory _typeFactory;

        public ServiceContractGenerationContext(ServiceContractGenerator serviceContractGenerator, ContractDescription contract, CodeTypeDeclaration contractType)
        {
            if (serviceContractGenerator == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("serviceContractGenerator"));
            if (contract == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("contract"));
            if (contractType == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("contractType"));

            _serviceContractGenerator = serviceContractGenerator;
            _contract = contract;
            _contractType = contractType;
        }

        public ServiceContractGenerationContext(ServiceContractGenerator serviceContractGenerator, ContractDescription contract, CodeTypeDeclaration contractType, CodeTypeDeclaration duplexCallbackType)
            : this(serviceContractGenerator, contract, contractType)
        {
            _duplexCallbackType = duplexCallbackType;
        }

        internal CodeTypeDeclaration ChannelType
        {
            get { return _channelType; }
            set { _channelType = value; }
        }

        internal CodeTypeReference ChannelTypeReference
        {
            get { return _channelTypeReference; }
            set { _channelTypeReference = value; }
        }

        internal CodeTypeDeclaration ClientType
        {
            get { return _clientType; }
            set { _clientType = value; }
        }

        internal CodeTypeReference ClientTypeReference
        {
            get { return _clientTypeReference; }
            set { _clientTypeReference = value; }
        }

        public ContractDescription Contract
        {
            get { return _contract; }
        }

        public CodeTypeDeclaration ContractType
        {
            get { return _contractType; }
        }

        internal CodeTypeReference ContractTypeReference
        {
            get { return _contractTypeReference; }
            set { _contractTypeReference = value; }
        }

        public CodeTypeDeclaration DuplexCallbackType
        {
            get { return _duplexCallbackType; }
        }

        internal CodeTypeReference DuplexCallbackTypeReference
        {
            get { return _duplexCallbackTypeReference; }
            set { _duplexCallbackTypeReference = value; }
        }

        internal CodeNamespace Namespace
        {
            get { return _codeNamespace; }
            set { _codeNamespace = value; }
        }

        public Collection<OperationContractGenerationContext> Operations
        {
            get { return _operations; }
        }

        public ServiceContractGenerator ServiceContractGenerator
        {
            get { return _serviceContractGenerator; }
        }

        internal ServiceContractGenerator.CodeTypeFactory TypeFactory
        {
            get { return _typeFactory; }
            set { _typeFactory = value; }
        }
    }
}
