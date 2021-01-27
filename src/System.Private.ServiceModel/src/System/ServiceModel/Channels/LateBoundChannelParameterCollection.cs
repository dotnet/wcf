// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    internal class LateBoundChannelParameterCollection : ChannelParameterCollection
    {
        private IChannel _channel;

        protected override IChannel Channel
        {
            get { return _channel; }
        }

        internal void SetChannel(IChannel channel)
        {
            _channel = channel;
        }
    }
}
