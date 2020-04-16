// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Threading;

namespace System.ServiceModel.Security
{
    // NOTE: this class does minimum argument checking as it is all internal 
    internal class TimeBoundedCache
    {
        private static Action<object> s_purgeCallback;
        private ReaderWriterLockSlim _cacheLock;
        private Hashtable _entries;
        // if there are less than lowWaterMark entries, no purging is done
        private int _lowWaterMark;
        private int _maxCacheItems;
        private DateTime _nextPurgeTimeUtc;
        private TimeSpan _purgeInterval;
        private PurgingMode _purgingMode;
        private Timer _purgingTimer;
        private bool _doRemoveNotification;

        protected TimeBoundedCache(int lowWaterMark, int maxCacheItems, IEqualityComparer keyComparer, PurgingMode purgingMode, TimeSpan purgeInterval, bool doRemoveNotification)
        {
            _entries = new Hashtable(keyComparer);
            _cacheLock = new ReaderWriterLockSlim();
            _lowWaterMark = lowWaterMark;
            _maxCacheItems = maxCacheItems;
            _purgingMode = purgingMode;
            _purgeInterval = purgeInterval;
            _doRemoveNotification = doRemoveNotification;
            _nextPurgeTimeUtc = DateTime.UtcNow.Add(_purgeInterval);
        }

        public int Count
        {
            get
            {
                return _entries.Count;
            }
        }

        private static Action<object> PurgeCallback
        {
            get
            {
                if (s_purgeCallback == null)
                {
                    s_purgeCallback = new Action<object>(PurgeCallbackStatic);
                }
                return s_purgeCallback;
            }
        }

        protected int Capacity
        {
            get
            {
                return _maxCacheItems;
            }
        }

        protected Hashtable Entries
        {
            get
            {
                return _entries;
            }
        }

        protected ReaderWriterLockSlim CacheLock
        {
            get
            {
                return _cacheLock;
            }
        }

        protected bool TryAddItem(object key, object item, DateTime expirationTime, bool replaceExistingEntry)
        {
            return this.TryAddItem(key, new ExpirableItem(item, expirationTime), replaceExistingEntry);
        }

        private void CancelTimerIfNeeded()
        {
            if (this.Count == 0 && _purgingTimer != null)
            {
                _purgingTimer.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
                _purgingTimer.Dispose();
                _purgingTimer = null;
            }
        }

        private void StartTimerIfNeeded()
        {
            if (_purgingMode != PurgingMode.TimerBasedPurge)
            {
                return;
            }
            if (_purgingTimer == null)
            {
                _purgingTimer = new Timer(new TimerCallback(PurgeCallback), this, _purgeInterval, TimeSpan.FromMilliseconds(-1));
            }
        }

        protected bool TryAddItem(object key, IExpirableItem item, bool replaceExistingEntry)
        {
            bool lockHeld = false;
            try
            {
                try { }
                finally
                {
                    _cacheLock.EnterWriteLock();
                    lockHeld = true;
                }
                PurgeIfNeeded();
                EnforceQuota();
                IExpirableItem currentItem = _entries[key] as IExpirableItem;
                if (currentItem == null || IsExpired(currentItem))
                {
                    _entries[key] = item;
                }
                else if (!replaceExistingEntry)
                {
                    return false;
                }
                else
                {
                    _entries[key] = item;
                }
                if (currentItem != null && _doRemoveNotification)
                {
                    this.OnRemove(ExtractItem(currentItem));
                }
                StartTimerIfNeeded();
                return true;
            }
            finally
            {
                if (lockHeld)
                {
                    _cacheLock.ExitWriteLock();
                }
            }
        }

        protected bool TryReplaceItem(object key, object item, DateTime expirationTime)
        {
            bool lockHeld = false;
            try
            {
                try { }
                finally
                {
                    _cacheLock.EnterWriteLock();
                    lockHeld = true;
                }
                PurgeIfNeeded();
                EnforceQuota();
                IExpirableItem currentItem = _entries[key] as IExpirableItem;
                if (currentItem == null || IsExpired(currentItem))
                {
                    return false;
                }
                else
                {
                    _entries[key] = new ExpirableItem(item, expirationTime);
                    if (currentItem != null && _doRemoveNotification)
                    {
                        this.OnRemove(ExtractItem(currentItem));
                    }
                    StartTimerIfNeeded();
                    return true;
                }
            }
            finally
            {
                if (lockHeld)
                {
                    _cacheLock.ExitWriteLock();
                }
            }
        }

        protected void ClearItems()
        {
            bool lockHeld = false;
            try
            {
                try { }
                finally
                {
                    _cacheLock.EnterWriteLock();
                    lockHeld = true;
                }

                int count = _entries.Count;
                if (_doRemoveNotification)
                {
                    foreach (IExpirableItem item in _entries.Values)
                    {
                        OnRemove(ExtractItem(item));
                    }
                }
                _entries.Clear();
                CancelTimerIfNeeded();
            }
            finally
            {
                if (lockHeld)
                {
                    _cacheLock.ExitWriteLock();
                }
            }
        }

        protected object GetItem(object key)
        {
            bool lockHeld = false;
            try
            {
                try { }
                finally
                {
                    _cacheLock.EnterReadLock();
                    lockHeld = true;
                }
                IExpirableItem item = _entries[key] as IExpirableItem;
                if (item == null)
                {
                    return null;
                }
                else if (IsExpired(item))
                {
                    // this is a stale item
                    return null;
                }
                else
                {
                    return ExtractItem(item);
                }
            }
            finally
            {
                if (lockHeld)
                {
                    _cacheLock.ExitReadLock();
                }
            }
        }

        protected virtual ArrayList OnQuotaReached(Hashtable cacheTable)
        {
            this.ThrowQuotaReachedException();
            return null;
        }

        protected virtual void OnRemove(object item)
        {
        }

        protected bool TryRemoveItem(object key)
        {
            bool lockHeld = false;
            try
            {
                try { }
                finally
                {
                    _cacheLock.EnterWriteLock();
                    lockHeld = true;
                }
                PurgeIfNeeded();
                IExpirableItem currentItem = _entries[key] as IExpirableItem;
                bool result = (currentItem != null) && !IsExpired(currentItem);
                if (currentItem != null)
                {
                    _entries.Remove(key);
                    if (_doRemoveNotification)
                    {
                        this.OnRemove(ExtractItem(currentItem));
                    }
                    CancelTimerIfNeeded();
                }
                return result;
            }
            finally
            {
                if (lockHeld)
                {
                    _cacheLock.ExitWriteLock();
                }
            }
        }


        private void EnforceQuota()
        {
            if (!(_cacheLock.IsWriteLockHeld == true))
            {
                // we failfast here because if we don't have the lock we could corrupt the cache
                Fx.Assert("Cache write lock is not held.");
                Environment.FailFast("Cache write lock is not held.");
            }
            if (this.Count >= _maxCacheItems)
            {
                ArrayList keysToBeRemoved;
                keysToBeRemoved = this.OnQuotaReached(_entries);
                if (keysToBeRemoved != null)
                {
                    for (int i = 0; i < keysToBeRemoved.Count; ++i)
                    {
                        _entries.Remove(keysToBeRemoved[i]);
                    }
                }
                CancelTimerIfNeeded();
                if (this.Count >= _maxCacheItems)
                {
                    this.ThrowQuotaReachedException();
                }
            }
        }

        protected object ExtractItem(IExpirableItem val)
        {
            ExpirableItem wrapper = (val as ExpirableItem);
            if (wrapper != null)
            {
                return wrapper.Item;
            }
            else
            {
                return val;
            }
        }

        private bool IsExpired(IExpirableItem item)
        {
            Fx.Assert(item.ExpirationTime == DateTime.MaxValue || item.ExpirationTime.Kind == DateTimeKind.Utc, "");
            return (item.ExpirationTime <= DateTime.UtcNow);
        }

        private bool ShouldPurge()
        {
            if (this.Count >= _maxCacheItems)
            {
                return true;
            }
            else if (_purgingMode == PurgingMode.AccessBasedPurge && DateTime.UtcNow > _nextPurgeTimeUtc && this.Count > _lowWaterMark)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void PurgeIfNeeded()
        {
            if (!(_cacheLock.IsWriteLockHeld == true))
            {
                // we failfast here because if we don't have the lock we could corrupt the cache
                Fx.Assert("Cache write lock is not held.");
                Environment.FailFast("Cache write lock is not held.");
            }
            if (ShouldPurge())
            {
                PurgeStaleItems();
            }
        }

        /// <summary>
        /// This method must be called from within a writer lock
        /// </summary>
        private void PurgeStaleItems()
        {
            if (!(_cacheLock.IsWriteLockHeld == true))
            {
                // we failfast here because if we don't have the lock we could corrupt the cache
                Fx.Assert("Cache write lock is not held.");
                Environment.FailFast("Cache write lock is not held.");
            }
            ArrayList expiredItems = new ArrayList();
            foreach (object key in _entries.Keys)
            {
                IExpirableItem item = _entries[key] as IExpirableItem;
                if (IsExpired(item))
                {
                    // this is a stale item. Remove!
                    this.OnRemove(ExtractItem(item));
                    expiredItems.Add(key);
                }
            }
            for (int i = 0; i < expiredItems.Count; ++i)
            {
                _entries.Remove(expiredItems[i]);
            }
            CancelTimerIfNeeded();
            _nextPurgeTimeUtc = DateTime.UtcNow.Add(_purgeInterval);
        }

        private void ThrowQuotaReachedException()
        {
            string message = SRServiceModel.Format(SRServiceModel.CacheQuotaReached, _maxCacheItems);
            Exception inner = new QuotaExceededException(message);
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(message, inner));
        }

        private static void PurgeCallbackStatic(object state)
        {
            TimeBoundedCache self = (TimeBoundedCache)state;

            bool lockHeld = false;
            try
            {
                try { }
                finally
                {
                    self._cacheLock.EnterWriteLock();
                    lockHeld = true;
                }

                if (self._purgingTimer == null)
                {
                    return;
                }
                self.PurgeStaleItems();
                if (self.Count > 0 && self._purgingTimer != null)
                {
                    self._purgingTimer.Change(self._purgeInterval, TimeSpan.FromMilliseconds(-1));
                }
            }
            finally
            {
                if (lockHeld)
                {
                    self._cacheLock.ExitWriteLock();
                }
            }
        }

        internal interface IExpirableItem
        {
            DateTime ExpirationTime { get; }
        }

        internal class ExpirableItemComparer : IComparer<IExpirableItem>
        {
            private static ExpirableItemComparer s_instance;

            public static ExpirableItemComparer Default
            {
                get
                {
                    if (s_instance == null)
                    {
                        s_instance = new ExpirableItemComparer();
                    }
                    return s_instance;
                }
            }

            // positive, if item1 will expire before item2. 
            public int Compare(IExpirableItem item1, IExpirableItem item2)
            {
                if (ReferenceEquals(item1, item2))
                {
                    return 0;
                }
                Fx.Assert(item1.ExpirationTime.Kind == item2.ExpirationTime.Kind, "");
                if (item1.ExpirationTime < item2.ExpirationTime)
                {
                    return 1;
                }
                else if (item1.ExpirationTime > item2.ExpirationTime)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }

        internal sealed class ExpirableItem : IExpirableItem
        {
            private DateTime _expirationTime;
            private object _item;

            public ExpirableItem(object item, DateTime expirationTime)
            {
                _item = item;
                Fx.Assert(expirationTime == DateTime.MaxValue || expirationTime.Kind == DateTimeKind.Utc, "");
                _expirationTime = expirationTime;
            }

            public DateTime ExpirationTime { get { return _expirationTime; } }
            public object Item { get { return _item; } }
        }
    }

    internal enum PurgingMode
    {
        TimerBasedPurge,
        AccessBasedPurge
    }
}
