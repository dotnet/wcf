// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using System.Runtime;

    public sealed class NamedPipeConnectionPoolSettings
    {
        string _groupName;
        TimeSpan _idleTimeout;
        int _maxOutputConnectionsPerEndpoint;

        internal NamedPipeConnectionPoolSettings()
        {
            _groupName = ConnectionOrientedTransportDefaults.ConnectionPoolGroupName;
            _idleTimeout = ConnectionOrientedTransportDefaults.IdleTimeout;
            _maxOutputConnectionsPerEndpoint = ConnectionOrientedTransportDefaults.MaxOutboundConnectionsPerEndpoint;
        }

        internal NamedPipeConnectionPoolSettings(NamedPipeConnectionPoolSettings namedPipe)
        {
            this._groupName = namedPipe._groupName;
            this._idleTimeout = namedPipe._idleTimeout;
            this._maxOutputConnectionsPerEndpoint = namedPipe._maxOutputConnectionsPerEndpoint;
        }

        public string GroupName
        {
            get { return this._groupName; }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");

                this._groupName = value;
            }
        }

        public TimeSpan IdleTimeout
        {
            get { return this._idleTimeout; }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        "SR.SFxTimeoutOutOfRange0"));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        "SR.SFxTimeoutOutOfRangeTooBig"));
                }

                this._idleTimeout = value;
            }
        }

        public int MaxOutboundConnectionsPerEndpoint
        {
            get { return this._maxOutputConnectionsPerEndpoint; }
            set
            {
                if (value < 0)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        "SR.ValueMustBeNonNegative"));

                this._maxOutputConnectionsPerEndpoint = value;
            }
        }

        internal NamedPipeConnectionPoolSettings Clone()
        {
            return new NamedPipeConnectionPoolSettings(this);
        }

        internal bool IsMatch(NamedPipeConnectionPoolSettings namedPipe)
        {
            if (this._groupName != namedPipe._groupName)
                return false;

            if (this._idleTimeout != namedPipe._idleTimeout)
                return false;

            if (this._maxOutputConnectionsPerEndpoint != namedPipe._maxOutputConnectionsPerEndpoint)
                return false;

            return true;
        }
    }
}
