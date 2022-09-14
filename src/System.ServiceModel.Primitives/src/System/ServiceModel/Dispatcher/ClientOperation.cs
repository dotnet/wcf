// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace System.ServiceModel.Dispatcher
{
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "Compat is an accepted abbreviation")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ClientOperationCompatBase
    {
        internal ClientOperationCompatBase() { }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
        public IList<IParameterInspector> ParameterInspectors
        {
            get
            {
                return parameterInspectors;
            }
        }
        internal SynchronizedCollection<IParameterInspector> parameterInspectors;
    }

    public sealed class ClientOperation : ClientOperationCompatBase
    {
        private bool _serializeRequest;
        private bool _deserializeReply;
        private IClientFaultFormatter _faultFormatter;
        private bool _isInitiating = true;
        private bool _isOneWay;
        private bool _isTerminating;
        private bool _isSessionOpenNotificationEnabled;
        private ClientRuntime _parent;
        private MethodInfo _beginMethod;
        private MethodInfo _endMethod;
        private MethodInfo _syncMethod;
        private MethodInfo _taskMethod;
        private Type _taskTResult;

        public ClientOperation(ClientRuntime parent, string name, string action)
            : this(parent, name, action, null)
        {
        }

        public ClientOperation(ClientRuntime parent, string name, string action, string replyAction)
        {
            _parent = parent ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(parent));
            Name = name ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(name));
            Action = action;
            ReplyAction = replyAction;

            FaultContractInfos = parent.NewBehaviorCollection<FaultContractInfo>();
            parameterInspectors = parent.NewBehaviorCollection<IParameterInspector>();
        }

        public string Action { get; }

        public SynchronizedCollection<FaultContractInfo> FaultContractInfos { get; }

        public MethodInfo BeginMethod
        {
            get { return _beginMethod; }
            set
            {
                lock (_parent.ThisLock)
                {
                    _parent.InvalidateRuntime();
                    _beginMethod = value;
                }
            }
        }

        public MethodInfo EndMethod
        {
            get { return _endMethod; }
            set
            {
                lock (_parent.ThisLock)
                {
                    _parent.InvalidateRuntime();
                    _endMethod = value;
                }
            }
        }

        public MethodInfo SyncMethod
        {
            get { return _syncMethod; }
            set
            {
                lock (_parent.ThisLock)
                {
                    _parent.InvalidateRuntime();
                    _syncMethod = value;
                }
            }
        }

        public IClientMessageFormatter Formatter
        {
            get { return InternalFormatter; }
            set
            {
                lock (_parent.ThisLock)
                {
                    _parent.InvalidateRuntime();
                    InternalFormatter = value;
                }
            }
        }

        internal IClientFaultFormatter FaultFormatter
        {
            get
            {
                if (_faultFormatter == null)
                {
                    _faultFormatter = new DataContractSerializerFaultFormatter(FaultContractInfos);
                }
                return _faultFormatter;
            }
            set
            {
                lock (_parent.ThisLock)
                {
                    _parent.InvalidateRuntime();
                    _faultFormatter = value;
                    IsFaultFormatterSetExplicit = true;
                }
            }
        }

        internal bool IsFaultFormatterSetExplicit { get; private set; } = false;

        internal IClientMessageFormatter InternalFormatter { get; set; }

        public bool IsInitiating
        {
            get { return _isInitiating; }
            set
            {
                lock (_parent.ThisLock)
                {
                    _parent.InvalidateRuntime();
                    _isInitiating = value;
                }
            }
        }

        public bool IsOneWay
        {
            get { return _isOneWay; }
            set
            {
                lock (_parent.ThisLock)
                {
                    _parent.InvalidateRuntime();
                    _isOneWay = value;
                }
            }
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

        public string Name { get; }

        public ICollection<IParameterInspector> ClientParameterInspectors
        {
            get { return ParameterInspectors; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new SynchronizedCollection<IParameterInspector> ParameterInspectors
        {
            get { return parameterInspectors; }
        }

        public ClientRuntime Parent
        {
            get { return _parent; }
        }

        public string ReplyAction { get; }

        public bool SerializeRequest
        {
            get { return _serializeRequest; }
            set
            {
                lock (_parent.ThisLock)
                {
                    _parent.InvalidateRuntime();
                    _serializeRequest = value;
                }
            }
        }

        public bool DeserializeReply
        {
            get { return _deserializeReply; }
            set
            {
                lock (_parent.ThisLock)
                {
                    _parent.InvalidateRuntime();
                    _deserializeReply = value;
                }
            }
        }

        public MethodInfo TaskMethod
        {
            get { return _taskMethod; }
            set
            {
                lock (_parent.ThisLock)
                {
                    _parent.InvalidateRuntime();
                    _taskMethod = value;
                }
            }
        }

        public Type TaskTResult
        {
            get { return _taskTResult; }
            set
            {
                lock (_parent.ThisLock)
                {
                    _parent.InvalidateRuntime();
                    _taskTResult = value;
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
    }
}
