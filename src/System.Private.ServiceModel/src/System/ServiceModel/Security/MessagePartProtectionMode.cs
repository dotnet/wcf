// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Security
{
    enum MessagePartProtectionMode
    {
        None,
        Sign,
        Encrypt,
        SignThenEncrypt,
        EncryptThenSign,
    }

    static class MessagePartProtectionModeHelper
    {
        public static MessagePartProtectionMode GetProtectionMode(bool sign, bool encrypt, bool signThenEncrypt)
        {
            if (sign)
            {
                if (encrypt)
                {
                    if (signThenEncrypt)
                    {
                        return MessagePartProtectionMode.SignThenEncrypt;
                    }
                    else
                    {
                        return MessagePartProtectionMode.EncryptThenSign;
                    }
                }
                else
                {
                    return MessagePartProtectionMode.Sign;
                }
            }
            else if (encrypt)
            {
                return MessagePartProtectionMode.Encrypt;
            }
            else
            {
                return MessagePartProtectionMode.None;
            }
        }
    }
}

