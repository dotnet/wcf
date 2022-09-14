// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Runtime;
using System.Threading;

namespace System.ServiceModel.Channels
{
    internal abstract class DeliveryStrategy<ItemType> : IDisposable
        where ItemType : class, IDisposable
    {
        public DeliveryStrategy(InputQueueChannel<ItemType> channel, int quota)
        {
            if (quota <= 0)
            {
                throw Fx.AssertAndThrow("Argument quota must be positive.");
            }

            Channel = channel;
            Quota = quota;
        }

        protected InputQueueChannel<ItemType> Channel { get; }

        public Action DequeueCallback { get; set; }

        public abstract int EnqueuedCount
        {
            get;
        }

        protected int Quota { get; }

        public abstract bool CanEnqueue(Int64 sequenceNumber);

        public virtual void Dispose()
        {
        }

        public abstract bool Enqueue(ItemType item, Int64 sequenceNumber);
    }

    internal class OrderedDeliveryStrategy<ItemType> : DeliveryStrategy<ItemType>
        where ItemType : class, IDisposable
    {
        private bool isEnqueueInOrder;
        private Dictionary<Int64, ItemType> items;
        private Action<object> onDispatchCallback;
        private Int64 windowStart;

        public OrderedDeliveryStrategy(
            InputQueueChannel<ItemType> channel,
            int quota,
            bool isEnqueueInOrder)
            : base(channel, quota)
        {
            this.isEnqueueInOrder = isEnqueueInOrder;
            items = new Dictionary<Int64, ItemType>();
            windowStart = 1;
        }

        public override int EnqueuedCount
        {
            get
            {
                return Channel.InternalPendingItems + items.Count;
            }
        }

        private Action<object> OnDispatchCallback
        {
            get
            {
                if (onDispatchCallback == null)
                {
                    onDispatchCallback = OnDispatch;
                }

                return onDispatchCallback;
            }
        }

        public override bool CanEnqueue(long sequenceNumber)
        {
            if (EnqueuedCount >= Quota)
            {
                return false;
            }

            if (isEnqueueInOrder && (sequenceNumber > windowStart))
            {
                return false;
            }

            return (Channel.InternalPendingItems + sequenceNumber - windowStart < Quota);
        }

        public override bool Enqueue(ItemType item, long sequenceNumber)
        {
            if (sequenceNumber > windowStart)
            {
                items.Add(sequenceNumber, item);
                return false;
            }

            windowStart++;

            while (items.ContainsKey(windowStart))
            {
                if (Channel.EnqueueWithoutDispatch(item, DequeueCallback))
                {
                    ActionItem.Schedule(OnDispatchCallback, null);
                }

                item = items[windowStart];
                items.Remove(windowStart);
                windowStart++;
            }

            return Channel.EnqueueWithoutDispatch(item, DequeueCallback);
        }

        private static void DisposeItems(Dictionary<Int64, ItemType>.Enumerator items)
        {
            if (items.MoveNext())
            {
                using (ItemType item = items.Current.Value)
                {
                    DisposeItems(items);
                }
            }
        }

        public override void Dispose()
        {
            DisposeItems(items.GetEnumerator());
            items.Clear();

            base.Dispose();
        }

        private void OnDispatch(object state)
        {
            Channel.Dispatch();
        }
    }

    internal class UnorderedDeliveryStrategy<ItemType> : DeliveryStrategy<ItemType>
        where ItemType : class, IDisposable
    {
        public UnorderedDeliveryStrategy(InputQueueChannel<ItemType> channel, int quota)
            : base(channel, quota)
        {
        }

        public override int EnqueuedCount
        {
            get
            {
                return Channel.InternalPendingItems;
            }
        }

        public override bool CanEnqueue(Int64 sequenceNumber)
        {
            return (EnqueuedCount < Quota);
        }

        public override bool Enqueue(ItemType item, long sequenceNumber)
        {
            return Channel.EnqueueWithoutDispatch(item, DequeueCallback);
        }
    }
}
