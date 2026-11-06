// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace System.Collections.ObjectModel
{
    internal class FreezableCollection<T> : Collection<T>, ICollection<T>
    {
        public FreezableCollection()
            : base()
        {
        }

        public FreezableCollection(IList<T> list)
            : base(list)
        {
        }

        public bool IsFrozen { get; private set; }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return IsFrozen;
            }
        }

        public void Freeze()
        {
            IsFrozen = true;
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

        private void ThrowIfFrozen()
        {
            if (IsFrozen)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.ObjectIsReadOnly));
            }
        }
    }
}
