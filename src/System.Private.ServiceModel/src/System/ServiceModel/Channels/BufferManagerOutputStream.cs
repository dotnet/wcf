// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime;
using System.ServiceModel.Diagnostics.Application; // for QuotaExceededException

namespace System.ServiceModel.Channels
{
    internal class BufferManagerOutputStream : BufferedOutputStream
    {
        private string _quotaExceededString;

        public BufferManagerOutputStream(string quotaExceededString)
            : base()
        {
            _quotaExceededString = quotaExceededString;
        }

        public BufferManagerOutputStream(string quotaExceededString, int maxSize)
            : base(maxSize)
        {
            _quotaExceededString = quotaExceededString;
        }

        public BufferManagerOutputStream(string quotaExceededString, int initialSize, int maxSize, BufferManager bufferManager)
            : base(initialSize, maxSize, BufferManager.GetInternalBufferManager(bufferManager))
        {
            _quotaExceededString = quotaExceededString;
        }

        public void Init(int initialSize, int maxSizeQuota, BufferManager bufferManager)
        {
            Init(initialSize, maxSizeQuota, maxSizeQuota, bufferManager);
        }

        public void Init(int initialSize, int maxSizeQuota, int effectiveMaxSize, BufferManager bufferManager)
        {
            base.Reinitialize(initialSize, maxSizeQuota, effectiveMaxSize, BufferManager.GetInternalBufferManager(bufferManager));
        }

        protected override Exception CreateQuotaExceededException(int maxSizeQuota)
        {
            string excMsg = SR.Format(_quotaExceededString, maxSizeQuota);
            if (TD.MaxSentMessageSizeExceededIsEnabled())
            {
                TD.MaxSentMessageSizeExceeded(excMsg);
            }
            return new QuotaExceededException(excMsg);
        }
    }
}
