﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
#if DEBUG
using System.Diagnostics;
#endif

namespace System.Runtime
{
    internal class AsyncLock
    {
#if DEBUG
        private StackTrace _lockTakenCallStack;
        private string _lockTakenCallStackString;
#endif
        private readonly SemaphoreSlim _semaphore;
        private readonly SafeSemaphoreRelease _semaphoreRelease;
        private AsyncLocal<bool> _lockTaken;

        public AsyncLock()
        {
            _semaphore = new SemaphoreSlim(1);
            _semaphoreRelease = new SafeSemaphoreRelease(this);
            _lockTaken = new AsyncLocal<bool>(LockTakenValueChanged);
            _lockTaken.Value = false;
        }

        private void LockTakenValueChanged(AsyncLocalValueChangedArgs<bool> obj)
        {
            // Without this fixup, when completing the call to await TakeLockAsync there is
            // a switch of Context and _localTaken will be reset to false. This is because
            // of leaving the task.

            if (obj.ThreadContextChanged)
            {
                _lockTaken.Value = obj.PreviousValue;
            }
        }

        public async Task<IDisposable> TakeLockAsync()
        {
            if (_lockTaken.Value)
            {
                return null;
            }

            await _semaphore.WaitAsync();
            _lockTaken.Value = true;
#if DEBUG
            _lockTakenCallStack = new StackTrace();
            _lockTakenCallStackString = _lockTakenCallStack.ToString();
#endif
            return _semaphoreRelease;
        }

        public async Task<IDisposable> TakeLockAsync(CancellationToken token)
        {
            if (_lockTaken.Value)
            {
                return null;
            }

            await _semaphore.WaitAsync(token);
            _lockTaken.Value = true;
#if DEBUG
            _lockTakenCallStack = new StackTrace();
            _lockTakenCallStackString = _lockTakenCallStack.ToString();
#endif
            return _semaphoreRelease;
        }

        public IDisposable TakeLock()
        {
            if (_lockTaken.Value)
            {
                return null;
            }

            _semaphore.Wait();
            _lockTaken.Value = true;
#if DEBUG
            _lockTakenCallStack = new StackTrace();
            _lockTakenCallStackString = _lockTakenCallStack.ToString();
#endif
            return _semaphoreRelease;
        }

        public IDisposable TakeLock(TimeSpan timeout)
        {
            if (_lockTaken.Value)
            {
                return null;
            }

            _semaphore.Wait(timeout);
            _lockTaken.Value = true;
#if DEBUG
            _lockTakenCallStack = new StackTrace();
            _lockTakenCallStackString = _lockTakenCallStack.ToString();
#endif
            return _semaphoreRelease;
        }

        public IDisposable TakeLock(int timeout)
        {
            if (_lockTaken.Value)
            {
                return null;
            }

            _semaphore.Wait(timeout);
            _lockTaken.Value = true;
#if DEBUG
            _lockTakenCallStack = new StackTrace();
            _lockTakenCallStackString = _lockTakenCallStack.ToString();
#endif
            return _semaphoreRelease;
        }

        public struct SafeSemaphoreRelease : IDisposable
        {
            private readonly AsyncLock _asyncLock;

            public SafeSemaphoreRelease(AsyncLock asyncLock)
            {
                _asyncLock = asyncLock;
            }

            public void Dispose()
            {
#if DEBUG
                _asyncLock._lockTakenCallStack = null;
                _asyncLock._lockTakenCallStackString = null;
#endif
                _asyncLock._lockTaken.Value = false;
                _asyncLock._semaphore.Release();
            }
        }
    }
}
