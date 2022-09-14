// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ServiceModel.Channels
{
    internal class ConnectionUtilities
    {
        internal static void ValidateBufferBounds(ArraySegment<byte> buffer)
        {
            ValidateBufferBounds(buffer.Array, buffer.Offset, buffer.Count);
        }

        internal static void ValidateBufferBounds(byte[] buffer, int offset, int size)
        {
            if (buffer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(buffer));
            }

            ValidateBufferBounds(buffer.Length, offset, size);
        }

        internal static void ValidateBufferBounds(int bufferSize, int offset, int size)
        {
            if (offset < 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(offset), offset, SRP.ValueMustBeNonNegative));
            }

            if (offset > bufferSize)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(offset), offset, SRP.Format(SRP.OffsetExceedsBufferSize, bufferSize)));
            }

            if (size <= 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(size), size, SRP.ValueMustBePositive));
            }

            int remainingBufferSpace = bufferSize - offset;
            if (size > remainingBufferSpace)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(size), size, SRP.Format(
                    SRP.SizeExceedsRemainingBufferSpace, remainingBufferSpace)));
            }
        }
    }
}
