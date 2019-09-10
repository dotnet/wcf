// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
