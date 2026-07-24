// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;
using System.ServiceModel.Channels;

namespace System.ServiceModel.MsmqIntegration
{
    public sealed class MsmqIntegrationSecurity
    {
        internal const MsmqIntegrationSecurityMode DefaultMode = MsmqIntegrationSecurityMode.Transport;

        private MsmqIntegrationSecurityMode _mode;
        private MsmqTransportSecurity _transportSecurity;

        public MsmqIntegrationSecurity()
        {
            _mode = DefaultMode;
            _transportSecurity = new MsmqTransportSecurity();
        }

        [DefaultValue(DefaultMode)]
        public MsmqIntegrationSecurityMode Mode
        {
            get { return _mode; }
            set
            {
                if (!MsmqIntegrationSecurityModeHelper.IsDefined(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _mode = value;
            }
        }

        public MsmqTransportSecurity Transport
        {
            get { return _transportSecurity; }
            set { _transportSecurity = value; }
        }

        internal void ConfigureTransportSecurity(MsmqBindingElementBase msmq)
        {
            if (_mode == MsmqIntegrationSecurityMode.Transport)
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
