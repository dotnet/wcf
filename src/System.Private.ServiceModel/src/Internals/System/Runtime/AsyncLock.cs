// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;

namespace System.Runtime
{
    internal class AsyncLock : IAsyncDisposable
    {
        private static ObjectPool<SemaphoreSlim> s_semaphorePool = (new DefaultObjectPoolProvider { MaximumRetained = 100 })
            .Create(new SemaphoreSlimPooledObjectPolicy());

        private AsyncLocal<SemaphoreSlim> _currentSemaphore;
        private SemaphoreSlim _topLevelSemaphore;
        private bool _isDisposed;

        public AsyncLock()
        {
            _topLevelSemaphore = s_semaphorePool.Get();
            _currentSemaphore = new AsyncLocal<SemaphoreSlim>();
        }

        public Task<IAsyncDisposable> TakeLockAsync(CancellationToken cancellationToken = default)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(AsyncLock));
            if (_currentSemaphore.Value == null)
            {
                _currentSemaphore.Value = _topLevelSemaphore;
            }
            SemaphoreSlim currentSem = _currentSemaphore.Value;
            var nextSem = s_semaphorePool.Get();
            _currentSemaphore.Value = nextSem;
            var safeRelease = new SafeSemaphoreRelease(currentSem, nextSem, this);
            return TakeLockCoreAsync(currentSem, safeRelease);
        }

        private async Task<IAsyncDisposable> TakeLockCoreAsync(SemaphoreSlim currentSemaphore, SafeSemaphoreRelease safeSemaphoreRelease, CancellationToken cancellationToken = default)
        {
            await currentSemaphore.WaitAsync(cancellationToken);
            return safeSemaphoreRelease;
        }

        public IDisposable TakeLock()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(AsyncLock));
            if (_currentSemaphore.Value == null)
            {
                _currentSemaphore.Value = _topLevelSemaphore;
            }
            SemaphoreSlim currentSem = _currentSemaphore.Value;
            currentSem.Wait(/*cancellationToken*/);
            var nextSem = s_semaphorePool.Get();
            _currentSemaphore.Value = nextSem;
            return new SafeSemaphoreRelease(currentSem, nextSem, this);
        }

        public async ValueTask DisposeAsync()
        {
            if (_isDisposed)
                return;
            _isDisposed = true;
            // Ensure not in use
            await _topLevelSemaphore.WaitAsync();
            _topLevelSemaphore.Release();
            s_semaphorePool.Return(_topLevelSemaphore);
            _topLevelSemaphore = null;
        }

        private struct SafeSemaphoreRelease : IAsyncDisposable, IDisposable
        {
            private SemaphoreSlim _currentSemaphore;
            private SemaphoreSlim _nextSemaphore;
            private AsyncLock _asyncLock;

            public SafeSemaphoreRelease(SemaphoreSlim currentSemaphore, SemaphoreSlim nextSemaphore, AsyncLock asyncLock)
            {
                _currentSemaphore = currentSemaphore;
                _nextSemaphore = nextSemaphore;
                _asyncLock = asyncLock;
            }

            public async ValueTask DisposeAsync()
            {
                Fx.Assert(_nextSemaphore == _asyncLock._currentSemaphore.Value, "_nextSemaphore was expected to by the current semaphore");
                await _nextSemaphore.WaitAsync();
                _asyncLock._currentSemaphore.Value = _currentSemaphore;
                _currentSemaphore.Release();
                _nextSemaphore.Release();
                s_semaphorePool.Return(_nextSemaphore);
                if (_asyncLock._currentSemaphore.Value == _asyncLock._topLevelSemaphore)
                {
                    _asyncLock._currentSemaphore.Value = null;
                }
            }

            public void Dispose()
            {
                Fx.Assert(_nextSemaphore == _asyncLock._currentSemaphore.Value, "_nextSemaphore was expected to by the current semaphore");
                _nextSemaphore.Wait();
                _asyncLock._currentSemaphore.Value = _currentSemaphore;
                _currentSemaphore.Release();
                _nextSemaphore.Release();
                s_semaphorePool.Return(_nextSemaphore);
                if (_asyncLock._currentSemaphore.Value == _asyncLock._topLevelSemaphore)
                {
                    _asyncLock._currentSemaphore.Value = null;
                }
            }
        }

        private class SemaphoreSlimPooledObjectPolicy : PooledObjectPolicy<SemaphoreSlim>
        {
            public override SemaphoreSlim Create()
            {
                return new SemaphoreSlim(1);
            }

            public override bool Return(SemaphoreSlim obj)
            {
                if (obj.CurrentCount != 1)
                {
                    Fx.Assert("Shouldn't be returning semaphore with a count != 1");
                    return false;
                }

                return true;
            }
        }
    }
}
