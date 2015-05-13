// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace System.ServiceModel.Dispatcher
{
    public sealed class DispatchOperation
    {
        private string _action;
        private SynchronizedCollection<FaultContractInfo> _faultContractInfos;
        private bool _isTerminating;
        private bool _isSessionOpenNotificationEnabled;
        private string _name;
        private DispatchRuntime _parent;
        private string _replyAction;
        private bool _deserializeRequest = true;
        private bool _serializeReply = true;
        private bool _isOneWay;
        private bool _autoDisposeParameters = true;
        private bool _hasNoDisposableParameters;
        private bool _isInsideTransactedReceiveScope = false;

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

        internal bool HasNoDisposableParameters
        {
            get { return _hasNoDisposableParameters; }
            set { _hasNoDisposableParameters = value; }
        }

        public bool IsTerminating
        {
            get { return _isTerminating; }
            set
            {
                lock (_parent.ThisLock)
                {
                    _parent.InvalidateRuntime();
                    _isTerminating = value;
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

        public DispatchRuntime Parent
        {
            get { return _parent; }
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

        public bool IsInsideTransactedReceiveScope
        {
            get { return _isInsideTransactedReceiveScope; }
            set
            {
                lock (_parent.ThisLock)
                {
                    _parent.InvalidateRuntime();
                    _isInsideTransactedReceiveScope = value;
                }
            }
        }
    }
}
