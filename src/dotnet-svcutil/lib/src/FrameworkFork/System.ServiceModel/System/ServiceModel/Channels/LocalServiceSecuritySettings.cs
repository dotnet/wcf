//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace System.ServiceModel.Channels
{
    using System;
    using System.Globalization;
    using System.Runtime;
    using System.ServiceModel;
    using System.ServiceModel.Security;

    public sealed class LocalServiceSecuritySettings
    {
        bool detectReplays;
        int replayCacheSize;
        TimeSpan replayWindow;
        TimeSpan maxClockSkew;
        TimeSpan issuedCookieLifetime;
        int maxStatefulNegotiations;
        TimeSpan negotiationTimeout;
        int maxCachedCookies;
        int maxPendingSessions;
        TimeSpan inactivityTimeout;
        TimeSpan sessionKeyRenewalInterval;
        TimeSpan sessionKeyRolloverInterval;
        bool reconnectTransportOnFailure;
        TimeSpan timestampValidityDuration;
        NonceCache nonceCache = null;

        LocalServiceSecuritySettings(LocalServiceSecuritySettings other)
        {
            this.detectReplays = other.detectReplays;
            this.replayCacheSize = other.replayCacheSize;
            this.replayWindow = other.replayWindow;
            this.maxClockSkew = other.maxClockSkew;
            this.issuedCookieLifetime = other.issuedCookieLifetime;
            this.maxStatefulNegotiations = other.maxStatefulNegotiations;
            this.negotiationTimeout = other.negotiationTimeout;
            this.maxPendingSessions = other.maxPendingSessions;
            this.inactivityTimeout = other.inactivityTimeout;
            this.sessionKeyRenewalInterval = other.sessionKeyRenewalInterval;
            this.sessionKeyRolloverInterval = other.sessionKeyRolloverInterval;
            this.reconnectTransportOnFailure = other.reconnectTransportOnFailure;
            this.timestampValidityDuration = other.timestampValidityDuration;
            this.maxCachedCookies = other.maxCachedCookies;
            this.nonceCache = other.nonceCache;
        }

        public bool DetectReplays
        {
            get
            {
                return this.detectReplays;
            }
            set
            {
                this.detectReplays = value;
            }
        }

        public int ReplayCacheSize
        {
            get
            {
                return this.replayCacheSize;
            }
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.ValueMustBeNonNegative));
                }
                this.replayCacheSize = value;
            }
        }

        public TimeSpan ReplayWindow
        {
            get
            {
                return this.replayWindow;
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

                this.replayWindow = value;
            }
        }

        public TimeSpan MaxClockSkew
        {
            get
            {
                return this.maxClockSkew;
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

                this.maxClockSkew = value;
            }
        }


        public NonceCache NonceCache
        {
            get
            {
                return this.nonceCache;
            }
            set
            {
                this.nonceCache = value;
            }
        }

        public TimeSpan IssuedCookieLifetime
        {
            get
            {
                return this.issuedCookieLifetime;
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

                this.issuedCookieLifetime = value;
            }
        }

        public int MaxStatefulNegotiations
        {
            get
            {
                return this.maxStatefulNegotiations;
            }
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.ValueMustBeNonNegative));
                }
                this.maxStatefulNegotiations = value;
            }
        }

        public TimeSpan NegotiationTimeout
        {
            get
            {
                return this.negotiationTimeout;
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

                this.negotiationTimeout = value;
            }
        }

        public int MaxPendingSessions
        {
            get
            {
                return this.maxPendingSessions;
            }
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.ValueMustBeNonNegative));
                }
                this.maxPendingSessions = value;
            }
        }

        public TimeSpan InactivityTimeout
        {
            get
            {
                return this.inactivityTimeout;
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

                this.inactivityTimeout = value;
            }
        }

        public TimeSpan SessionKeyRenewalInterval
        {
            get
            {
                return this.sessionKeyRenewalInterval;
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

                this.sessionKeyRenewalInterval = value;
            }
        }

        public TimeSpan SessionKeyRolloverInterval
        {
            get
            {
                return this.sessionKeyRolloverInterval;
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

                this.sessionKeyRolloverInterval = value;
            }
        }

        public bool ReconnectTransportOnFailure
        {
            get
            {
                return this.reconnectTransportOnFailure;
            }
            set
            {
                this.reconnectTransportOnFailure = value;
            }
        }

        public TimeSpan TimestampValidityDuration
        {
            get
            {
                return this.timestampValidityDuration;
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

                this.timestampValidityDuration = value;
            }
        }

        public int MaxCachedCookies
        {
            get
            {
                return this.maxCachedCookies;
            }
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.ValueMustBeNonNegative));
                }
                this.maxCachedCookies = value;
            }
        }

        // TODO: taken from different sources
#if disabled
#else
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
#endif
        public LocalServiceSecuritySettings()
        {
            this.DetectReplays = SecurityProtocolFactory.defaultDetectReplays;
            this.ReplayCacheSize = SecurityProtocolFactory.defaultMaxCachedNonces;
            this.ReplayWindow = SecurityProtocolFactory.defaultReplayWindow;
            this.MaxClockSkew = SecurityProtocolFactory.defaultMaxClockSkew;
#if disabled
            this.IssuedCookieLifetime = NegotiationTokenAuthenticator<NegotiationTokenAuthenticatorState>.defaultServerIssuedTokenLifetime;
            this.MaxStatefulNegotiations = NegotiationTokenAuthenticator<NegotiationTokenAuthenticatorState>.defaultServerMaxActiveNegotiations;
            this.NegotiationTimeout = NegotiationTokenAuthenticator<NegotiationTokenAuthenticatorState>.defaultServerMaxNegotiationLifetime;
            this.maxPendingSessions = SecuritySessionServerSettings.defaultMaximumPendingSessions;
            this.inactivityTimeout = SecuritySessionServerSettings.defaultInactivityTimeout;
            this.sessionKeyRenewalInterval = SecuritySessionServerSettings.defaultKeyRenewalInterval;
            this.sessionKeyRolloverInterval = SecuritySessionServerSettings.defaultKeyRolloverInterval;
            this.reconnectTransportOnFailure = SecuritySessionServerSettings.defaultTolerateTransportFailures;
            this.TimestampValidityDuration = SecurityProtocolFactory.defaultTimestampValidityDuration;
            this.maxCachedCookies = NegotiationTokenAuthenticator<NegotiationTokenAuthenticatorState>.defaultServerMaxCachedTokens;
            this.nonceCache = null;
#else
            this.IssuedCookieLifetime = defaultServerIssuedTokenLifetime;
            this.MaxStatefulNegotiations = defaultServerMaxActiveNegotiations;
            this.NegotiationTimeout = defaultServerMaxNegotiationLifetime;
            this.maxPendingSessions = defaultMaximumPendingSessions;
            this.inactivityTimeout = defaultInactivityTimeout;
            this.sessionKeyRenewalInterval = defaultKeyRenewalInterval;
            this.sessionKeyRolloverInterval = defaultKeyRolloverInterval;
            this.reconnectTransportOnFailure = defaultTolerateTransportFailures;
            this.TimestampValidityDuration = SecurityProtocolFactory.defaultTimestampValidityDuration;
            this.maxCachedCookies = defaultServerMaxCachedTokens;
            this.nonceCache = null;
#endif
        }

        public LocalServiceSecuritySettings Clone()
        {
            return new LocalServiceSecuritySettings(this);
        }
    }
}
