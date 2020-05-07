// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Description
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeDom;
    using Microsoft.CodeDom.Compiler;
    using System.ServiceModel;

    public class OperationContractGenerationContext
    {
        private readonly CodeMemberMethod _syncMethod;
        private readonly CodeMemberMethod _beginMethod;
        private readonly ServiceContractGenerationContext _contract;
        private readonly CodeMemberMethod _endMethod;
        private readonly OperationDescription _operation;
        private readonly ServiceContractGenerator _serviceContractGenerator;
        private readonly CodeTypeDeclaration _declaringType;
        private readonly CodeMemberMethod _taskMethod;

        private CodeTypeReference _declaringTypeReference;


        private OperationContractGenerationContext(ServiceContractGenerator serviceContractGenerator, ServiceContractGenerationContext contract, OperationDescription operation, CodeTypeDeclaration declaringType)
        {
            if (serviceContractGenerator == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("serviceContractGenerator"));
            if (contract == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("contract"));
            if (declaringType == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("declaringType"));

            _serviceContractGenerator = serviceContractGenerator;
            _contract = contract;
            _operation = operation;
            _declaringType = declaringType;
        }

        public OperationContractGenerationContext(ServiceContractGenerator serviceContractGenerator, ServiceContractGenerationContext contract, OperationDescription operation, CodeTypeDeclaration declaringType, CodeMemberMethod syncMethod, CodeMemberMethod beginMethod, CodeMemberMethod endMethod, CodeMemberMethod taskMethod)
            : this(serviceContractGenerator, contract, operation, declaringType)
        {
            if (syncMethod == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("syncMethod"));
            if (beginMethod == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("beginMethod"));
            if (endMethod == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("endMethod"));
            if (taskMethod == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("taskMethod"));

            _syncMethod = syncMethod;
            _beginMethod = beginMethod;
            _endMethod = endMethod;
            _taskMethod = taskMethod;
        }

        public OperationContractGenerationContext(ServiceContractGenerator serviceContractGenerator, ServiceContractGenerationContext contract, OperationDescription operation, CodeTypeDeclaration declaringType, CodeMemberMethod syncMethod, CodeMemberMethod beginMethod, CodeMemberMethod endMethod)
            : this(serviceContractGenerator, contract, operation, declaringType)
        {
            if (syncMethod == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("syncMethod"));
            if (beginMethod == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("beginMethod"));
            if (endMethod == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("endMethod"));

            _syncMethod = syncMethod;
            _beginMethod = beginMethod;
            _endMethod = endMethod;
        }

        public OperationContractGenerationContext(ServiceContractGenerator serviceContractGenerator, ServiceContractGenerationContext contract, OperationDescription operation, CodeTypeDeclaration declaringType, CodeMemberMethod syncMethod, CodeMemberMethod taskMethod)
            : this(serviceContractGenerator, contract, operation, declaringType)
        {
            if (syncMethod == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("syncMethod"));
            if (taskMethod == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("taskMethod"));

            _syncMethod = syncMethod;
            _taskMethod = taskMethod;
        }

        public OperationContractGenerationContext(ServiceContractGenerator serviceContractGenerator, ServiceContractGenerationContext contract, OperationDescription operation, CodeTypeDeclaration declaringType, CodeMemberMethod method)
            : this(serviceContractGenerator, contract, operation, declaringType)
        {
            if (method == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("method"));

            _syncMethod = method;
            _beginMethod = null;
            _endMethod = null;
        }

        public ServiceContractGenerationContext Contract
        {
            get { return _contract; }
        }

        public CodeTypeDeclaration DeclaringType
        {
            get { return _declaringType; }
        }

        internal CodeTypeReference DeclaringTypeReference
        {
            get { return _declaringTypeReference; }
            set { _declaringTypeReference = value; }
        }

        public CodeMemberMethod BeginMethod
        {
            get { return _beginMethod; }
        }

        public CodeMemberMethod EndMethod
        {
            get { return _endMethod; }
        }

        public CodeMemberMethod TaskMethod
        {
            get { return _taskMethod; }
        }

        public CodeMemberMethod SyncMethod
        {
            get { return _syncMethod; }
        }

        public bool IsAsync
        {
            get { return _beginMethod != null; }
        }

        public bool IsTask
        {
            get { return _taskMethod != null; }
        }

        // true if this operation was declared somewhere up the hierarchy (rather than at this level)
        internal bool IsInherited
        {
            get { return !(_declaringType == _contract.ContractType || _declaringType == _contract.DuplexCallbackType); }
        }

        public OperationDescription Operation
        {
            get { return _operation; }
        }

        public ServiceContractGenerator ServiceContractGenerator
        {
            get { return _serviceContractGenerator; }
        }
    }
}
