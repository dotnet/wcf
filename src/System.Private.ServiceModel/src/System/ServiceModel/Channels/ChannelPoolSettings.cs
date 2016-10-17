// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime;

namespace System.ServiceModel.Channels
{
    public class ChannelPoolSettings
    {
        private TimeSpan _idleTimeout;
        private TimeSpan _leaseTimeout;
        private int _maxOutboundChannelsPerEndpoint;

        public ChannelPoolSettings()
        {
            _idleTimeout = OneWayDefaults.IdleTimeout;
            _leaseTimeout = OneWayDefaults.LeaseTimeout;
            _maxOutboundChannelsPerEndpoint = OneWayDefaults.MaxOutboundChannelsPerEndpoint;
        }

        ChannelPoolSettings(ChannelPoolSettings poolToBeCloned)
        {
            _idleTimeout = poolToBeCloned._idleTimeout;
            _leaseTimeout = poolToBeCloned._leaseTimeout;
            _maxOutboundChannelsPerEndpoint = poolToBeCloned._maxOutboundChannelsPerEndpoint;
        }

        [DefaultValue(typeof(TimeSpan), OneWayDefaults.IdleTimeoutString)]
        public TimeSpan IdleTimeout
        {
            get
            {
                return _idleTimeout;
            }

            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SR.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SR.SFxTimeoutOutOfRangeTooBig));
                }

                _idleTimeout = value;
            }
        }

        [DefaultValue(typeof(TimeSpan), OneWayDefaults.LeaseTimeoutString)]
        public TimeSpan LeaseTimeout
        {
            get
            {
                return _leaseTimeout;
            }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SR.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SR.SFxTimeoutOutOfRangeTooBig));
                }

                _leaseTimeout = value;
            }
        }

        [DefaultValue(OneWayDefaults.MaxOutboundChannelsPerEndpoint)]
        public int MaxOutboundChannelsPerEndpoint
        {
            get
            {
                return _maxOutboundChannelsPerEndpoint;
            }
            set
            {
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SR.ValueMustBePositive));
                }

                _maxOutboundChannelsPerEndpoint = value;
            }
        }

        internal ChannelPoolSettings Clone()
        {
            return new ChannelPoolSettings(this);
        }

        internal bool IsMatch(ChannelPoolSettings channelPoolSettings)
        {
            if (channelPoolSettings == null)
            {
                return false;
            }

            if (_idleTimeout != channelPoolSettings._idleTimeout)
            {
                return false;
            }

            if (_leaseTimeout != channelPoolSettings._leaseTimeout)
            {
                return false;
            }

            if (_maxOutboundChannelsPerEndpoint != channelPoolSettings._maxOutboundChannelsPerEndpoint)
            {
                return false;
            }

            return true;
        }
    }
}
