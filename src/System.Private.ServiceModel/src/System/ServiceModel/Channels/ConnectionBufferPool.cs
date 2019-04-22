// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Diagnostics.Contracts;
using System.Runtime;

namespace System.ServiceModel.Channels
{
    internal class ConnectionBufferPool : QueuedObjectPool<byte[]>
    {
        private const int SingleBatchSize = 128 * 1024;
        private const int MaxBatchCount = 16;
        private const int MaxFreeCountFactor = 4;

        public ConnectionBufferPool(int bufferSize)
        {
            int batchCount = ComputeBatchCount(bufferSize);
            Initialize(bufferSize, batchCount, batchCount * MaxFreeCountFactor);
        }

        public ConnectionBufferPool(int bufferSize, int maxFreeCount)
        {
            Initialize(bufferSize, ComputeBatchCount(bufferSize), maxFreeCount);
        }

        private void Initialize(int bufferSize, int batchCount, int maxFreeCount)
        {
            Contract.Assert(bufferSize >= 0, "bufferSize must be non-negative");
            Contract.Assert(batchCount > 0, "batchCount must be positive");
            Contract.Assert(maxFreeCount >= 0, "maxFreeCount must be non-negative");

            BufferSize = bufferSize;
            if (maxFreeCount < batchCount)
            {
                maxFreeCount = batchCount;
            }
            base.Initialize(batchCount, maxFreeCount);
        }

        public int BufferSize { get; private set; }

        protected override byte[] Create()
        {
            return Fx.AllocateByteArray(BufferSize);
        }

        private static int ComputeBatchCount(int bufferSize)
        {
            int batchCount;
            if (bufferSize != 0)
            {
                batchCount = (SingleBatchSize + bufferSize - 1) / bufferSize;
                if (batchCount > MaxBatchCount)
                {
                    batchCount = MaxBatchCount;
                }
            }
            else
            {
                // It's OK to have zero bufferSize
                batchCount = MaxBatchCount;
            }
            return batchCount;
        }
    }
}
