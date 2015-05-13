// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace System.ServiceModel.Security
{
    public abstract class IdentityVerifier
    {
        protected IdentityVerifier()
        {
            // empty
        }

        public static IdentityVerifier CreateDefault()
        {
            return DefaultIdentityVerifier.Instance;
        }

        internal class DefaultIdentityVerifier : IdentityVerifier
        {
            private static readonly DefaultIdentityVerifier s_instance = new DefaultIdentityVerifier();

            public static DefaultIdentityVerifier Instance
            {
                get { return s_instance; }
            }
        }
    }
}
