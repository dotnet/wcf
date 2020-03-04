// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Channels
{
    using System;

    static class ContextExchangeMechanismHelper
    {
        public static bool IsDefined(ContextExchangeMechanism value)
        {
            return value == ContextExchangeMechanism.ContextSoapHeader ||
                value == ContextExchangeMechanism.HttpCookie;
        }
    }
}
