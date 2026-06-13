// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Collections.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;

    internal class FreezableCollection<T> : Collection<T>, ICollection<T>
    {
        bool frozen;

        public FreezableCollection()
            : base()
        {
        }

        public FreezableCollection(IList<T> list)
            : base(list)
        {
        }

        public bool IsFrozen
        {
            get
            {
                return this.frozen;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return this.frozen;
            }
        }

        public void Freeze()
        {
            this.frozen = true;
        }

        protected override void ClearItems()
        {
            ThrowIfFrozen();
            base.ClearItems();
        }

        protected override void InsertItem(int index, T item)
        {
            ThrowIfFrozen();
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            ThrowIfFrozen();
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, T item)
        {
            ThrowIfFrozen();
            base.SetItem(index, item);
        }

        void ThrowIfFrozen()
        {
            if (this.frozen)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.ObjectIsReadOnly));
            }
        }
    }
}
