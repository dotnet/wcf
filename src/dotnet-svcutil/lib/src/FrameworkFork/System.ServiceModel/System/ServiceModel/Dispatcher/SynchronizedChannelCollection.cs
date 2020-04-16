// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
    internal class SynchronizedChannelCollection<TChannel> : SynchronizedCollection<TChannel>
        where TChannel : IChannel
    {
        private EventHandler _onChannelClosed;
        private EventHandler _onChannelFaulted;

        internal SynchronizedChannelCollection(object syncRoot)
            : base(syncRoot)
        {
            _onChannelClosed = new EventHandler(OnChannelClosed);
            _onChannelFaulted = new EventHandler(OnChannelFaulted);
        }

        private void AddingChannel(TChannel channel)
        {
            channel.Faulted += _onChannelFaulted;
            channel.Closed += _onChannelClosed;
        }

        private void RemovingChannel(TChannel channel)
        {
            channel.Faulted -= _onChannelFaulted;
            channel.Closed -= _onChannelClosed;
        }

        private void OnChannelClosed(object sender, EventArgs args)
        {
            TChannel channel = (TChannel)sender;
            this.Remove(channel);
        }

        private void OnChannelFaulted(object sender, EventArgs args)
        {
            TChannel channel = (TChannel)sender;
            this.Remove(channel);
        }

        protected override void ClearItems()
        {
            List<TChannel> items = this.Items;

            for (int i = 0; i < items.Count; i++)
            {
                this.RemovingChannel(items[i]);
            }

            base.ClearItems();
        }

        protected override void InsertItem(int index, TChannel item)
        {
            this.AddingChannel(item);
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            TChannel oldItem = this.Items[index];

            base.RemoveItem(index);
            this.RemovingChannel(oldItem);
        }

        protected override void SetItem(int index, TChannel item)
        {
            TChannel oldItem = this.Items[index];

            this.AddingChannel(item);
            base.SetItem(index, item);
            this.RemovingChannel(oldItem);
        }
    }
}
