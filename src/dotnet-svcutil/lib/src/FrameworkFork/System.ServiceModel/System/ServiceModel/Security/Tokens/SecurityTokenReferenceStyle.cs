// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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

