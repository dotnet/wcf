// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel
{
    // see SynchronizedPool<T> for a threadsafe implementation
    public class Pool<T> where T : class
    {
        private T[] _items;
        private int _count;

        public Pool(int maxCount)
        {
            _items = new T[maxCount];
        }

        public int Count
        {
            get { return _count; }
        }

        public T Take()
        {
            if (_count > 0)
            {
                T item = _items[--_count];
                _items[_count] = null;
                return item;
            }
            else
            {
                return null;
            }
        }

        public bool Return(T item)
        {
            if (_count < _items.Length)
            {
                _items[_count++] = item;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Clear()
        {
            for (int i = 0; i < _count; i++)
                _items[i] = null;
            _count = 0;
        }
    }
}
