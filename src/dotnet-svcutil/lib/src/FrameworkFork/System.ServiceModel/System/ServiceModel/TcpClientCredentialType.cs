// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Net;

namespace System.ServiceModel
{
    public enum TcpClientCredentialType
    {
        None,
        Windows,
        Certificate
    }

    internal static class TcpClientCredentialTypeHelper
    {
        internal static bool IsDefined(TcpClientCredentialType value)
        {
            return (value == TcpClientCredentialType.None ||
                value == TcpClientCredentialType.Windows ||
                value == TcpClientCredentialType.Certificate);
        }
    }
}
