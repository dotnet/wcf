// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Dispatcher;

namespace System.ServiceModel.Channels
{
    internal class InternalDuplexBindingElement
    {
    }

    internal class LocalAddressProvider
    {
        private int _priority;

        public LocalAddressProvider(EndpointAddress localAddress, MessageFilter filter)
        {
            LocalAddress = localAddress ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(localAddress));
            Filter = filter ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(filter));

            if (localAddress.Headers.FindHeader(XD.UtilityDictionary.UniqueEndpointHeaderName.Value,
                    XD.UtilityDictionary.UniqueEndpointHeaderNamespace.Value) == null)
            {
                _priority = Int32.MaxValue - 1;
            }
            else
            {
                _priority = Int32.MaxValue;
            }
        }

        public EndpointAddress LocalAddress { get; }

        public MessageFilter Filter { get; }

        public int Priority
        {
            get { return _priority; }
        }
    }
}
