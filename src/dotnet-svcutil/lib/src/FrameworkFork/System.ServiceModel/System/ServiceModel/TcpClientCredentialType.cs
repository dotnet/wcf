// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
