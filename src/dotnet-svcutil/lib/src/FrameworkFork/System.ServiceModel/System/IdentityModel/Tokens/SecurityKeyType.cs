// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.ComponentModel;
using System.Runtime;

namespace System.IdentityModel.Tokens
{
    public enum SecurityKeyType
    {
        SymmetricKey,
        AsymmetricKey,
        BearerKey
    }

    internal static class SecurityKeyTypeHelper
    {
        internal static bool IsDefined(SecurityKeyType value)
        {
            return (value == SecurityKeyType.SymmetricKey
                || value == SecurityKeyType.AsymmetricKey
                || value == SecurityKeyType.BearerKey);
        }

        internal static void Validate(SecurityKeyType value)
        {
            if (!IsDefined(value))
            {
                throw Fx.Exception.AsError(new InvalidEnumArgumentException("value", (int)value,
                    typeof(SecurityKeyType)));
            }
        }
    }
}
