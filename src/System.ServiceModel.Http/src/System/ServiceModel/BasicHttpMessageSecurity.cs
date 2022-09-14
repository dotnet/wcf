// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.ComponentModel;

namespace System.ServiceModel
{
    public sealed class BasicHttpMessageSecurity
    {
        internal const BasicHttpMessageCredentialType DefaultClientCredentialType = BasicHttpMessageCredentialType.UserName;

        BasicHttpMessageCredentialType clientCredentialType;
        SecurityAlgorithmSuite algorithmSuite;

        public BasicHttpMessageSecurity()
        {
            clientCredentialType = DefaultClientCredentialType;
            algorithmSuite = SecurityAlgorithmSuite.Default;
        }

        public BasicHttpMessageCredentialType ClientCredentialType
        {
            get { return this.clientCredentialType; }
            set
            {
                if (!BasicHttpMessageCredentialTypeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
                }
                this.clientCredentialType = value;
            }
        }

        public SecurityAlgorithmSuite AlgorithmSuite
        {
            get { return this.algorithmSuite; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }
                this.algorithmSuite = value;
            }
        }

        internal SecurityBindingElement CreateMessageSecurity(bool isSecureTransportMode)
        {
            SecurityBindingElement result;

            if (isSecureTransportMode)
            {
                MessageSecurityVersion version = MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;
                switch (this.clientCredentialType)
                {
                    case BasicHttpMessageCredentialType.Certificate:
                        result = SecurityBindingElement.CreateCertificateOverTransportBindingElement(version);
                        break;
                    case BasicHttpMessageCredentialType.UserName:
                        result = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
                        result.MessageSecurityVersion = version;
                        break;
                    default:
                        Fx.Assert("Unsupported basic http message credential type");
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
                }
            }
            else
            {
                if (this.clientCredentialType != BasicHttpMessageCredentialType.Certificate)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.BasicHttpMessageSecurityRequiresCertificate));
                }
                result = SecurityBindingElement.CreateMutualCertificateBindingElement(MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10, true);
            }

            result.DefaultAlgorithmSuite = this.AlgorithmSuite;
            result.SecurityHeaderLayout = SecurityHeaderLayout.Lax;
            result.SetKeyDerivation(false);

            return result;
        }
    }
}
