// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;
using System.Runtime.Diagnostics;

namespace System.ServiceModel.Dispatcher
{
    internal struct ProxyRpc
    {
        internal readonly string Action;
        internal ServiceModelActivity Activity;
        internal Guid ActivityId;
        internal readonly ServiceChannel Channel;
        internal object[] Correlation;
        internal readonly object[] InputParameters;
        internal readonly ProxyOperationRuntime Operation;
        internal object[] OutputParameters;
        internal Message Request;
        internal Message Reply;
        internal object ReturnValue;
        internal MessageVersion MessageVersion;
        internal readonly TimeoutHelper TimeoutHelper;
        private EventTraceActivity _eventTraceActivity;

        internal ProxyRpc(ServiceChannel channel, ProxyOperationRuntime operation, string action, object[] inputs, TimeSpan timeout)
        {
            Action = action;
            Activity = null;
            _eventTraceActivity = null;
            Channel = channel;
            Correlation = EmptyArray<object>.Allocate(operation.Parent.CorrelationCount);
            InputParameters = inputs;
            Operation = operation;
            OutputParameters = null;
            Request = null;
            Reply = null;
            ActivityId = Guid.Empty;
            ReturnValue = null;
            MessageVersion = channel.MessageVersion;
            TimeoutHelper = new TimeoutHelper(timeout);
        }

        internal EventTraceActivity EventTraceActivity
        {
            get
            {
                if (_eventTraceActivity == null)
                {
                    _eventTraceActivity = new EventTraceActivity();
                }
                return _eventTraceActivity;
            }

            set
            {
                _eventTraceActivity = value;
            }
        }
    }
}
