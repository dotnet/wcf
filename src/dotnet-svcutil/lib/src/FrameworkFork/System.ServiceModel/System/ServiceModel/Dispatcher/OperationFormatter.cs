// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Description;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Reflection;
using Microsoft.Xml;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Channels;
using System.Runtime.Diagnostics;
using System.Runtime;
using System.Threading.Tasks;

namespace System.ServiceModel.Dispatcher
{
    internal abstract class OperationFormatter : IClientMessageFormatter, IDispatchMessageFormatter
    {
        private MessageDescription _replyDescription;
        private MessageDescription _requestDescription;
        private XmlDictionaryString _action;
        private XmlDictionaryString _replyAction;
        protected StreamFormatter requestStreamFormatter, replyStreamFormatter;
        private XmlDictionary _dictionary;
        private string _operationName;

        public OperationFormatter(OperationDescription description, bool isRpc, bool isEncoded)
        {
            Validate(description, isRpc, isEncoded);
            _requestDescription = description.Messages[0];
            if (description.Messages.Count == 2)
                _replyDescription = description.Messages[1];

            int stringCount = 3 + _requestDescription.Body.Parts.Count;
            if (_replyDescription != null)
                stringCount += 2 + _replyDescription.Body.Parts.Count;

            _dictionary = new XmlDictionary(stringCount * 2);
            GetActions(description, _dictionary, out _action, out _replyAction);
            _operationName = description.Name;
            requestStreamFormatter = StreamFormatter.Create(_requestDescription, _operationName, true/*isRequest*/);
            if (_replyDescription != null)
                replyStreamFormatter = StreamFormatter.Create(_replyDescription, _operationName, false/*isResponse*/);
        }

        protected abstract void AddHeadersToMessage(Message message, MessageDescription messageDescription, object[] parameters, bool isRequest);
        protected abstract void SerializeBody(XmlDictionaryWriter writer, MessageVersion version, string action, MessageDescription messageDescription, object returnValue, object[] parameters, bool isRequest);
        protected virtual Task SerializeBodyAsync(XmlDictionaryWriter writer, MessageVersion version, string action, MessageDescription messageDescription, object returnValue, object[] parameters, bool isRequest)
        {
            SerializeBody(writer, version, action, messageDescription, returnValue, parameters, isRequest);
            return Task.CompletedTask;
        }

        protected abstract void GetHeadersFromMessage(Message message, MessageDescription messageDescription, object[] parameters, bool isRequest);
        protected abstract object DeserializeBody(XmlDictionaryReader reader, MessageVersion version, string action, MessageDescription messageDescription, object[] parameters, bool isRequest);

        protected virtual void WriteBodyAttributes(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
        }

        internal string RequestAction
        {
            get
            {
                if (_action != null)
                    return _action.Value;
                return null;
            }
        }
        internal string ReplyAction
        {
            get
            {
                if (_replyAction != null)
                    return _replyAction.Value;
                return null;
            }
        }

        protected XmlDictionary Dictionary
        {
            get { return _dictionary; }
        }

        protected string OperationName
        {
            get { return _operationName; }
        }

        protected MessageDescription ReplyDescription
        {
            get { return _replyDescription; }
        }

        protected MessageDescription RequestDescription
        {
            get { return _requestDescription; }
        }

        protected XmlDictionaryString AddToDictionary(string s)
        {
            return AddToDictionary(_dictionary, s);
        }

        public object DeserializeReply(Message message, object[] parameters)
        {
            if (message == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");

            if (parameters == null)
                throw TraceUtility.ThrowHelperError(new ArgumentNullException("parameters"), message);

            try
            {
                object result = null;
                if (_replyDescription.IsTypedMessage)
                {
                    object typeMessageInstance = CreateTypedMessageInstance(_replyDescription.MessageType);
                    TypedMessageParts typedMessageParts = new TypedMessageParts(typeMessageInstance, _replyDescription);
                    object[] parts = new object[typedMessageParts.Count];

                    GetPropertiesFromMessage(message, _replyDescription, parts);
                    GetHeadersFromMessage(message, _replyDescription, parts, false/*isRequest*/);
                    DeserializeBodyContents(message, parts, false/*isRequest*/);

                    // copy values into the actual field/properties
                    typedMessageParts.SetTypedMessageParts(parts);

                    result = typeMessageInstance;
                }
                else
                {
                    GetPropertiesFromMessage(message, _replyDescription, parameters);
                    GetHeadersFromMessage(message, _replyDescription, parameters, false/*isRequest*/);
                    result = DeserializeBodyContents(message, parameters, false/*isRequest*/);
                }
                return result;
            }
            catch (XmlException xe)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(
                    string.Format(SRServiceModel.SFxErrorDeserializingReplyBodyMore, _operationName, xe.Message), xe));
            }
            catch (FormatException fe)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(
                    string.Format(SRServiceModel.SFxErrorDeserializingReplyBodyMore, _operationName, fe.Message), fe));
            }
            catch (SerializationException se)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(
                    string.Format(SRServiceModel.SFxErrorDeserializingReplyBodyMore, _operationName, se.Message), se));
            }
        }

        private static object CreateTypedMessageInstance(Type messageContractType)
        {
            try
            {
                object typeMessageInstance = Activator.CreateInstance(messageContractType);
                return typeMessageInstance;
            }
            catch (MissingMethodException mme)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxMessageContractRequiresDefaultConstructor, messageContractType.FullName), mme));
            }
        }

        public void DeserializeRequest(Message message, object[] parameters)
        {
            if (message == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");

            if (parameters == null)
                throw TraceUtility.ThrowHelperError(new ArgumentNullException("parameters"), message);

            try
            {
                if (_requestDescription.IsTypedMessage)
                {
                    object typeMessageInstance = CreateTypedMessageInstance(_requestDescription.MessageType);
                    TypedMessageParts typedMessageParts = new TypedMessageParts(typeMessageInstance, _requestDescription);
                    object[] parts = new object[typedMessageParts.Count];

                    GetPropertiesFromMessage(message, _requestDescription, parts);
                    GetHeadersFromMessage(message, _requestDescription, parts, true/*isRequest*/);
                    DeserializeBodyContents(message, parts, true/*isRequest*/);

                    // copy values into the actual field/properties
                    typedMessageParts.SetTypedMessageParts(parts);

                    parameters[0] = typeMessageInstance;
                }
                else
                {
                    GetPropertiesFromMessage(message, _requestDescription, parameters);
                    GetHeadersFromMessage(message, _requestDescription, parameters, true/*isRequest*/);
                    DeserializeBodyContents(message, parameters, true/*isRequest*/);
                }
            }
            catch (XmlException xe)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    OperationFormatter.CreateDeserializationFailedFault(
                        string.Format(SRServiceModel.SFxErrorDeserializingRequestBodyMore, _operationName, xe.Message),
                        xe));
            }
            catch (FormatException fe)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    OperationFormatter.CreateDeserializationFailedFault(
                        string.Format(SRServiceModel.SFxErrorDeserializingRequestBodyMore, _operationName, fe.Message),
                        fe));
            }
            catch (SerializationException se)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(
                    string.Format(SRServiceModel.SFxErrorDeserializingRequestBodyMore, _operationName, se.Message),
                    se));
            }
        }

        private object DeserializeBodyContents(Message message, object[] parameters, bool isRequest)
        {
            MessageDescription messageDescription;
            StreamFormatter streamFormatter;

            SetupStreamAndMessageDescription(isRequest, out streamFormatter, out messageDescription);

            if (streamFormatter != null)
            {
                object retVal = null;
                streamFormatter.Deserialize(parameters, ref retVal, message);
                return retVal;
            }

            if (message.IsEmpty)
            {
                return null;
            }
            else
            {
                XmlDictionaryReader reader = message.GetReaderAtBodyContents();
                using (reader)
                {
                    object body = DeserializeBody(reader, message.Version, RequestAction, messageDescription, parameters, isRequest);
                    message.ReadFromBodyContentsToEnd(reader);
                    return body;
                }
            }
        }

        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            object[] parts = null;

            if (messageVersion == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("messageVersion");

            if (parameters == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");
            if (_requestDescription.IsTypedMessage)
            {
                TypedMessageParts typedMessageParts = new TypedMessageParts(parameters[0], _requestDescription);

                // copy values from the actual field/properties
                parts = new object[typedMessageParts.Count];
                typedMessageParts.GetTypedMessageParts(parts);
            }
            else
            {
                parts = parameters;
            }
            Message msg = new OperationFormatterMessage(this, messageVersion,
                _action == null ? null : ActionHeader.Create(_action, messageVersion.Addressing),
                parts, null, true/*isRequest*/);
            AddPropertiesToMessage(msg, _requestDescription, parts);
            AddHeadersToMessage(msg, _requestDescription, parts, true /*isRequest*/);

            return msg;
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            object[] parts = null;
            object resultPart = null;

            if (messageVersion == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("messageVersion");

            if (parameters == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");

            if (_replyDescription.IsTypedMessage)
            {
                // If the response is a typed message then it must 
                // be the response (as opposed to an out param).  We will
                // serialize the response in the exact same way that we
                // would serialize a bunch of outs (with no return value).

                TypedMessageParts typedMessageParts = new TypedMessageParts(result, _replyDescription);

                // make a copy of the list so that we have the actual values of the field/properties
                parts = new object[typedMessageParts.Count];
                typedMessageParts.GetTypedMessageParts(parts);
            }
            else
            {
                parts = parameters;
                resultPart = result;
            }

            Message msg = new OperationFormatterMessage(this, messageVersion,
                _replyAction == null ? null : ActionHeader.Create(_replyAction, messageVersion.Addressing),
                parts, resultPart, false/*isRequest*/);
            AddPropertiesToMessage(msg, _replyDescription, parts);
            AddHeadersToMessage(msg, _replyDescription, parts, false /*isRequest*/);
            return msg;
        }

        private void SetupStreamAndMessageDescription(bool isRequest, out StreamFormatter streamFormatter, out MessageDescription messageDescription)
        {
            if (isRequest)
            {
                streamFormatter = requestStreamFormatter;
                messageDescription = _requestDescription;
            }
            else
            {
                streamFormatter = replyStreamFormatter;
                messageDescription = _replyDescription;
            }
        }

        private void SerializeBodyContents(XmlDictionaryWriter writer, MessageVersion version, object[] parameters, object returnValue, bool isRequest)
        {
            MessageDescription messageDescription;
            StreamFormatter streamFormatter;

            SetupStreamAndMessageDescription(isRequest, out streamFormatter, out messageDescription);

            if (streamFormatter != null)
            {
                streamFormatter.Serialize(writer, parameters, returnValue);
                return;
            }

            SerializeBody(writer, version, RequestAction, messageDescription, returnValue, parameters, isRequest);
        }

        private async Task SerializeBodyContentsAsync(XmlDictionaryWriter writer, MessageVersion version, object[] parameters, object returnValue, bool isRequest)
        {
            MessageDescription messageDescription;
            StreamFormatter streamFormatter;

            SetupStreamAndMessageDescription(isRequest, out streamFormatter, out messageDescription);

            if (streamFormatter != null)
            {
                streamFormatter.Serialize(writer, parameters, returnValue);

                return;
            }

            await SerializeBodyAsync(writer, version, RequestAction, messageDescription, returnValue, parameters, isRequest);
        }

        private IAsyncResult BeginSerializeBodyContents(XmlDictionaryWriter writer, MessageVersion version, object[] parameters, object returnValue, bool isRequest,
            AsyncCallback callback, object state)
        {
            return new SerializeBodyContentsAsyncResult(this, writer, version, parameters, returnValue, isRequest, callback, state);
        }

        private void EndSerializeBodyContents(IAsyncResult result)
        {
            SerializeBodyContentsAsyncResult.End(result);
        }

        internal class SerializeBodyContentsAsyncResult : AsyncResult
        {
            private static AsyncCompletion s_handleEndSerializeBodyContents = new AsyncCompletion(HandleEndSerializeBodyContents);

            private StreamFormatter _streamFormatter;

            internal SerializeBodyContentsAsyncResult(OperationFormatter operationFormatter, XmlDictionaryWriter writer, MessageVersion version, object[] parameters,
                object returnValue, bool isRequest, AsyncCallback callback, object state)
                : base(callback, state)
            {
                bool completeSelf = true;

                MessageDescription messageDescription;
                StreamFormatter streamFormatter;

                operationFormatter.SetupStreamAndMessageDescription(isRequest, out streamFormatter, out messageDescription);

                if (streamFormatter != null)
                {
                    _streamFormatter = streamFormatter;
                    IAsyncResult result = streamFormatter.BeginSerialize(writer, parameters, returnValue, PrepareAsyncCompletion(s_handleEndSerializeBodyContents), this);
                    completeSelf = SyncContinue(result);
                }
                else
                {
                    operationFormatter.SerializeBody(writer, version, operationFormatter.RequestAction, messageDescription, returnValue, parameters, isRequest);
                    completeSelf = true;
                }


                if (completeSelf)
                {
                    Complete(true);
                }
            }


            private static bool HandleEndSerializeBodyContents(IAsyncResult result)
            {
                SerializeBodyContentsAsyncResult thisPtr = (SerializeBodyContentsAsyncResult)result.AsyncState;
                thisPtr._streamFormatter.EndSerialize(result);
                return true;
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<SerializeBodyContentsAsyncResult>(result);
            }
        }

        private void AddPropertiesToMessage(Message message, MessageDescription messageDescription, object[] parameters)
        {
            if (messageDescription.Properties.Count > 0)
            {
                AddPropertiesToMessageCore(message, messageDescription, parameters);
            }
        }

        private void AddPropertiesToMessageCore(Message message, MessageDescription messageDescription, object[] parameters)
        {
            MessageProperties properties = message.Properties;
            MessagePropertyDescriptionCollection propertyDescriptions = messageDescription.Properties;
            for (int i = 0; i < propertyDescriptions.Count; i++)
            {
                MessagePropertyDescription propertyDescription = propertyDescriptions[i];
                object parameter = parameters[propertyDescription.Index];
                if (null != parameter)
                    properties.Add(propertyDescription.Name, parameter);
            }
        }

        private void GetPropertiesFromMessage(Message message, MessageDescription messageDescription, object[] parameters)
        {
            if (messageDescription.Properties.Count > 0)
            {
                GetPropertiesFromMessageCore(message, messageDescription, parameters);
            }
        }

        private void GetPropertiesFromMessageCore(Message message, MessageDescription messageDescription, object[] parameters)
        {
            MessageProperties properties = message.Properties;
            MessagePropertyDescriptionCollection propertyDescriptions = messageDescription.Properties;
            for (int i = 0; i < propertyDescriptions.Count; i++)
            {
                MessagePropertyDescription propertyDescription = propertyDescriptions[i];
                if (properties.ContainsKey(propertyDescription.Name))
                {
                    parameters[propertyDescription.Index] = properties[propertyDescription.Name];
                }
            }
        }

        internal static object GetContentOfMessageHeaderOfT(MessageHeaderDescription headerDescription, object parameterValue, out bool mustUnderstand, out bool relay, out string actor)
        {
            actor = headerDescription.Actor;
            mustUnderstand = headerDescription.MustUnderstand;
            relay = headerDescription.Relay;

            if (headerDescription.TypedHeader && parameterValue != null)
                parameterValue = TypedHeaderManager.GetContent(headerDescription.Type, parameterValue, out mustUnderstand, out relay, out actor);
            return parameterValue;
        }

        internal static bool IsValidReturnValue(MessagePartDescription returnValue)
        {
            return (returnValue != null) && (returnValue.Type != typeof(void));
        }

        internal static XmlDictionaryString AddToDictionary(XmlDictionary dictionary, string s)
        {
            XmlDictionaryString dictionaryString;
            if (!dictionary.TryLookup(s, out dictionaryString))
            {
                dictionaryString = dictionary.Add(s);
            }
            return dictionaryString;
        }

        internal static void Validate(OperationDescription operation, bool isRpc, bool isEncoded)
        {
            if (isEncoded && !isRpc)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxDocEncodedNotSupported, operation.Name)));
            }

            bool hasVoid = false;
            bool hasTypedOrUntypedMessage = false;
            bool hasParameter = false;
            for (int i = 0; i < operation.Messages.Count; i++)
            {
                MessageDescription message = operation.Messages[i];
                if (message.IsTypedMessage || message.IsUntypedMessage)
                {
                    if (isRpc && operation.IsValidateRpcWrapperName)
                    {
                        if (!isEncoded)
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxTypedMessageCannotBeRpcLiteral, operation.Name)));
                    }
                    hasTypedOrUntypedMessage = true;
                }
                else if (message.IsVoid)
                    hasVoid = true;
                else
                    hasParameter = true;
            }
            if (hasParameter && hasTypedOrUntypedMessage)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxTypedOrUntypedMessageCannotBeMixedWithParameters, operation.Name)));
            if (isRpc && hasTypedOrUntypedMessage && hasVoid)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxTypedOrUntypedMessageCannotBeMixedWithVoidInRpc, operation.Name)));
        }

        internal static void GetActions(OperationDescription description, XmlDictionary dictionary, out XmlDictionaryString action, out XmlDictionaryString replyAction)
        {
            string actionString, replyActionString;
            actionString = description.Messages[0].Action;
            if (actionString == MessageHeaders.WildcardAction)
                actionString = null;
            if (!description.IsOneWay)
                replyActionString = description.Messages[1].Action;
            else
                replyActionString = null;
            if (replyActionString == MessageHeaders.WildcardAction)
                replyActionString = null;
            action = replyAction = null;
            if (actionString != null)
                action = AddToDictionary(dictionary, actionString);
            if (replyActionString != null)
                replyAction = AddToDictionary(dictionary, replyActionString);
        }

        internal static NetDispatcherFaultException CreateDeserializationFailedFault(string reason, Exception innerException)
        {
            reason = string.Format(SRServiceModel.SFxDeserializationFailed1, reason);
            FaultCode code = new FaultCode(FaultCodeConstants.Codes.DeserializationFailed, FaultCodeConstants.Namespaces.NetDispatch);
            code = FaultCode.CreateSenderFaultCode(code);
            return new NetDispatcherFaultException(reason, code, innerException);
        }

        internal static void TraceAndSkipElement(XmlReader xmlReader)
        {
            xmlReader.Skip();
        }

        internal class TypedMessageParts
        {
            private object _instance;
            private MemberInfo[] _members;

            public TypedMessageParts(object instance, MessageDescription description)
            {
                if (description == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("description"));
                }

                if (instance == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(string.Format(SRServiceModel.SFxTypedMessageCannotBeNull, description.Action)));
                }

                _members = new MemberInfo[description.Body.Parts.Count + description.Properties.Count + description.Headers.Count];

                foreach (MessagePartDescription part in description.Headers)
                    _members[part.Index] = part.MemberInfo;

                foreach (MessagePartDescription part in description.Properties)
                    _members[part.Index] = part.MemberInfo;

                foreach (MessagePartDescription part in description.Body.Parts)
                    _members[part.Index] = part.MemberInfo;

                _instance = instance;
            }

            private object GetValue(int index)
            {
                MemberInfo memberInfo = _members[index];
                PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                if (propertyInfo != null)
                {
                    return propertyInfo.GetValue(_instance, null);
                }
                else
                {
                    return ((FieldInfo)memberInfo).GetValue(_instance);
                }
            }

            private void SetValue(object value, int index)
            {
                MemberInfo memberInfo = _members[index];
                PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(_instance, value, null);
                }
                else
                {
                    ((FieldInfo)memberInfo).SetValue(_instance, value);
                }
            }

            internal void GetTypedMessageParts(object[] values)
            {
                for (int i = 0; i < _members.Length; i++)
                {
                    values[i] = GetValue(i);
                }
            }

            internal void SetTypedMessageParts(object[] values)
            {
                for (int i = 0; i < _members.Length; i++)
                {
                    SetValue(values[i], i);
                }
            }

            internal int Count
            {
                get { return _members.Length; }
            }
        }

        internal class OperationFormatterMessage : BodyWriterMessage
        {
            private OperationFormatter _operationFormatter;
            public OperationFormatterMessage(OperationFormatter operationFormatter, MessageVersion version, ActionHeader action,
               object[] parameters, object returnValue, bool isRequest)
                : base(version, action, new OperationFormatterBodyWriter(operationFormatter, version, parameters, returnValue, isRequest))
            {
                _operationFormatter = operationFormatter;
            }


            public OperationFormatterMessage(MessageVersion version, string action, BodyWriter bodyWriter) : base(version, action, bodyWriter) { }

            private OperationFormatterMessage(MessageHeaders headers, KeyValuePair<string, object>[] properties, OperationFormatterBodyWriter bodyWriter)
                : base(headers, properties, bodyWriter)
            {
                _operationFormatter = bodyWriter.OperationFormatter;
            }

            protected override void OnWriteStartBody(XmlDictionaryWriter writer)
            {
                base.OnWriteStartBody(writer);
                _operationFormatter.WriteBodyAttributes(writer, this.Version);
            }

            protected override MessageBuffer OnCreateBufferedCopy(int maxBufferSize)
            {
                BodyWriter bufferedBodyWriter;
                if (BodyWriter.IsBuffered)
                {
                    bufferedBodyWriter = base.BodyWriter;
                }
                else
                {
                    bufferedBodyWriter = base.BodyWriter.CreateBufferedCopy(maxBufferSize);
                }
                KeyValuePair<string, object>[] properties = new KeyValuePair<string, object>[base.Properties.Count];
                ((ICollection<KeyValuePair<string, object>>)base.Properties).CopyTo(properties, 0);
                return new OperationFormatterMessageBuffer(base.Headers, properties, bufferedBodyWriter);
            }

            internal class OperationFormatterBodyWriter : BodyWriter
            {
                private bool _isRequest;
                private OperationFormatter _operationFormatter;
                private object[] _parameters;
                private object _returnValue;
                private MessageVersion _version;
                private bool _onBeginWriteBodyContentsCalled;

                public OperationFormatterBodyWriter(OperationFormatter operationFormatter, MessageVersion version,
                    object[] parameters, object returnValue, bool isRequest)
                    : base(AreParametersBuffered(isRequest, operationFormatter))
                {
                    _parameters = parameters;
                    _returnValue = returnValue;
                    _isRequest = isRequest;
                    _operationFormatter = operationFormatter;
                    _version = version;
                }

                private object ThisLock
                {
                    get { return this; }
                }

                private static bool AreParametersBuffered(bool isRequest, OperationFormatter operationFormatter)
                {
                    StreamFormatter streamFormatter = isRequest ? operationFormatter.requestStreamFormatter : operationFormatter.replyStreamFormatter;
                    return streamFormatter == null;
                }

                protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
                {
                    lock (ThisLock)
                    {
                        _operationFormatter.SerializeBodyContents(writer, _version, _parameters, _returnValue, _isRequest);
                    }
                }

                protected override Task OnWriteBodyContentsAsync(XmlDictionaryWriter writer)
                {
                    return _operationFormatter.SerializeBodyContentsAsync(writer, _version, _parameters, _returnValue, _isRequest);
                }

                protected override IAsyncResult OnBeginWriteBodyContents(XmlDictionaryWriter writer, AsyncCallback callback, object state)
                {
                    Fx.Assert(!_onBeginWriteBodyContentsCalled, "OnBeginWriteBodyContents has already been called");
                    _onBeginWriteBodyContentsCalled = true;
                    return new OnWriteBodyContentsAsyncResult(this, writer, callback, state);
                }

                protected override void OnEndWriteBodyContents(IAsyncResult result)
                {
                    OnWriteBodyContentsAsyncResult.End(result);
                }

                internal OperationFormatter OperationFormatter
                {
                    get { return _operationFormatter; }
                }

                internal class OnWriteBodyContentsAsyncResult : AsyncResult
                {
                    private static AsyncCompletion s_handleEndOnWriteBodyContents = new AsyncCompletion(HandleEndOnWriteBodyContents);

                    private OperationFormatter _operationFormatter;

                    internal OnWriteBodyContentsAsyncResult(OperationFormatterBodyWriter operationFormatterBodyWriter, XmlDictionaryWriter writer, AsyncCallback callback, object state)
                        : base(callback, state)
                    {
                        bool completeSelf = true;
                        _operationFormatter = operationFormatterBodyWriter.OperationFormatter;

                        IAsyncResult result = _operationFormatter.BeginSerializeBodyContents(writer, operationFormatterBodyWriter._version,
                            operationFormatterBodyWriter._parameters, operationFormatterBodyWriter._returnValue, operationFormatterBodyWriter._isRequest,
                            PrepareAsyncCompletion(s_handleEndOnWriteBodyContents), this);
                        completeSelf = SyncContinue(result);

                        if (completeSelf)
                        {
                            Complete(true);
                        }
                    }

                    private static bool HandleEndOnWriteBodyContents(IAsyncResult result)
                    {
                        OnWriteBodyContentsAsyncResult thisPtr = (OnWriteBodyContentsAsyncResult)result.AsyncState;
                        thisPtr._operationFormatter.EndSerializeBodyContents(result);
                        return true;
                    }

                    public static void End(IAsyncResult result)
                    {
                        AsyncResult.End<OnWriteBodyContentsAsyncResult>(result);
                    }
                }
            }

            private class OperationFormatterMessageBuffer : BodyWriterMessageBuffer
            {
                public OperationFormatterMessageBuffer(MessageHeaders headers,
                    KeyValuePair<string, object>[] properties, BodyWriter bodyWriter)
                    : base(headers, properties, bodyWriter)
                {
                }

                public override Message CreateMessage()
                {
                    OperationFormatterBodyWriter operationFormatterBodyWriter = base.BodyWriter as OperationFormatterBodyWriter;
                    if (operationFormatterBodyWriter == null)
                        return base.CreateMessage();
                    lock (ThisLock)
                    {
                        if (base.Closed)
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateBufferDisposedException());
                        return new OperationFormatterMessage(base.Headers, base.Properties, operationFormatterBodyWriter);
                    }
                }
            }
        }

        internal abstract class OperationFormatterHeader : MessageHeader
        {
            protected MessageHeader innerHeader; //use innerHeader to handle versionSupported, actor/role handling etc.
            protected OperationFormatter operationFormatter;
            protected MessageVersion version;

            public OperationFormatterHeader(OperationFormatter operationFormatter, MessageVersion version, string name, string ns, bool mustUnderstand, string actor, bool relay)
            {
                this.operationFormatter = operationFormatter;
                this.version = version;
                if (actor != null)
                    innerHeader = MessageHeader.CreateHeader(name, ns, null/*headerValue*/, mustUnderstand, actor, relay);
                else
                    innerHeader = MessageHeader.CreateHeader(name, ns, null/*headerValue*/, mustUnderstand, "", relay);
            }


            public override bool IsMessageVersionSupported(MessageVersion messageVersion)
            {
                return innerHeader.IsMessageVersionSupported(messageVersion);
            }


            public override string Name
            {
                get { return innerHeader.Name; }
            }

            public override string Namespace
            {
                get { return innerHeader.Namespace; }
            }

            public override bool MustUnderstand
            {
                get { return innerHeader.MustUnderstand; }
            }

            public override bool Relay
            {
                get { return innerHeader.Relay; }
            }

            public override string Actor
            {
                get { return innerHeader.Actor; }
            }

            protected override void OnWriteStartHeader(XmlDictionaryWriter writer, MessageVersion messageVersion)
            {
                //Prefix needed since there may be xsi:type attribute at toplevel with qname value where ns = ""
                writer.WriteStartElement((this.Namespace == null || this.Namespace.Length == 0) ? string.Empty : "h", this.Name, this.Namespace);
                OnWriteHeaderAttributes(writer, messageVersion);
            }

            protected virtual void OnWriteHeaderAttributes(XmlDictionaryWriter writer, MessageVersion messageVersion)
            {
                base.WriteHeaderAttributes(writer, messageVersion);
            }
        }

        internal class XmlElementMessageHeader : OperationFormatterHeader
        {
            protected XmlElement headerValue;
            public XmlElementMessageHeader(OperationFormatter operationFormatter, MessageVersion version, string name, string ns, bool mustUnderstand, string actor, bool relay, XmlElement headerValue) :
                base(operationFormatter, version, name, ns, mustUnderstand, actor, relay)
            {
                this.headerValue = headerValue;
            }

            protected override void OnWriteHeaderAttributes(XmlDictionaryWriter writer, MessageVersion messageVersion)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
            {
                headerValue.WriteContentTo(writer);
            }
        }

        internal struct QName
        {
            internal string Name;
            internal string Namespace;
            internal QName(string name, string ns)
            {
                Name = name;
                Namespace = ns;
            }
        }
        internal class QNameComparer : IEqualityComparer<QName>
        {
            static internal QNameComparer Singleton = new QNameComparer();
            private QNameComparer() { }

            public bool Equals(QName x, QName y)
            {
                return x.Name == y.Name && x.Namespace == y.Namespace;
            }

            public int GetHashCode(QName obj)
            {
                return obj.Name.GetHashCode();
            }
        }
        internal class MessageHeaderDescriptionTable : Dictionary<QName, MessageHeaderDescription>
        {
            internal MessageHeaderDescriptionTable() : base(QNameComparer.Singleton) { }
            internal void Add(string name, string ns, MessageHeaderDescription message)
            {
                base.Add(new QName(name, ns), message);
            }
            internal MessageHeaderDescription Get(string name, string ns)
            {
                MessageHeaderDescription message;
                if (base.TryGetValue(new QName(name, ns), out message))
                    return message;
                return null;
            }
        }
    }
}
