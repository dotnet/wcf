// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Security.Authentication;

namespace System.ServiceModel.Channels
{
    internal static class SslProtocolsHelper
    {
        internal static bool IsDefined(SslProtocols value)
        {
            SslProtocols allValues = SslProtocols.None;
            foreach (var protocol in Enum.GetValues(typeof(SslProtocols)))
            {
                allValues |= (SslProtocols)protocol;
            }
            return (value & allValues) == value;
        }

        internal static void Validate(SslProtocols value)
        {
            if (!IsDefined(value))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException(nameof(value), (int)value,
                    typeof(SslProtocols)));
            }
        }
    }
}
