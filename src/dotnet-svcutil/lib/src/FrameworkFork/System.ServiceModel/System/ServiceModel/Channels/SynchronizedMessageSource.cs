// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Runtime;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

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
                    new TimeoutException(SRServiceModel.Format(SRServiceModel.WaitForMessageTimedOut, timeout),
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

        public bool WaitForMessage(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            // If timeout == TimeSpan.MaxValue, then we want to pass Timeout.Infinite as 
            // SemaphoreSlim doesn't accept timeouts > Int32.MaxValue.
            // Using TimeoutHelper.RemainingTime() would yield a value less than TimeSpan.MaxValue
            // and would result in the value Int32.MaxValue so we must use the original timeout specified.
            if (!_sourceLock.Wait(TimeoutHelper.ToMilliseconds(timeout)))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new TimeoutException(SRServiceModel.Format(SRServiceModel.WaitForMessageTimedOut, timeout),
                    TimeoutHelper.CreateEnterTimedOutException(timeout)));
            }

            try
            {
                return _source.WaitForMessage(timeoutHelper.RemainingTime());
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
                    new TimeoutException(SRServiceModel.Format(SRServiceModel.ReceiveTimedOut2, timeout),
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

        public Message Receive(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            // If timeout == TimeSpan.MaxValue, then we want to pass Timeout.Infinite as 
            // SemaphoreSlim doesn't accept timeouts > Int32.MaxValue.
            // Using TimeoutHelper.RemainingTime() would yield a value less than TimeSpan.MaxValue
            // and would result in the value Int32.MaxValue so we must use the original timeout specified.
            if (!_sourceLock.Wait(TimeoutHelper.ToMilliseconds(timeout)))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new TimeoutException(SRServiceModel.Format(SRServiceModel.ReceiveTimedOut2, timeout),
                    TimeoutHelper.CreateEnterTimedOutException(timeout)));
            }

            try
            {
                return _source.Receive(timeoutHelper.RemainingTime());
            }
            finally
            {
                _sourceLock.Release();
            }
        }
    }
}
