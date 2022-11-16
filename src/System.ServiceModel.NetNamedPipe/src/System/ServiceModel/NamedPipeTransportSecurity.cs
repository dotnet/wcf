﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Net.Security;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace System.ServiceModel
{
    public sealed class NamedPipeTransportSecurity
    {
        internal const ProtectionLevel DefaultProtectionLevel = ProtectionLevel.EncryptAndSign;
        private ProtectionLevel _protectionLevel;

        public NamedPipeTransportSecurity()
        {
            _protectionLevel = DefaultProtectionLevel;
        }

        [DefaultValue(ConnectionOrientedTransportDefaults.ProtectionLevel)]
        public ProtectionLevel ProtectionLevel
        {
            get { return _protectionLevel; }
            set
            {
                if (!ProtectionLevelHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
                }
                _protectionLevel = value;
            }
        }

        internal WindowsStreamSecurityBindingElement CreateTransportProtectionAndAuthentication()
        {
            WindowsStreamSecurityBindingElement result = new WindowsStreamSecurityBindingElement();
            result.ProtectionLevel = _protectionLevel;
            return result;
        }
    }
}
