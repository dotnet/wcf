// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime;

namespace System.ServiceModel.Channels
{
    public sealed class UnixDomainSocketConnectionPoolSettings
    {
        private string _groupName;
        private TimeSpan _idleTimeout;
        private TimeSpan _leaseTimeout;
        private int _maxOutboundConnectionsPerEndpoint;

        internal UnixDomainSocketConnectionPoolSettings()
        {
            _groupName = ConnectionOrientedTransportDefaults.ConnectionPoolGroupName;
            _idleTimeout = ConnectionOrientedTransportDefaults.IdleTimeout;
            _leaseTimeout = UnixDomainSocketTransportDefaults.ConnectionLeaseTimeout;
            _maxOutboundConnectionsPerEndpoint = ConnectionOrientedTransportDefaults.MaxOutboundConnectionsPerEndpoint;
        }

        internal UnixDomainSocketConnectionPoolSettings(UnixDomainSocketConnectionPoolSettings unixDomainSocketConnectionPoolSettings)
        {
            _groupName = unixDomainSocketConnectionPoolSettings._groupName;
            _idleTimeout = unixDomainSocketConnectionPoolSettings._idleTimeout;
            _leaseTimeout = unixDomainSocketConnectionPoolSettings._leaseTimeout;
            _maxOutboundConnectionsPerEndpoint = unixDomainSocketConnectionPoolSettings._maxOutboundConnectionsPerEndpoint;
        }

        public string GroupName
        {
            get { return _groupName; }
            set
            {
                _groupName = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
            }
        }

        public TimeSpan IdleTimeout
        {
            get { return _idleTimeout; }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SR.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SR.SFxTimeoutOutOfRangeTooBig));
                }

                _idleTimeout = value;
            }
        }

        public TimeSpan LeaseTimeout
        {
            get { return _leaseTimeout; }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SR.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SR.SFxTimeoutOutOfRangeTooBig));
                }

                _leaseTimeout = value;
            }
        }

        public int MaxOutboundConnectionsPerEndpoint
        {
            get { return _maxOutboundConnectionsPerEndpoint; }
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SR.ValueMustBeNonNegative));
                }

                _maxOutboundConnectionsPerEndpoint = value;
            }
        }

        internal UnixDomainSocketConnectionPoolSettings Clone()
        {
            return new UnixDomainSocketConnectionPoolSettings(this);
        }

        internal bool IsMatch(UnixDomainSocketConnectionPoolSettings unixDomainSocketConnectionPoolSettings)
        {
            if (_groupName != unixDomainSocketConnectionPoolSettings._groupName)
            {
                return false;
            }

            if (_idleTimeout != unixDomainSocketConnectionPoolSettings._idleTimeout)
            {
                return false;
            }

            if (_leaseTimeout != unixDomainSocketConnectionPoolSettings._leaseTimeout)
            {
                return false;
            }

            if (_maxOutboundConnectionsPerEndpoint != unixDomainSocketConnectionPoolSettings._maxOutboundConnectionsPerEndpoint)
            {
                return false;
            }

            return true;
        }
    }
}
