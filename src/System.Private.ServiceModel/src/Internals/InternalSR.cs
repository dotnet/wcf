// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel;

namespace System.Runtime
{
    internal class InternalSR
    {
        internal static readonly string ActionItemIsAlreadyScheduled = "ActionItemIsAlreadyScheduled";
        internal static readonly string AsyncCallbackThrewException = "AsyncCallbackThrewException";
        internal static readonly string AsyncResultAlreadyEnded = "AsyncResultAlreadyEnded";
        internal static readonly string BufferIsNotRightSizeForBufferManager = "BufferIsNotRightSizeForBufferManager";
        internal static readonly string InvalidAsyncResult = "InvalidAsyncResult";
        internal static readonly string InvalidAsyncResultImplementationGeneric = "InvalidAsyncResultImplementationGeneric";
        internal static readonly string InvalidNullAsyncResult = "InvalidNullAsyncResult";
        internal static readonly string InvalidSemaphoreExit = "InvalidSemaphoreExit";
        internal static readonly string ReadNotSupported = "ReadNotSupported";
        internal static readonly string SeekNotSupported = "SeekNotSupported";
        internal static readonly string ValueMustBeNonNegative = "ValueMustBeNonNegative";
        internal static readonly string AsyncResultCompletedTwice = "AsyncResultCompletedTwice";

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
