// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Description
{
    using Microsoft.Xml;

    using System.ServiceModel.Channels;

    using Microsoft.Xml.Schema;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using WsdlNS = System.Web.Services.Description;

    // This class is created as part of the export process and passed to 
    // Wsdlmporter and WsdlExporter implementations as a utility for
    // Correlating between the WSDL OM and the Indigo OM
    // in the conversion process.    
    public class WsdlContractConversionContext
    {
        private readonly ContractDescription _contract;
        private readonly WsdlNS.PortType _wsdlPortType;

        private readonly Dictionary<OperationDescription, WsdlNS.Operation> _wsdlOperations;
        private readonly Dictionary<WsdlNS.Operation, OperationDescription> _operationDescriptions;
        private readonly Dictionary<MessageDescription, WsdlNS.OperationMessage> _wsdlOperationMessages;
        private readonly Dictionary<FaultDescription, WsdlNS.OperationFault> _wsdlOperationFaults;
        private readonly Dictionary<WsdlNS.OperationMessage, MessageDescription> _messageDescriptions;
        private readonly Dictionary<WsdlNS.OperationFault, FaultDescription> _faultDescriptions;
        private readonly Dictionary<WsdlNS.Operation, Collection<WsdlNS.OperationBinding>> _operationBindings;

        internal WsdlContractConversionContext(ContractDescription contract, WsdlNS.PortType wsdlPortType)
        {
            _contract = contract;
            _wsdlPortType = wsdlPortType;

            _wsdlOperations = new Dictionary<OperationDescription, WsdlNS.Operation>();
            _operationDescriptions = new Dictionary<WsdlNS.Operation, OperationDescription>();
            _wsdlOperationMessages = new Dictionary<MessageDescription, WsdlNS.OperationMessage>();
            _messageDescriptions = new Dictionary<WsdlNS.OperationMessage, MessageDescription>();
            _wsdlOperationFaults = new Dictionary<FaultDescription, WsdlNS.OperationFault>();
            _faultDescriptions = new Dictionary<WsdlNS.OperationFault, FaultDescription>();
            _operationBindings = new Dictionary<WsdlNS.Operation, Collection<WsdlNS.OperationBinding>>();
        }

        internal IEnumerable<IWsdlExportExtension> ExportExtensions
        {
            get
            {
                foreach (IWsdlExportExtension extension in _contract.Behaviors.FindAll<IWsdlExportExtension>())
                {
                    yield return extension;
                }

                foreach (OperationDescription operation in _contract.Operations)
                {
                    if (!WsdlExporter.OperationIsExportable(operation))
                    {
                        continue;
                    }

                    // In 3.0SP1, the DCSOB and XSOB were moved from before to after the custom behaviors.  For
                    // IWsdlExportExtension compat, run them in the pre-SP1 order.
                    // TEF QFE 367607
                    Collection<IWsdlExportExtension> extensions = operation.Behaviors.FindAll<IWsdlExportExtension>();
                    for (int i = 0; i < extensions.Count;)
                    {
                        if (WsdlExporter.IsBuiltInOperationBehavior(extensions[i]))
                        {
                            yield return extensions[i];
                            extensions.RemoveAt(i);
                        }
                        else
                        {
                            i++;
                        }
                    }
                    foreach (IWsdlExportExtension extension in extensions)
                    {
                        yield return extension;
                    }
                }
            }
        }


        public ContractDescription Contract { get { return _contract; } }
        public WsdlNS.PortType WsdlPortType { get { return _wsdlPortType; } }

        public WsdlNS.Operation GetOperation(OperationDescription operation)
        {
            return _wsdlOperations[operation];
        }

        public WsdlNS.OperationMessage GetOperationMessage(MessageDescription message)
        {
            return _wsdlOperationMessages[message];
        }

        public WsdlNS.OperationFault GetOperationFault(FaultDescription fault)
        {
            return _wsdlOperationFaults[fault];
        }

        public OperationDescription GetOperationDescription(WsdlNS.Operation operation)
        {
            return _operationDescriptions[operation];
        }

        public MessageDescription GetMessageDescription(WsdlNS.OperationMessage operationMessage)
        {
            return _messageDescriptions[operationMessage];
        }

        public FaultDescription GetFaultDescription(WsdlNS.OperationFault operationFault)
        {
            return _faultDescriptions[operationFault];
        }

        // --------------------------------------------------------------------------------------------------

        internal void AddOperation(OperationDescription operationDescription, WsdlNS.Operation wsdlOperation)
        {
            _wsdlOperations.Add(operationDescription, wsdlOperation);
            _operationDescriptions.Add(wsdlOperation, operationDescription);
        }

        internal void AddMessage(MessageDescription messageDescription, WsdlNS.OperationMessage wsdlOperationMessage)
        {
            _wsdlOperationMessages.Add(messageDescription, wsdlOperationMessage);
            _messageDescriptions.Add(wsdlOperationMessage, messageDescription);
        }

        internal void AddFault(FaultDescription faultDescription, WsdlNS.OperationFault wsdlOperationFault)
        {
            _wsdlOperationFaults.Add(faultDescription, wsdlOperationFault);
            _faultDescriptions.Add(wsdlOperationFault, faultDescription);
        }

        internal Collection<WsdlNS.OperationBinding> GetOperationBindings(WsdlNS.Operation operation)
        {
            Collection<WsdlNS.OperationBinding> bindings;
            if (!_operationBindings.TryGetValue(operation, out bindings))
            {
                bindings = new Collection<WsdlNS.OperationBinding>();
                WsdlNS.ServiceDescriptionCollection wsdlDocuments = WsdlPortType.ServiceDescription.ServiceDescriptions;
                foreach (WsdlNS.ServiceDescription wsdl in wsdlDocuments)
                {
                    foreach (WsdlNS.Binding wsdlBinding in wsdl.Bindings)
                    {
                        if (wsdlBinding.Type.Name == WsdlPortType.Name && wsdlBinding.Type.Namespace == WsdlPortType.ServiceDescription.TargetNamespace)
                        {
                            foreach (WsdlNS.OperationBinding operationBinding in wsdlBinding.Operations)
                            {
                                if (WsdlImporter.Binding2DescriptionHelper.Match(operationBinding, operation) != WsdlImporter.Binding2DescriptionHelper.MatchResult.None)
                                {
                                    bindings.Add(operationBinding);
                                    break;
                                }
                            }
                        }
                    }
                }
                _operationBindings.Add(operation, bindings);
            }
            return bindings;
        }
    }
}
