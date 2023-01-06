// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Threading;

#if DEBUG
using System.Collections.Concurrent;

#endif //DEBUG
namespace System.Runtime
{
    internal abstract class InternalBufferManager
    {
        protected InternalBufferManager()
        {
        }

        public abstract byte[] TakeBuffer(int bufferSize);
        public abstract void ReturnBuffer(byte[] buffer);
        public abstract void Clear();

        public static InternalBufferManager Create(long maxBufferPoolSize, int maxBufferSize)
        {
            if (maxBufferPoolSize == 0)
            {
                return GCBufferManager.Value;
            }
            else
            {
                Fx.Assert(maxBufferPoolSize > 0 && maxBufferSize >= 0, "bad params, caller should verify");
                return new PooledBufferManager(maxBufferPoolSize, maxBufferSize);
            }
        }

        internal class PooledBufferManager : InternalBufferManager
        {
            private const int minBufferSize = 128;
            private const int maxMissesBeforeTuning = 8;
            private const int initialBufferCount = 1;
            private readonly object _tuningLock;

            private int[] _bufferSizes;
            private BufferPool[] _bufferPools;
            private long _memoryLimit;
            private long _remainingMemory;
            private bool _areQuotasBeingTuned;
            private int _totalMisses;
#if DEBUG
            private ConcurrentDictionary<int, string> _buffersPooled = new ConcurrentDictionary<int, string>();
#endif //DEBUG

            public PooledBufferManager(long maxMemoryToPool, int maxBufferSize)
            {
                _tuningLock = new object();
                _memoryLimit = maxMemoryToPool;
                _remainingMemory = maxMemoryToPool;
                List<BufferPool> bufferPoolList = new List<BufferPool>();

                for (int bufferSize = minBufferSize; ;)
                {
                    long bufferCountLong = _remainingMemory / bufferSize;

                    int bufferCount = bufferCountLong > int.MaxValue ? int.MaxValue : (int)bufferCountLong;

                    if (bufferCount > initialBufferCount)
                    {
                        bufferCount = initialBufferCount;
                    }

                    bufferPoolList.Add(BufferPool.CreatePool(bufferSize, bufferCount));

                    _remainingMemory -= (long)bufferCount * bufferSize;

                    if (bufferSize >= maxBufferSize)
                    {
                        break;
                    }

                    long newBufferSizeLong = (long)bufferSize * 2;

                    if (newBufferSizeLong > (long)maxBufferSize)
                    {
                        bufferSize = maxBufferSize;
                    }
                    else
                    {
                        bufferSize = (int)newBufferSizeLong;
                    }
                }

                _bufferPools = bufferPoolList.ToArray();
                _bufferSizes = new int[_bufferPools.Length];
                for (int i = 0; i < _bufferPools.Length; i++)
                {
                    _bufferSizes[i] = _bufferPools[i].BufferSize;
                }
            }

            public override void Clear()
            {
#if DEBUG
                _buffersPooled.Clear();
#endif //DEBUG

                for (int i = 0; i < _bufferPools.Length; i++)
                {
                    BufferPool bufferPool = _bufferPools[i];
                    bufferPool.Clear();
                }
            }

            private void ChangeQuota(ref BufferPool bufferPool, int delta)
            {
                if (TraceCore.BufferPoolChangeQuotaIsEnabled(Fx.Trace))
                {
                    TraceCore.BufferPoolChangeQuota(Fx.Trace, bufferPool.BufferSize, delta);
                }

                BufferPool oldBufferPool = bufferPool;
                int newLimit = oldBufferPool.Limit + delta;
                BufferPool newBufferPool = BufferPool.CreatePool(oldBufferPool.BufferSize, newLimit);
                for (int i = 0; i < newLimit; i++)
                {
                    byte[] buffer = oldBufferPool.Take();
                    if (buffer == null)
                    {
                        break;
                    }
                    newBufferPool.Return(buffer);
                    newBufferPool.IncrementCount();
                }
                _remainingMemory -= oldBufferPool.BufferSize * delta;
                bufferPool = newBufferPool;
            }

            private void DecreaseQuota(ref BufferPool bufferPool)
            {
                ChangeQuota(ref bufferPool, -1);
            }

            private int FindMostExcessivePool()
            {
                long maxBytesInExcess = 0;
                int index = -1;

                for (int i = 0; i < _bufferPools.Length; i++)
                {
                    BufferPool bufferPool = _bufferPools[i];

                    if (bufferPool.Peak < bufferPool.Limit)
                    {
                        long bytesInExcess = (bufferPool.Limit - bufferPool.Peak) * (long)bufferPool.BufferSize;

                        if (bytesInExcess > maxBytesInExcess)
                        {
                            index = i;
                            maxBytesInExcess = bytesInExcess;
                        }
                    }
                }

                return index;
            }

            private int FindMostStarvedPool()
            {
                long maxBytesMissed = 0;
                int index = -1;

                for (int i = 0; i < _bufferPools.Length; i++)
                {
                    BufferPool bufferPool = _bufferPools[i];

                    if (bufferPool.Peak == bufferPool.Limit)
                    {
                        long bytesMissed = bufferPool.Misses * (long)bufferPool.BufferSize;

                        if (bytesMissed > maxBytesMissed)
                        {
                            index = i;
                            maxBytesMissed = bytesMissed;
                        }
                    }
                }

                return index;
            }

            private BufferPool FindPool(int desiredBufferSize)
            {
                for (int i = 0; i < _bufferSizes.Length; i++)
                {
                    if (desiredBufferSize <= _bufferSizes[i])
                    {
                        return _bufferPools[i];
                    }
                }

                return null;
            }

            private void IncreaseQuota(ref BufferPool bufferPool)
            {
                ChangeQuota(ref bufferPool, 1);
            }

            public override void ReturnBuffer(byte[] buffer)
            {
                Fx.Assert(buffer != null, "caller must verify");


                BufferPool bufferPool = FindPool(buffer.Length);
                if (bufferPool != null)
                {
                    if (buffer.Length != bufferPool.BufferSize)
                    {
                        throw Fx.Exception.Argument("buffer", InternalSR.BufferIsNotRightSizeForBufferManager);
                    }

                    if (bufferPool.Return(buffer))
                    {
                        bufferPool.IncrementCount();
                    }
                }
            }

            public override byte[] TakeBuffer(int bufferSize)
            {
                Fx.Assert(bufferSize >= 0, "caller must ensure a non-negative argument");

                BufferPool bufferPool = FindPool(bufferSize);
                byte[] returnValue;
                if (bufferPool != null)
                {
                    byte[] buffer = bufferPool.Take();
                    if (buffer != null)
                    {
                        bufferPool.DecrementCount();
                        returnValue = buffer;
                    }
                    else
                    {
                        if (bufferPool.Peak == bufferPool.Limit)
                        {
                            bufferPool.Misses++;
                            if (++_totalMisses >= maxMissesBeforeTuning)
                            {
                                TuneQuotas();
                            }
                        }

                        if (TraceCore.BufferPoolAllocationIsEnabled(Fx.Trace))
                        {
                            TraceCore.BufferPoolAllocation(Fx.Trace, bufferPool.BufferSize);
                        }

                        returnValue = Fx.AllocateByteArray(bufferPool.BufferSize);
                    }
                }
                else
                {
                    if (TraceCore.BufferPoolAllocationIsEnabled(Fx.Trace))
                    {
                        TraceCore.BufferPoolAllocation(Fx.Trace, bufferSize);
                    }

                    returnValue = Fx.AllocateByteArray(bufferSize);
                }

#if DEBUG
                string dummy;
                _buffersPooled.TryRemove(returnValue.GetHashCode(), out dummy);
#endif //DEBUG

                return returnValue;
            }


            private void TuneQuotas()
            {
                if (_areQuotasBeingTuned)
                {
                    return;
                }

                bool lockHeld = false;
                try
                {
                    Monitor.TryEnter(_tuningLock, ref lockHeld);

                    // Don't bother if another thread already has the lock
                    if (!lockHeld || _areQuotasBeingTuned)
                    {
                        return;
                    }

                    _areQuotasBeingTuned = true;
                }
                finally
                {
                    if (lockHeld)
                    {
                        Monitor.Exit(_tuningLock);
                    }
                }

                // find the "poorest" pool
                int starvedIndex = FindMostStarvedPool();
                if (starvedIndex >= 0)
                {
                    BufferPool starvedBufferPool = _bufferPools[starvedIndex];

                    if (_remainingMemory < starvedBufferPool.BufferSize)
                    {
                        // find the "richest" pool
                        int excessiveIndex = FindMostExcessivePool();
                        if (excessiveIndex >= 0)
                        {
                            // steal from the richest
                            DecreaseQuota(ref _bufferPools[excessiveIndex]);
                        }
                    }

                    if (_remainingMemory >= starvedBufferPool.BufferSize)
                    {
                        // give to the poorest
                        IncreaseQuota(ref _bufferPools[starvedIndex]);
                    }
                }

                // reset statistics
                for (int i = 0; i < _bufferPools.Length; i++)
                {
                    BufferPool bufferPool = _bufferPools[i];
                    bufferPool.Misses = 0;
                }

                _totalMisses = 0;
                _areQuotasBeingTuned = false;
            }

            internal abstract class BufferPool
            {
                private int _count;
                private int _peak;

                public BufferPool(int bufferSize, int limit)
                {
                    BufferSize = bufferSize;
                    Limit = limit;
                }

                public int BufferSize { get; }

                public int Limit { get; }

                public int Misses { get; set; }

                public int Peak
                {
                    get { return _peak; }
                }

                public void Clear()
                {
                    OnClear();
                    _count = 0;
                }

                public void DecrementCount()
                {
                    int newValue = _count - 1;
                    if (newValue >= 0)
                    {
                        _count = newValue;
                    }
                }

                public void IncrementCount()
                {
                    int newValue = _count + 1;
                    if (newValue <= Limit)
                    {
                        _count = newValue;
                        if (newValue > _peak)
                        {
                            _peak = newValue;
                        }
                    }
                }

                internal abstract byte[] Take();
                internal abstract bool Return(byte[] buffer);
                internal abstract void OnClear();

                internal static BufferPool CreatePool(int bufferSize, int limit)
                {
                    // To avoid many buffer drops during training of large objects which
                    // get allocated on the LOH, we use the LargeBufferPool and for 
                    // bufferSize < 85000, the SynchronizedPool. However if bufferSize < 85000
                    // and (bufferSize + array-overhead) > 85000, this would still use 
                    // the SynchronizedPool even though it is allocated on the LOH.
                    if (bufferSize < 85000)
                    {
                        return new SynchronizedBufferPool(bufferSize, limit);
                    }
                    else
                    {
                        return new LargeBufferPool(bufferSize, limit);
                    }
                }

                internal class SynchronizedBufferPool : BufferPool
                {
                    private SynchronizedPool<byte[]> _innerPool;

                    internal SynchronizedBufferPool(int bufferSize, int limit)
                        : base(bufferSize, limit)
                    {
                        _innerPool = new SynchronizedPool<byte[]>(limit);
                    }

                    internal override void OnClear()
                    {
                        _innerPool.Clear();
                    }

                    internal override byte[] Take()
                    {
                        return _innerPool.Take();
                    }

                    internal override bool Return(byte[] buffer)
                    {
                        return _innerPool.Return(buffer);
                    }
                }

                internal class LargeBufferPool : BufferPool
                {
                    private Stack<byte[]> _items;

                    internal LargeBufferPool(int bufferSize, int limit)
                        : base(bufferSize, limit)
                    {
                        _items = new Stack<byte[]>(limit);
                    }

                    private object ThisLock
                    {
                        get
                        {
                            return _items;
                        }
                    }

                    internal override void OnClear()
                    {
                        lock (ThisLock)
                        {
                            _items.Clear();
                        }
                    }

                    internal override byte[] Take()
                    {
                        lock (ThisLock)
                        {
                            if (_items.Count > 0)
                            {
                                return _items.Pop();
                            }
                        }

                        return null;
                    }

                    internal override bool Return(byte[] buffer)
                    {
                        lock (ThisLock)
                        {
                            if (_items.Count < Limit)
                            {
                                _items.Push(buffer);
                                return true;
                            }
                        }

                        return false;
                    }
                }
            }
        }

        internal class GCBufferManager : InternalBufferManager
        {
            private GCBufferManager()
            {
            }

            public static GCBufferManager Value { get; } = new GCBufferManager();

            public override void Clear()
            {
            }

            public override byte[] TakeBuffer(int bufferSize)
            {
                return Fx.AllocateByteArray(bufferSize);
            }

            public override void ReturnBuffer(byte[] buffer)
            {
                // do nothing, GC will reclaim this buffer
            }
        }
    }
}
