// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel.Security;

namespace System.ServiceModel.Channels
{
    public sealed class LocalClientSecuritySettings
    {
        private bool _detectReplays;
        private int _replayCacheSize;
        private TimeSpan _replayWindow;
        private TimeSpan _maxClockSkew;
        private bool _cacheCookies;
        private TimeSpan _maxCookieCachingTime;
        private TimeSpan _sessionKeyRenewalInterval;
        private TimeSpan _sessionKeyRolloverInterval;
        private bool _reconnectTransportOnFailure;
        private TimeSpan _timestampValidityDuration;
        private IdentityVerifier _identityVerifier;
        private int _cookieRenewalThresholdPercentage;
        private NonceCache _nonceCache = null;

        private LocalClientSecuritySettings(LocalClientSecuritySettings other)
        {
            _detectReplays = other._detectReplays;
            _replayCacheSize = other._replayCacheSize;
            _replayWindow = other._replayWindow;
            _maxClockSkew = other._maxClockSkew;
            _cacheCookies = other._cacheCookies;
            _maxCookieCachingTime = other._maxCookieCachingTime;
            _sessionKeyRenewalInterval = other._sessionKeyRenewalInterval;
            _sessionKeyRolloverInterval = other._sessionKeyRolloverInterval;
            _reconnectTransportOnFailure = other._reconnectTransportOnFailure;
            _timestampValidityDuration = other._timestampValidityDuration;
            _identityVerifier = other._identityVerifier;
            _cookieRenewalThresholdPercentage = other._cookieRenewalThresholdPercentage;
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                                                    SRServiceModel.ValueMustBeNonNegative));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SRServiceModel.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SRServiceModel.SFxTimeoutOutOfRangeTooBig));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SRServiceModel.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SRServiceModel.SFxTimeoutOutOfRangeTooBig));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SRServiceModel.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SRServiceModel.SFxTimeoutOutOfRangeTooBig));
                }

                _timestampValidityDuration = value;
            }
        }

        public bool CacheCookies
        {
            get
            {
                return _cacheCookies;
            }
            set
            {
                _cacheCookies = value;
            }
        }

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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SRServiceModel.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SRServiceModel.SFxTimeoutOutOfRangeTooBig));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                                                    string.Format(SRServiceModel.ValueMustBeInRange, 0, 100)));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SRServiceModel.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SRServiceModel.SFxTimeoutOutOfRangeTooBig));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SRServiceModel.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SRServiceModel.SFxTimeoutOutOfRangeTooBig));
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

        public IdentityVerifier IdentityVerifier
        {
            get
            {
                return _identityVerifier;
            }
            set
            {
                _identityVerifier = value;
            }
        }

        public LocalClientSecuritySettings()
        {
            this.DetectReplays = SecurityProtocolFactory.defaultDetectReplays;
            this.ReplayCacheSize = SecurityProtocolFactory.defaultMaxCachedNonces;
            this.ReplayWindow = SecurityProtocolFactory.defaultReplayWindow;
            this.MaxClockSkew = SecurityProtocolFactory.defaultMaxClockSkew;
            this.TimestampValidityDuration = SecurityProtocolFactory.defaultTimestampValidityDuration;
        }

        public LocalClientSecuritySettings Clone()
        {
            return new LocalClientSecuritySettings(this);
        }
    }
}


