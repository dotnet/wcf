// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public sealed class UnixDomainSocketSecurity
    {
        internal const UnixDomainSocketSecurityMode DefaultMode = UnixDomainSocketSecurityMode.Transport;

        private UnixDomainSocketSecurityMode _mode;

        public UnixDomainSocketSecurity() : this(DefaultMode, new UnixDomainSocketTransportSecurity()) { }

        private UnixDomainSocketSecurity(UnixDomainSocketSecurityMode mode, UnixDomainSocketTransportSecurity transportSecurity)
        {
            Contract.Assert(UnixDomainSocketSecurityModeHelper.IsDefined(mode),
                            string.Format("Invalid SecurityMode value: {0} = {1} (default is {2} = {3}).",
                                            (int)mode,
                                            mode.ToString(),
                                            (int)SecurityMode.Transport,
                                            SecurityMode.Transport.ToString()));

            _mode = mode;
            Transport = transportSecurity ?? new UnixDomainSocketTransportSecurity();
        }

        [DefaultValue(DefaultMode)]
        public UnixDomainSocketSecurityMode Mode
        {
            get { return _mode; }
            set
            {
                if (!UnixDomainSocketSecurityModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
                }
                _mode = value;
            }
        }

        public UnixDomainSocketTransportSecurity Transport { get; set; }

        internal BindingElement CreateTransportSecurity()
        {
            if (_mode == UnixDomainSocketSecurityMode.Transport || _mode == UnixDomainSocketSecurityMode.TransportCredentialOnly)
            {
                if(_mode == UnixDomainSocketSecurityMode.TransportCredentialOnly && Transport.ClientCredentialType != UnixDomainSocketClientCredentialType.PosixIdentity)
                {
                    throw new NotSupportedException();
                }
                return Transport.CreateTransportProtectionAndAuthentication();
            }
            else if(_mode == UnixDomainSocketSecurityMode.None)
            {
                return null;
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
