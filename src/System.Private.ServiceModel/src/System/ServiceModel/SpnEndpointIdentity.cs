// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel
{
    public class SpnEndpointIdentity : EndpointIdentity
    {
        private static TimeSpan s_spnLookupTime = TimeSpan.FromMinutes(1);
        private Object _thisLock = new Object();
        private static Object s_typeLock = new Object();

        public static TimeSpan SpnLookupTime
        {
            get
            {
                return s_spnLookupTime;
            }
            set
            {
                if (value.Ticks < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value.Ticks,
                                                    SR.ValueMustBeNonNegative));
                }
                s_spnLookupTime = value;
            }
        }

        public SpnEndpointIdentity(string spnName)
        {
            if (spnName == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("spnName");

            throw ExceptionHelper.PlatformNotSupported("SpnEndpointIdentity is not supported");
        }
        
        public static TimeSpan SpnLookupTime {get;set;}
        
    }
}
