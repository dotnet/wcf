// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Net.Security;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace System.ServiceModel
{
    public sealed class TcpTransportSecurity
    {
        internal const TcpClientCredentialType DefaultClientCredentialType = TcpClientCredentialType.Windows;
        internal const ProtectionLevel DefaultProtectionLevel = ProtectionLevel.EncryptAndSign;

        private TcpClientCredentialType _clientCredentialType;
        private ProtectionLevel _protectionLevel;

        public TcpTransportSecurity()
        {
            _clientCredentialType = DefaultClientCredentialType;
            _protectionLevel = DefaultProtectionLevel;
        }

        [DefaultValue(DefaultClientCredentialType)]
        public TcpClientCredentialType ClientCredentialType
        {
            get { return _clientCredentialType; }
            set
            {
                if (!TcpClientCredentialTypeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                _clientCredentialType = value;
            }
        }

        private SslStreamSecurityBindingElement CreateSslBindingElement(bool requireClientCertificate)
        {
            throw ExceptionHelper.PlatformNotSupported("TcpTransportSecurity.CreateSslBindingElement is not supported.");
        }

        private static bool IsSslBindingElement(BindingElement element, TcpTransportSecurity transportSecurity, out bool requireClientCertificate)
        {
            requireClientCertificate = false;
            SslStreamSecurityBindingElement ssl = element as SslStreamSecurityBindingElement;
            if (ssl == null)
                return false;
            transportSecurity._protectionLevel = ProtectionLevel.EncryptAndSign;
            requireClientCertificate = ssl.RequireClientCertificate;
            return true;
        }

        internal BindingElement CreateTransportProtectionOnly()
        {
            throw ExceptionHelper.PlatformNotSupported("TcpTransportSecurity.CreateTransportProtectionOnly is not supported.");
        }

        internal static bool SetTransportProtectionOnly(BindingElement transport, TcpTransportSecurity transportSecurity)
        {
            bool requireClientCertificate;
            return IsSslBindingElement(transport, transportSecurity, out requireClientCertificate);
        }

        internal BindingElement CreateTransportProtectionAndAuthentication()
        {
            if (_clientCredentialType == TcpClientCredentialType.Certificate || _clientCredentialType == TcpClientCredentialType.None)
            {
                return this.CreateSslBindingElement(_clientCredentialType == TcpClientCredentialType.Certificate);
            }
            else
            {
                WindowsStreamSecurityBindingElement result = new WindowsStreamSecurityBindingElement();
                result.ProtectionLevel = _protectionLevel;
                return result;
            }
        }

        internal static bool SetTransportProtectionAndAuthentication(BindingElement transport, TcpTransportSecurity transportSecurity)
        {
            bool requireClientCertificate = false;
            if (transport is WindowsStreamSecurityBindingElement)
            {
                transportSecurity.ClientCredentialType = TcpClientCredentialType.Windows;
                transportSecurity._protectionLevel = ((WindowsStreamSecurityBindingElement)transport).ProtectionLevel;
                return true;
            }
            else if (IsSslBindingElement(transport, transportSecurity, out requireClientCertificate))
            {
                transportSecurity.ClientCredentialType = requireClientCertificate ? TcpClientCredentialType.Certificate : TcpClientCredentialType.None;
                return true;
            }
            return false;
        }

        internal bool InternalShouldSerialize()
        {
            return this.ClientCredentialType != TcpTransportSecurity.DefaultClientCredentialType
                || _protectionLevel != TcpTransportSecurity.DefaultProtectionLevel;
        }
    }
}
