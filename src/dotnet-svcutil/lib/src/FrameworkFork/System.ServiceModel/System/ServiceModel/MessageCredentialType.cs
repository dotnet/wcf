// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel
{
    public enum MessageCredentialType
    {
        None,
        Windows,
        UserName,
        Certificate,
        IssuedToken
    }

    internal static class MessageCredentialTypeHelper
    {
        internal static bool IsDefined(MessageCredentialType value)
        {
            return (value == MessageCredentialType.None ||
                value == MessageCredentialType.UserName ||
                value == MessageCredentialType.Windows ||
                value == MessageCredentialType.Certificate ||
                value == MessageCredentialType.IssuedToken);
        }
    }
}
