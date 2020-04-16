// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
//--------------------------------------------------------------------------------------------
// C`opyright(c) 2015 Microsoft Corporation
//--------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    /// <summary>
    /// Represents an option that can be specified multiple times in the command line.
    /// </summary>
    internal class ListValue<TValue> : ICollection, IList<TValue>, IList
    {
        private List<TValue> InnerList { get; set; } = new List<TValue>();
        public ListValueOption<TValue> Owner { get; set; }

        public event EventHandler<ListOptionEventArgs> Inserting;
        public event EventHandler<ListOptionEventArgs> Inserted;
        public event EventHandler<ListOptionEventArgs> Removing;
        public event EventHandler<ListOptionEventArgs> Removed;

        public void Insert(int index, TValue value)
        {
            if (this.Contains(value))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorOptionDuplicateValueFormat, this.Owner.Name, value));
            }
            var e = new ListOptionEventArgs(value);
            this.Inserting?.Invoke(this, e);
            this.InnerList.Insert(index, CastValue(e.Value));
            this.Inserted?.Invoke(this, e);
        }
        public void RemoveAt(int index)
        {
            var e = new ListOptionEventArgs(null);
            this.Removing?.Invoke(this, e);
            this.InnerList.RemoveAt(index);
            this.Removed?.Invoke(this, e);
        }

        public void ReplaceAt(int idx, TValue value)
        {
            RemoveAt(idx);
            Insert(idx, value);
        }

        private TValue CastValue(object value)
        {
            OptionValueParser.ThrowInvalidValueIf(value == null || value.GetType() != typeof(TValue), value, this.Owner);
            return (TValue)value;
        }

        #region interface methods
        public bool IsFixedSize { get { return false; } }

        public bool IsReadOnly { get { return false; } }

        bool IList.IsReadOnly { get { return this.IsReadOnly; } }

        public int Count { get { return this.InnerList.Count; } }

        int ICollection.Count { get { return this.Count; } }

        public bool IsSynchronized { get { return false; } }

        bool ICollection.IsSynchronized { get { return this.IsSynchronized; } }

        public object SyncRoot { get { return this.InnerList; } }

        object ICollection.SyncRoot { get { return this.SyncRoot; } }

        bool IList.IsFixedSize { get { return false; } }

        public TValue this[int index]
        {
            get { return this.InnerList[index]; }
            set { ReplaceAt(index, value); }
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = CastValue(value); }
        }

        int IList.Add(object value)
        {
            this.Add(OptionValueParser.ParseValue<TValue>(value, this.Owner));
            return this.Count - 1;
        }

        public int Add(TValue value)
        {
            Insert(this.Count, value);
            return this.Count - 1;
        }

        public void AddRange(IEnumerable<TValue> collection)
        {
            foreach (var item in collection)
            {
                this.Add(item);
            }
        }

        void ICollection<TValue>.Add(TValue value)
        {
            this.Add(value);
        }

        void IList.Insert(int index, object value)
        {
            this.Insert(index, CastValue(value));
        }

        public void Clear()
        {
            for (int idx = this.InnerList.Count - 1; idx >= 0; idx--)
            {
                RemoveAt(idx);
            }
        }

        void IList.Clear()
        {
            this.Clear();
        }

        public bool Contains(TValue value)
        {
            return this.InnerList.Contains(value);
        }

        bool IList.Contains(object value)
        {
            return this.Contains(CastValue(value));
        }

        public int IndexOf(TValue value)
        {
            return this.InnerList.IndexOf(value);
        }

        int IList.IndexOf(object value)
        {
            return this.IndexOf(CastValue(value));
        }

        void IList.Remove(object value)
        {
            Remove(CastValue(value));
        }

        void IList.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        public bool Remove(TValue value)
        {
            int idx = this.IndexOf(value);
            if (idx != -1)
            {
                RemoveAt(idx);
                return true;
            }
            return false;
        }

        public void Sort()
        {
            this.InnerList.Sort();
        }

        public void CopyTo(Array array, int index)
        {
            this.CopyTo(array, index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.CopyTo(array, index);
        }

        public void CopyTo(TValue[] array, int index)
        {
            this.InnerList.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return this.InnerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.InnerList.GetEnumerator();
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return this.InnerList.GetEnumerator();
        }
        #endregion

        public override string ToString()
        {
            var value = this.InnerList.Count > 0 ? this.InnerList.Select(i => $"\"{i}\"").Aggregate((str, s) => $"{str}, {s}") : string.Empty;
            return value;
        }
    }

    public class ListOptionEventArgs : EventArgs
    {
        public object Value { get; set; }
        public ListOptionEventArgs(object value)
        {
            this.Value = value;
        }
    }
}
