// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Net.Security;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace System.ServiceModel
{
    public sealed class NamedPipeTransportSecurity
    {
        internal const ProtectionLevel DefaultProtectionLevel = ProtectionLevel.EncryptAndSign;
        ProtectionLevel _protectionLevel;

        public NamedPipeTransportSecurity()
        {
            this._protectionLevel = DefaultProtectionLevel;
        }

        [DefaultValue(ConnectionOrientedTransportDefaults.ProtectionLevel)]
        public ProtectionLevel ProtectionLevel
        {
            get { return this._protectionLevel; }
            set
            {
                if (!ProtectionLevelHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this._protectionLevel = value;
            }
        }

        internal WindowsStreamSecurityBindingElement CreateTransportProtectionAndAuthentication()
        {
            WindowsStreamSecurityBindingElement result = new WindowsStreamSecurityBindingElement();
            result.ProtectionLevel = this._protectionLevel;
            return result;
        }

        internal static bool IsTransportProtectionAndAuthentication(WindowsStreamSecurityBindingElement wssbe, NamedPipeTransportSecurity transportSecurity)
        {
            transportSecurity._protectionLevel = wssbe.ProtectionLevel;
            return true;
        }
    }
}
