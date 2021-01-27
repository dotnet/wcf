// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.ServiceModel.Dispatcher
{
    public sealed class DispatchOperation
    {
        private readonly string _action;
        private readonly SynchronizedCollection<FaultContractInfo> _faultContractInfos;
        private IDispatchMessageFormatter _formatter;
        private IDispatchFaultFormatter _faultFormatter;
        private IOperationInvoker _invoker;
        private bool _isSessionOpenNotificationEnabled;
        private readonly string _name;
        private readonly SynchronizedCollection<IParameterInspector> _parameterInspectors;
        private readonly DispatchRuntime _parent;
        private readonly string _replyAction;
        private bool _deserializeRequest = true;
        private bool _serializeReply = true;
        private readonly bool _isOneWay;
        private bool _autoDisposeParameters = true;
        private bool _hasNoDisposableParameters;
        private bool _isFaultFormatterSetExplicit;

        public DispatchOperation(DispatchRuntime parent, string name, string action)
        {
            if (parent == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parent");
            if (name == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("name");

            _parent = parent;
            _name = name;
            _action = action;

            _faultContractInfos = parent.NewBehaviorCollection<FaultContractInfo>();
            _parameterInspectors = parent.NewBehaviorCollection<IParameterInspector>();
            _isOneWay = true;
        }

        public DispatchOperation(DispatchRuntime parent, string name, string action, string replyAction)
            : this(parent, name, action)
        {
            _replyAction = replyAction;
            _isOneWay = false;
        }

        public bool IsOneWay
        {
            get { return _isOneWay; }
        }

        public string Action
        {
            get { return _action; }
        }

        public SynchronizedCollection<FaultContractInfo> FaultContractInfos
        {
            get { return _faultContractInfos; }
        }

        public bool AutoDisposeParameters
        {
            get { return _autoDisposeParameters; }

            set
            {
                lock (_parent.ThisLock)
                {
                    _parent.InvalidateRuntime();
                    _autoDisposeParameters = value;
                }
            }
        }

        internal IDispatchMessageFormatter Formatter
        {
            get { return _formatter; }
            set
            {
                lock (_parent.ThisLock)
                {
                    _parent.InvalidateRuntime();
                    _formatter = value;
                }
            }
        }

        internal IDispatchFaultFormatter FaultFormatter
        {
            get
            {
                if (_faultFormatter == null)
                {
                    _faultFormatter = new DataContractSerializerFaultFormatter(_faultContractInfos);
                }
                return _faultFormatter;
            }
            set
            {
                lock (_parent.ThisLock)
                {
                    _parent.InvalidateRuntime();
                    _faultFormatter = value;
                    _isFaultFormatterSetExplicit = true;
                }
            }
        }

        internal bool IsFaultFormatterSetExplicit
        {
            get { return _isFaultFormatterSetExplicit; }
        }

        internal bool HasNoDisposableParameters
        {
            get { return _hasNoDisposableParameters; }
            set { _hasNoDisposableParameters = value; }
        }

        internal IDispatchMessageFormatter InternalFormatter
        {
            get { return _formatter; }
            set { _formatter = value; }
        }

        internal IOperationInvoker InternalInvoker
        {
            get { return _invoker; }
            set { _invoker = value; }
        }

        public IOperationInvoker Invoker
        {
            get { return _invoker; }
            set
            {
                lock (_parent.ThisLock)
                {
                    _parent.InvalidateRuntime();
                    _invoker = value;
                }
            }
        }

        internal bool IsSessionOpenNotificationEnabled
        {
            get { return _isSessionOpenNotificationEnabled; }
            set
            {
                lock (_parent.ThisLock)
                {
                    _parent.InvalidateRuntime();
                    _isSessionOpenNotificationEnabled = value;
                }
            }
        }

        public string Name
        {
            get { return _name; }
        }

        public SynchronizedCollection<IParameterInspector> ParameterInspectors
        {
            get { return _parameterInspectors; }
        }

        public DispatchRuntime Parent
        {
            get { return _parent; }
        }

        public string ReplyAction
        {
            get { return _replyAction; }
        }

        public bool DeserializeRequest
        {
            get { return _deserializeRequest; }
            set
            {
                lock (_parent.ThisLock)
                {
                    _parent.InvalidateRuntime();
                    _deserializeRequest = value;
                }
            }
        }

        public bool SerializeReply
        {
            get { return _serializeReply; }
            set
            {
                lock (_parent.ThisLock)
                {
                    _parent.InvalidateRuntime();
                    _serializeReply = value;
                }
            }
        }
    }
}
