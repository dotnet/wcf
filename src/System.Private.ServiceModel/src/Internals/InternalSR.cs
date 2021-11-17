// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel;

namespace System.Runtime
{
    internal class InternalSR
    {
        internal static readonly string ActionItemIsAlreadyScheduled = "The ActionItem was already scheduled for execution that hasn't been completed yet.";
        internal static readonly string AsyncCallbackThrewException = "Async Callback threw an exception.";
        internal static readonly string AsyncResultAlreadyEnded = "End cannot be called twice on an AsyncResult.";
        internal static readonly string BufferIsNotRightSizeForBufferManager = "This buffer cannot be returned to the buffer manager because it is the wrong size.";
        internal static readonly string InvalidAsyncResult = "An incorrect IAsyncResult was provided to an 'End' method. The IAsyncResult object passed to 'End' must be the one returned from the matching 'Begin' or passed to the callback provided to 'Begin'.";
        internal static readonly string InvalidAsyncResultImplementationGeneric = "An incorrect implementation of the IAsyncResult interface may be returning incorrect values from the CompletedSynchronously property or calling the AsyncCallback more than once.";
        internal static readonly string InvalidNullAsyncResult = "A null value was returned from an async 'Begin' method or passed to an AsyncCallback. Async 'Begin' implementations must return a non-null IAsyncResult and pass the same IAsyncResult object as the parameter to the AsyncCallback.";
        internal static readonly string InvalidSemaphoreExit = "Object synchronization method was called from an unsynchronized block of code.";
        internal static readonly string ReadNotSupported = "Read not supported on this stream.";
        internal static readonly string SeekNotSupported = "Seek not supported on this stream.";
        internal static readonly string ValueMustBeNonNegative = "Value must be non-negative.";

        internal static string ArgumentNullOrEmpty(object param0) => $"The argument {param0} is null or empty.";
        internal static string AsyncResultCompletedTwice(object param0) => $"The IAsyncResult implementation '{param0}' tried to complete a single operation multiple times. This could be caused by an incorrect application IAsyncResult implementation or other extensibility code, such as an IAsyncResult that returns incorrect CompletedSynchronously values or invokes the AsyncCallback multiple times.";
        internal static string BufferedOutputStreamQuotaExceeded(object param0) => $"The size quota for this stream ({param0}) has been exceeded.";
        internal static string BufferAllocationFailed(int size) => $"Failed to allocate a managed memory buffer of {size} bytes. The amount of available memory may be low.";
        internal static string FailFastMessage(object param0) => $"An unrecoverable error occurred. For diagnostic purposes, this English message is associated with the failure: '{param0}'.";
        internal static string InvalidAsyncResultImplementation(object param0) => $"An incorrect implementation of the IAsyncResult interface may be returning incorrect values from the CompletedSynchronously property or calling the AsyncCallback more than once. The type {param0} could be the incorrect implementation.";
        internal static string ShipAssertExceptionMessage(object param0) => $"An unexpected failure occurred. Applications should not attempt to handle this error. For diagnostic purposes, this English message is associated with the failure: '{param0}'.";
        internal static string TaskTimedOutError(object param0) => $"The task timed out after {param0}. The time allotted to this operation may have been a portion of a longer timeout.";
        internal static string TimeoutInputQueueDequeue(object param0) => $"A Dequeue operation timed out after {param0}. The time allotted to this operation may have been a portion of a longer timeout.";
        internal static string TimeoutMustBeNonNegative(object param0, object param1) => $"Argument {param0} must be a non-negative timeout value. Provided value was {param1}.";
        internal static string TimeoutMustBePositive(object param0, object param1) => $"Argument {param0} must be a positive timeout value. Provided value was {param1}.";
    }
}
