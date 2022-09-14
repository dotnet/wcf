// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel;

namespace System.Collections.Generic
{
    [Runtime.InteropServices.ComVisible(false)]
    public class SynchronizedReadOnlyCollection<T> : IList<T>, IList
    {
        private object _sync;

        public SynchronizedReadOnlyCollection()
        {
            Items = new List<T>();
            _sync = new Object();
        }

        public SynchronizedReadOnlyCollection(object syncRoot)
        {
            Items = new List<T>();
            _sync = syncRoot ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(syncRoot)));
        }

        public SynchronizedReadOnlyCollection(object syncRoot, IEnumerable<T> list)
        {
            if (list == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(list)));
            }

            Items = new List<T>(list);
            _sync = syncRoot ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(syncRoot)));
        }

        public SynchronizedReadOnlyCollection(object syncRoot, params T[] list)
        {
            if (list == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(list)));
            }

            Items = new List<T>(list.Length);
            for (int i = 0; i < list.Length; i++)
            {
                Items.Add(list[i]);
            }

            _sync = syncRoot ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(syncRoot)));
        }

        internal SynchronizedReadOnlyCollection(object syncRoot, List<T> list, bool makeCopy)
        {
            if (list == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(list)));
            }

            if (makeCopy)
            {
                Items = new List<T>(list);
            }
            else
            {
                Items = list;
            }

            _sync = syncRoot ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(syncRoot)));
        }

        public int Count
        {
            get { lock (_sync) { return Items.Count; } }
        }

        protected IList<T> Items { get; }

        public T this[int index]
        {
            get { lock (_sync) { return Items[index]; } }
        }

        public bool Contains(T value)
        {
            lock (_sync)
            {
                return Items.Contains(value);
            }
        }

        public void CopyTo(T[] array, int index)
        {
            lock (_sync)
            {
                Items.CopyTo(array, index);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (_sync)
            {
                return Items.GetEnumerator();
            }
        }

        public int IndexOf(T value)
        {
            lock (_sync)
            {
                return Items.IndexOf(value);
            }
        }

        private void ThrowReadOnly()
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.SFxCollectionReadOnly));
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
                ThrowReadOnly();
            }
        }

        void ICollection<T>.Add(T value)
        {
            ThrowReadOnly();
        }

        void ICollection<T>.Clear()
        {
            ThrowReadOnly();
        }

        bool ICollection<T>.Remove(T value)
        {
            ThrowReadOnly();
            return false;
        }

        void IList<T>.Insert(int index, T value)
        {
            ThrowReadOnly();
        }

        void IList<T>.RemoveAt(int index)
        {
            ThrowReadOnly();
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
            ICollection asCollection = Items as ICollection;
            if (asCollection == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.SFxCopyToRequiresICollection));
            }

            lock (_sync)
            {
                asCollection.CopyTo(array, index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (_sync)
            {
                IEnumerable asEnumerable = Items as IEnumerable;
                if (asEnumerable != null)
                {
                    return asEnumerable.GetEnumerator();
                }
                else
                {
                    return new EnumeratorAdapter(Items);
                }
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
                ThrowReadOnly();
            }
        }

        int IList.Add(object value)
        {
            ThrowReadOnly();
            return 0;
        }

        void IList.Clear()
        {
            ThrowReadOnly();
        }

        bool IList.Contains(object value)
        {
            VerifyValueType(value);
            return Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            VerifyValueType(value);
            return IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            ThrowReadOnly();
        }

        void IList.Remove(object value)
        {
            ThrowReadOnly();
        }

        void IList.RemoveAt(int index)
        {
            ThrowReadOnly();
        }

        private static void VerifyValueType(object value)
        {
            if ((value is T) || (value == null && !typeof(T).IsValueType()))
            {
                return;
            }

            Type type = (value == null) ? typeof(Object) : value.GetType();
            string message = SRP.Format(SRP.SFxCollectionWrongType2, type.ToString(), typeof(T).ToString());
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
