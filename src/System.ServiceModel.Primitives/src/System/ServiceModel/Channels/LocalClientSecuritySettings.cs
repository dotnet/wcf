// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel.Security;

namespace System.ServiceModel.Channels
{
    public sealed class LocalClientSecuritySettings
    {
        private int _replayCacheSize;
        private TimeSpan _replayWindow;
        private TimeSpan _maxClockSkew;
        private TimeSpan _maxCookieCachingTime;
        private TimeSpan _sessionKeyRenewalInterval;
        private TimeSpan _sessionKeyRolloverInterval;
        private TimeSpan _timestampValidityDuration;
        private int _cookieRenewalThresholdPercentage;

        private LocalClientSecuritySettings(LocalClientSecuritySettings other)
        {
            DetectReplays = other.DetectReplays;
            _replayCacheSize = other._replayCacheSize;
            _replayWindow = other._replayWindow;
            _maxClockSkew = other._maxClockSkew;
            CacheCookies = other.CacheCookies;
            _maxCookieCachingTime = other._maxCookieCachingTime;
            _sessionKeyRenewalInterval = other._sessionKeyRenewalInterval;
            _sessionKeyRolloverInterval = other._sessionKeyRolloverInterval;
            ReconnectTransportOnFailure = other.ReconnectTransportOnFailure;
            _timestampValidityDuration = other._timestampValidityDuration;
            IdentityVerifier = other.IdentityVerifier;
            _cookieRenewalThresholdPercentage = other._cookieRenewalThresholdPercentage;
            NonceCache = other.NonceCache;
        }

        public bool DetectReplays { get; set; }

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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                                                    SRP.ValueMustBeNonNegative));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SRP.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SRP.SFxTimeoutOutOfRangeTooBig));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SRP.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SRP.SFxTimeoutOutOfRangeTooBig));
                }

                _maxClockSkew = value;
            }
        }

        public NonceCache NonceCache { get; set; } = null;

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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SRP.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SRP.SFxTimeoutOutOfRangeTooBig));
                }

                _timestampValidityDuration = value;
            }
        }

        public bool CacheCookies { get; set; }

        public TimeSpan MaxCookieCachingTime
        {
            get
            {
                return _maxCookieCachingTime;
            }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SRP.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SRP.SFxTimeoutOutOfRangeTooBig));
                }

                _maxCookieCachingTime = value;
            }
        }

        public int CookieRenewalThresholdPercentage
        {
            get
            {
                return _cookieRenewalThresholdPercentage;
            }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                                                    SRP.Format(SRP.ValueMustBeInRange, 0, 100)));
                }
                _cookieRenewalThresholdPercentage = value;
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SRP.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SRP.SFxTimeoutOutOfRangeTooBig));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SRP.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SRP.SFxTimeoutOutOfRangeTooBig));
                }

                _sessionKeyRolloverInterval = value;
            }
        }

        public bool ReconnectTransportOnFailure { get; set; }

        public IdentityVerifier IdentityVerifier { get; set; }

        public LocalClientSecuritySettings()
        {
            DetectReplays = SecurityProtocolFactory.defaultDetectReplays;
            ReplayCacheSize = SecurityProtocolFactory.defaultMaxCachedNonces;
            ReplayWindow = SecurityProtocolFactory.defaultReplayWindow;
            MaxClockSkew = SecurityProtocolFactory.defaultMaxClockSkew;
            TimestampValidityDuration = SecurityProtocolFactory.defaultTimestampValidityDuration;
            CacheCookies = IssuanceTokenProviderBase<IssuanceTokenProviderState>.defaultClientCacheTokens;
            MaxCookieCachingTime = IssuanceTokenProviderBase<IssuanceTokenProviderState>.DefaultClientMaxTokenCachingTime;
            SessionKeyRenewalInterval = SecuritySessionClientSettings.s_defaultKeyRenewalInterval;
            SessionKeyRolloverInterval = SecuritySessionClientSettings.s_defaultKeyRolloverInterval;
            ReconnectTransportOnFailure = SecuritySessionClientSettings.DefaultTolerateTransportFailures;
            CookieRenewalThresholdPercentage = AcceleratedTokenProvider.defaultServiceTokenValidityThresholdPercentage;
            IdentityVerifier = IdentityVerifier.CreateDefault();
            NonceCache = null;
        }

        public LocalClientSecuritySettings Clone()
        {
            return new LocalClientSecuritySettings(this);
        }
    }
}


