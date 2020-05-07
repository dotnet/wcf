// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.CodeDom;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class ConfigToCode
    {
        public bool IsVB { get; set; }

        public void MoveBindingsToCode(CodeCompileUnit codeCompileUnit, Collection<ServiceEndpoint> endpoints)
        {
            foreach (CodeNamespace namespaceDecl in codeCompileUnit.Namespaces)
            {
                foreach (var clientType in FindClientTypes(namespaceDecl))
                {
                    this.MoveConfigIntoCode(clientType, endpoints, namespaceDecl);
                }
            }
        }

        private void MoveConfigIntoCode(CodeTypeDeclaration clientType, Collection<ServiceEndpoint> endpoints, CodeNamespace namespaceDecl)
        {
            string contractName = ExtractContract(clientType);
            List<string> endpointNames = new List<string>();
            MethodCreationHelper helperMethodCreator = new MethodCreationHelper(clientType);

            foreach (ServiceEndpoint endpoint in endpoints)
            {
                if (contractName.EndsWith(endpoint.Contract.Name, StringComparison.Ordinal) || contractName.EndsWith(UniqueCodeIdentifierScope.MakeValid(endpoint.Contract.CodeName, endpoint.Contract.CodeName), StringComparison.Ordinal))
                {
                    endpoint.Name = CodeDomHelpers.GetValidValueTypeIdentifier(endpoint.Name);

                    // resolve duplicated names.
                    int i = 1;
                    while (endpointNames.Contains(endpoint.Name))
                    {
                        endpoint.Name += i;
                        i++;
                    }

                    if (helperMethodCreator.AddClientEndpoint(endpoint))
                    {
                        endpointNames.Add(endpoint.Name);
                    }
                }
            }

            bool endpointConfigurationExists = false;
            if (endpointNames.Count > 0)
            {
                helperMethodCreator.AddConfigurationEnum(endpointNames);
                helperMethodCreator.AddMethods(endpointNames, this.IsVB);
                endpointConfigurationExists = true;
            }

            bool shouldRemoveDefault = (endpointNames.Count != 1);
            this.FixupConstructors(clientType, shouldRemoveDefault, endpointConfigurationExists, namespaceDecl, endpointNames);
        }

        private static void AddConfigEndpoint(CodeConstructor ctor, List<string> endpointNames)
        {
            if (endpointNames != null)
            {
                // Use the default endpoint name
                ctor.Statements.Add(new CodeAssignStatement(
                    new CodePropertyReferenceExpression(
                        new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), ConfigToCodeConstants.EndpointPropertyName),
                        ConfigToCodeConstants.EndpointNamePropertyName),
                    new CodeMethodInvokeExpression(
                        new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(ConfigToCodeConstants.EndpointConfigurationEnumTypeName), endpointNames[0]),
                        ConfigToCodeConstants.ToStringMethod)));
            }
            else
            {
                ctor.Statements.Add(new CodeAssignStatement(
                    new CodePropertyReferenceExpression(
                        new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), ConfigToCodeConstants.EndpointPropertyName),
                        ConfigToCodeConstants.EndpointNamePropertyName),
                    new CodeMethodInvokeExpression(
                        new CodeVariableReferenceExpression(ConfigToCodeConstants.EndpointConfigurationParameter),
                        ConfigToCodeConstants.ToStringMethod)));
            }

            ctor.Statements.Add(new CodeMethodInvokeExpression(
             new CodeMethodReferenceExpression(
                 null, "ConfigureEndpoint"),
                 new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), ConfigToCodeConstants.EndpointPropertyName),
                 new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), ConfigToCodeConstants.ClientCredentialsPropertyName)));
        }

        private void FixupConstructors(CodeTypeDeclaration clientType, bool shouldRemoveDefault, bool endpointConfigurationExists, CodeNamespace namespaceDecl, List<string> endpointNames)
        {
            if (string.Equals(clientType.BaseTypes[0].BaseType, ConfigToCodeConstants.ClientBaseOfTBaseName, StringComparison.Ordinal))
            {
                FixupConstructorsNonDuplex(clientType, shouldRemoveDefault, endpointConfigurationExists, endpointNames);
            }
            else if (string.Equals(clientType.BaseTypes[0].BaseType, ConfigToCodeConstants.DuplexClientBaseOfTBaseName, StringComparison.Ordinal))
            {
                this.FixupConstructorsDuplex(clientType, shouldRemoveDefault, endpointConfigurationExists, namespaceDecl, endpointNames);
            }
        }

        private static void FixupConstructorsEventBasedDuplex(CodeTypeDeclaration clientType, bool shouldRemoveDefault, bool endpointConfigurationExists)
        {
            IList<CodeTypeMember> toRemoves = new List<CodeTypeMember>();
            foreach (CodeTypeMember member in clientType.Members)
            {
                CodeConstructor ctor = member as CodeConstructor;
                if (ctor != null)
                {
                    // Fix the constructors added by CreateCallbackImpl.
                    // public TestClient() : 
                    if (ctor.Parameters.Count == 0)
                    {
                        if (shouldRemoveDefault)
                        {
                            toRemoves.Add(ctor);
                        }
                    }
                    // private TestClient(DuplexServiceClientCallback callbackImpl)
                    else if (ctor.Parameters.Count == 1 && !string.Equals(ctor.Parameters[0].Type.BaseType, typeof(string).FullName, StringComparison.Ordinal))
                    {
                        if (shouldRemoveDefault)
                        {
                            toRemoves.Add(ctor);
                        }
                    }
                    // public TestClient(string endpointConfiguration)  => TestClient(Configuration endpointConfiguration)
                    else if (ctor.Parameters.Count == 1 && string.Equals(ctor.Parameters[0].Type.BaseType, typeof(string).FullName, StringComparison.Ordinal))
                    {
                        if (endpointConfigurationExists)
                        {
                            ctor.Parameters[0] = new CodeParameterDeclarationExpression(
                                ConfigToCodeConstants.EndpointConfigurationEnumTypeName,
                                ConfigToCodeConstants.EndpointConfigurationParameter);
                            ctor.ChainedConstructorArgs[1] = new CodeVariableReferenceExpression(ConfigToCodeConstants.EndpointConfigurationParameter);
                        }
                        else
                        {
                            toRemoves.Add(ctor);
                        }
                    }

                    // private TestClient(DuplexServiceClientCallback callbackImpl, string endpointConfiguration) => TestClient(DuplexServiceClientCallback callbackImpl, Configuration endpointConfiguration)
                    else if (ctor.Parameters.Count == 2 && string.Equals(ctor.Parameters[1].Type.BaseType, typeof(string).FullName, StringComparison.Ordinal))
                    {
                        if (endpointConfigurationExists)
                        {
                            ctor.Parameters[1] = new CodeParameterDeclarationExpression(
                               ConfigToCodeConstants.EndpointConfigurationEnumTypeName,
                               ConfigToCodeConstants.EndpointConfigurationParameter);
                        }
                        else
                        {
                            toRemoves.Add(ctor);
                        }
                    }
                }
            }

            if (toRemoves.Count > 0)
            {
                foreach (CodeTypeMember member in toRemoves)
                {
                    clientType.Members.Remove(member);
                }
            }
        }

        private static void FixupConstructorsNonDuplex(CodeTypeDeclaration clientType, bool shouldRemoveDefault, bool endpointConfigurationExists, List<string> endpointNames)
        {
            IList<CodeTypeMember> toRemoves = new List<CodeTypeMember>();
            foreach (CodeTypeMember member in clientType.Members)
            {
                CodeConstructor ctor = member as CodeConstructor;
                if (ctor != null)
                {
                    if (ctor.Parameters.Count == 0)
                    {
                        if (!shouldRemoveDefault)
                        {
                            // TestClient()
                            ctor.BaseConstructorArgs.Add(
                                new CodeMethodInvokeExpression(
                                    new CodeTypeReferenceExpression(clientType.Name),
                                    ConfigToCodeConstants.GetDefaultBindingMethod));
                            ctor.BaseConstructorArgs.Add(
                                new CodeMethodInvokeExpression(
                                    new CodeTypeReferenceExpression(clientType.Name),
                                    ConfigToCodeConstants.GetDefaultEndpointAddressMethod));

                            AddConfigEndpoint(ctor, endpointNames);
                        }
                        else
                        {
                            toRemoves.Add(ctor);
                        }
                    }
                    else if (ctor.Parameters.Count == 1 && string.Equals(ctor.Parameters[0].Type.BaseType, typeof(string).FullName, StringComparison.Ordinal))
                    {
                        if (endpointConfigurationExists)
                        {
                            // TestClient(string endpointConfigurationName) => TestClient(Configuration endpointConfiguration)
                            ctor.Parameters.Clear();
                            ctor.Parameters.Add(new CodeParameterDeclarationExpression(
                                ConfigToCodeConstants.EndpointConfigurationEnumTypeName,
                                ConfigToCodeConstants.EndpointConfigurationParameter));

                            ctor.BaseConstructorArgs.Clear();
                            ctor.BaseConstructorArgs.Add(
                                new CodeMethodInvokeExpression(
                                    new CodeTypeReferenceExpression(clientType.Name),
                                    ConfigToCodeConstants.GetBindingMethod,
                                    new CodeArgumentReferenceExpression(ConfigToCodeConstants.EndpointConfigurationParameter)));
                            ctor.BaseConstructorArgs.Add(
                                new CodeMethodInvokeExpression(
                                    new CodeTypeReferenceExpression(clientType.Name),
                                    ConfigToCodeConstants.GetEndpointAddressMethod,
                                    new CodeArgumentReferenceExpression(ConfigToCodeConstants.EndpointConfigurationParameter)));

                            AddConfigEndpoint(ctor, null);
                        }
                        else
                        {
                            toRemoves.Add(ctor);
                        }
                    }
                    else if (ctor.Parameters.Count == 2 && string.Equals(ctor.Parameters[0].Type.BaseType, typeof(string).FullName, StringComparison.Ordinal)
                        && string.Equals(ctor.Parameters[1].Type.BaseType, typeof(string).FullName, StringComparison.Ordinal))
                    {
                        if (endpointConfigurationExists)
                        {
                            // TestClient(string endpointConfigurationName, string remoteAddress) => TestClient(Configuration endpointConfiguration, string remoteAddress)
                            ctor.Parameters[0] = new CodeParameterDeclarationExpression(
                                ConfigToCodeConstants.EndpointConfigurationEnumTypeName,
                                ConfigToCodeConstants.EndpointConfigurationParameter);

                            string remoteAddressParam = ctor.Parameters[1].Name;
                            ctor.BaseConstructorArgs.Clear();
                            ctor.BaseConstructorArgs.Add(
                                new CodeMethodInvokeExpression(
                                    new CodeTypeReferenceExpression(clientType.Name),
                                    ConfigToCodeConstants.GetBindingMethod,
                                    new CodeArgumentReferenceExpression(ConfigToCodeConstants.EndpointConfigurationParameter)));
                            ctor.BaseConstructorArgs.Add(
                                new CodeObjectCreateExpression(
                                    typeof(EndpointAddress),
                                    new CodeArgumentReferenceExpression(remoteAddressParam)));

                            AddConfigEndpoint(ctor, null);
                        }
                        else
                        {
                            toRemoves.Add(ctor);
                        }
                    }
                    else if (ctor.Parameters.Count == 2 && string.Equals(ctor.Parameters[0].Type.BaseType, typeof(string).FullName, StringComparison.Ordinal) &&
                        string.Equals(ctor.Parameters[1].Type.BaseType, typeof(EndpointAddress).FullName, StringComparison.Ordinal))
                    {
                        if (endpointConfigurationExists)
                        {
                            // TestClient(string endpointConfigurationName, EndpointAddress remoteAddress) => TestClient(Configuration endpointConfiguration, EndpointAddress remoteAddress)
                            ctor.Parameters[0] = new CodeParameterDeclarationExpression(
                                ConfigToCodeConstants.EndpointConfigurationEnumTypeName,
                                ConfigToCodeConstants.EndpointConfigurationParameter);

                            string remoteAddressParam = ctor.Parameters[1].Name;
                            ctor.BaseConstructorArgs.Clear();
                            ctor.BaseConstructorArgs.Add(
                                new CodeMethodInvokeExpression(
                                    new CodeTypeReferenceExpression(clientType.Name),
                                    ConfigToCodeConstants.GetBindingMethod,
                                    new CodeArgumentReferenceExpression(ConfigToCodeConstants.EndpointConfigurationParameter)));
                            ctor.BaseConstructorArgs.Add(
                                new CodeArgumentReferenceExpression(remoteAddressParam));

                            AddConfigEndpoint(ctor, null);
                        }
                        else
                        {
                            toRemoves.Add(ctor);
                        }
                    }
                }
            }

            if (toRemoves.Count > 0)
            {
                foreach (CodeTypeMember member in toRemoves)
                {
                    clientType.Members.Remove(member);
                }
            }
        }

        private void FixupConstructorsDuplex(CodeTypeDeclaration clientType, bool shouldRemoveDefault, bool endpointConfigurationExists, CodeNamespace namespaceDecl, List<string> endpointNames)
        {
            // "New" duplex constructors use chained constructor calling ( DuplexTestClient(...) : this(...) ), so they
            //   don't need to be modified
            IList<CodeTypeMember> toRemoves = new List<CodeTypeMember>();
            foreach (CodeTypeMember member in clientType.Members)
            {
                CodeConstructor ctor = member as CodeConstructor;
                if (ctor != null)
                {
                    if (ctor.Parameters.Count > 0 && string.Equals(ctor.Parameters[0].Type.BaseType, typeof(InstanceContext).FullName))
                    {
                        if (ctor.Parameters.Count == 1)
                        {
                            if (!shouldRemoveDefault)
                            {
                                // DuplexTestClient(InstanceContext callbackInstance)
                                ctor.BaseConstructorArgs.Add(
                                    new CodeMethodInvokeExpression(
                                        new CodeTypeReferenceExpression(clientType.Name),
                                        ConfigToCodeConstants.GetDefaultBindingMethod));
                                ctor.BaseConstructorArgs.Add(
                                    new CodeMethodInvokeExpression(
                                        new CodeTypeReferenceExpression(clientType.Name),
                                        ConfigToCodeConstants.GetDefaultEndpointAddressMethod));

                                AddConfigEndpoint(ctor, endpointNames);
                            }
                            else
                            {
                                toRemoves.Add(ctor);
                            }
                        }
                        else if (ctor.Parameters.Count == 2 && string.Equals(ctor.Parameters[1].Type.BaseType, typeof(string).FullName, StringComparison.Ordinal))
                        {
                            if (endpointConfigurationExists)
                            {
                                // DuplexTestClient(InstanceContext callbackInstance, string endpointConfigurationName) => DuplexTestClient(InstanceContext callbackInstance, Configuration endpointConfiguration)
                                ctor.Parameters[1] = new CodeParameterDeclarationExpression(
                                    ConfigToCodeConstants.EndpointConfigurationEnumTypeName,
                                    ConfigToCodeConstants.EndpointConfigurationParameter);

                                ctor.BaseConstructorArgs.RemoveAt(1);

                                ctor.BaseConstructorArgs.Add(
                                    new CodeMethodInvokeExpression(
                                        new CodeTypeReferenceExpression(clientType.Name),
                                        ConfigToCodeConstants.GetBindingMethod,
                                        new CodeArgumentReferenceExpression(ConfigToCodeConstants.EndpointConfigurationParameter)));
                                ctor.BaseConstructorArgs.Add(
                                    new CodeMethodInvokeExpression(
                                        new CodeTypeReferenceExpression(clientType.Name),
                                        ConfigToCodeConstants.GetEndpointAddressMethod,
                                        new CodeArgumentReferenceExpression(ConfigToCodeConstants.EndpointConfigurationParameter)));

                                AddConfigEndpoint(ctor, null);
                            }
                            else
                            {
                                toRemoves.Add(ctor);
                            }
                        }
                        else if (ctor.Parameters.Count == 3 && string.Equals(ctor.Parameters[1].Type.BaseType, typeof(string).FullName, StringComparison.Ordinal)
                            && string.Equals(ctor.Parameters[2].Type.BaseType, typeof(string).FullName, StringComparison.Ordinal))
                        {
                            if (endpointConfigurationExists)
                            {
                                // DuplexTestClient(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) 
                                // => DuplexTestClient(InstanceContext callbackInstance, Configuration endpointConfiguration, string remoteAddress)
                                string remoteAddressParam = ctor.Parameters[2].Name;
                                ctor.Parameters[1] = new CodeParameterDeclarationExpression(
                                    ConfigToCodeConstants.EndpointConfigurationEnumTypeName,
                                    ConfigToCodeConstants.EndpointConfigurationParameter);
                                ctor.BaseConstructorArgs[1] =
                                    new CodeMethodInvokeExpression(
                                        new CodeTypeReferenceExpression(clientType.Name),
                                        ConfigToCodeConstants.GetBindingMethod,
                                        new CodeArgumentReferenceExpression(ConfigToCodeConstants.EndpointConfigurationParameter));
                                ctor.BaseConstructorArgs[2] =
                                    new CodeObjectCreateExpression(
                                        typeof(EndpointAddress),
                                        new CodeArgumentReferenceExpression(remoteAddressParam));

                                AddConfigEndpoint(ctor, null);
                            }
                            else
                            {
                                toRemoves.Add(ctor);
                            }
                        }
                        else if (ctor.Parameters.Count == 3 && string.Equals(ctor.Parameters[1].Type.BaseType, typeof(string).FullName, StringComparison.Ordinal)
                            && string.Equals(ctor.Parameters[2].Type.BaseType, typeof(EndpointAddress).FullName, StringComparison.Ordinal))
                        {
                            if (endpointConfigurationExists)
                            {
                                // DuplexTestClient(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress)
                                // => DuplexTestClient(InstanceContext callbackInstance, Configuration endpointConfiguration, EndpointAddress remoteAddress)
                                ctor.Parameters[1] = new CodeParameterDeclarationExpression(
                                    ConfigToCodeConstants.EndpointConfigurationEnumTypeName,
                                    ConfigToCodeConstants.EndpointConfigurationParameter);
                                ctor.BaseConstructorArgs[1] =
                                    new CodeMethodInvokeExpression(
                                        new CodeTypeReferenceExpression(clientType.Name),
                                        ConfigToCodeConstants.GetBindingMethod,
                                        new CodeArgumentReferenceExpression(ConfigToCodeConstants.EndpointConfigurationParameter));

                                AddConfigEndpoint(ctor, null);
                            }
                            else
                            {
                                toRemoves.Add(ctor);
                            }
                        }
                    }
                }
            }

            if (toRemoves.Count > 0)
            {
                foreach (CodeTypeMember member in toRemoves)
                {
                    clientType.Members.Remove(member);
                }
            }

            foreach (var extendedClientType in FindExtendedClientTypes(clientType.Name, namespaceDecl))
            {
                FixupConstructorsEventBasedDuplex(extendedClientType, shouldRemoveDefault, endpointConfigurationExists);
            }
        }


        private static string ExtractContract(CodeTypeDeclaration clientType)
        {
            foreach (CodeTypeReference baseType in clientType.BaseTypes)
            {
                if (string.Equals(baseType.BaseType, ConfigToCodeConstants.ClientBaseOfTBaseName, StringComparison.Ordinal) && baseType.TypeArguments.Count == 1)
                {
                    return baseType.TypeArguments[0].BaseType;
                }
                else if (string.Equals(baseType.BaseType, ConfigToCodeConstants.DuplexClientBaseOfTBaseName, StringComparison.Ordinal) && baseType.TypeArguments.Count == 1)
                {
                    return baseType.TypeArguments[0].BaseType;
                }
            }

            throw new ArgumentException(SR.NotClientBaseType);
        }

        private IEnumerable<CodeTypeDeclaration> FindExtendedClientTypes(string clientTypeName, CodeNamespace namespaceDecl)
        {
            foreach (CodeTypeDeclaration type in namespaceDecl.Types)
            {
                if (type.BaseTypes.Count > 0 && string.Equals(clientTypeName, type.BaseTypes[0].BaseType, StringComparison.Ordinal))
                {
                    yield return type;
                }
            }
        }

        private IEnumerable<CodeTypeDeclaration> FindClientTypes(CodeNamespace namespaceDecl)
        {
            foreach (CodeTypeDeclaration codeDecl in namespaceDecl.Types)
            {
                foreach (CodeTypeReference baseType in codeDecl.BaseTypes)
                {
                    if (string.Equals(baseType.BaseType, ConfigToCodeConstants.ClientBaseOfTBaseName, StringComparison.Ordinal)
                        || string.Equals(baseType.BaseType, ConfigToCodeConstants.DuplexClientBaseOfTBaseName, StringComparison.Ordinal))
                    {
                        yield return codeDecl;
                    }
                }
            }
        }
    }
}
