// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class EndpointSelector : MetadataFixup
    {
        private static List<string> s_bindingValidationErrors = new List<string>();

        public EndpointSelector(WsdlImporter importer, Collection<ServiceEndpoint> endpoints, Collection<Binding> bindings, Collection<ContractDescription> contracts)
            : base(importer, endpoints, bindings, contracts)
        { }

        public override void Fixup()
        {
            CollectionHelpers.MapList<ServiceEndpoint>(this.endpoints, EndpointSelector.SelectEndpoint, this.AddWarning);
            bindings.Clear();
            foreach (ServiceEndpoint endpoint in endpoints)
            {
                bindings.Add(endpoint.Binding);
            }
        }

        private static bool SelectEndpoint(ServiceEndpoint endpoint)
        {
            return IsBindingSupported(endpoint.Binding);
        }

        private static bool IsBindingSupported(Binding binding)
        {
            s_bindingValidationErrors.Clear();

            if (!(binding is BasicHttpBinding || binding is NetHttpBinding || binding is WSHttpBinding || binding is NetTcpBinding || binding is CustomBinding))
            {
                s_bindingValidationErrors.Add(string.Format(SR.BindingTypeNotSupportedFormat, binding.GetType().FullName,
                    typeof(BasicHttpBinding).FullName, typeof(NetHttpBinding).FullName, typeof(WSHttpBinding).FullName, typeof(NetTcpBinding).FullName, typeof(CustomBinding).FullName));
            }
            else
            {
                WSHttpBinding wsHttpBinding = binding as WSHttpBinding;
                if (wsHttpBinding != null)
                {
                    if (wsHttpBinding.ReliableSession.Enabled)
                    {
                        s_bindingValidationErrors.Add(SR.BindingReliableSessionNotSupported);
                    }
                    if (wsHttpBinding.TransactionFlow)
                    {
                        s_bindingValidationErrors.Add(SR.BindingTransactionFlowNotSupported);
                    }
                    if (wsHttpBinding.MessageEncoding != WSMessageEncoding.Text)
                    {
                        s_bindingValidationErrors.Add(string.Format(SR.BindingMessageEncodingNotSupportedFormat, wsHttpBinding.MessageEncoding, WSMessageEncoding.Text));
                    }
                }
                else
                {
                    NetTcpBinding netTcpBinding = binding as NetTcpBinding;
                    if (netTcpBinding != null)
                    {
                        if (netTcpBinding.ReliableSession.Enabled)
                        {
                            s_bindingValidationErrors.Add(SR.BindingReliableSessionNotSupported);
                        }
                        if (netTcpBinding.TransactionFlow)
                        {
                            s_bindingValidationErrors.Add(SR.BindingTransactionFlowNotSupported);
                        }
                    }
                    else
                    {
                        NetHttpBinding netHttpBinding = binding as NetHttpBinding;
                        if (netHttpBinding != null && netHttpBinding.ReliableSession.Enabled)
                        {
                            s_bindingValidationErrors.Add(SR.BindingReliableSessionNotSupported);
                        }
                    }
                }

                ValidateBindingElements(binding);
            }

            return s_bindingValidationErrors.Count == 0;
        }

        private static void ValidateBindingElements(Binding binding)
        {
            BindingElementCollection bindingElements = binding.CreateBindingElements();

            foreach (BindingElement bindingElement in bindingElements)
            {
                if (bindingElement is TransportBindingElement)
                {
                    if (!(bindingElement is HttpTransportBindingElement || bindingElement is HttpsTransportBindingElement || bindingElement is TcpTransportBindingElement))
                    {
                        s_bindingValidationErrors.Add(string.Format(SR.BindingTransportTypeNotSupportedFormat, bindingElement.GetType().FullName,
                            typeof(HttpTransportBindingElement).FullName, typeof(HttpsTransportBindingElement).FullName, typeof(TcpTransportBindingElement).FullName));
                    }
                }
                else if (bindingElement is MessageEncodingBindingElement)
                {
                    if (!(bindingElement is BinaryMessageEncodingBindingElement || bindingElement is TextMessageEncodingBindingElement))
                    {
                        s_bindingValidationErrors.Add(string.Format(SR.BindingMessageEncodingElementNotSupportedFormat, bindingElement.GetType().FullName,
                            typeof(BinaryMessageEncodingBindingElement).FullName, typeof(TextMessageEncodingBindingElement).FullName));
                    }
                    else
                    {
                        var binMsgEncodingElement = bindingElement as BinaryMessageEncodingBindingElement;
                        if (binMsgEncodingElement != null)
                        {
                            if (binMsgEncodingElement.MessageVersion != MessageVersion.Soap12WSAddressing10)
                            {
                                s_bindingValidationErrors.Add(string.Format(SR.BindingBinaryMessageEncodingVersionNotSupportedFormat,
                                    binMsgEncodingElement.MessageVersion, MessageVersion.Soap12WSAddressing10));
                            }
                        }
                        else
                        {
                            var txtMsgEncodingElement = bindingElement as TextMessageEncodingBindingElement;
                            if (txtMsgEncodingElement != null &&
                                txtMsgEncodingElement.MessageVersion != MessageVersion.None &&
                                txtMsgEncodingElement.MessageVersion != MessageVersion.Soap11 &&
                                txtMsgEncodingElement.MessageVersion != MessageVersion.Soap12 &&
                                txtMsgEncodingElement.MessageVersion != MessageVersion.Soap11WSAddressing10 &&
                                txtMsgEncodingElement.MessageVersion != MessageVersion.Soap12WSAddressing10)
                            {
                                s_bindingValidationErrors.Add(string.Format(SR.BindingTextMessageEncodingVersionNotSupportedFormat,
                                    txtMsgEncodingElement.MessageVersion,
                                    MessageVersion.None,
                                    MessageVersion.Soap11,
                                    MessageVersion.Soap12,
                                    MessageVersion.Soap11WSAddressing10,
                                    MessageVersion.Soap12WSAddressing10));
                            }
                        }
                    }
                }
                else if (bindingElement is SslStreamSecurityBindingElement)
                {
                    // do nothing
                }
                else if (bindingElement is WindowsStreamSecurityBindingElement)
                {
                    // do nothing
                }
                else if (bindingElement is TransactionFlowBindingElement)
                {
                    if (binding is WSHttpBinding && ((WSHttpBinding)binding).TransactionFlow)
                    {
                        s_bindingValidationErrors.Add(SR.BindingTransactionFlowNotSupported);
                    }
                    if (binding is NetTcpBinding && ((NetTcpBinding)binding).TransactionFlow)
                    {
                        s_bindingValidationErrors.Add(SR.BindingTransactionFlowNotSupported);
                    }
                }
                else if (bindingElement is TransportSecurityBindingElement)
                {
                    ValidateTransportSecurityBindingElement(bindingElement as TransportSecurityBindingElement);
                }
                else
                {
                    s_bindingValidationErrors.Add(string.Format(SR.BindingElementTypeNotSupportedFormat, bindingElement.GetType().FullName));
                }
            }
        }

        private static void ValidateTransportSecurityBindingElement(TransportSecurityBindingElement transportSecurityBindingElement)
        {
            if (transportSecurityBindingElement.EndpointSupportingTokenParameters.Signed.Count != 0 ||
                transportSecurityBindingElement.EndpointSupportingTokenParameters.SignedEndorsing.Count != 0)
            {
                s_bindingValidationErrors.Add(SR.BindingTransportSecurityTokenSignedOrSignedEndorsingNotSupported);
            }

            if (!(transportSecurityBindingElement.EndpointSupportingTokenParameters.Endorsing.Count != 1 ^
                  transportSecurityBindingElement.EndpointSupportingTokenParameters.SignedEncrypted.Count != 1))
            {
                s_bindingValidationErrors.Add(SR.BindingTransportSecurityTokenMustBeOneOfEitherEndorsingOrSignedEncrypted);
            }
            else if (transportSecurityBindingElement.EndpointSupportingTokenParameters.SignedEncrypted.Count == 1)
            {
                ValidateUserNamePasswordSecurityBindingElement(transportSecurityBindingElement);
            }
            else //if (transportSecurityBindingElement.EndpointSupportingTokenParameters.Endorsing.Count == 1)
            {
                SecureConversationSecurityTokenParameters endorsingTokenParams = transportSecurityBindingElement.EndpointSupportingTokenParameters.Endorsing[0] as SecureConversationSecurityTokenParameters;

                if (endorsingTokenParams == null)
                {
                    s_bindingValidationErrors.Add(string.Format(SR.BindingTransportSecurityTokenEndorsingParamsTypeFormat,
                        transportSecurityBindingElement.EndpointSupportingTokenParameters.Endorsing[0].GetType().FullName, typeof(SecureConversationSecurityTokenParameters).FullName));
                }
                else
                {
                    if (endorsingTokenParams.RequireDerivedKeys)
                    {
                        s_bindingValidationErrors.Add(SR.BindingTransportSecurityTokenParamsRequiringDerivedKeysNotSupported);
                    }

                    TransportSecurityBindingElement bootstrapElement = endorsingTokenParams.BootstrapSecurityBindingElement as TransportSecurityBindingElement;

                    if (bootstrapElement == null)
                    {
                        s_bindingValidationErrors.Add(string.Format(SR.BindingTransportSecurityElementTypeNotSupportedFormat,
                                            endorsingTokenParams.BootstrapSecurityBindingElement.GetType().FullName, typeof(TransportSecurityBindingElement).FullName));
                    }
                    else
                    {
                        ValidateTransportSecurityBindingElement(bootstrapElement);
                    }
                }
            }

            if (transportSecurityBindingElement.MessageSecurityVersion != MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10 &&
                transportSecurityBindingElement.MessageSecurityVersion != MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10)
            {
                string values = string.Format(CultureInfo.InvariantCulture, "'{0}', '{1}'",
                    MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10,
                    MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10);

                s_bindingValidationErrors.Add(string.Format(SR.BindingTransportMessageSecurityVersionNotSupportedFormat, transportSecurityBindingElement.MessageSecurityVersion, values));
            }

            if (!transportSecurityBindingElement.IncludeTimestamp)
            {
                s_bindingValidationErrors.Add(SR.BindingTransportSecurityMustIncludeTimestamp);
            }

            if (transportSecurityBindingElement.DefaultAlgorithmSuite != SecurityAlgorithmSuite.Default)
            {
                s_bindingValidationErrors.Add(string.Format(SR.BindingTransportSecurityDefaultAlgorithmSuiteNotSupportedFormat,
                    transportSecurityBindingElement.DefaultAlgorithmSuite.GetType().FullName, SecurityAlgorithmSuite.Default.GetType().FullName));
            }

            if (transportSecurityBindingElement.SecurityHeaderLayout != SecurityHeaderLayout.Strict)
            {
                s_bindingValidationErrors.Add(string.Format(SR.BindingTransportSecurityHeaderLayoutValueNotSupportedFormat,
                            transportSecurityBindingElement.SecurityHeaderLayout, SecurityHeaderLayout.Strict));
            }

            if (transportSecurityBindingElement.LocalClientSettings.DetectReplays || transportSecurityBindingElement.LocalServiceSettings.DetectReplays)
            {
                s_bindingValidationErrors.Add(SR.BindingTransportDetectReplaysNotSupported);
            }
        }

        private static void ValidateUserNamePasswordSecurityBindingElement(TransportSecurityBindingElement transportSecurityBindingElement)
        {
            bool singleSignedEncryptedParam = transportSecurityBindingElement.EndpointSupportingTokenParameters.SignedEncrypted.Count == 1;
            System.Diagnostics.Debug.Assert(singleSignedEncryptedParam, "Unexpected number of SignedEncrypted token parameters in transport security binding!");

            if (!singleSignedEncryptedParam)
            {
                return;
            }

            if (transportSecurityBindingElement.MessageSecurityVersion != MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10 &&
                transportSecurityBindingElement.MessageSecurityVersion != MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10 &&
                transportSecurityBindingElement.MessageSecurityVersion != MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11)
            {
                string values = string.Format(CultureInfo.InvariantCulture, "'{0}', '{1}', '{2}'",
                    MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10,
                    MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10,
                    MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11);

                s_bindingValidationErrors.Add(string.Format(SR.BindingTransportMessageSecurityVersionNotSupportedFormat, transportSecurityBindingElement.MessageSecurityVersion, values));
            }

            if (transportSecurityBindingElement.DefaultAlgorithmSuite != SecurityAlgorithmSuite.Default)
            {
                s_bindingValidationErrors.Add(string.Format(SR.BindingTransportSecurityDefaultAlgorithmSuiteNotSupportedFormat,
                    transportSecurityBindingElement.DefaultAlgorithmSuite.GetType().FullName, SecurityAlgorithmSuite.Default.GetType().FullName));
            }

            var userNameParams = transportSecurityBindingElement.EndpointSupportingTokenParameters.SignedEncrypted[0] as UserNameSecurityTokenParameters;

            if (userNameParams != null)
            {
                if (userNameParams.InclusionMode != SecurityTokenInclusionMode.AlwaysToRecipient)
                {
                    s_bindingValidationErrors.Add(string.Format(SR.BindingTransportSecurityTokenParamsInclusionModeValueNotSupportedFormat,
                        userNameParams.InclusionMode, SecurityTokenInclusionMode.AlwaysToRecipient));
                }
                if (userNameParams.ReferenceStyle != SecurityTokenReferenceStyle.Internal)
                {
                    s_bindingValidationErrors.Add(string.Format(SR.BindingTransportSecurityTokenParamsReferenceStyleNotSupportedFormat,
                        userNameParams.ReferenceStyle, SecurityTokenReferenceStyle.Internal));
                }
                if (userNameParams.RequireDerivedKeys != false)
                {
                    s_bindingValidationErrors.Add(SR.BindingTransportSecurityTokenParamsRequiringDerivedKeysNotSupported);
                }
            }
            else
            {
                s_bindingValidationErrors.Add(string.Format(SR.BindingTransportSecurityTokenParamsTypeNotSupportedFormat,
                    transportSecurityBindingElement.EndpointSupportingTokenParameters.SignedEncrypted[0].GetType().FullName, typeof(UserNameSecurityTokenParameters).FullName));
            }
        }

        private void AddWarning(ServiceEndpoint endpoint, int i)
        {
            MetadataConversionError warning;

            foreach (var validationErrorMsg in s_bindingValidationErrors)
            {
                warning = new MetadataConversionError(validationErrorMsg, isWarning: true);
                if (!importer.Errors.Contains(warning))
                {
                    importer.Errors.Add(warning);
                }
            }

            string incompatEndpointMsg = string.Format(CultureInfo.InvariantCulture, SR.WrnIncompatibleEndpointFormat, endpoint.Name, endpoint.Address);

            warning = new MetadataConversionError(incompatEndpointMsg, isWarning: true);
            if (!importer.Errors.Contains(warning))
            {
                importer.Errors.Add(warning);
            }
        }
    }
}
