// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class WcfCodeGenerationExtension : IWcfCodeGenerationExtension
    {
        private Collection<ServiceEndpoint> endpoints;
        private bool _errorDetected;
        private bool isVB;
        private CommandProcessorOptions options;

        public WcfCodeGenerationExtension(CommandProcessorOptions options)
        {
            this.options = options;
        }

        public bool ErrorDetected
        {
            get { return _errorDetected; }
            set { _errorDetected = value; }
        }

        public void WsdlImporting(WsdlImporter importer)
        {
            RunFixups(MetadataFixup.GetPreFixups(importer), this.options);
            this.isVB = IsVB(importer);
            UpdateExitStatus(importer.Errors);
        }

        public void WsdlImported(WsdlImporter importer, Collection<ServiceEndpoint> endpoints, Collection<Binding> bindings, Collection<ContractDescription> contracts)
        {
            RunFixups(MetadataFixup.GetPostFixups(importer, endpoints, bindings, contracts), this.options);

            bool hasContractOperations = false;
            foreach (var contract in contracts)
            {
                if (contract.Operations.Count > 0)
                {
                    hasContractOperations = true;
                    break;
                }
            }

            if (endpoints.Count == 0 || bindings.Count == 0 || contracts.Count == 0 || !hasContractOperations)
            {
                MetadataConversionError error = new MetadataConversionError(SR.ErrNoCompatibleEndpoints, isWarning: true);
                if (!importer.Errors.Contains(error))
                {
                    importer.Errors.Add(error);
                }
            }

            UpdateExitStatus(importer.Errors);
            this.endpoints = endpoints;
        }

        public void ClientGenerating(ServiceContractGenerator generator)
        {
            if (generator != null)
            {
                generator.Options &= ~ServiceContractGenerationOptions.AsynchronousMethods;
                generator.Options &= ~ServiceContractGenerationOptions.EventBasedAsynchronousMethods;
                generator.Options |= ServiceContractGenerationOptions.TaskBasedAsynchronousMethod;
            }
        }

        public void ClientGenerated(ServiceContractGenerator generator)
        {
            if (generator != null)
            {
                RunFixups(CodeFixup.GetFixups(generator), this.options);
                ConfigToCode converter = new ConfigToCode() { IsVB = this.isVB };
                converter.MoveBindingsToCode(generator.TargetCompileUnit, this.endpoints);
                CodeDomVisitor.Visit(new CodeDomVisitor[] { new NamespaceFixup() }, generator.TargetCompileUnit);
                UpdateExitStatus(generator.Errors);
            }
        }

        void UpdateExitStatus(Collection<MetadataConversionError> errors)
        {
            foreach(var err in errors)
            {
                if(!err.IsWarning)
                {
                    ErrorDetected = true;
                    break;
                }
            }
        }

        static void RunFixups(IFixup[] fixups, CommandProcessorOptions options)
        {
            for (int i = 0; i < fixups.Length; i++)
            {
                fixups[i].Fixup(options);
            }
        }

        static bool IsVB(WsdlImporter importer)
        {
            if (importer.State.ContainsKey(typeof(XsdDataContractImporter)))
            {
                XsdDataContractImporter xsdImporter = (XsdDataContractImporter)importer.State[typeof(XsdDataContractImporter)];
                return string.Equals("vb", xsdImporter.Options.CodeProvider.FileExtension, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

    }
}
