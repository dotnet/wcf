// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace System.ServiceModel.Security.Tokens
{
    public enum SecurityTokenReferenceStyle
    {
        Internal = 0,
        External = 1,
    }

    internal static class TokenReferenceStyleHelper
    {
        public static bool IsDefined(SecurityTokenReferenceStyle value)
        {
            return (value == SecurityTokenReferenceStyle.External || value == SecurityTokenReferenceStyle.Internal);
        }

        public static void Validate(SecurityTokenReferenceStyle value)
        {
            if (!IsDefined(value))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException("value", (int)value,
                    typeof(SecurityTokenReferenceStyle)));
            }
        }
    }
}

