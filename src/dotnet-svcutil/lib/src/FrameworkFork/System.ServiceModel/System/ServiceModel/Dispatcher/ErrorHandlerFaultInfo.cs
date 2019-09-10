// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
    internal struct ErrorHandlerFaultInfo
    {
        private Message _fault;   // if this is null, then we aren't interested in sending back a fault
        private bool _isConsideredUnhandled;  // if this is true, it means Fault is the 'internal server error' fault
        private string _defaultFaultAction;

        public ErrorHandlerFaultInfo(string defaultFaultAction)
        {
            _defaultFaultAction = defaultFaultAction;
            _fault = null;
            _isConsideredUnhandled = false;
        }

        public Message Fault
        {
            get { return _fault; }
            set { _fault = value; }
        }

        public string DefaultFaultAction
        {
            get { return _defaultFaultAction; }
            set { _defaultFaultAction = value; }
        }

        public bool IsConsideredUnhandled
        {
            get { return _isConsideredUnhandled; }
            set { _isConsideredUnhandled = value; }
        }
    }
}
