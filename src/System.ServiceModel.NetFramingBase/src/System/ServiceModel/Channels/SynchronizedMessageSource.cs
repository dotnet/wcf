// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime;
using System.Threading.Tasks;
using System.Threading;

namespace System.ServiceModel.Channels
{
    internal class SynchronizedMessageSource
    {
        private IMessageSource _source;
        private SemaphoreSlim _sourceLock;

        public SynchronizedMessageSource(IMessageSource source)
        {
            _source = source;
            _sourceLock = new SemaphoreSlim(1);
        }

        public async Task<bool> WaitForMessageAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            // If timeout == TimeSpan.MaxValue, then we want to pass Timeout.Infinite as 
            // SemaphoreSlim doesn't accept timeouts > Int32.MaxValue.
            // Using TimeoutHelper.RemainingTime() would yield a value less than TimeSpan.MaxValue
            // and would result in the value Int32.MaxValue so we must use the original timeout specified.
            if (!await _sourceLock.WaitAsync(TimeoutHelper.ToMilliseconds(timeout)))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new TimeoutException(SR.Format(SR.WaitForMessageTimedOut, timeout),
                    TimeoutHelper.CreateEnterTimedOutException(timeout)));
            }
            try
            {
                return await _source.WaitForMessageAsync(timeoutHelper.RemainingTime());
            }
            finally
            {
                _sourceLock.Release();
            }
        }

        public async Task<Message> ReceiveAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            // If timeout == TimeSpan.MaxValue, then we want to pass Timeout.Infinite as 
            // SemaphoreSlim doesn't accept timeouts > Int32.MaxValue.
            // Using TimeoutHelper.RemainingTime() would yield a value less than TimeSpan.MaxValue
            // and would result in the value Int32.MaxValue so we must use the original timeout specified.
            if (!await _sourceLock.WaitAsync(TimeoutHelper.ToMilliseconds(timeout)))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new TimeoutException(SR.Format(SR.ReceiveTimedOut2, timeout),
                    TimeoutHelper.CreateEnterTimedOutException(timeout)));
            }

            try
            {
                return await _source.ReceiveAsync(timeoutHelper.RemainingTime());
            }
            finally
            {
                _sourceLock.Release();
            }
        }
    }
}
