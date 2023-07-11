// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class WcfCodeGenerationExtension : IWcfCodeGenerationExtension
    {
        private Collection<ServiceEndpoint> _endpoints;
        private bool _errorDetected;
        private bool _isVB;
        private CommandProcessorOptions _options;

        public WcfCodeGenerationExtension(CommandProcessorOptions options)
        {
            _options = options;
        }

        public bool ErrorDetected
        {
            get { return _errorDetected; }
            set { _errorDetected = value; }
        }

        public void WsdlImporting(WsdlImporter importer)
        {
            RunFixups(MetadataFixup.GetPreFixups(importer), _options);
            _isVB = IsVB(importer);
            UpdateExitStatus(importer.Errors);
        }

        public void WsdlImported(WsdlImporter importer, Collection<ServiceEndpoint> endpoints, Collection<Binding> bindings, Collection<ContractDescription> contracts)
        {
            RunFixups(MetadataFixup.GetPostFixups(importer, endpoints, bindings, contracts), _options);

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

            foreach (var binding in bindings)
            {
                if (binding is NetNamedPipeBinding && _options.Project != null && !_options.Project.TargetFrameworks.FirstOrDefault().ToLower().Contains("windows"))
                {
                    MetadataConversionError error = new MetadataConversionError(SR.WrnTargetFrameworkNotSupported_NetNamedPipe, isWarning: true);
                    if (!importer.Errors.Contains(error))
                    {
                        importer.Errors.Add(error);
                    }
                }
            }

            UpdateExitStatus(importer.Errors);
            _endpoints = endpoints;
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
                RunFixups(CodeFixup.GetFixups(generator), _options);
                ConfigToCode converter = new ConfigToCode() { IsVB = _isVB };
                converter.MoveBindingsToCode(generator.TargetCompileUnit, _endpoints);
                CodeDomVisitor.Visit(new CodeDomVisitor[] { new NamespaceFixup() }, generator.TargetCompileUnit);
                UpdateExitStatus(generator.Errors);
            }
        }

        private void UpdateExitStatus(Collection<MetadataConversionError> errors)
        {
            foreach (var err in errors)
            {
                if (!err.IsWarning)
                {
                    ErrorDetected = true;
                    break;
                }
            }
        }

        private static void RunFixups(IFixup[] fixups, CommandProcessorOptions options)
        {
            for (int i = 0; i < fixups.Length; i++)
            {
                fixups[i].Fixup(options);
            }
        }

        private static bool IsVB(WsdlImporter importer)
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
