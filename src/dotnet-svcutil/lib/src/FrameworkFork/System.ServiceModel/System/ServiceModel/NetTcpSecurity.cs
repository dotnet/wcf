// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public sealed class NetTcpSecurity
    {
        internal const SecurityMode DefaultMode = SecurityMode.Transport;

        private SecurityMode _mode;
        private TcpTransportSecurity _transportSecurity;
        private MessageSecurityOverTcp _messageSecurity;

        public NetTcpSecurity()
            : this(DefaultMode, new TcpTransportSecurity(), new MessageSecurityOverTcp())
        {
        }

        private NetTcpSecurity(SecurityMode mode, TcpTransportSecurity transportSecurity, MessageSecurityOverTcp messageSecurity)
        {
            Contract.Assert(SecurityModeHelper.IsDefined(mode),
                            string.Format("Invalid SecurityMode value: {0} = {1} (default is {2} = {3}).",
                                            (int)mode,
                                            mode.ToString(),
                                            (int)SecurityMode.Transport,
                                            SecurityMode.Transport.ToString()));

            _mode = mode;
            _transportSecurity = transportSecurity == null ? new TcpTransportSecurity() : transportSecurity;
            _messageSecurity = messageSecurity == null ? new MessageSecurityOverTcp() : messageSecurity;
        }

        [DefaultValue(DefaultMode)]
        public SecurityMode Mode
        {
            get { return _mode; }
            set
            {
                if (!SecurityModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                _mode = value;
            }
        }

        public TcpTransportSecurity Transport
        {
            get { return _transportSecurity; }
            set { _transportSecurity = value; }
        }

        public MessageSecurityOverTcp Message
        {
            get { return _messageSecurity; }
            set { _messageSecurity = value; }
        }


        internal BindingElement CreateTransportSecurity()
        {
            if (_mode == SecurityMode.TransportWithMessageCredential)
            {
                return _transportSecurity.CreateTransportProtectionOnly();
            }
            else if (_mode == SecurityMode.Transport)
            {
                return _transportSecurity.CreateTransportProtectionAndAuthentication();
            }
            else
            {
                return null;
            }
        }

        internal static UnifiedSecurityMode GetModeFromTransportSecurity(BindingElement transport)
        {
            if (transport == null)
            {
                return UnifiedSecurityMode.None | UnifiedSecurityMode.Message;
            }
            else
            {
                return UnifiedSecurityMode.TransportWithMessageCredential | UnifiedSecurityMode.Transport;
            }
        }

        internal static bool SetTransportSecurity(BindingElement transport, SecurityMode mode, TcpTransportSecurity transportSecurity)
        {
            if (mode == SecurityMode.TransportWithMessageCredential)
            {
                return TcpTransportSecurity.SetTransportProtectionOnly(transport, transportSecurity);
            }
            else if (mode == SecurityMode.Transport)
            {
                return TcpTransportSecurity.SetTransportProtectionAndAuthentication(transport, transportSecurity);
            }
            return transport == null;
        }

        internal SecurityBindingElement CreateMessageSecurity(bool isReliableSessionEnabled)
        {
            if (_mode == SecurityMode.Message)
            {
                return _messageSecurity.CreateSecurityBindingElement(false, isReliableSessionEnabled, null);
            }
            else if (_mode == SecurityMode.TransportWithMessageCredential)
            {
                return _messageSecurity.CreateSecurityBindingElement(true, isReliableSessionEnabled, this.CreateTransportSecurity());
            }
            else
            {
                return null;
            }
        }

        internal static bool TryCreate(SecurityBindingElement wsSecurity, SecurityMode mode, bool isReliableSessionEnabled, BindingElement transportSecurity, TcpTransportSecurity tcpTransportSecurity, out NetTcpSecurity security)
        {
            security = null;
            MessageSecurityOverTcp messageSecurity = null;
            if (mode == SecurityMode.Message)
            {
                if (!MessageSecurityOverTcp.TryCreate(wsSecurity, isReliableSessionEnabled, null, out messageSecurity))
                    return false;
            }
            else if (mode == SecurityMode.TransportWithMessageCredential)
            {
                if (!MessageSecurityOverTcp.TryCreate(wsSecurity, isReliableSessionEnabled, transportSecurity, out messageSecurity))
                    return false;
            }
            security = new NetTcpSecurity(mode, tcpTransportSecurity, messageSecurity);
            return System.ServiceModel.Configuration.SecurityElement.AreBindingsMatching(security.CreateMessageSecurity(isReliableSessionEnabled), wsSecurity, false);
        }
    }
}

