// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Contracts;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public class ExceptionHelper
    {
        public const string WinsdowsStreamSecurityNotSupported = "Windows Stream Security is not supported";

        public static Exception AsError(Exception exception)
        {
            return exception;
        }

        public static PlatformNotSupportedException PlatformNotSupported()
        {
            return new PlatformNotSupportedException();
        }

        public static PlatformNotSupportedException PlatformNotSupported(string message)
        {
            return new PlatformNotSupportedException(message);
        }

        public static Exception CreateMaxReceivedMessageSizeExceededException(long maxMessageSize)
        {
            return MaxMessageSizeStream.CreateMaxReceivedMessageSizeExceededException(maxMessageSize);
        }
    }
}
