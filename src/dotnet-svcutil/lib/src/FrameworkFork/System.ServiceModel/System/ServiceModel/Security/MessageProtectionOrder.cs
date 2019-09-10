// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel.Security
{
    public enum MessageProtectionOrder
    {
        SignBeforeEncrypt,
        SignBeforeEncryptAndEncryptSignature,
        EncryptBeforeSign,
    }

    internal static class MessageProtectionOrderHelper
    {
        internal static bool IsDefined(MessageProtectionOrder value)
        {
            return value == MessageProtectionOrder.SignBeforeEncrypt
                || value == MessageProtectionOrder.SignBeforeEncryptAndEncryptSignature
                || value == MessageProtectionOrder.EncryptBeforeSign;
        }
    }
}
