// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ServiceModel;

namespace System.Runtime
{
    internal class MruCache<TKey, TValue> : IDisposable
        where TKey : class
        where TValue : class
    {
        private LinkedList<TKey> _mruList;
        private Dictionary<TKey, CacheEntry> _items;
        private readonly int _lowWatermark;
        private readonly int _highWatermark;
        private CacheEntry _mruEntry;
        private int _refCount = 1;
        private object _mutex = new object();

        public MruCache(int watermark)
            : this(watermark * 4 / 5, watermark)
        {
        }

        // Returns whether the cache can be used by the caller after calling AddRef
        public bool AddRef()
        {
            lock(_mutex)
            {
                if (_refCount == 0)
                {
                    return false;
                }

                _refCount++;
                return true;
            }
        }

        //
        // The cache will grow until the high watermark. At which point, the least recently used items
        // will be purge until the cache's size is reduced to low watermark
        //
        public MruCache(int lowWatermark, int highWatermark)
            : this(lowWatermark, highWatermark, null)
        {
        }

        public MruCache(int lowWatermark, int highWatermark, IEqualityComparer<TKey> comparer)
        {
            Fx.Assert(lowWatermark < highWatermark, "");
            Fx.Assert(lowWatermark >= 0, "");

            _lowWatermark = lowWatermark;
            _highWatermark = highWatermark;
            _mruList = new LinkedList<TKey>();
            if (comparer == null)
            {
                _items = new Dictionary<TKey, CacheEntry>();
            }
            else
            {
                _items = new Dictionary<TKey, CacheEntry>(comparer);
            }
        }

        public int Count
        {
            get
            {
                ThrowIfDisposed();
                return _items.Count;
            }
        }

        public bool IsDisposed { get; private set; }

        public void Add(TKey key, TValue value)
        {
            Fx.Assert(null != key, "");
            ThrowIfDisposed();

            // if anything goes wrong (duplicate entry, etc) we should 
            // clear our caches so that we don't get out of sync
            bool success = false;
            try
            {
                if (_items.Count == _highWatermark)
                {
                    // If the cache is full, purge enough LRU items to shrink the 
                    // cache down to the low watermark
                    int countToPurge = _highWatermark - _lowWatermark;
                    for (int i = 0; i < countToPurge; i++)
                    {
                        TKey keyRemove = _mruList.Last.Value;
                        _mruList.RemoveLast();
                        TValue item = _items[keyRemove].value;
                        _items.Remove(keyRemove);
                        OnSingleItemRemoved(item);
                        OnItemAgedOutOfCache(item);
                    }
                }
                // Add  the new entry to the cache and make it the MRU element
                CacheEntry entry;
                entry.node = _mruList.AddFirst(key);
                entry.value = value;
                _items.Add(key, entry);
                _mruEntry = entry;
                success = true;
            }
            finally
            {
                if (!success)
                {
                    Clear();
                }
            }
        }

        public void Clear()
        {
            ThrowIfDisposed();
            Clear(false);
        }

        private void Clear(bool dispose)
        {
            _mruList.Clear();
            if (dispose)
            {
                foreach (CacheEntry cacheEntry in _items.Values)
                {
                    var item = cacheEntry.value as IDisposable;
                    if (item != null)
                    {
                        try
                        {
                            item.Dispose();
                        }
                        catch (Exception e)
                        {
                            if (Fx.IsFatal(e))
                            {
                                throw;
                            }
                        }
                    }
                }
            }

            _items.Clear();
            _mruEntry.value = null;
            _mruEntry.node = null;
        }

        public bool Remove(TKey key)
        {
            Fx.Assert(null != key, "");
            ThrowIfDisposed();

            CacheEntry entry;
            if (_items.TryGetValue(key, out entry))
            {
                _items.Remove(key);
                OnSingleItemRemoved(entry.value);
                _mruList.Remove(entry.node);
                if (object.ReferenceEquals(_mruEntry.node, entry.node))
                {
                    _mruEntry.value = null;
                    _mruEntry.node = null;
                }
                return true;
            }

            return false;
        }

        protected virtual void OnSingleItemRemoved(TValue item)
        {
            ThrowIfDisposed();
        }

        protected virtual void OnItemAgedOutOfCache(TValue item)
        {
            ThrowIfDisposed();
        }

        //
        // If found, make the entry most recently used
        //
        public bool TryGetValue(TKey key, out TValue value)
        {
            // first check our MRU item
            if (_mruEntry.node != null && key != null && key.Equals(_mruEntry.node.Value))
            {
                value = _mruEntry.value;
                return true;
            }

            CacheEntry entry;

            bool found = _items.TryGetValue(key, out entry);
            value = entry.value;

            // Move the node to the head of the MRU list if it's not already there
            if (found && _mruList.Count > 1
                && !object.ReferenceEquals(_mruList.First, entry.node))
            {
                _mruList.Remove(entry.node);
                _mruList.AddFirst(entry.node);
                _mruEntry = entry;
            }

            return found;
        }

        public void Dispose()
        {
            int refCount;
            lock (_mutex)
            {
                refCount = --_refCount;
            }
            Fx.Assert(refCount >= 0, "Ref count shouldn't go below zero");
            if (refCount == 0)
            {
                Dispose(true);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_mutex)
                {
                    if (!IsDisposed)
                    {
                        IsDisposed = true;
                        Clear(true);
                    }
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(GetType().FullName));
            }
        }

        private struct CacheEntry
        {
            internal TValue value;
            internal LinkedListNode<TKey> node;
        }
    }
}
