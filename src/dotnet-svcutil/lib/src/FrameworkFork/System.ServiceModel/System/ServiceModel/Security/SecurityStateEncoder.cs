// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel.Security
{
    // The rationale for making abstract methods protected instead of public is following:
    // 1. No scenarios for making them public.
    // 2. Reduction of threat area (other assemblies on the channel can't call these methods other than through reflection).
    // 3. Reduction of test area (feature is testable only through other high-level features).
    public abstract class SecurityStateEncoder
    {
        protected SecurityStateEncoder() { }

        protected internal abstract byte[] DecodeSecurityState(byte[] data);
        protected internal abstract byte[] EncodeSecurityState(byte[] data);
    }
}

