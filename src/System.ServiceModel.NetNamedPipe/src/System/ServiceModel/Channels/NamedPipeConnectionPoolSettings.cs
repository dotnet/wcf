// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime;
using System.Runtime.Versioning;

namespace System.ServiceModel.Channels
{
    [SupportedOSPlatform("windows")]
    public sealed class NamedPipeConnectionPoolSettings
    {
        private string _groupName;
        private TimeSpan _idleTimeout;
        private int _maxOutputConnectionsPerEndpoint;

        internal NamedPipeConnectionPoolSettings()
        {
            _groupName = ConnectionOrientedTransportDefaults.ConnectionPoolGroupName;
            _idleTimeout = ConnectionOrientedTransportDefaults.IdleTimeout;
            _maxOutputConnectionsPerEndpoint = ConnectionOrientedTransportDefaults.MaxOutboundConnectionsPerEndpoint;
        }

        internal NamedPipeConnectionPoolSettings(NamedPipeConnectionPoolSettings namedPipe)
        {
            _groupName = namedPipe._groupName;
            _idleTimeout = namedPipe._idleTimeout;
            _maxOutputConnectionsPerEndpoint = namedPipe._maxOutputConnectionsPerEndpoint;
        }

        public string GroupName
        {
            get { return _groupName; }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));

                _groupName = value;
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

        public int MaxOutboundConnectionsPerEndpoint
        {
            get { return _maxOutputConnectionsPerEndpoint; }
            set
            {
                if (value < 0)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SR.ValueMustBeNonNegative));

                _maxOutputConnectionsPerEndpoint = value;
            }
        }

        internal NamedPipeConnectionPoolSettings Clone()
        {
            return new NamedPipeConnectionPoolSettings(this);
        }

        internal bool IsMatch(NamedPipeConnectionPoolSettings namedPipe)
        {
            if (_groupName != namedPipe._groupName)
                return false;

            if (_idleTimeout != namedPipe._idleTimeout)
                return false;

            if (_maxOutputConnectionsPerEndpoint != namedPipe._maxOutputConnectionsPerEndpoint)
                return false;

            return true;
        }
    }
}
