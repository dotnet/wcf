// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.ServiceModel
{
    /// <summary>
    /// Modes that can be set in a to determine whether a channel 
    /// supports streamed and/or buffered mode.
    /// </summary>
    public enum TransferMode
    {
        Buffered,
        Streamed,
        StreamedRequest,
        StreamedResponse,
    }

    internal static class TransferModeHelper
    {
        public static bool IsDefined(TransferMode v)
        {
            return ((v == TransferMode.Buffered) || (v == TransferMode.Streamed) ||
                (v == TransferMode.StreamedRequest) || (v == TransferMode.StreamedResponse));
        }

        public static bool IsRequestStreamed(TransferMode v)
        {
            return ((v == TransferMode.StreamedRequest) || (v == TransferMode.Streamed));
        }

        public static bool IsResponseStreamed(TransferMode v)
        {
            return ((v == TransferMode.StreamedResponse) || (v == TransferMode.Streamed));
        }

        public static void Validate(TransferMode value)
        {
            if (!IsDefined(value))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException("value", (int)value,
                    typeof(TransferMode)));
            }
        }
    }
}


