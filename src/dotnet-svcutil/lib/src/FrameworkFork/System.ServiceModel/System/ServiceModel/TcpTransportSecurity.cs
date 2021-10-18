// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Net.Security;
using System.Security.Authentication;
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
        private SslProtocols _sslProtocols;

        public TcpTransportSecurity()
        {
            _clientCredentialType = DefaultClientCredentialType;
            _protectionLevel = DefaultProtectionLevel;
            _sslProtocols = TransportDefaults.SslProtocols;
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

        [DefaultValue(DefaultProtectionLevel)]
        public ProtectionLevel ProtectionLevel
        {
            get { return _protectionLevel; }
            set
            {
                if (!ProtectionLevelHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                _protectionLevel = value;
            }
        }


        [DefaultValue(TransportDefaults.SslProtocols)]
        public SslProtocols SslProtocols
        {
            get { return _sslProtocols; }
            set
            {
                SslProtocolsHelper.Validate(value);
                _sslProtocols = value;
            }
        }

        private SslStreamSecurityBindingElement CreateSslBindingElement(bool requireClientCertificate)
        {
            if (_protectionLevel != ProtectionLevel.EncryptAndSign)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(
                    SRServiceModel.UnsupportedSslProtectionLevel, _protectionLevel)));
            }

            SslStreamSecurityBindingElement result = new SslStreamSecurityBindingElement();
            result.RequireClientCertificate = requireClientCertificate;
            result.SslProtocols = _sslProtocols;
            return result;
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
            return CreateSslBindingElement(false);
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
