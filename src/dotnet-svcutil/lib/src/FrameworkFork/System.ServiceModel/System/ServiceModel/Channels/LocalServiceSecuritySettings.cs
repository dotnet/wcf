// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using System;
    using System.Globalization;
    using System.Runtime;
    using System.ServiceModel;
    using System.ServiceModel.Security;

    public sealed class LocalServiceSecuritySettings
    {
        private bool _detectReplays;
        private int _replayCacheSize;
        private TimeSpan _replayWindow;
        private TimeSpan _maxClockSkew;
        private TimeSpan _issuedCookieLifetime;
        private int _maxStatefulNegotiations;
        private TimeSpan _negotiationTimeout;
        private int _maxCachedCookies;
        private int _maxPendingSessions;
        private TimeSpan _inactivityTimeout;
        private TimeSpan _sessionKeyRenewalInterval;
        private TimeSpan _sessionKeyRolloverInterval;
        private bool _reconnectTransportOnFailure;
        private TimeSpan _timestampValidityDuration;
        private NonceCache _nonceCache = null;

        private LocalServiceSecuritySettings(LocalServiceSecuritySettings other)
        {
            _detectReplays = other._detectReplays;
            _replayCacheSize = other._replayCacheSize;
            _replayWindow = other._replayWindow;
            _maxClockSkew = other._maxClockSkew;
            _issuedCookieLifetime = other._issuedCookieLifetime;
            _maxStatefulNegotiations = other._maxStatefulNegotiations;
            _negotiationTimeout = other._negotiationTimeout;
            _maxPendingSessions = other._maxPendingSessions;
            _inactivityTimeout = other._inactivityTimeout;
            _sessionKeyRenewalInterval = other._sessionKeyRenewalInterval;
            _sessionKeyRolloverInterval = other._sessionKeyRolloverInterval;
            _reconnectTransportOnFailure = other._reconnectTransportOnFailure;
            _timestampValidityDuration = other._timestampValidityDuration;
            _maxCachedCookies = other._maxCachedCookies;
            _nonceCache = other._nonceCache;
        }

        public bool DetectReplays
        {
            get
            {
                return _detectReplays;
            }
            set
            {
                _detectReplays = value;
            }
        }

        public int ReplayCacheSize
        {
            get
            {
                return _replayCacheSize;
            }
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.ValueMustBeNonNegative));
                }
                _replayCacheSize = value;
            }
        }

        public TimeSpan ReplayWindow
        {
            get
            {
                return _replayWindow;
            }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRangeTooBig));
                }

                _replayWindow = value;
            }
        }

        public TimeSpan MaxClockSkew
        {
            get
            {
                return _maxClockSkew;
            }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRangeTooBig));
                }

                _maxClockSkew = value;
            }
        }


        public NonceCache NonceCache
        {
            get
            {
                return _nonceCache;
            }
            set
            {
                _nonceCache = value;
            }
        }

        public TimeSpan IssuedCookieLifetime
        {
            get
            {
                return _issuedCookieLifetime;
            }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRangeTooBig));
                }

                _issuedCookieLifetime = value;
            }
        }

        public int MaxStatefulNegotiations
        {
            get
            {
                return _maxStatefulNegotiations;
            }
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.ValueMustBeNonNegative));
                }
                _maxStatefulNegotiations = value;
            }
        }

        public TimeSpan NegotiationTimeout
        {
            get
            {
                return _negotiationTimeout;
            }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRangeTooBig));
                }

                _negotiationTimeout = value;
            }
        }

        public int MaxPendingSessions
        {
            get
            {
                return _maxPendingSessions;
            }
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.ValueMustBeNonNegative));
                }
                _maxPendingSessions = value;
            }
        }

        public TimeSpan InactivityTimeout
        {
            get
            {
                return _inactivityTimeout;
            }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRangeTooBig));
                }

                _inactivityTimeout = value;
            }
        }

        public TimeSpan SessionKeyRenewalInterval
        {
            get
            {
                return _sessionKeyRenewalInterval;
            }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRangeTooBig));
                }

                _sessionKeyRenewalInterval = value;
            }
        }

        public TimeSpan SessionKeyRolloverInterval
        {
            get
            {
                return _sessionKeyRolloverInterval;
            }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRangeTooBig));
                }

                _sessionKeyRolloverInterval = value;
            }
        }

        public bool ReconnectTransportOnFailure
        {
            get
            {
                return _reconnectTransportOnFailure;
            }
            set
            {
                _reconnectTransportOnFailure = value;
            }
        }

        public TimeSpan TimestampValidityDuration
        {
            get
            {
                return _timestampValidityDuration;
            }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRangeTooBig));
                }

                _timestampValidityDuration = value;
            }
        }

        public int MaxCachedCookies
        {
            get
            {
                return _maxCachedCookies;
            }
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.ValueMustBeNonNegative));
                }
                _maxCachedCookies = value;
            }
        }

        // TODO: taken from different sources
        internal const string defaultServerMaxNegotiationLifetimeString = "00:01:00";
        internal const string defaultServerIssuedTokenLifetimeString = "10:00:00";
        internal const string defaultServerIssuedTransitionTokenLifetimeString = "00:15:00";
        internal const int defaultServerMaxActiveNegotiations = 128;
        internal static readonly TimeSpan defaultServerMaxNegotiationLifetime = TimeSpan.Parse(defaultServerMaxNegotiationLifetimeString, CultureInfo.InvariantCulture);
        internal static readonly TimeSpan defaultServerIssuedTokenLifetime = TimeSpan.Parse(defaultServerIssuedTokenLifetimeString, CultureInfo.InvariantCulture);
        internal static readonly TimeSpan defaultServerIssuedTransitionTokenLifetime = TimeSpan.Parse(defaultServerIssuedTransitionTokenLifetimeString, CultureInfo.InvariantCulture);
        internal const int defaultServerMaxCachedTokens = 1000;
        internal const bool defaultServerMaintainState = true;
        internal static readonly SecurityStandardsManager defaultStandardsManager = SecurityStandardsManager.DefaultInstance;

        internal const string defaultKeyRenewalIntervalString = "15:00:00";
        internal const string defaultKeyRolloverIntervalString = "00:05:00";
        internal const string defaultInactivityTimeoutString = "00:02:00";

        internal static readonly TimeSpan defaultKeyRenewalInterval = TimeSpan.Parse(defaultKeyRenewalIntervalString, CultureInfo.InvariantCulture);
        internal static readonly TimeSpan defaultKeyRolloverInterval = TimeSpan.Parse(defaultKeyRolloverIntervalString, CultureInfo.InvariantCulture);
        internal const bool defaultTolerateTransportFailures = true;
        internal const int defaultMaximumPendingSessions = 128;
        internal static readonly TimeSpan defaultInactivityTimeout = TimeSpan.Parse(defaultInactivityTimeoutString, CultureInfo.InvariantCulture);

        public LocalServiceSecuritySettings()
        {
            this.DetectReplays = SecurityProtocolFactory.defaultDetectReplays;
            this.ReplayCacheSize = SecurityProtocolFactory.defaultMaxCachedNonces;
            this.ReplayWindow = SecurityProtocolFactory.defaultReplayWindow;
            this.MaxClockSkew = SecurityProtocolFactory.defaultMaxClockSkew;

            // Replace with defaults we define that aren't in the original source.
            this.IssuedCookieLifetime = defaultServerIssuedTokenLifetime;
            this.MaxStatefulNegotiations = defaultServerMaxActiveNegotiations;
            this.NegotiationTimeout = defaultServerMaxNegotiationLifetime;
            _maxPendingSessions = defaultMaximumPendingSessions;
            _inactivityTimeout = defaultInactivityTimeout;
            _sessionKeyRenewalInterval = defaultKeyRenewalInterval;
            _sessionKeyRolloverInterval = defaultKeyRolloverInterval;
            _reconnectTransportOnFailure = defaultTolerateTransportFailures;
            this.TimestampValidityDuration = SecurityProtocolFactory.defaultTimestampValidityDuration;
            _maxCachedCookies = defaultServerMaxCachedTokens;
            _nonceCache = null;
        }

        public LocalServiceSecuritySettings Clone()
        {
            return new LocalServiceSecuritySettings(this);
        }
    }
}
