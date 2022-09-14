// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public sealed class NetTcpSecurity
    {
        internal const SecurityMode DefaultMode = SecurityMode.Transport;

        private SecurityMode _mode;

        public NetTcpSecurity() : this(DefaultMode, new TcpTransportSecurity(), new MessageSecurityOverTcp()) { }

        private NetTcpSecurity(SecurityMode mode, TcpTransportSecurity transportSecurity, MessageSecurityOverTcp messageSecurity)
        {
            Contract.Assert(SecurityModeHelper.IsDefined(mode),
                            string.Format("Invalid SecurityMode value: {0} = {1} (default is {2} = {3}).",
                                            (int)mode,
                                            mode.ToString(),
                                            (int)SecurityMode.Transport,
                                            SecurityMode.Transport.ToString()));

            _mode = mode;
            Transport = transportSecurity ?? new TcpTransportSecurity();
            Message = messageSecurity ?? new MessageSecurityOverTcp();
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

        public MessageSecurityOverTcp Message { get; set; }

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

        internal SecurityBindingElement CreateMessageSecurity(bool isReliableSessionEnabled)
        {
            if (_mode == SecurityMode.Message)
            {
                throw new PlatformNotSupportedException();
            }
            else if (_mode == SecurityMode.TransportWithMessageCredential)
            {
                return Message.CreateSecurityBindingElement(true, isReliableSessionEnabled, CreateTransportSecurity());
            }
            else
            {
                return null;
            }
        }
    }
}


