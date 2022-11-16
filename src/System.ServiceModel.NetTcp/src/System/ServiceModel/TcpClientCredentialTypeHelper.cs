﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ServiceModel
{
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
