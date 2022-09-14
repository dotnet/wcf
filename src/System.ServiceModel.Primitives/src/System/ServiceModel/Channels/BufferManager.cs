// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;

namespace System.ServiceModel.Channels
{
    public abstract class BufferManager
    {
        public abstract byte[] TakeBuffer(int bufferSize);
        public abstract void ReturnBuffer(byte[] buffer);
        public abstract void Clear();

        public static BufferManager CreateBufferManager(long maxBufferPoolSize, int maxBufferSize)
        {
            if (maxBufferPoolSize < 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(maxBufferPoolSize),
                    maxBufferPoolSize, SRP.ValueMustBeNonNegative));
            }

            if (maxBufferSize < 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(maxBufferSize),
                    maxBufferSize, SRP.ValueMustBeNonNegative));
            }

            return new WrappingBufferManager(InternalBufferManager.Create(maxBufferPoolSize, maxBufferSize));
        }

        internal static InternalBufferManager GetInternalBufferManager(BufferManager bufferManager)
        {
            if (bufferManager is WrappingBufferManager)
            {
                return ((WrappingBufferManager)bufferManager).InternalBufferManager;
            }
            else
            {
                return new WrappingInternalBufferManager(bufferManager);
            }
        }

        internal class WrappingBufferManager : BufferManager
        {
            public WrappingBufferManager(InternalBufferManager innerBufferManager)
            {
                InternalBufferManager = innerBufferManager;
            }

            public InternalBufferManager InternalBufferManager { get; }

            public override byte[] TakeBuffer(int bufferSize)
            {
                if (bufferSize < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(bufferSize), bufferSize,
                        SRP.ValueMustBeNonNegative));
                }

                return InternalBufferManager.TakeBuffer(bufferSize);
            }

            public override void ReturnBuffer(byte[] buffer)
            {
                if (buffer == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(buffer));
                }

                InternalBufferManager.ReturnBuffer(buffer);
            }

            public override void Clear()
            {
                InternalBufferManager.Clear();
            }
        }

        internal class WrappingInternalBufferManager : InternalBufferManager
        {
            private BufferManager _innerBufferManager;

            public WrappingInternalBufferManager(BufferManager innerBufferManager)
            {
                _innerBufferManager = innerBufferManager;
            }

            public override void Clear()
            {
                _innerBufferManager.Clear();
            }

            public override void ReturnBuffer(byte[] buffer)
            {
                _innerBufferManager.ReturnBuffer(buffer);
            }

            public override byte[] TakeBuffer(int bufferSize)
            {
                return _innerBufferManager.TakeBuffer(bufferSize);
            }
        }
    }
}
