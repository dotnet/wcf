// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using System;

    internal static class ContextExchangeMechanismHelper
    {
        public static bool IsDefined(ContextExchangeMechanism value)
        {
            return value == ContextExchangeMechanism.ContextSoapHeader ||
                value == ContextExchangeMechanism.HttpCookie;
        }
    }
}
