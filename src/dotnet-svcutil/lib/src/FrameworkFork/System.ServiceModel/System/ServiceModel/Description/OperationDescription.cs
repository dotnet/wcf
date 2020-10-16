// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Security;
using System.Reflection;

namespace System.ServiceModel.Description
{
    [DebuggerDisplay("Name={name}, IsInitiating={isInitiating}, IsTerminating={isTerminating}")]
    public class OperationDescription
    {
        internal const string SessionOpenedAction = Channels.WebSocketTransportSettings.ConnectionOpenedAction;

        private XmlName _name;
        private bool _isInitiating;
        private bool _isTerminating;
        private bool _isSessionOpenNotificationEnabled;
        private ContractDescription _declaringContract;
        private FaultDescriptionCollection _faults;
        private MessageDescriptionCollection _messages;
        private KeyedByTypeCollection<IOperationBehavior> _behaviors;
        private Collection<Type> _knownTypes;
        private MethodInfo _beginMethod;
        private MethodInfo _endMethod;
        private MethodInfo _syncMethod;
        private MethodInfo _taskMethod;
        private ProtectionLevel _protectionLevel;
        private bool _hasProtectionLevel;
        private bool _validateRpcWrapperName = true;
        private bool _hasNoDisposableParameters;

        public OperationDescription(string name, ContractDescription declaringContract)
        {
            if (name == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("name");
            }
            if (name.Length == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ArgumentOutOfRangeException("name", SRServiceModel.SFxOperationDescriptionNameCannotBeEmpty));
            }
            _name = new XmlName(name, true /*isEncoded*/);
            if (declaringContract == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("declaringContract");
            }
            _declaringContract = declaringContract;
            _isInitiating = true;
            _isTerminating = true;
            _faults = new FaultDescriptionCollection();
            _messages = new MessageDescriptionCollection();
            _behaviors = new KeyedByTypeCollection<IOperationBehavior>();
            _knownTypes = new Collection<Type>();
        }

        internal OperationDescription(string name, ContractDescription declaringContract, bool validateRpcWrapperName)
            : this(name, declaringContract)
        {
            _validateRpcWrapperName = validateRpcWrapperName;
        }

        public KeyedCollection<Type, IOperationBehavior> OperationBehaviors
        {
            get { return this.Behaviors; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public KeyedByTypeCollection<IOperationBehavior> Behaviors
        {
            get { return _behaviors; }
        }

        public ProtectionLevel ProtectionLevel
        {
            get { return _protectionLevel; }
            set
            {
                if (!(value == ProtectionLevel.None || value == ProtectionLevel.Sign || value == ProtectionLevel.EncryptAndSign))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                _protectionLevel = value;
                _hasProtectionLevel = true;
            }
        }

        public bool HasProtectionLevel
        {
            get { return _hasProtectionLevel; }
        }


        // Not serializable on purpose, metadata import/export cannot
        // produce it, only available when binding to runtime
        public MethodInfo TaskMethod
        {
            get { return _taskMethod; }
            set { _taskMethod = value; }
        }

        // Not serializable on purpose, metadata import/export cannot
        // produce it, only available when binding to runtime
        public MethodInfo SyncMethod
        {
            get { return _syncMethod; }
            set { _syncMethod = value; }
        }

        // Not serializable on purpose, metadata import/export cannot
        // produce it, only available when binding to runtime
        public MethodInfo BeginMethod
        {
            get { return _beginMethod; }
            set { _beginMethod = value; }
        }

        internal MethodInfo OperationMethod
        {
            get
            {
                if (this.SyncMethod == null)
                {
                    return this.TaskMethod ?? this.BeginMethod;
                }
                else
                {
                    return this.SyncMethod;
                }
            }
        }


        internal bool HasNoDisposableParameters
        {
            get { return _hasNoDisposableParameters; }
            set { _hasNoDisposableParameters = value; }
        }

        // Not serializable on purpose, metadata import/export cannot
        // produce it, only available when binding to runtime
        public MethodInfo EndMethod
        {
            get { return _endMethod; }
            set { _endMethod = value; }
        }

        public ContractDescription DeclaringContract
        {
            get { return _declaringContract; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("DeclaringContract");
                }
                else
                {
                    _declaringContract = value;
                }
            }
        }

        public FaultDescriptionCollection Faults
        {
            get { return _faults; }
        }

        public bool IsOneWay
        {
            get { return this.Messages.Count == 1; }
        }

        public bool IsInitiating
        {
            get { return _isInitiating; }
            set { _isInitiating = value; }
        }

        internal bool IsServerInitiated()
        {
            EnsureInvariants();
            return Messages[0].Direction == MessageDirection.Output;
        }

        public bool IsTerminating
        {
            get { return _isTerminating; }
            set { _isTerminating = value; }
        }

        public Collection<Type> KnownTypes
        {
            get { return _knownTypes; }
        }

        // Messages[0] is the 'request' (first of MEP), and for non-oneway MEPs, Messages[1] is the 'response' (second of MEP)
        public MessageDescriptionCollection Messages
        {
            get { return _messages; }
        }

        internal XmlName XmlName
        {
            get { return _name; }
        }

        internal string CodeName
        {
            get { return _name.DecodedName; }
        }

        public string Name
        {
            get { return _name.EncodedName; }
        }

        internal bool IsValidateRpcWrapperName { get { return _validateRpcWrapperName; } }

        internal Type TaskTResult
        {
            get;
            set;
        }

        internal bool HasOutputParameters
        {
            get
            {
                // For non-oneway operations, Messages[1] is the 'response'
                return (this.Messages.Count > 1) &&
                    (this.Messages[1].Body.Parts.Count > 0);
            }
        }

        internal bool IsSessionOpenNotificationEnabled
        {
            get { return _isSessionOpenNotificationEnabled; }
            set { _isSessionOpenNotificationEnabled = value; }
        }

        internal void EnsureInvariants()
        {
            if (this.Messages.Count != 1 && this.Messages.Count != 2)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new System.InvalidOperationException(string.Format(SRServiceModel.SFxOperationMustHaveOneOrTwoMessages, this.Name)));
            }
        }

        internal void ResetProtectionLevel()
        {
            _protectionLevel = ProtectionLevel.None;
            _hasProtectionLevel = false;
        }
    }
}
