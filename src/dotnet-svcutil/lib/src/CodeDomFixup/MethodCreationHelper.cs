// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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

            CustomBinding custom = binding as CustomBinding;
            if (custom != null)
            {
                AddCustomBindingConfiguration(statements, custom);
                return;
            }

            WSHttpBinding httpBinding = binding as WSHttpBinding;
            if (httpBinding != null)
            {
                AddCustomBindingConfiguration(statements, new CustomBinding(httpBinding));
                return;
            }

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, SR.ErrBindingTypeNotSupportedFormat, binding.GetType().FullName));
        }

        private static void AddCustomBindingConfiguration(CodeStatementCollection statements, CustomBinding custom)
        {
            const string ResultVarName = "result";
            statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(CustomBinding),
                    ResultVarName,
                    new CodeObjectCreateExpression(typeof(CustomBinding))));
            CodeVariableReferenceExpression resultVar = new CodeVariableReferenceExpression(ResultVarName);
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
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, SR.ErrBindingElementNotSupportedFormat, bindingElement.GetType().FullName));
                }
            }

            statements.Add(new CodeMethodReturnStatement(resultVar));
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

        private static void AddNetTcpBindingConfiguration(CodeStatementCollection statements, NetTcpBinding netTcp)
        {
            const string ResultVarName = "result";
            NetTcpBinding defaultBinding = new NetTcpBinding();
            statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(NetTcpBinding),
                    ResultVarName,
                    new CodeObjectCreateExpression(typeof(NetTcpBinding))));
            CodeVariableReferenceExpression resultVar = new CodeVariableReferenceExpression(ResultVarName);

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

            statements.Add(new CodeMethodReturnStatement(resultVar));
        }

        private static void AddBasicHttpBindingConfiguration(CodeStatementCollection statements, BasicHttpBinding basicHttp)
        {
            const string ResultVarName = "result";

            BasicHttpBinding defaultBinding = new BasicHttpBinding();

            statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(BasicHttpBinding),
                    ResultVarName,
                    new CodeObjectCreateExpression(typeof(BasicHttpBinding))));
            CodeVariableReferenceExpression resultVar = new CodeVariableReferenceExpression(ResultVarName);

            MaxOutProperties(statements, resultVar);

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

            statements.Add(new CodeMethodReturnStatement(resultVar));
        }

        private static void AddNetHttpBindingConfiguration(CodeStatementCollection statements, NetHttpBinding netHttp)
        {
            const string ResultVarName = "result";

            NetHttpBinding defaultBinding = new NetHttpBinding();
            
            statements.Add(
                new CodeVariableDeclarationStatement(
                    typeof(NetHttpBinding),
                    ResultVarName,
                    new CodeObjectCreateExpression(typeof(NetHttpBinding))));
            CodeVariableReferenceExpression resultVar = new CodeVariableReferenceExpression(ResultVarName);

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

            statements.Add(new CodeMethodReturnStatement(resultVar));
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
