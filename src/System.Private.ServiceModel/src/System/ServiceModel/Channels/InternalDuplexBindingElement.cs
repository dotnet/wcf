// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal class InternalDuplexBindingElement
    {
    }

    internal class LocalAddressProvider
    {
        private EndpointAddress _localAddress;
        private MessageFilter _filter;
        private int _priority;

        public LocalAddressProvider(EndpointAddress localAddress, MessageFilter filter)
        {
            if (localAddress == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("localAddress");
            }
            if (filter == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("filter");
            }
            _localAddress = localAddress;
            _filter = filter;

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

        public EndpointAddress LocalAddress
        {
            get { return _localAddress; }
        }

        public MessageFilter Filter
        {
            get { return _filter; }
        }

        public int Priority
        {
            get { return _priority; }
        }
    }
}
