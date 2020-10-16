// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.ServiceModel.Channels
{
    internal class CommunicationObjectManager<ItemType> : LifetimeManager where ItemType : class, ICommunicationObject
    {
        private bool _inputClosed;
        private HashSet<ItemType> _table;

        public CommunicationObjectManager(object mutex)
            : base(mutex)
        {
            _table = new HashSet<ItemType>();
        }

        public void Add(ItemType item)
        {
            bool added = false;

            lock (this.ThisLock)
            {
                if (this.State == LifetimeState.Opened && !_inputClosed)
                {
                    if (_table.Contains(item))
                        return;

                    _table.Add(item);
                    base.IncrementBusyCountWithoutLock();
                    item.Closed += this.OnItemClosed;
                    added = true;
                }
            }

            if (!added)
            {
                item.Abort();
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(this.GetType().ToString()));
            }
        }

        public void CloseInput()
        {
            //Abort can reenter this call as a result of 
            //close timeout, Closing input twice is not a
            //FailFast case.
            _inputClosed = true;
        }

        public void DecrementActivityCount()
        {
            this.DecrementBusyCount();
        }

        public void IncrementActivityCount()
        {
            this.IncrementBusyCount();
        }

        private void OnItemClosed(object sender, EventArgs args)
        {
            this.Remove((ItemType)sender);
        }

        public void Remove(ItemType item)
        {
            lock (this.ThisLock)
            {
                if (!_table.Contains(item))
                    return;
                _table.Remove(item);
            }

            item.Closed -= this.OnItemClosed;
            base.DecrementBusyCount();
        }

        public ItemType[] ToArray()
        {
            lock (this.ThisLock)
            {
                int index = 0;
                ItemType[] items = new ItemType[_table.Count];
                foreach (ItemType item in _table)
                    items[index++] = item;

                return items;
            }
        }
    }
}
