// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public sealed class NetTcpSecurity
    {
        internal const SecurityMode DefaultMode = SecurityMode.Transport;

        private SecurityMode _mode;
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
            Transport = transportSecurity == null ? new TcpTransportSecurity() : transportSecurity;
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
                }
                _mode = value;
            }
        }

        public TcpTransportSecurity Transport { get; set; }

        public MessageSecurityOverTcp Message
        {
            get { return _messageSecurity; }
            set { _messageSecurity = value; }
        }


        internal BindingElement CreateTransportSecurity()
        {
            if (_mode == SecurityMode.TransportWithMessageCredential)
            {
                return Transport.CreateTransportProtectionOnly();
            }
            else if (_mode == SecurityMode.Transport)
            {
                return Transport.CreateTransportProtectionAndAuthentication();
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
    }
}

