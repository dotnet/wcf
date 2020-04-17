// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel;

namespace System.Collections.Generic
{
    [System.Runtime.InteropServices.ComVisible(false)]
    public class SynchronizedReadOnlyCollection<T> : IList<T>, IList
    {
        private IList<T> _items;
        private object _sync;

        public SynchronizedReadOnlyCollection()
        {
            _items = new List<T>();
            _sync = new Object();
        }

        public SynchronizedReadOnlyCollection(object syncRoot)
        {
            if (syncRoot == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("syncRoot"));

            _items = new List<T>();
            _sync = syncRoot;
        }

        public SynchronizedReadOnlyCollection(object syncRoot, IEnumerable<T> list)
        {
            if (syncRoot == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("syncRoot"));
            if (list == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("list"));

            _items = new List<T>(list);
            _sync = syncRoot;
        }

        public SynchronizedReadOnlyCollection(object syncRoot, params T[] list)
        {
            if (syncRoot == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("syncRoot"));
            if (list == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("list"));

            _items = new List<T>(list.Length);
            for (int i = 0; i < list.Length; i++)
                _items.Add(list[i]);

            _sync = syncRoot;
        }

        internal SynchronizedReadOnlyCollection(object syncRoot, List<T> list, bool makeCopy)
        {
            if (syncRoot == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("syncRoot"));
            if (list == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("list"));

            if (makeCopy)
                _items = new List<T>(list);
            else
                _items = list;

            _sync = syncRoot;
        }

        public int Count
        {
            get { lock (_sync) { return _items.Count; } }
        }

        protected IList<T> Items
        {
            get
            {
                return _items;
            }
        }

        public T this[int index]
        {
            get { lock (_sync) { return _items[index]; } }
        }

        public bool Contains(T value)
        {
            lock (_sync)
            {
                return _items.Contains(value);
            }
        }

        public void CopyTo(T[] array, int index)
        {
            lock (_sync)
            {
                _items.CopyTo(array, index);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (_sync)
            {
                return _items.GetEnumerator();
            }
        }

        public int IndexOf(T value)
        {
            lock (_sync)
            {
                return _items.IndexOf(value);
            }
        }

        private void ThrowReadOnly()
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRServiceModel.SFxCollectionReadOnly));
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
        }

        T IList<T>.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this.ThrowReadOnly();
            }
        }

        void ICollection<T>.Add(T value)
        {
            this.ThrowReadOnly();
        }

        void ICollection<T>.Clear()
        {
            this.ThrowReadOnly();
        }

        bool ICollection<T>.Remove(T value)
        {
            this.ThrowReadOnly();
            return false;
        }

        void IList<T>.Insert(int index, T value)
        {
            this.ThrowReadOnly();
        }

        void IList<T>.RemoveAt(int index)
        {
            this.ThrowReadOnly();
        }

        bool ICollection.IsSynchronized
        {
            get { return true; }
        }

        object ICollection.SyncRoot
        {
            get { return _sync; }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ICollection asCollection = _items as ICollection;
            if (asCollection == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRServiceModel.SFxCopyToRequiresICollection));

            lock (_sync)
            {
                asCollection.CopyTo(array, index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (_sync)
            {
                IEnumerable asEnumerable = _items as IEnumerable;
                if (asEnumerable != null)
                    return asEnumerable.GetEnumerator();
                else
                    return new EnumeratorAdapter(_items);
            }
        }

        bool IList.IsFixedSize
        {
            get { return true; }
        }

        bool IList.IsReadOnly
        {
            get { return true; }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this.ThrowReadOnly();
            }
        }

        int IList.Add(object value)
        {
            this.ThrowReadOnly();
            return 0;
        }

        void IList.Clear()
        {
            this.ThrowReadOnly();
        }

        bool IList.Contains(object value)
        {
            VerifyValueType(value);
            return this.Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            VerifyValueType(value);
            return this.IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            this.ThrowReadOnly();
        }

        void IList.Remove(object value)
        {
            this.ThrowReadOnly();
        }

        void IList.RemoveAt(int index)
        {
            this.ThrowReadOnly();
        }

        private static void VerifyValueType(object value)
        {
            if ((value is T) || (value == null && !typeof(T).IsValueType()))
                return;

            Type type = (value == null) ? typeof(Object) : value.GetType();
            string message = string.Format(SRServiceModel.SFxCollectionWrongType2, type.ToString(), typeof(T).ToString());
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(message));
        }

        internal sealed class EnumeratorAdapter : IEnumerator, IDisposable
        {
            private IList<T> _list;
            private IEnumerator<T> _e;

            public EnumeratorAdapter(IList<T> list)
            {
                _list = list;
                _e = list.GetEnumerator();
            }

            public object Current
            {
                get { return _e.Current; }
            }

            public bool MoveNext()
            {
                return _e.MoveNext();
            }

            public void Dispose()
            {
                _e.Dispose();
            }

            public void Reset()
            {
                _e = _list.GetEnumerator();
            }
        }
    }
}
