// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using Microsoft.Xml;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class MethodCreationHelper
    {
        public MethodCreationHelper(CodeTypeDeclaration clientType)
        {
            this.ClientType = clientType;

            this.GetEndpointAddress = new CodeMemberMethod
            {
                Name = ConfigToCodeConstants.GetEndpointAddressMethod,
                Attributes = MemberAttributes.Private | MemberAttributes.Static,
                ReturnType = new CodeTypeReference(typeof(EndpointAddress)),
            };

            this.GetEndpointAddress.Parameters.Add(new CodeParameterDeclarationExpression(ConfigToCodeConstants.EndpointConfigurationEnumTypeName, ConfigToCodeConstants.EndpointConfigurationParameter));

            this.GetBinding = new CodeMemberMethod
            {
                Name = ConfigToCodeConstants.GetBindingMethod,
                Attributes = MemberAttributes.Private | MemberAttributes.Static,
                ReturnType = new CodeTypeReference(typeof(Binding)),
            };

            this.GetBinding.Parameters.Add(new CodeParameterDeclarationExpression(ConfigToCodeConstants.EndpointConfigurationEnumTypeName, ConfigToCodeConstants.EndpointConfigurationParameter));
        }

        private CodeTypeDeclaration ClientType { get; set; }

        private CodeMemberMethod GetEndpointAddress { get; set; }

        private CodeMemberMethod GetBinding { get; set; }

        private CodeMemberMethod GetDefaultEndpointAddress { get; set; }

        private CodeMemberMethod GetDefaultBinding { get; set; }

        public bool AddClientEndpoint(ServiceEndpoint endpoint)
        {
            string endpointAddress = endpoint?.Address?.ToString();
            if (!string.IsNullOrEmpty(endpointAddress))
            {
                this.AddNewEndpointAddress(endpoint.Name, endpointAddress, endpoint);
                this.AddNewBinding(endpoint.Name, endpoint.Binding);
                return true;
            }
            return false;
        }

        public void AddConfigurationEnum(List<string> endpointNames)
        {
            CodeTypeDeclaration configurationsEnum = new CodeTypeDeclaration(ConfigToCodeConstants.EndpointConfigurationEnumTypeName);
            configurationsEnum.IsEnum = true;
            foreach (string endpointName in endpointNames)
            {
                configurationsEnum.Members.Add(new CodeMemberField(ConfigToCodeConstants.EndpointConfigurationEnumTypeName, CodeDomHelpers.EscapeName(endpointName)));
            }

            this.ClientType.Members.Add(configurationsEnum);
        }

        public void AddMethods(List<string> endpointNames, bool isVB)
        {
            this.AddFinalThrowStatement();

            this.ClientType.Members.Add(this.GetBinding);
            this.ClientType.Members.Add(this.GetEndpointAddress);

            // Only single endpoint support getting default binding and endpoint address.
            if (endpointNames.Count == 1)
            {
                this.CreateDefaultEndpointMethods(this.ClientType.Name, endpointNames);
                this.ClientType.Members.Add(this.GetDefaultBinding);
                this.ClientType.Members.Add(this.GetDefaultEndpointAddress);
            }

            this.AddConfigureEndpoint(isVB);
        }

        private void AddConfigureEndpoint(bool isVB)
        {
            string indent = "    ";

            if (this.ClientType.UserData.Contains("Namespace"))
            {
                CodeNamespace ns = this.ClientType.UserData["Namespace"] as CodeNamespace;
                if (!string.IsNullOrEmpty(ns?.Name))
                {
                    indent += indent;
                }
            }

            CodeSnippetTypeMember snippet;
            string comment =
                    indent + "{0} <summary>" + Environment.NewLine +
                    indent + "{0} " + SR.ConfigureEndpointCommentSummary + Environment.NewLine +
                    indent + "{0} </summary>" + Environment.NewLine +
                    indent + "{0} <param name=\"serviceEndpoint\">" + SR.ServiceEndpointComment + "</param>" + Environment.NewLine +
                    indent + "{0} <param name=\"clientCredentials\">" + SR.ClientCredentialsComment + "</param>" + Environment.NewLine;

            if (!isVB)
            {
                snippet = new CodeSnippetTypeMember(
                    string.Format(CultureInfo.InvariantCulture, comment, "///") +
                    indent + "static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);");
            }
            else
            {
                snippet = new CodeSnippetTypeMember(
                    string.Format(CultureInfo.InvariantCulture, comment, "'''") +
                    indent + "Partial Private Shared Sub ConfigureEndpoint(ByVal serviceEndpoint As System.ServiceModel.Description.ServiceEndpoint, ByVal clientCredentials As System.ServiceModel.Description.ClientCredentials)" + Environment.NewLine +
                    indent + "End Sub");
            }

            this.ClientType.Members.Add(snippet);
        }

        private void AddNewBinding(string endpointConfigurationName, Binding binding)
        {
            CodeConditionStatement condition =
                new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(
                        new CodeArgumentReferenceExpression(ConfigToCodeConstants.EndpointConfigurationParameter),
                        CodeBinaryOperatorType.ValueEquality,
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(ConfigToCodeConstants.EndpointConfigurationEnumTypeName), endpointConfigurationName)));

            this.GetBinding.Statements.Add(condition);
            AddBindingConfiguration(condition.TrueStatements, binding);
        }

        private static void AddBindingConfiguration(CodeStatementCollection statements, Binding binding)
        {
            BasicHttpBinding basicHttp = binding as BasicHttpBinding;
            if (basicHttp != null)
            {
                AddBasicHttpBindingConfiguration(statements, basicHttp);
                return;
            }

            NetHttpBinding netHttp = binding as NetHttpBinding;
            if (netHttp != null)
            {
                AddNetHttpBindingConfiguration(statements, netHttp);
                return;
            }

            NetTcpBinding netTcp = binding as NetTcpBinding;
            if (netTcp != null)
            {
                AddNetTcpBindingConfiguration(statements, netTcp);
                return;
            }

            NetNamedPipeBinding namedPipe = binding as NetNamedPipeBinding;
            if (namedPipe != null)
            {
                AddNamedPipeBindingConfiguration(statements, namedPipe);
                return;
            }

            CustomBinding custom = binding as CustomBinding;
            if (custom != null)
            {
                AddCustomBindingConfiguration(statements, custom);
                return;
            }

            WS2007FederationHttpBinding ws2007FederationHttpBinding = binding as WS2007FederationHttpBinding;
            if (ws2007FederationHttpBinding != null)
            {
                AddWS2007FederationBindingConfiguration(statements, ws2007FederationHttpBinding);
                return;
            }

            WSFederationHttpBinding wsFederationHttpBinding = binding as WSFederationHttpBinding;
            if (wsFederationHttpBinding != null)
            {
                AddWSFederationBindingConfiguration(statements, wsFederationHttpBinding);
                return;
            }

            if (binding is WS2007HttpBinding ws2007HttpBinding)
            {
                AddWS2007HttpBindingConfiguration(statements, ws2007HttpBinding);
                return;
            }

            if (binding is WSHttpBinding wsHttpBinding)
            {
                AddWSHttpBindingConfiguration(statements, wsHttpBinding);
                return;
            }

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, SR.ErrBindingTypeNotSupportedFormat, binding.GetType().FullName));
        }

        private static void AddIssuerBindingConfiguration(CodeStatementCollection statements, Binding binding)
        {
            BasicHttpBinding basicHttp = binding as BasicHttpBinding;
            if (basicHttp != null)
            {
                AddBasicHttpBindingConfiguration(statements, basicHttp, true);
                return;
            }

            NetHttpBinding netHttp = binding as NetHttpBinding;
            if (netHttp != null)
            {
                AddNetHttpBindingConfiguration(statements, netHttp, true);
                return;
            }

            NetTcpBinding netTcp = binding as NetTcpBinding;
            if (netTcp != null)
            {
                AddNetTcpBindingConfiguration(statements, netTcp, true);
                return;
            }

            CustomBinding custom = binding as CustomBinding;
            if (custom != null)
            {
                AddCustomBindingConfiguration(statements, custom, true);
                return;
            }

            if (binding is WS2007HttpBinding ws2007HttpBinding)
            {
                AddWS2007HttpBindingConfiguration(statements, ws2007HttpBinding, true);
                return;
            }

            if (binding is WSHttpBinding wsHttpBinding)
            {
                AddWSHttpBindingConfiguration(statements, wsHttpBinding, true);
                return;
            }
        }

        private static void AddWSHttpBindingConfiguration(CodeStatementCollection statements, WSHttpBinding wsHttp, bool isIssuerBinding = false)
        {
            const string ResultVarName = "result";
            const string IssuerBingdingVarName = "issuerBinding";
            CodeVariableReferenceExpression resultVar;
            WSHttpBinding defaultBinding = new WSHttpBinding();

            if(isIssuerBinding)
            {
                statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(WSHttpBinding),
                    IssuerBingdingVarName,
                    new CodeObjectCreateExpression(typeof(WSHttpBinding))));
                resultVar = new CodeVariableReferenceExpression(IssuerBingdingVarName);
            }
            else
            {
                statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(WSHttpBinding),
                    ResultVarName,
                    new CodeObjectCreateExpression(typeof(WSHttpBinding))));
                resultVar = new CodeVariableReferenceExpression(ResultVarName);
            }

            WSHttpMaxOutProperties(statements, resultVar);

            // Set AllowCookies's default value to true.
            statements.Add(
                   new CodeAssignStatement(
                       new CodePropertyReferenceExpression(
                           resultVar,
                           "AllowCookies"),
                       new CodePrimitiveExpression(true)));

            if (defaultBinding.MessageEncoding != wsHttp.MessageEncoding)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            resultVar,
                            "MessageEncoding"),
                        new CodePropertyReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(WSMessageEncoding)),
                            wsHttp.MessageEncoding.ToString())));
            }

            if (defaultBinding.TransactionFlow != wsHttp.TransactionFlow)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            resultVar,
                            "TransactionFlow"),
                        new CodePrimitiveExpression(wsHttp.TransactionFlow)));
            }

            if (defaultBinding.ReliableSession.Enabled != wsHttp.ReliableSession.Enabled)
            {
                if (wsHttp.ReliableSession.Enabled)
                {
                    statements.Add(
                        new CodeAssignStatement(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "ReliableSession"),
                                "Enabled"),
                            new CodePrimitiveExpression(wsHttp.ReliableSession.Enabled)));
                    statements.Add(
                        new CodeAssignStatement(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "ReliableSession"),
                                "Ordered"),
                            new CodePrimitiveExpression(wsHttp.ReliableSession.Ordered)));
                    statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(resultVar, "ReliableSession"),
                            "InactivityTimeout"),
                        CreateTimeSpanExpression(wsHttp.ReliableSession.InactivityTimeout)));
                }
            }

            if (defaultBinding.Security.Mode != wsHttp.Security.Mode)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(resultVar, "Security"),
                            "Mode"),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(SecurityMode)),
                            wsHttp.Security.Mode.ToString())));
            }

            if (defaultBinding.Security.Transport.ClientCredentialType != wsHttp.Security.Transport.ClientCredentialType)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "Security"),
                                "Transport"),
                            "ClientCredentialType"),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(HttpClientCredentialType)),
                            wsHttp.Security.Transport.ClientCredentialType.ToString())));
            }

            if (defaultBinding.Security.Message.ClientCredentialType != wsHttp.Security.Message.ClientCredentialType)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "Security"),
                                "Message"),
                            "ClientCredentialType"),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(MessageCredentialType)),
                            wsHttp.Security.Message.ClientCredentialType.ToString())));
            }

            if (defaultBinding.Security.Message.EstablishSecurityContext != wsHttp.Security.Message.EstablishSecurityContext)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "Security"),
                                "Message"),
                            "EstablishSecurityContext"),
                        new CodePrimitiveExpression(wsHttp.Security.Message.EstablishSecurityContext)));
            }

            if (!isIssuerBinding)
            {
                statements.Add(new CodeMethodReturnStatement(resultVar));
            }
        }

        private static void AddWS2007HttpBindingConfiguration(CodeStatementCollection statements, WS2007HttpBinding ws2007Http, bool isIssuerBinding = false)
        {
            const string ResultVarName = "result";
            const string IssuerBingdingVarName = "issuerBinding";
            CodeVariableReferenceExpression resultVar;
            WS2007HttpBinding defaultBinding = new WS2007HttpBinding();

            if(isIssuerBinding)
            {
                statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(WS2007HttpBinding),
                    IssuerBingdingVarName,
                    new CodeObjectCreateExpression(typeof(WS2007HttpBinding))));
                resultVar = new CodeVariableReferenceExpression(IssuerBingdingVarName);
            }
            else
            {
                statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(WS2007HttpBinding),
                    ResultVarName,
                    new CodeObjectCreateExpression(typeof(WS2007HttpBinding))));
                resultVar = new CodeVariableReferenceExpression(ResultVarName);
            }            

            WSHttpMaxOutProperties(statements, resultVar);

            // Set AllowCookies's default value to true.
            statements.Add(
                   new CodeAssignStatement(
                       new CodePropertyReferenceExpression(
                           resultVar,
                           "AllowCookies"),
                       new CodePrimitiveExpression(true)));

            if (defaultBinding.MessageEncoding != ws2007Http.MessageEncoding)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            resultVar,
                            "MessageEncoding"),
                        new CodePropertyReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(WSMessageEncoding)),
                            ws2007Http.MessageEncoding.ToString())));
            }

            if (defaultBinding.TransactionFlow != ws2007Http.TransactionFlow)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            resultVar,
                            "TransactionFlow"),
                        new CodePrimitiveExpression(ws2007Http.TransactionFlow)));
            }

            if (defaultBinding.ReliableSession.Enabled != ws2007Http.ReliableSession.Enabled)
            {
                if (ws2007Http.ReliableSession.Enabled)
                {
                    statements.Add(
                        new CodeAssignStatement(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "ReliableSession"),
                                "Enabled"),
                            new CodePrimitiveExpression(ws2007Http.ReliableSession.Enabled)));
                    statements.Add(
                        new CodeAssignStatement(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "ReliableSession"),
                                "Ordered"),
                            new CodePrimitiveExpression(ws2007Http.ReliableSession.Ordered)));
                    statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(resultVar, "ReliableSession"),
                            "InactivityTimeout"),
                        CreateTimeSpanExpression(ws2007Http.ReliableSession.InactivityTimeout)));
                }
            }

            if (defaultBinding.Security.Mode != ws2007Http.Security.Mode)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(resultVar, "Security"),
                            "Mode"),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(SecurityMode)),
                            ws2007Http.Security.Mode.ToString())));
            }

            if (defaultBinding.Security.Transport.ClientCredentialType != ws2007Http.Security.Transport.ClientCredentialType)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "Security"),
                                "Transport"),
                            "ClientCredentialType"),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(HttpClientCredentialType)),
                            ws2007Http.Security.Transport.ClientCredentialType.ToString())));
            }

            if (defaultBinding.Security.Message.ClientCredentialType != ws2007Http.Security.Message.ClientCredentialType)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "Security"),
                                "Message"),
                            "ClientCredentialType"),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(MessageCredentialType)),
                            ws2007Http.Security.Message.ClientCredentialType.ToString())));
            }

            if (defaultBinding.Security.Message.EstablishSecurityContext != ws2007Http.Security.Message.EstablishSecurityContext)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "Security"),
                                "Message"),
                            "EstablishSecurityContext"),
                        new CodePrimitiveExpression(ws2007Http.Security.Message.EstablishSecurityContext)));
            }

            if (!isIssuerBinding)
            {
                statements.Add(new CodeMethodReturnStatement(resultVar));
            }
        }

        private static void AddWSFederationBindingConfiguration(CodeStatementCollection statements, WSFederationHttpBinding wsFedHttp, bool isIssuerBinding = false)
        {
            string ResultVarName = "result";
            string ResultRef = "result";
            string WSTrustTokenVarName = "wsTrustTokenParams";
            string IssuerBindingName = "issuerBinding";
            string IssuerAddressName = "issuerAddress";

            if (wsFedHttp.Security.Message.IssuerBinding is WSFederationHttpBinding)
            {
                ResultVarName = "federationIssuerBinding";
                WSTrustTokenVarName = "federationWsTrustTokenParams";
                IssuerBindingName = "federationIssuerBinding";
                IssuerAddressName = "federationissuerAddress";
            }

            WSFederationHttpBinding defaultBinding = new WSFederationHttpBinding();

            CodeVariableReferenceExpression wsTrustTokenVar = new CodeVariableReferenceExpression(WSTrustTokenVarName);
            CodeVariableReferenceExpression issuerBindingVar = new CodeVariableReferenceExpression(IssuerBindingName);
            CodeVariableReferenceExpression issuerBindingVar2 = new CodeVariableReferenceExpression(ResultRef);
            CodeVariableReferenceExpression issuerAddressVar = new CodeVariableReferenceExpression(IssuerAddressName);

            if (wsFedHttp.Security.Message.IssuerBinding != null)
            {
                if (wsFedHttp.Security.Message.IssuerBinding is WS2007FederationHttpBinding ws2007FedHttpIssuer)
                {
                    AddWS2007FederationBindingConfiguration(statements, ws2007FedHttpIssuer, true);
                }
                else if (wsFedHttp.Security.Message.IssuerBinding is WSFederationHttpBinding wsFedHttpIssuer)
                {
                    AddWSFederationBindingConfiguration(statements, wsFedHttpIssuer, true);
                }
                else
                {
                    AddIssuerBindingConfiguration(statements, wsFedHttp.Security.Message.IssuerBinding);
                }
            }
            else
            {
                statements.Add(
                    new CodeVariableDeclarationStatement(
                        typeof(WSHttpBinding),
                        IssuerBindingName,
                        new CodeObjectCreateExpression(typeof(WSHttpBinding),
                            new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(SecurityMode)), "Transport"))));

                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(issuerBindingVar, "Security"),
                            "Transport"), "ClientCredentialType"),
                        new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(System.ServiceModel.HttpClientCredentialType)), "Basic")));
            }

            statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(EndpointAddress),
                    IssuerAddressName,
                    new CodeObjectCreateExpression(typeof(EndpointAddress),
                        new CodeObjectCreateExpression(typeof(Uri),
                        new CodePrimitiveExpression(wsFedHttp.Security.Message.IssuerAddress.ToString())))));

            //if the WSFederationHttpBinding instance's issuer binding is still WSFederationHttpBinding,
            //then init the parent WSFederationHttpBinding's WSTrustTokenParameters with the already generated WSFederationHttpBinding result var
            if (!isIssuerBinding && wsFedHttp.Security.Message.IssuerBinding is WSFederationHttpBinding)
            {
                statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(System.ServiceModel.Federation.WSTrustTokenParameters),
                    WSTrustTokenVarName,
                    new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(System.ServiceModel.Federation.WSTrustTokenParameters)), "CreateWSFederationTokenParameters",
                        issuerBindingVar2, issuerAddressVar)));
            }
            else
            {
                statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(System.ServiceModel.Federation.WSTrustTokenParameters),
                    WSTrustTokenVarName,
                    new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(System.ServiceModel.Federation.WSTrustTokenParameters)), "CreateWSFederationTokenParameters",
                        issuerBindingVar, issuerAddressVar)));
            }

            statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(System.ServiceModel.Federation.WSFederationHttpBinding),
                    ResultVarName,
                    new CodeObjectCreateExpression(typeof(System.ServiceModel.Federation.WSFederationHttpBinding), wsTrustTokenVar)));

            CodeVariableReferenceExpression resultVar = new CodeVariableReferenceExpression(ResultVarName);

            // Set AllowCookies's default value to true.
            statements.Add(
                   new CodeAssignStatement(
                       new CodePropertyReferenceExpression(
                           resultVar,
                           "AllowCookies"),
                       new CodePrimitiveExpression(true)));

            if (defaultBinding.MessageEncoding != wsFedHttp.MessageEncoding)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            resultVar,
                            "MessageEncoding"),
                        new CodePropertyReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(WSMessageEncoding)),
                            wsFedHttp.MessageEncoding.ToString())));
            }

            if (defaultBinding.TransactionFlow != wsFedHttp.TransactionFlow)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            resultVar,
                            "TransactionFlow"),
                        new CodePrimitiveExpression(wsFedHttp.TransactionFlow)));
            }

            if (defaultBinding.ReliableSession.Enabled != wsFedHttp.ReliableSession.Enabled)
            {
                if (wsFedHttp.ReliableSession.Enabled)
                {
                    statements.Add(
                        new CodeAssignStatement(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "ReliableSession"),
                                "Enabled"),
                            new CodePrimitiveExpression(wsFedHttp.ReliableSession.Enabled)));
                    statements.Add(
                        new CodeAssignStatement(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "ReliableSession"),
                                "Ordered"),
                            new CodePrimitiveExpression(wsFedHttp.ReliableSession.Ordered)));
                    statements.Add(
                        new CodeAssignStatement(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "ReliableSession"),
                                "InactivityTimeout"),
                            new CodePrimitiveExpression(wsFedHttp.ReliableSession.InactivityTimeout)));
                }
            }

            if (defaultBinding.Security.Mode != wsFedHttp.Security.Mode)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(resultVar, "Security"),
                            "Mode"),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(SecurityMode)),
                            wsFedHttp.Security.Mode.ToString())));
            }

            if (defaultBinding.Security.Message.EstablishSecurityContext != wsFedHttp.Security.Message.EstablishSecurityContext)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "Security"),
                                "Message"),
                            "EstablishSecurityContext"),
                        new CodePrimitiveExpression(wsFedHttp.Security.Message.EstablishSecurityContext)));
            }

            if (defaultBinding.Security.Message.NegotiateServiceCredential != wsFedHttp.Security.Message.NegotiateServiceCredential)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "Security"),
                                "Message"),
                            "NegotiateServiceCredential"),
                        new CodePrimitiveExpression(wsFedHttp.Security.Message.EstablishSecurityContext)));
            }

            if (defaultBinding.Security.Message.AlgorithmSuite != wsFedHttp.Security.Message.AlgorithmSuite)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "Security"),
                                "Message"),
                            "AlgorithmSuite"),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(System.ServiceModel.Security.SecurityAlgorithmSuite)),
                            wsFedHttp.Security.Message.AlgorithmSuite.ToString())));
            }

            if (!isIssuerBinding)
            {
                statements.Add(new CodeMethodReturnStatement(resultVar));
            }
        }

        private static void AddWS2007FederationBindingConfiguration(CodeStatementCollection statements, WS2007FederationHttpBinding ws2007FedHttp, bool isIssuerBinding = false)
        {
            string ResultVarName = "result";
            string ResultRef = "result";
            string WSTrustTokenVarName = "wsTrustTokenParams";
            string IssuerBindingName = "issuerBinding";
            string IssuerAddressName = "issuerAddress";

            if (ws2007FedHttp.Security.Message.IssuerBinding is WSFederationHttpBinding)
            {
                ResultVarName = "federationIssuerBinding";
                WSTrustTokenVarName = "federationWsTrustTokenParams";
                IssuerBindingName = "federationIssuerBinding";
                IssuerAddressName = "federationissuerAddress";
            }

            WS2007FederationHttpBinding defaultBinding = new WS2007FederationHttpBinding();

            CodeVariableReferenceExpression wsTrustTokenVar = new CodeVariableReferenceExpression(WSTrustTokenVarName);
            CodeVariableReferenceExpression issuerBindingVar = new CodeVariableReferenceExpression(IssuerBindingName);
            CodeVariableReferenceExpression issuerBindingVar2 = new CodeVariableReferenceExpression(ResultRef);
            CodeVariableReferenceExpression issuerAddressVar = new CodeVariableReferenceExpression(IssuerAddressName);

            if (ws2007FedHttp.Security.Message.IssuerBinding != null)
            {
                if (ws2007FedHttp.Security.Message.IssuerBinding is WS2007FederationHttpBinding ws2007FedHttpIssuer)
                {
                    AddWS2007FederationBindingConfiguration(statements, ws2007FedHttpIssuer, true);
                }
                else if (ws2007FedHttp.Security.Message.IssuerBinding is WSFederationHttpBinding wsFedHttpIssuer)
                {
                    AddWSFederationBindingConfiguration(statements, wsFedHttpIssuer, true);
                }
                else
                {
                    AddIssuerBindingConfiguration(statements, ws2007FedHttp.Security.Message.IssuerBinding);
                }
            }
            else
            {
                statements.Add(
                    new CodeVariableDeclarationStatement(
                        typeof(WSHttpBinding),
                        IssuerBindingName,
                        new CodeObjectCreateExpression(typeof(WSHttpBinding),
                            new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(SecurityMode)), "Transport"))));

                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(issuerBindingVar, "Security"),
                            "Transport"), "ClientCredentialType"),
                        new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(System.ServiceModel.HttpClientCredentialType)), "Basic")));
            }

            statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(EndpointAddress),
                    IssuerAddressName,
                    new CodeObjectCreateExpression(typeof(EndpointAddress),
                        new CodeObjectCreateExpression(typeof(Uri),
                        new CodePrimitiveExpression(ws2007FedHttp.Security.Message.IssuerAddress.ToString())))));

            //if the WS2007FederationHttpBinding instance's issuer binding is still WSFederationHttpBinding,
            //then init the parent WS2007FederationHttpBinding's WSTrustTokenParameters with the already generated WSFederationHttpBinding result var
            if (!isIssuerBinding && ws2007FedHttp.Security.Message.IssuerBinding is WSFederationHttpBinding)
            {
                statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(System.ServiceModel.Federation.WSTrustTokenParameters),
                    WSTrustTokenVarName,
                    new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(System.ServiceModel.Federation.WSTrustTokenParameters)), "CreateWS2007FederationTokenParameters",
                        issuerBindingVar2, issuerAddressVar)));
            }
            else
            {
                statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(System.ServiceModel.Federation.WSTrustTokenParameters),
                    WSTrustTokenVarName,
                    new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(System.ServiceModel.Federation.WSTrustTokenParameters)), "CreateWS2007FederationTokenParameters",
                        issuerBindingVar, issuerAddressVar)));
            }

            statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(System.ServiceModel.Federation.WSFederationHttpBinding),
                    ResultVarName,
                    new CodeObjectCreateExpression(typeof(System.ServiceModel.Federation.WSFederationHttpBinding), wsTrustTokenVar)));

            CodeVariableReferenceExpression resultVar = new CodeVariableReferenceExpression(ResultVarName);

            // Set AllowCookies's default value to true.
            statements.Add(
                   new CodeAssignStatement(
                       new CodePropertyReferenceExpression(
                           resultVar,
                           "AllowCookies"),
                       new CodePrimitiveExpression(true)));

            if (defaultBinding.MessageEncoding != ws2007FedHttp.MessageEncoding)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            resultVar,
                            "MessageEncoding"),
                        new CodePropertyReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(WSMessageEncoding)),
                            ws2007FedHttp.MessageEncoding.ToString())));
            }

            if (defaultBinding.TransactionFlow != ws2007FedHttp.TransactionFlow)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            resultVar,
                            "TransactionFlow"),
                        new CodePrimitiveExpression(ws2007FedHttp.TransactionFlow)));
            }

            if (defaultBinding.ReliableSession.Enabled != ws2007FedHttp.ReliableSession.Enabled)
            {
                if (ws2007FedHttp.ReliableSession.Enabled)
                {
                    statements.Add(
                        new CodeAssignStatement(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "ReliableSession"),
                                "Enabled"),
                            new CodePrimitiveExpression(ws2007FedHttp.ReliableSession.Enabled)));
                    statements.Add(
                        new CodeAssignStatement(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "ReliableSession"),
                                "Ordered"),
                            new CodePrimitiveExpression(ws2007FedHttp.ReliableSession.Ordered)));
                    statements.Add(
                        new CodeAssignStatement(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "ReliableSession"),
                                "InactivityTimeout"),
                            new CodePrimitiveExpression(ws2007FedHttp.ReliableSession.InactivityTimeout)));
                }
            }

            if (defaultBinding.Security.Mode != ws2007FedHttp.Security.Mode)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(resultVar, "Security"),
                            "Mode"),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(SecurityMode)),
                            ws2007FedHttp.Security.Mode.ToString())));
            }

            if (defaultBinding.Security.Message.EstablishSecurityContext != ws2007FedHttp.Security.Message.EstablishSecurityContext)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "Security"),
                                "Message"),
                            "EstablishSecurityContext"),
                        new CodePrimitiveExpression(ws2007FedHttp.Security.Message.EstablishSecurityContext)));
            }

            if (defaultBinding.Security.Message.NegotiateServiceCredential != ws2007FedHttp.Security.Message.NegotiateServiceCredential)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "Security"),
                                "Message"),
                            "NegotiateServiceCredential"),
                        new CodePrimitiveExpression(ws2007FedHttp.Security.Message.EstablishSecurityContext)));
            }

            if (defaultBinding.Security.Message.AlgorithmSuite != ws2007FedHttp.Security.Message.AlgorithmSuite)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "Security"),
                                "Message"),
                            "AlgorithmSuite"),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(System.ServiceModel.Security.SecurityAlgorithmSuite)),
                            ws2007FedHttp.Security.Message.AlgorithmSuite.ToString())));
            }

            if (!isIssuerBinding)
            {
                statements.Add(new CodeMethodReturnStatement(resultVar));
            }
        }

        private static void AddCustomBindingConfiguration(CodeStatementCollection statements, CustomBinding custom, bool isIssuerBinding = false)
        {
            const string ResultVarName = "result";
            const string IssuerBingdingVarName = "issuerBinding";
            CodeVariableReferenceExpression resultVar;

            if(isIssuerBinding)
            {
                statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(CustomBinding),
                    IssuerBingdingVarName,
                    new CodeObjectCreateExpression(typeof(CustomBinding))));
                resultVar = new CodeVariableReferenceExpression(IssuerBingdingVarName);
            }
            else
            {
                statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(CustomBinding),
                    ResultVarName,
                    new CodeObjectCreateExpression(typeof(CustomBinding))));
                resultVar = new CodeVariableReferenceExpression(ResultVarName);
            }            

            foreach (BindingElement bindingElement in custom.Elements)
            {
                bool handled = false;
                TextMessageEncodingBindingElement textBE = bindingElement as TextMessageEncodingBindingElement;
                if (textBE != null)
                {
                    AddTextBindingElement(statements, resultVar, textBE);
                    handled = true;
                }

                if (!handled)
                {
                    BinaryMessageEncodingBindingElement binaryBE = bindingElement as BinaryMessageEncodingBindingElement;
                    if (binaryBE != null)
                    {
                        AddBinaryBindingElement(statements, resultVar);
                        handled = true;
                    }
                }

                if (!handled)
                {
                    MtomMessageEncodingBindingElement mtomBE = bindingElement as MtomMessageEncodingBindingElement;
                    if (mtomBE != null)
                    {
                        AddMtomBindingElement(statements, resultVar, mtomBE);
                        handled = true;
                    }
                }

                if (!handled)
                {
                    HttpTransportBindingElement httpTE = bindingElement as HttpTransportBindingElement;
                    if (httpTE != null)
                    {
                        AddHttpBindingElement(statements, resultVar, httpTE);
                        handled = true;
                    }
                }

                if (!handled)
                {
                    TcpTransportBindingElement tcpTE = bindingElement as TcpTransportBindingElement;
                    if (tcpTE != null)
                    {
                        AddTcpBindingElement(statements, resultVar, tcpTE);
                        handled = true;
                    }
                }

                if (!handled)
                {
                    NamedPipeTransportBindingElement namedPipeTE = bindingElement as NamedPipeTransportBindingElement;
                    if (namedPipeTE != null)
                    {
                        AddNamedPipeBindingElement(statements, resultVar, namedPipeTE);
                        handled = true;
                    }
                }

                if (!handled)
                {
                    TransportSecurityBindingElement transportSE = bindingElement as TransportSecurityBindingElement;
                    if (transportSE != null)
                    {
                        AddTransportSecurityBindingElement(statements, resultVar, transportSE);
                        handled = true;
                    }
                }

                if (!handled)
                {
                    TransactionFlowBindingElement transactionBE = bindingElement as TransactionFlowBindingElement;
                    if (transactionBE != null)
                    {
                        // if transaction is enabled, the binding should have been filtered before. Nothing to do here.
                        handled = true;
                    }
                }

                if(!handled)
                {
                    ReliableSessionBindingElement reliableSessionBE = bindingElement as ReliableSessionBindingElement;
                    if(reliableSessionBE != null)
                    {
                        AddReliableSessionBindingElement(statements, resultVar, reliableSessionBE);
                        handled = true;
                    }
                }

                if (!handled)
                {
                    SslStreamSecurityBindingElement sslStreamSE = bindingElement as SslStreamSecurityBindingElement;
                    if (sslStreamSE != null)
                    {
                        AddSslStreamSecurityBindingElement(statements, resultVar);
                        handled = true;
                    }
                }

                if (!handled)
                {
                    WindowsStreamSecurityBindingElement winSecurityStreamBE = bindingElement as WindowsStreamSecurityBindingElement;
                    if (winSecurityStreamBE != null)
                    {
                        AddWinStreamSecurityBindingElement(statements, resultVar);
                        handled = true;
                    }
                 }

                if (!handled)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, SR.ErrBindingElementNotSupportedFormat, bindingElement.GetType().FullName));
                }
            }

            if (!isIssuerBinding)
            {
                statements.Add(new CodeMethodReturnStatement(resultVar));
            }
        }

        private static void AddSslStreamSecurityBindingElement(CodeStatementCollection statements, CodeVariableReferenceExpression customBinding)
        {
            statements.Add(
                new CodeMethodInvokeExpression(
                    new CodePropertyReferenceExpression(
                        customBinding,
                        "Elements"),
                    "Add",
                    new CodeObjectCreateExpression(typeof(SslStreamSecurityBindingElement))));
        }

        private static void AddWinStreamSecurityBindingElement(CodeStatementCollection statements, CodeVariableReferenceExpression customBinding)
        {
            statements.Add(
                new CodeMethodInvokeExpression(
                    new CodePropertyReferenceExpression(
                        customBinding,
                        "Elements"),
                    "Add",
                    new CodeObjectCreateExpression(typeof(WindowsStreamSecurityBindingElement))));
        }
        
        private static void AddTransportSecurityBindingElement(CodeStatementCollection statements, CodeVariableReferenceExpression customBinding, TransportSecurityBindingElement bindingElement)
        {
            // Security binding validation is done in EndpointSelector.cs - Add UserNameOverTransportBindingElement
            TransportSecurityBindingElement defaultBindingElement = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
            CodeVariableDeclarationStatement userNameOverTransportSecurityBindingElement = new CodeVariableDeclarationStatement(
                typeof(TransportSecurityBindingElement),
                "userNameOverTransportSecurityBindingElement",
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(SecurityBindingElement)),
                    "CreateUserNameOverTransportBindingElement"));
            statements.Add(userNameOverTransportSecurityBindingElement);
            CodeVariableReferenceExpression bindingElementRef = new CodeVariableReferenceExpression(userNameOverTransportSecurityBindingElement.Name);

            if (defaultBindingElement.IncludeTimestamp != bindingElement.IncludeTimestamp)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            bindingElementRef,
                            "IncludeTimestamp"),
                        new CodePrimitiveExpression(bindingElement.IncludeTimestamp)));
            }

            if (defaultBindingElement.LocalClientSettings.MaxClockSkew != bindingElement.LocalClientSettings.MaxClockSkew)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(bindingElementRef, "LocalClientSettings"),
                            "MaxClockSkew"),
                            CreateTimeSpanExpression(bindingElement.LocalClientSettings.MaxClockSkew)));
            }

            if (defaultBindingElement.LocalClientSettings.ReplayWindow != bindingElement.LocalClientSettings.ReplayWindow)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(bindingElementRef, "LocalClientSettings"),
                            "ReplayWindow"),
                            CreateTimeSpanExpression(bindingElement.LocalClientSettings.ReplayWindow)));
            }

            if (defaultBindingElement.LocalClientSettings.TimestampValidityDuration != bindingElement.LocalClientSettings.TimestampValidityDuration)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(bindingElementRef, "LocalClientSettings"),
                            "TimestampValidityDuration"),
                            CreateTimeSpanExpression(bindingElement.LocalClientSettings.TimestampValidityDuration)));
            }

            if (defaultBindingElement.MessageSecurityVersion != bindingElement.MessageSecurityVersion)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(bindingElementRef,
                            "MessageSecurityVersion"),
                        new CodePropertyReferenceExpression(
                          new CodeTypeReferenceExpression(typeof(MessageSecurityVersion)),
                          bindingElement.MessageSecurityVersion.ToString())));
            }

            if (defaultBindingElement.SecurityHeaderLayout != bindingElement.SecurityHeaderLayout)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(bindingElementRef,
                            "SecurityHeaderLayout"),
                        new CodePropertyReferenceExpression(
                          new CodeTypeReferenceExpression(typeof(SecurityHeaderLayout)),
                          bindingElement.SecurityHeaderLayout.ToString())));
            }

            statements.Add(
                new CodeMethodInvokeExpression(
                    new CodePropertyReferenceExpression(
                        customBinding,
                        "Elements"),
                    "Add",
                    bindingElementRef));
        }

        private static void AddTcpBindingElement(CodeStatementCollection statements, CodeVariableReferenceExpression customBinding, TcpTransportBindingElement bindingElement)
        {
            TcpTransportBindingElement defaultBindingElement = new TcpTransportBindingElement();
            CodeVariableDeclarationStatement tcpBindingElement = new CodeVariableDeclarationStatement(
                typeof(TcpTransportBindingElement),
                "tcpBindingElement",
                new CodeObjectCreateExpression(typeof(TcpTransportBindingElement)));
            statements.Add(tcpBindingElement);
            CodeVariableReferenceExpression bindingElementRef = new CodeVariableReferenceExpression(tcpBindingElement.Name);

            statements.Add(
                new CodeAssignStatement(
                    new CodePropertyReferenceExpression(
                        bindingElementRef,
                        "MaxBufferSize"),
                    new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(int)), "MaxValue")));

            if (defaultBindingElement.TransferMode != bindingElement.TransferMode)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            bindingElementRef,
                            "TransferMode"),
                        new CodePropertyReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(TransferMode)),
                            bindingElement.TransferMode.ToString())));
            }

            statements.Add(
                new CodeAssignStatement(
                    new CodePropertyReferenceExpression(
                        bindingElementRef,
                        "MaxReceivedMessageSize"),
                    new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(int)), "MaxValue")));

            if (defaultBindingElement.ConnectionBufferSize != bindingElement.ConnectionBufferSize)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            bindingElementRef,
                            "ConnectionBufferSize"),
                        new CodePrimitiveExpression(bindingElement.ConnectionBufferSize)));
            }

            if (defaultBindingElement.ConnectionPoolSettings.GroupName != bindingElement.ConnectionPoolSettings.GroupName)
            {
                statements.Add(
                new CodeAssignStatement(
                    new CodePropertyReferenceExpression(
                        new CodePropertyReferenceExpression(bindingElementRef, "ConnectionPoolSettings"),
                        "GroupName"),
                    new CodePrimitiveExpression(bindingElement.ConnectionPoolSettings.GroupName)));
            }

            if (defaultBindingElement.ConnectionPoolSettings.MaxOutboundConnectionsPerEndpoint != bindingElement.ConnectionPoolSettings.MaxOutboundConnectionsPerEndpoint)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(bindingElementRef, "ConnectionPoolSettings"),
                            "MaxOutboundConnectionsPerEndpoint"),
                        new CodePrimitiveExpression(bindingElement.ConnectionPoolSettings.MaxOutboundConnectionsPerEndpoint)));
            }

            if (defaultBindingElement.ConnectionPoolSettings.IdleTimeout != bindingElement.ConnectionPoolSettings.IdleTimeout)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(bindingElementRef, "ConnectionPoolSettings"),
                            "IdleTimeout"),
                            CreateTimeSpanExpression(bindingElement.ConnectionPoolSettings.IdleTimeout)));
            }

            if (defaultBindingElement.ConnectionPoolSettings.LeaseTimeout != bindingElement.ConnectionPoolSettings.LeaseTimeout)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(bindingElementRef, "ConnectionPoolSettings"),
                            "LeaseTimeout"),
                            CreateTimeSpanExpression(bindingElement.ConnectionPoolSettings.LeaseTimeout)));
            }

            statements.Add(
                new CodeMethodInvokeExpression(
                    new CodePropertyReferenceExpression(
                        customBinding,
                        "Elements"),
                    "Add",
                    bindingElementRef));
        }

        private static void AddNamedPipeBindingElement(CodeStatementCollection statements, CodeVariableReferenceExpression customBinding, NamedPipeTransportBindingElement bindingElement)
        {
            NamedPipeTransportBindingElement defaultBindingElement = new NamedPipeTransportBindingElement();
            CodeVariableDeclarationStatement namedPipeBindingElement = new CodeVariableDeclarationStatement(
                typeof(TcpTransportBindingElement),
                "namedPipeBindingElement",
                new CodeObjectCreateExpression(typeof(NamedPipeTransportBindingElement)));
            statements.Add(namedPipeBindingElement);
            CodeVariableReferenceExpression bindingElementRef = new CodeVariableReferenceExpression(namedPipeBindingElement.Name);

            statements.Add(
                new CodeAssignStatement(
                    new CodePropertyReferenceExpression(
                        bindingElementRef,
                        "MaxBufferSize"),
                    new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(int)), "MaxValue")));

            if (defaultBindingElement.TransferMode != bindingElement.TransferMode)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            bindingElementRef,
                            "TransferMode"),
                        new CodePropertyReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(TransferMode)),
                            bindingElement.TransferMode.ToString())));
            }

            statements.Add(
                new CodeAssignStatement(
                    new CodePropertyReferenceExpression(
                        bindingElementRef,
                        "MaxReceivedMessageSize"),
                    new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(int)), "MaxValue")));

            if (defaultBindingElement.ConnectionBufferSize != bindingElement.ConnectionBufferSize)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            bindingElementRef,
                            "ConnectionBufferSize"),
                        new CodePrimitiveExpression(bindingElement.ConnectionBufferSize)));
            }

            statements.Add(
                new CodeMethodInvokeExpression(
                    new CodePropertyReferenceExpression(
                        customBinding,
                        "Elements"),
                    "Add",
                    bindingElementRef));
        }

        private static CodeExpression CreateTimeSpanExpression(TimeSpan value)
        {
            return new CodeObjectCreateExpression(typeof(TimeSpan), new CodePrimitiveExpression(value.Ticks));
        }

        private static void AddHttpBindingElement(CodeStatementCollection statements, CodeVariableReferenceExpression customBinding, HttpTransportBindingElement bindingElement)
        {
            bool isHttps = bindingElement is HttpsTransportBindingElement;
            Type bindingElementType = isHttps ? typeof(HttpsTransportBindingElement) : typeof(HttpTransportBindingElement);
            HttpTransportBindingElement defaultBindingElement = isHttps ? new HttpsTransportBindingElement() : new HttpTransportBindingElement();

            CodeVariableDeclarationStatement httpBindingElement = new CodeVariableDeclarationStatement(
                bindingElementType,
                isHttps ? "httpsBindingElement" : "httpBindingElement",
                new CodeObjectCreateExpression(bindingElementType));
            statements.Add(httpBindingElement);
            CodeVariableReferenceExpression bindingElementRef = new CodeVariableReferenceExpression(httpBindingElement.Name);

            // Set AllowCookies's default value to true.
            statements.Add(new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            bindingElementRef,
                            "AllowCookies"),
                        new CodePrimitiveExpression(true)));

            statements.Add(
                new CodeAssignStatement(
                    new CodePropertyReferenceExpression(
                        bindingElementRef,
                        "MaxBufferSize"),
                    new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(int)), "MaxValue")));

            statements.Add(
                new CodeAssignStatement(
                    new CodePropertyReferenceExpression(
                        bindingElementRef,
                        "MaxReceivedMessageSize"),
                    new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(int)), "MaxValue")));

            if (defaultBindingElement.TransferMode != bindingElement.TransferMode)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            bindingElementRef,
                            "TransferMode"),
                        new CodePropertyReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(TransferMode)),
                            bindingElement.TransferMode.ToString())));
            }

            if (defaultBindingElement.AuthenticationScheme != bindingElement.AuthenticationScheme)
            {
                statements.Add(
                  new CodeAssignStatement(
                      new CodePropertyReferenceExpression(
                          bindingElementRef,
                          "AuthenticationScheme"),
                      new CodePropertyReferenceExpression(
                          new CodeTypeReferenceExpression(typeof(AuthenticationSchemes)),
                          bindingElement.AuthenticationScheme.ToString())));
            }

            statements.Add(
                new CodeMethodInvokeExpression(
                    new CodePropertyReferenceExpression(
                        customBinding,
                        "Elements"),
                    "Add",
                    bindingElementRef));
        }

        private static void AddBinaryBindingElement(CodeStatementCollection statements, CodeVariableReferenceExpression customBinding)
        {
            statements.Add(
                new CodeMethodInvokeExpression(
                    new CodePropertyReferenceExpression(
                        customBinding,
                        "Elements"),
                    "Add",
                    new CodeObjectCreateExpression(typeof(BinaryMessageEncodingBindingElement))));
        }

        private static void AddMtomBindingElement(CodeStatementCollection statements, CodeVariableReferenceExpression customBinding, MtomMessageEncodingBindingElement bindingElement)
        {
            MtomMessageEncodingBindingElement defaultBindingElement = new MtomMessageEncodingBindingElement();
            CodeVariableDeclarationStatement mtomBindingElement = new CodeVariableDeclarationStatement(
                typeof(MtomMessageEncodingBindingElement),
                "mtomBindingElement",
                new CodeObjectCreateExpression(typeof(MtomMessageEncodingBindingElement)));
            statements.Add(mtomBindingElement);
            CodeVariableReferenceExpression bindingElementRef = new CodeVariableReferenceExpression(mtomBindingElement.Name);
            if (defaultBindingElement.MessageVersion != bindingElement.MessageVersion)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            bindingElementRef,
                            "MessageVersion"),
                        new CodePropertyReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(MessageVersion)),
                            GetMessageVersionName(bindingElement.MessageVersion))));
            }

            if (defaultBindingElement.WriteEncoding.WebName != bindingElement.WriteEncoding.WebName)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            bindingElementRef,
                            "WriteEncoding"),
                        new CodePropertyReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(Encoding)),
                            GetEncoding(bindingElement.WriteEncoding))));
            }

            statements.Add(
                new CodeMethodInvokeExpression(
                    new CodePropertyReferenceExpression(
                        customBinding,
                        "Elements"),
                    "Add",
                    bindingElementRef));
        }

        private static void AddTextBindingElement(CodeStatementCollection statements, CodeVariableReferenceExpression customBinding, TextMessageEncodingBindingElement bindingElement)
        {
            TextMessageEncodingBindingElement defaultBindingElement = new TextMessageEncodingBindingElement();
            CodeVariableDeclarationStatement textBindingElement = new CodeVariableDeclarationStatement(
                typeof(TextMessageEncodingBindingElement),
                "textBindingElement",
                new CodeObjectCreateExpression(typeof(TextMessageEncodingBindingElement)));
            statements.Add(textBindingElement);
            CodeVariableReferenceExpression bindingElementRef = new CodeVariableReferenceExpression(textBindingElement.Name);
            if (defaultBindingElement.MessageVersion != bindingElement.MessageVersion)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            bindingElementRef,
                            "MessageVersion"),
                        new CodePropertyReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(MessageVersion)),
                            GetMessageVersionName(bindingElement.MessageVersion))));
            }

            if (defaultBindingElement.WriteEncoding.WebName != bindingElement.WriteEncoding.WebName)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            bindingElementRef,
                            "WriteEncoding"),
                        new CodePropertyReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(Encoding)),
                            GetEncoding(bindingElement.WriteEncoding))));
            }

            statements.Add(
                new CodeMethodInvokeExpression(
                    new CodePropertyReferenceExpression(
                        customBinding,
                        "Elements"),
                    "Add",
                    bindingElementRef));
        }

        private static void AddReliableSessionBindingElement(CodeStatementCollection statements, CodeVariableReferenceExpression customBinding, ReliableSessionBindingElement bindingElement)
        {
            ReliableSessionBindingElement defaultBindingElement = new ReliableSessionBindingElement();
            CodeVariableDeclarationStatement reliableBindingElement = new CodeVariableDeclarationStatement(
                typeof(ReliableSessionBindingElement),
                "reliableBindingElement",
                new CodeObjectCreateExpression(typeof(ReliableSessionBindingElement)));
            statements.Add(reliableBindingElement);
            CodeVariableReferenceExpression bindingElementRef = new CodeVariableReferenceExpression(reliableBindingElement.Name);
            if (defaultBindingElement.Ordered != bindingElement.Ordered)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            bindingElementRef,
                            "Ordered"),
                        new CodePrimitiveExpression(bindingElement.Ordered)));
            }

            if (defaultBindingElement.AcknowledgementInterval != bindingElement.AcknowledgementInterval)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            bindingElementRef,
                            "AcknowledgementInterval"),
                        CreateTimeSpanExpression(bindingElement.AcknowledgementInterval)));
            }

            if (defaultBindingElement.FlowControlEnabled != bindingElement.FlowControlEnabled)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            bindingElementRef,
                            "FlowControlEnabled"),
                        new CodePrimitiveExpression(bindingElement.FlowControlEnabled)));
            }

            if (defaultBindingElement.InactivityTimeout != bindingElement.InactivityTimeout)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            bindingElementRef,
                            "InactivityTimeout"),
                        CreateTimeSpanExpression(bindingElement.InactivityTimeout)));
            }

            if (defaultBindingElement.MaxPendingChannels != bindingElement.MaxPendingChannels)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            bindingElementRef,
                            "MaxPendingChannels"),
                        new CodePrimitiveExpression(bindingElement.MaxPendingChannels)));
            }

            if (defaultBindingElement.MaxRetryCount != bindingElement.MaxRetryCount)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            bindingElementRef,
                            "MaxRetryCount"),
                        new CodePrimitiveExpression(bindingElement.MaxRetryCount)));
            }

            if (defaultBindingElement.MaxTransferWindowSize != bindingElement.MaxTransferWindowSize)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            bindingElementRef,
                            "MaxTransferWindowSize"),
                        new CodePrimitiveExpression(bindingElement.MaxTransferWindowSize)));
            }

            if (defaultBindingElement.ReliableMessagingVersion != bindingElement.ReliableMessagingVersion)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            bindingElementRef,
                            "ReliableMessagingVersion"),
                        new CodePropertyReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(ReliableMessagingVersion)),
                            GetReliableMessagingVersionName(bindingElement.ReliableMessagingVersion))));
            }

            statements.Add(
                new CodeMethodInvokeExpression(
                    new CodePropertyReferenceExpression(
                        customBinding,
                        "Elements"),
                    "Add",
                    bindingElementRef));
        }

        private static string GetEncoding(Encoding encoding)
        {
            if (encoding.WebName == Encoding.UTF8.WebName)
            {
                return "UTF8";
            }
            else if (encoding.WebName == Encoding.Unicode.WebName)
            {
                return "Unicode";
            }
            else if (encoding.WebName == Encoding.BigEndianUnicode.WebName)
            {
                return "BigEndianUnicode";
            }

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, SR.ErrEncodingNotSupportedFormat, encoding.WebName));
        }

        private static string GetMessageVersionName(MessageVersion messageVersion)
        {
            if (messageVersion == MessageVersion.None)
            {
                return "None";
            }
            else if (messageVersion == MessageVersion.Soap11)
            {
                return "Soap11";
            }
            else if (messageVersion == MessageVersion.Soap12)
            {
                return "CreateVersion(System.ServiceModel.EnvelopeVersion.Soap12, System.ServiceModel.Channels.AddressingVersion.None)";
            }
            else if (messageVersion == MessageVersion.Soap11WSAddressing10)
            {
                return "CreateVersion(System.ServiceModel.EnvelopeVersion.Soap11, System.ServiceModel.Channels.AddressingVersion.WSAddressing10)";
            }
            else if (messageVersion == MessageVersion.Soap12WSAddressing10)
            {
                return "Soap12WSAddressing10";
            }

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, SR.ErrMessageVersionNotSupportedFormat, messageVersion));
        }

        private static string GetReliableMessagingVersionName(ReliableMessagingVersion messagingVersion)
        {
            if (messagingVersion == ReliableMessagingVersion.Default)
            {
                return "Default";
            }
            else if (messagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                return "WSReliableMessaging11";
            }
            else if (messagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                return "WSReliableMessagingFebruary2005";
            }

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, SR.ErrMessageVersionNotSupportedFormat, messagingVersion));
        }

        private static void AddNetTcpBindingConfiguration(CodeStatementCollection statements, NetTcpBinding netTcp, bool isIssuerBinding = false)
        {
            const string ResultVarName = "result";
            const string IssuerBingdingVarName = "issuerBinding";
            CodeVariableReferenceExpression resultVar;
            NetTcpBinding defaultBinding = new NetTcpBinding();

            if(isIssuerBinding)
            {
                statements.Add(
                                new CodeVariableDeclarationStatement(
                                    typeof(NetTcpBinding),
                                    IssuerBingdingVarName,
                                    new CodeObjectCreateExpression(typeof(NetTcpBinding))));
                resultVar = new CodeVariableReferenceExpression(IssuerBingdingVarName);
            }
            else
            {
                statements.Add(
                                                new CodeVariableDeclarationStatement(
                                                    typeof(NetTcpBinding),
                                                    ResultVarName,
                                                    new CodeObjectCreateExpression(typeof(NetTcpBinding))));
                resultVar = new CodeVariableReferenceExpression(ResultVarName);
            }

            MaxOutProperties(statements, resultVar);

            if (defaultBinding.TransferMode != netTcp.TransferMode)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            resultVar,
                            "TransferMode"),
                    new CodePropertyReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(TransferMode)),
                            netTcp.TransferMode.ToString())));
            }

            if (defaultBinding.Security.Mode != netTcp.Security.Mode)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(resultVar, "Security"),
                            "Mode"),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(SecurityMode)),
                            netTcp.Security.Mode.ToString())));
            }

            if (defaultBinding.ReliableSession.Enabled != netTcp.ReliableSession.Enabled)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(resultVar, "ReliableSession"),
                            "Enabled"),
                        new CodePrimitiveExpression(netTcp.ReliableSession.Enabled)));
            }

            if (defaultBinding.Security.Transport.ClientCredentialType != netTcp.Security.Transport.ClientCredentialType)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "Security"),
                                "Transport"),
                            "ClientCredentialType"),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(TcpClientCredentialType)),
                            netTcp.Security.Transport.ClientCredentialType.ToString())));
            }

            if (defaultBinding.Security.Transport.ProtectionLevel != netTcp.Security.Transport.ProtectionLevel)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "Security"),
                                "Transport"),
                            "ProtectionLevel"),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(ProtectionLevel)),
                            netTcp.Security.Transport.ProtectionLevel.ToString())));
            }

            if (defaultBinding.Security.Message.ClientCredentialType != netTcp.Security.Message.ClientCredentialType)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "Security"),
                                "Message"),
                            "ClientCredentialType"),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(MessageCredentialType)),
                            netTcp.Security.Message.ClientCredentialType.ToString())));
            }

            if (!isIssuerBinding)
            {
                statements.Add(new CodeMethodReturnStatement(resultVar));
            }
        }

        private static void AddNamedPipeBindingConfiguration(CodeStatementCollection statements, NetNamedPipeBinding namedPipe)
        {
            const string ResultVarName = "result";
            CodeVariableReferenceExpression resultVar;
            NetNamedPipeBinding defaultBinding = new NetNamedPipeBinding();

            statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(NetNamedPipeBinding),
                    ResultVarName,
                    new CodeObjectCreateExpression(typeof(NetNamedPipeBinding))));

            resultVar = new CodeVariableReferenceExpression(ResultVarName);

            //if (defaultBinding.HostNameComparisonMode != namedPipe.HostNameComparisonMode)
            //{
            //    statements.Add(
            //        new CodeAssignStatement(
            //            new CodePropertyReferenceExpression(
            //                resultVar,
            //                "HostNameComparisonMode"),
            //        new CodeFieldReferenceExpression(
            //                new CodeTypeReferenceExpression(typeof(HostNameComparisonMode)),
            //                namedPipe.HostNameComparisonMode.ToString())));
            //}

            MaxOutProperties(statements, resultVar);

            //if (defaultBinding.TransactionFlow != namedPipe.TransactionFlow)
            //{
            //    statements.Add(
            //        new CodeAssignStatement(
            //            new CodePropertyReferenceExpression(
            //                resultVar,
            //                "TransactionFlow"),
            //        new CodePrimitiveExpression(namedPipe.TransactionFlow)));
            //}

            if (defaultBinding.TransferMode != namedPipe.TransferMode)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            resultVar,
                            "TransferMode"),
                    new CodePropertyReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(TransferMode)),
                            namedPipe.TransferMode.ToString())));
            }

            if (defaultBinding.Security.Mode != namedPipe.Security.Mode)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(resultVar, "Security"),
                            "Mode"),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(NetNamedPipeSecurityMode)),
                            namedPipe.Security.Mode.ToString())));
            }

            if(namedPipe.Security.Mode == NetNamedPipeSecurityMode.Transport)
            {
                if (defaultBinding.Security.Transport.ProtectionLevel != namedPipe.Security.Transport.ProtectionLevel)
                {
                    statements.Add(
                        new CodeAssignStatement(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(
                                    new CodePropertyReferenceExpression(resultVar, "Security"),
                                    "Transport"),
                                "ProtectionLevel"),
                            new CodeFieldReferenceExpression(
                                new CodeTypeReferenceExpression(typeof(ProtectionLevel)),
                                namedPipe.Security.Transport.ProtectionLevel.ToString())));
                }
            }

            statements.Add(new CodeMethodReturnStatement(resultVar)); 
        }

        private static void AddBasicHttpBindingConfiguration(CodeStatementCollection statements, BasicHttpBinding basicHttp, bool isIssuerBinding = false)
        {
            const string ResultVarName = "result";
            const string IssuerBingdingVarName = "issuerBinding";
            CodeVariableReferenceExpression resultVar;
            BasicHttpBinding defaultBinding = new BasicHttpBinding();

            if(isIssuerBinding)
            {
                statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(BasicHttpBinding),
                    IssuerBingdingVarName,
                    new CodeObjectCreateExpression(typeof(BasicHttpBinding))));
                resultVar = new CodeVariableReferenceExpression(IssuerBingdingVarName);
            }
            else
            {
                statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(BasicHttpBinding),
                    ResultVarName,
                    new CodeObjectCreateExpression(typeof(BasicHttpBinding))));
                resultVar = new CodeVariableReferenceExpression(ResultVarName);
            }

            MaxOutProperties(statements, resultVar);

            if (defaultBinding.MessageEncoding != basicHttp.MessageEncoding)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            resultVar,
                            "MessageEncoding"),
                        new CodePropertyReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(WSMessageEncoding)),
                            basicHttp.MessageEncoding.ToString())));
            }

            // Set AllowCookies's default value to true.
            statements.Add(
                   new CodeAssignStatement(
                       new CodePropertyReferenceExpression(
                           resultVar,
                           "AllowCookies"),
                       new CodePrimitiveExpression(true)));

            if (defaultBinding.TransferMode != basicHttp.TransferMode)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            resultVar,
                            "TransferMode"),
                        new CodePropertyReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(TransferMode)),
                            basicHttp.TransferMode.ToString())));
            }

            if (defaultBinding.Security.Mode != basicHttp.Security.Mode)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(resultVar, "Security"),
                            "Mode"),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(BasicHttpSecurityMode)),
                            basicHttp.Security.Mode.ToString())));
            }

            if (defaultBinding.Security.Transport.ClientCredentialType != basicHttp.Security.Transport.ClientCredentialType)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "Security"),
                                "Transport"),
                            "ClientCredentialType"),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(HttpClientCredentialType)),
                            basicHttp.Security.Transport.ClientCredentialType.ToString())));
            }

            if (defaultBinding.Security.Message.ClientCredentialType != basicHttp.Security.Message.ClientCredentialType)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "Security"),
                                "Message"),
                            "ClientCredentialType"),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(BasicHttpMessageCredentialType)),
                            basicHttp.Security.Message.ClientCredentialType.ToString())));
            }

            if (!isIssuerBinding)
            {
                statements.Add(new CodeMethodReturnStatement(resultVar));
            }
        }

        private static void AddNetHttpBindingConfiguration(CodeStatementCollection statements, NetHttpBinding netHttp, bool isIssuerBinding = false)
        {
            const string ResultVarName = "result";
            const string IssuerBingdingVarName = "issuerBinding";
            CodeVariableReferenceExpression resultVar;

            NetHttpBinding defaultBinding = new NetHttpBinding();

            if(isIssuerBinding)
            {
                statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(NetHttpBinding),
                    IssuerBingdingVarName,
                    new CodeObjectCreateExpression(typeof(NetHttpBinding))));
                resultVar = new CodeVariableReferenceExpression(IssuerBingdingVarName);
            }
            else
            {
                statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(NetHttpBinding),
                    ResultVarName,
                    new CodeObjectCreateExpression(typeof(NetHttpBinding))));
                resultVar = new CodeVariableReferenceExpression(ResultVarName);
            }

            MaxOutProperties(statements, resultVar);

            // Set AllowCookies's default value to true.
            statements.Add(
                   new CodeAssignStatement(
                       new CodePropertyReferenceExpression(
                           resultVar,
                           "AllowCookies"),
                       new CodePrimitiveExpression(true)));

            if (defaultBinding.MessageEncoding != netHttp.MessageEncoding)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            resultVar,
                            "MessageEncoding"),
                        new CodePropertyReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(NetHttpMessageEncoding)),
                            netHttp.MessageEncoding.ToString())));
            }

            if (defaultBinding.TransferMode != netHttp.TransferMode)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            resultVar,
                            "TransferMode"),
                        new CodePropertyReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(TransferMode)),
                            netHttp.TransferMode.ToString())));
            }

            if (defaultBinding.Security.Mode != netHttp.Security.Mode)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(resultVar, "Security"),
                            "Mode"),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(BasicHttpSecurityMode)),
                            netHttp.Security.Mode.ToString())));
            }

            if (defaultBinding.ReliableSession.Enabled != netHttp.ReliableSession.Enabled)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(resultVar, "ReliableSession"),
                            "Enabled"),
                        new CodePrimitiveExpression(netHttp.ReliableSession.Enabled)));
            }

            if (defaultBinding.Security.Transport.ClientCredentialType != netHttp.Security.Transport.ClientCredentialType)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "Security"),
                                "Transport"),
                            "ClientCredentialType"),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(HttpClientCredentialType)),
                            netHttp.Security.Transport.ClientCredentialType.ToString())));
            }

            if (defaultBinding.Security.Message.ClientCredentialType != netHttp.Security.Message.ClientCredentialType)
            {
                statements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(resultVar, "Security"),
                                "Message"),
                            "ClientCredentialType"),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(BasicHttpMessageCredentialType)),
                            netHttp.Security.Message.ClientCredentialType.ToString())));
            }

            if (!isIssuerBinding)
            {
                statements.Add(new CodeMethodReturnStatement(resultVar));
            }
        }

        private static void MaxOutProperties(CodeStatementCollection statements, CodeVariableReferenceExpression resultVar)
        {
            statements.Add(
                new CodeAssignStatement(
                    new CodePropertyReferenceExpression(
                        resultVar,
                        "MaxBufferSize"),
                    new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(int)), "MaxValue")));

            statements.Add(
                new CodeAssignStatement(
                    new CodePropertyReferenceExpression(
                        resultVar,
                        "ReaderQuotas"),
                    new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(XmlDictionaryReaderQuotas)), "Max")));

            statements.Add(
                new CodeAssignStatement(
                    new CodePropertyReferenceExpression(
                    resultVar,
                    "MaxReceivedMessageSize"),
                    new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(int)), "MaxValue")));
        }

        private static void WSHttpMaxOutProperties(CodeStatementCollection statements, CodeVariableReferenceExpression resultVar)
        {
            statements.Add(
                new CodeAssignStatement(
                    new CodePropertyReferenceExpression(
                        resultVar,
                        "ReaderQuotas"),
                    new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(XmlDictionaryReaderQuotas)), "Max")));

            statements.Add(
                new CodeAssignStatement(
                    new CodePropertyReferenceExpression(
                    resultVar,
                    "MaxReceivedMessageSize"),
                    new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(int)), "MaxValue")));
        }

        private void AddNewEndpointAddress(string endpointConfigurationName, string endpointAddress, ServiceEndpoint serviceEndpoint)
        {
            CodeExpression createIdentityExpression = null;
            EndpointIdentity identity = serviceEndpoint.Address.Identity;

            if (identity != null && (identity is DnsEndpointIdentity))
            {
                createIdentityExpression = new CodeObjectCreateExpression(identity.GetType(), new CodePrimitiveExpression(identity.IdentityClaim.Resource));
            }
            CodeExpression createEndpointAddressExpression = null;
            if (createIdentityExpression != null)
            {
                createEndpointAddressExpression = new CodeObjectCreateExpression(
                            typeof(EndpointAddress),
                            new CodeObjectCreateExpression(
                                typeof(Uri),
                                new CodePrimitiveExpression(endpointAddress)),
                            createIdentityExpression);
            }
            else
            {
                createEndpointAddressExpression = new CodeObjectCreateExpression(
                            typeof(EndpointAddress),
                            new CodePrimitiveExpression(endpointAddress));
            }
            this.GetEndpointAddress.Statements.Add(
                new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(
                        new CodeArgumentReferenceExpression(ConfigToCodeConstants.EndpointConfigurationParameter),
                        CodeBinaryOperatorType.ValueEquality,
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(ConfigToCodeConstants.EndpointConfigurationEnumTypeName), endpointConfigurationName)),
                    new CodeMethodReturnStatement(createEndpointAddressExpression)));
        }

        private void CreateDefaultEndpointMethods(string clientTypeName, List<string> endpointNames)
        {
            System.Diagnostics.Debug.Assert(endpointNames.Count == 1, "have and only have on endpoint has to exist for a given client type");

            this.GetDefaultBinding = new CodeMemberMethod
            {
                Attributes = MemberAttributes.Static | MemberAttributes.Private,
                Name = ConfigToCodeConstants.GetDefaultBindingMethod,
                ReturnType = new CodeTypeReference(typeof(Binding)),
            };

            this.GetDefaultEndpointAddress = new CodeMemberMethod
            {
                Attributes = MemberAttributes.Static | MemberAttributes.Private,
                Name = ConfigToCodeConstants.GetDefaultEndpointAddressMethod,
                ReturnType = new CodeTypeReference(typeof(EndpointAddress)),
            };

            this.GetDefaultBinding.Statements.Add(
                new CodeMethodReturnStatement(
                    new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression(clientTypeName),
                        ConfigToCodeConstants.GetBindingMethod,
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(
                                ConfigToCodeConstants.EndpointConfigurationEnumTypeName),
                                endpointNames[0]))));

            this.GetDefaultEndpointAddress.Statements.Add(
                new CodeMethodReturnStatement(
                    new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression(clientTypeName),
                        ConfigToCodeConstants.GetEndpointAddressMethod,
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(
                                ConfigToCodeConstants.EndpointConfigurationEnumTypeName),
                                endpointNames[0]))));
        }

        private void AddFinalThrowStatement()
        {
            this.GetBinding.Statements.Add(
                new CodeThrowExceptionStatement(
                    new CodeObjectCreateExpression(
                        typeof(InvalidOperationException),
                        new CodeMethodInvokeExpression(
                            new CodeTypeReferenceExpression(typeof(string)),
                            "Format",
                            new CodePrimitiveExpression(SR.CodeExpressionCouldNotFindEndpoint),
                            new CodeArgumentReferenceExpression(ConfigToCodeConstants.EndpointConfigurationParameter)))));

            this.GetEndpointAddress.Statements.Add(
                new CodeThrowExceptionStatement(
                    new CodeObjectCreateExpression(
                        typeof(InvalidOperationException),
                        new CodeMethodInvokeExpression(
                            new CodeTypeReferenceExpression(typeof(string)),
                            "Format",
                            new CodePrimitiveExpression(SR.CodeExpressionCouldNotFindEndpoint),
                            new CodeArgumentReferenceExpression(ConfigToCodeConstants.EndpointConfigurationParameter)))));
        }
    }
}
