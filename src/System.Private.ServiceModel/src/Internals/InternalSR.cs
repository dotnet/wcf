// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel;

namespace System.Runtime
{
    internal class InternalSR
    {
        internal static string ActionItemIsAlreadyScheduled = "ActionItemIsAlreadyScheduled";
        internal static string AsyncCallbackThrewException = "AsyncCallbackThrewException";
        internal static string AsyncResultAlreadyEnded = "AsyncResultAlreadyEnded";
        internal static string BufferIsNotRightSizeForBufferManager = "BufferIsNotRightSizeForBufferManager";
        internal static string InvalidAsyncResult = "InvalidAsyncResult";
        internal static string InvalidAsyncResultImplementationGeneric = "InvalidAsyncResultImplementationGeneric";
        internal static string InvalidNullAsyncResult = "InvalidNullAsyncResult";
        internal static string InvalidSemaphoreExit = "InvalidSemaphoreExit";
        internal static string ReadNotSupported = "ReadNotSupported";
        internal static string SeekNotSupported = "SeekNotSupported";
        internal static string ValueMustBeNonNegative = "ValueMustBeNonNegative";
        internal static string AsyncResultCompletedTwice = "AsyncResultCompletedTwice";

        internal static string ArgumentNullOrEmpty(object param0) { throw ExceptionHelper.PlatformNotSupported(); }
        internal static string BufferedOutputStreamQuotaExceeded(object param0) { throw ExceptionHelper.PlatformNotSupported(); }
        internal static string FailFastMessage(object param0) { throw ExceptionHelper.PlatformNotSupported(); }
        internal static string InvalidAsyncResultImplementation(object param0) { throw ExceptionHelper.PlatformNotSupported(); }
        internal static string ShipAssertExceptionMessage(object param0) { throw ExceptionHelper.PlatformNotSupported(); }
        internal static string TaskTimedOutError(object param0) { throw ExceptionHelper.PlatformNotSupported(); }
        internal static string TimeoutInputQueueDequeue(object param0) { throw ExceptionHelper.PlatformNotSupported(); }
        internal static string TimeoutMustBeNonNegative(object param0, object param1) { throw ExceptionHelper.PlatformNotSupported(); }
        internal static string TimeoutMustBePositive(object param0, object param1) { throw ExceptionHelper.PlatformNotSupported(); }
    }
}
