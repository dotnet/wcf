// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;

namespace System.ServiceModel.Security
{
    public abstract class NonceCache
    {
        private TimeSpan _cachingTime;
        private int _maxCachedNonces;

        /// <summary>
        /// TThe max timespan after which a Nonce is deleted from the NonceCache. This value should be atleast twice the maxclock Skew added to the replayWindow size.
        /// </summary>
        public TimeSpan CachingTimeSpan
        {
            get
            {
                return _cachingTime;
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

                _cachingTime = value;
            }
        }

        /// <summary>
        /// The maximum size of the NonceCache.
        /// </summary>
        public int CacheSize
        {
            get
            {
                return _maxCachedNonces;
            }
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                                                    SRServiceModel.ValueMustBeNonNegative));
                }
                _maxCachedNonces = value;
            }
        }

        public abstract bool TryAddNonce(byte[] nonce);
        public abstract bool CheckNonce(byte[] nonce);
    }
}
