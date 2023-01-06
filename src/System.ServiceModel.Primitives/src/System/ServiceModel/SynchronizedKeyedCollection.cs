// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;
using System.Runtime.InteropServices;
using System.ServiceModel;

namespace System.Collections.Generic
{
    [ComVisible(false)]
    public abstract class SynchronizedKeyedCollection<K, T> : SynchronizedCollection<T>
    {
        private const int defaultThreshold = 0;

        private IEqualityComparer<K> _comparer;
        private Dictionary<K, T> _dictionary;
        private int _keyCount;
        private int _threshold;

        protected SynchronizedKeyedCollection()
        {
            _comparer = EqualityComparer<K>.Default;
            _threshold = int.MaxValue;
        }

        protected SynchronizedKeyedCollection(object syncRoot)
            : base(syncRoot)
        {
            _comparer = EqualityComparer<K>.Default;
            _threshold = int.MaxValue;
        }

        protected SynchronizedKeyedCollection(object syncRoot, IEqualityComparer<K> comparer)
            : base(syncRoot)
        {
            _comparer = comparer ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(comparer)));
            _threshold = int.MaxValue;
        }

        protected SynchronizedKeyedCollection(object syncRoot, IEqualityComparer<K> comparer, int dictionaryCreationThreshold)
            : base(syncRoot)
        {
            if (dictionaryCreationThreshold < -1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(dictionaryCreationThreshold), dictionaryCreationThreshold,
                                                    SRP.Format(SRP.ValueMustBeInRange, -1, int.MaxValue)));
            }
            else if (dictionaryCreationThreshold == -1)
            {
                _threshold = int.MaxValue;
            }
            else
            {
                _threshold = dictionaryCreationThreshold;
            }

            _comparer = comparer ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(comparer)));
        }

        public T this[K key]
        {
            get
            {
                if (key == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(key)));
                }

                lock (SyncRoot)
                {
                    if (_dictionary != null)
                    {
                        return _dictionary[key];
                    }

                    for (int i = 0; i < Items.Count; i++)
                    {
                        T item = Items[i];
                        if (_comparer.Equals(key, GetKeyForItem(item)))
                        {
                            return item;
                        }
                    }

                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new KeyNotFoundException());
                }
            }
        }

        protected IDictionary<K, T> Dictionary
        {
            get { return _dictionary; }
        }

        private void AddKey(K key, T item)
        {
            if (_dictionary != null)
            {
                _dictionary.Add(key, item);
            }
            else if (_keyCount == _threshold)
            {
                CreateDictionary();
                _dictionary.Add(key, item);
            }
            else
            {
                if (Contains(key))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.CannotAddTwoItemsWithTheSameKeyToSynchronizedKeyedCollection0));
                }

                _keyCount++;
            }
        }

        protected void ChangeItemKey(T item, K newKey)
        {
            // check if the item exists in the collection
            if (!ContainsItem(item))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.ItemDoesNotExistInSynchronizedKeyedCollection0));
            }

            K oldKey = GetKeyForItem(item);
            if (!_comparer.Equals(newKey, oldKey))
            {
                if (newKey != null)
                {
                    AddKey(newKey, item);
                }

                if (oldKey != null)
                {
                    RemoveKey(oldKey);
                }
            }
        }

        protected override void ClearItems()
        {
            base.ClearItems();

            if (_dictionary != null)
            {
                _dictionary.Clear();
            }

            _keyCount = 0;
        }

        public bool Contains(K key)
        {
            if (key == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(key)));
            }

            lock (SyncRoot)
            {
                if (_dictionary != null)
                {
                    return _dictionary.ContainsKey(key);
                }

                if (key != null)
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        T item = Items[i];
                        if (_comparer.Equals(key, GetKeyForItem(item)))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        private bool ContainsItem(T item)
        {
            K key;
            if ((_dictionary == null) || ((key = GetKeyForItem(item)) == null))
            {
                return Items.Contains(item);
            }

            T itemInDict;

            if (_dictionary.TryGetValue(key, out itemInDict))
            {
                return EqualityComparer<T>.Default.Equals(item, itemInDict);
            }

            return false;
        }

        private void CreateDictionary()
        {
            _dictionary = new Dictionary<K, T>(_comparer);

            foreach (T item in Items)
            {
                K key = GetKeyForItem(item);
                if (key != null)
                {
                    _dictionary.Add(key, item);
                }
            }
        }

        protected abstract K GetKeyForItem(T item);

        protected override void InsertItem(int index, T item)
        {
            K key = GetKeyForItem(item);

            if (key != null)
            {
                AddKey(key, item);
            }

            base.InsertItem(index, item);
        }

        public bool Remove(K key)
        {
            if (key == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(key)));
            }

            lock (SyncRoot)
            {
                if (_dictionary != null)
                {
                    if (_dictionary.ContainsKey(key))
                    {
                        return Remove(_dictionary[key]);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (_comparer.Equals(key, GetKeyForItem(Items[i])))
                        {
                            RemoveItem(i);
                            return true;
                        }
                    }
                    return false;
                }
            }
        }

        protected override void RemoveItem(int index)
        {
            K key = GetKeyForItem(Items[index]);

            if (key != null)
            {
                RemoveKey(key);
            }

            base.RemoveItem(index);
        }

        private void RemoveKey(K key)
        {
            if (!(key != null))
            {
                Fx.Assert("key shouldn't be null!");
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(key));
            }
            if (_dictionary != null)
            {
                _dictionary.Remove(key);
            }
            else
            {
                _keyCount--;
            }
        }

        protected override void SetItem(int index, T item)
        {
            K newKey = GetKeyForItem(item);
            K oldKey = GetKeyForItem(Items[index]);

            if (_comparer.Equals(newKey, oldKey))
            {
                if ((newKey != null) && (_dictionary != null))
                {
                    _dictionary[newKey] = item;
                }
            }
            else
            {
                if (newKey != null)
                {
                    AddKey(newKey, item);
                }

                if (oldKey != null)
                {
                    RemoveKey(oldKey);
                }
            }
            base.SetItem(index, item);
        }
    }
}
