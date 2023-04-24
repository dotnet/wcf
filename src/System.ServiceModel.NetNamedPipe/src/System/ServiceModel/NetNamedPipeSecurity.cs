// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    [SupportedOSPlatform("windows")]
    public sealed class NetNamedPipeSecurity
    {
        internal const NetNamedPipeSecurityMode DefaultMode = NetNamedPipeSecurityMode.Transport;
        private NetNamedPipeSecurityMode _mode;
        private NamedPipeTransportSecurity _transport = new NamedPipeTransportSecurity();

        public NetNamedPipeSecurity()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw ExceptionHelper.PlatformNotSupported(SR.Format(SR.PlatformNotSupported_NetNamedPipe));
            }

            _mode = DefaultMode;
        }

        [DefaultValue(DefaultMode)]
        public NetNamedPipeSecurityMode Mode
        {
            get { return _mode; }
            set
            {
                if (!NetNamedPipeSecurityModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                _mode = value;
            }
        }

        public NamedPipeTransportSecurity Transport
        {
            get { return _transport; }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                _transport = value;
            }
        }

        internal WindowsStreamSecurityBindingElement CreateTransportSecurity()
        {
            if (_mode == NetNamedPipeSecurityMode.Transport)
            {
                return _transport.CreateTransportProtectionAndAuthentication();
            }
            else
            {
                return null;
            }
        }
    }
}
