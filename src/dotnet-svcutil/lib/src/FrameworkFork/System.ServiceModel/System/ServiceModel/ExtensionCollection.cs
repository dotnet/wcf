// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.ServiceModel
{
    public sealed class ExtensionCollection<T> : SynchronizedCollection<IExtension<T>>, IExtensionCollection<T>
        where T : IExtensibleObject<T>
    {
        private T _owner;

        public ExtensionCollection(T owner)
        {
            if (owner == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("owner");

            _owner = owner;
        }

        public ExtensionCollection(T owner, object syncRoot)
            : base(syncRoot)
        {
            if (owner == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("owner");

            _owner = owner;
        }

        bool ICollection<IExtension<T>>.IsReadOnly
        {
            get { return false; }
        }

        protected override void ClearItems()
        {
            IExtension<T>[] array;

            lock (this.SyncRoot)
            {
                array = new IExtension<T>[this.Count];
                this.CopyTo(array, 0);
                base.ClearItems();

                foreach (IExtension<T> extension in array)
                {
                    extension.Detach(_owner);
                }
            }
        }

        public E Find<E>()
        {
            List<IExtension<T>> items = this.Items;

            lock (this.SyncRoot)
            {
                for (int i = this.Count - 1; i >= 0; i--)
                {
                    IExtension<T> item = items[i];
                    if (item is E)
                        return (E)item;
                }
            }

            return default(E);
        }

        public Collection<E> FindAll<E>()
        {
            Collection<E> result = new Collection<E>();
            List<IExtension<T>> items = this.Items;

            lock (this.SyncRoot)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    IExtension<T> item = items[i];
                    if (item is E)
                        result.Add((E)item);
                }
            }

            return result;
        }

        protected override void InsertItem(int index, IExtension<T> item)
        {
            if (item == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");

            lock (this.SyncRoot)
            {
                item.Attach(_owner);
                base.InsertItem(index, item);
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (this.SyncRoot)
            {
                this.Items[index].Detach(_owner);
                base.RemoveItem(index);
            }
        }

        protected override void SetItem(int index, IExtension<T> item)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.SFxCannotSetExtensionsByIndex));
        }
    }
}
