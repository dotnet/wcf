// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public sealed class NetNamedPipeSecurity
    {
        internal const NetNamedPipeSecurityMode DefaultMode = NetNamedPipeSecurityMode.Transport;
        NetNamedPipeSecurityMode _mode;
        NamedPipeTransportSecurity _transport = new NamedPipeTransportSecurity();

        public NetNamedPipeSecurity()
        {
            this._mode = DefaultMode;
        }

        NetNamedPipeSecurity(NetNamedPipeSecurityMode mode, NamedPipeTransportSecurity transport)
        {
            this._mode = mode;
            this._transport = transport == null ? new NamedPipeTransportSecurity() : transport;
        }

        [DefaultValue(DefaultMode)]
        public NetNamedPipeSecurityMode Mode
        {
            get { return this._mode; }
            set
            {
                if (!NetNamedPipeSecurityModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this._mode = value;
            }
        }

        public NamedPipeTransportSecurity Transport
        {
            get { return this._transport; }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                this._transport = value;
            }
        }

        internal WindowsStreamSecurityBindingElement CreateTransportSecurity()
        {
            if (_mode == NetNamedPipeSecurityMode.Transport)
            {
                return this._transport.CreateTransportProtectionAndAuthentication();
            }
            else
            {
                return null;
            }
        }

        internal static bool TryCreate(WindowsStreamSecurityBindingElement wssbe, NetNamedPipeSecurityMode mode, out NetNamedPipeSecurity security)
        {
            security = null;
            NamedPipeTransportSecurity transportSecurity = new NamedPipeTransportSecurity();
            if (mode == NetNamedPipeSecurityMode.Transport)
            {
                if (!NamedPipeTransportSecurity.IsTransportProtectionAndAuthentication(wssbe, transportSecurity))
                    return false;
            }
            security = new NetNamedPipeSecurity(mode, transportSecurity);
            return true;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeTransport()
        {
            if (this._transport.ProtectionLevel == ConnectionOrientedTransportDefaults.ProtectionLevel)
            {
                return false;
            }
            return true;
        }
    }
}
