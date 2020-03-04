// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
