// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public sealed class NetMsmqSecurity
    {
        internal const NetMsmqSecurityMode DefaultMode = NetMsmqSecurityMode.Transport;

        private NetMsmqSecurityMode _mode;
        private MsmqTransportSecurity _transportSecurity;
        private MessageSecurityOverMsmq _messageSecurity;

        public NetMsmqSecurity()
            : this(DefaultMode, null, null)
        {
        }

        internal NetMsmqSecurity(NetMsmqSecurityMode mode)
            : this(mode, null, null)
        {
        }

        private NetMsmqSecurity(NetMsmqSecurityMode mode, MsmqTransportSecurity transportSecurity, MessageSecurityOverMsmq messageSecurity)
        {
            if (!NetMsmqSecurityModeHelper.IsDefined(mode))
            {
                throw new ArgumentOutOfRangeException(nameof(mode));
            }
            _mode = mode;
            _transportSecurity = transportSecurity ?? new MsmqTransportSecurity();
            _messageSecurity = messageSecurity ?? new MessageSecurityOverMsmq();
        }

        [DefaultValue(DefaultMode)]
        public NetMsmqSecurityMode Mode
        {
            get { return _mode; }
            set
            {
                if (!NetMsmqSecurityModeHelper.IsDefined(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _mode = value;
            }
        }

        public MsmqTransportSecurity Transport
        {
            get
            {
                return _transportSecurity ??= new MsmqTransportSecurity();
            }
            set { _transportSecurity = value; }
        }

        public MessageSecurityOverMsmq Message
        {
            get
            {
                return _messageSecurity ??= new MessageSecurityOverMsmq();
            }
            set { _messageSecurity = value; }
        }

        internal void ConfigureTransportSecurity(MsmqBindingElementBase msmq)
        {
            if (_mode == NetMsmqSecurityMode.Transport || _mode == NetMsmqSecurityMode.Both)
            {
                msmq.MsmqTransportSecurity = Transport;
            }
            else
            {
                msmq.MsmqTransportSecurity.Disable();
            }
        }
    }
}
