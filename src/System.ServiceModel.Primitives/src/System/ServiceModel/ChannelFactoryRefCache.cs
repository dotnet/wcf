// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Runtime;

namespace System.ServiceModel
{
    internal sealed class ChannelFactoryRef<TChannel> where TChannel : class
    {
        private ChannelFactory<TChannel> _channelFactory;
        private int _refCount = 1;

        public ChannelFactoryRef(ChannelFactory<TChannel> channelFactory)
        {
            _channelFactory = channelFactory;
        }

        public void AddRef()
        {
            _refCount++;
        }

        // The caller should call Close/Abort when the return value is true.
        public bool Release()
        {
            --_refCount;
            Fx.Assert(_refCount >= 0, "RefCount should not be less than zero.");

            if (_refCount == 0)
            {
                return true;
            }

            return false;
        }

        public void Close(TimeSpan timeout)
        {
            _channelFactory.Close(timeout);
        }

        public void Abort()
        {
            _channelFactory.Abort();
        }

        public ChannelFactory<TChannel> ChannelFactory
        {
            get
            {
                return _channelFactory;
            }
        }
    }

    internal class ChannelFactoryRefCache<TChannel> : MruCache<EndpointTrait<TChannel>, ChannelFactoryRef<TChannel>>
       where TChannel : class
    {
        private static EndpointTraitComparer DefaultEndpointTraitComparer = new EndpointTraitComparer();
        private readonly int _watermark;

        private class EndpointTraitComparer : IEqualityComparer<EndpointTrait<TChannel>>
        {
            public bool Equals(EndpointTrait<TChannel> x, EndpointTrait<TChannel> y)
            {
                if (x != null)
                {
                    if (y != null)
                        return x.Equals(y);

                    return false;
                }

                if (y != null)
                    return false;

                return true;
            }

            public int GetHashCode(EndpointTrait<TChannel> obj)
            {
                if (obj == null)
                    return 0;

                return obj.GetHashCode();
            }
        }

        public ChannelFactoryRefCache(int watermark)
            : base(watermark * 4 / 5, watermark, DefaultEndpointTraitComparer)
        {
            _watermark = watermark;
        }

        protected override void OnSingleItemRemoved(ChannelFactoryRef<TChannel> item)
        {
            // Remove from cache.
            if (item.Release())
            {
                item.Abort();
            }

            if (WcfEventSource.Instance.ClientBaseCachedChannelFactoryCountIsEnabled())
            {
                WcfEventSource.Instance.ClientBaseCachedChannelFactoryCount(Count, _watermark, this);
            }
        }

        protected override void OnItemAgedOutOfCache(ChannelFactoryRef<TChannel> item)
        {
            if (WcfEventSource.Instance.ClientBaseChannelFactoryAgedOutofCacheIsEnabled())
            {
                WcfEventSource.Instance.ClientBaseChannelFactoryAgedOutofCache(_watermark, this);
            }
        }
    }
}
