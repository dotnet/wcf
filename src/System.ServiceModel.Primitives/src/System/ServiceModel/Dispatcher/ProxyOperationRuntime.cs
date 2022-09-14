// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace System.ServiceModel.Dispatcher
{
    internal class ProxyOperationRuntime
    {
        private readonly IClientMessageFormatter _formatter;
        private readonly bool _isSessionOpenNotificationEnabled;
        private readonly IParameterInspector[] _parameterInspectors;
        private readonly ImmutableClientRuntime _parent;
        private bool _deserializeReply;
        private string _replyAction;

        private MethodInfo _beginMethod;
        private MethodInfo _syncMethod;
        private MethodInfo _taskMethod;
        private ParameterInfo[] _inParams;
        private ParameterInfo[] _outParams;
        private ParameterInfo[] _endOutParams;
        private ParameterInfo _returnParam;

        internal ProxyOperationRuntime(ClientOperation operation, ImmutableClientRuntime parent)
        {
            if (operation == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(operation));
            }

            _parent = parent ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(parent));
            _formatter = operation.Formatter;
            IsInitiating = operation.IsInitiating;
            IsOneWay = operation.IsOneWay;
            IsTerminating = operation.IsTerminating;
            _isSessionOpenNotificationEnabled = operation.IsSessionOpenNotificationEnabled;
            Name = operation.Name;
            _parameterInspectors = EmptyArray<IParameterInspector>.ToArray(operation.ParameterInspectors);
            FaultFormatter = operation.FaultFormatter;
            SerializeRequest = operation.SerializeRequest;
            _deserializeReply = operation.DeserializeReply;
            Action = operation.Action;
            _replyAction = operation.ReplyAction;
            _beginMethod = operation.BeginMethod;
            _syncMethod = operation.SyncMethod;
            _taskMethod = operation.TaskMethod;
            TaskTResult = operation.TaskTResult;

            if (_beginMethod != null)
            {
                _inParams = ServiceReflector.GetInputParameters(_beginMethod, true);
                if (_syncMethod != null)
                {
                    _outParams = ServiceReflector.GetOutputParameters(_syncMethod, false);
                }
                else
                {
                    _outParams = Array.Empty<ParameterInfo>();
                }
                _endOutParams = ServiceReflector.GetOutputParameters(operation.EndMethod, true);
                _returnParam = operation.EndMethod.ReturnParameter;
            }
            else if (_syncMethod != null)
            {
                _inParams = ServiceReflector.GetInputParameters(_syncMethod, false);
                _outParams = ServiceReflector.GetOutputParameters(_syncMethod, false);
                _returnParam = _syncMethod.ReturnParameter;
            }

            if (_formatter == null && (SerializeRequest || _deserializeReply))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.ClientRuntimeRequiresFormatter0, Name)));
            }
        }

        internal string Action { get; }

        internal IClientFaultFormatter FaultFormatter { get; }

        internal bool IsInitiating { get; }

        internal bool IsOneWay { get; }

        internal bool IsTerminating { get; }

        internal bool IsSessionOpenNotificationEnabled
        {
            get { return _isSessionOpenNotificationEnabled; }
        }

        internal string Name { get; }

        internal ImmutableClientRuntime Parent
        {
            get { return _parent; }
        }

        internal string ReplyAction
        {
            get { return _replyAction; }
        }

        internal bool DeserializeReply
        {
            get { return _deserializeReply; }
        }

        internal bool SerializeRequest { get; }

        internal Type TaskTResult
        {
            get;
            set;
        }

        internal void AfterReply(ref ProxyRpc rpc)
        {
            if (!IsOneWay)
            {
                Message reply = rpc.Reply;

                if (_deserializeReply)
                {
                    if (WcfEventSource.Instance.ClientFormatterDeserializeReplyStartIsEnabled())
                    {
                        WcfEventSource.Instance.ClientFormatterDeserializeReplyStart(rpc.EventTraceActivity);
                    }

                    rpc.ReturnValue = _formatter.DeserializeReply(reply, rpc.OutputParameters);

                    if (WcfEventSource.Instance.ClientFormatterDeserializeReplyStopIsEnabled())
                    {
                        WcfEventSource.Instance.ClientFormatterDeserializeReplyStop(rpc.EventTraceActivity);
                    }
                }
                else
                {
                    rpc.ReturnValue = reply;
                }

                int offset = _parent.ParameterInspectorCorrelationOffset;
                try
                {
                    for (int i = _parameterInspectors.Length - 1; i >= 0; i--)
                    {
                        _parameterInspectors[i].AfterCall(Name,
                                                              rpc.OutputParameters,
                                                              rpc.ReturnValue,
                                                              rpc.Correlation[offset + i]);
                        if (WcfEventSource.Instance.ClientParameterInspectorAfterCallInvokedIsEnabled())
                        {
                            WcfEventSource.Instance.ClientParameterInspectorAfterCallInvoked(rpc.EventTraceActivity, _parameterInspectors[i].GetType().FullName);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    if (ErrorBehavior.ShouldRethrowClientSideExceptionAsIs(e))
                    {
                        throw;
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(e);
                }

                if (_parent.ValidateMustUnderstand)
                {
                    Collection<MessageHeaderInfo> headersNotUnderstood = reply.Headers.GetHeadersNotUnderstood();
                    if (headersNotUnderstood != null && headersNotUnderstood.Count > 0)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ProtocolException(SRP.Format(SRP.SFxHeaderNotUnderstood, headersNotUnderstood[0].Name, headersNotUnderstood[0].Namespace)));
                    }
                }
            }
        }

        internal void BeforeRequest(ref ProxyRpc rpc)
        {
            int offset = _parent.ParameterInspectorCorrelationOffset;
            try
            {
                for (int i = 0; i < _parameterInspectors.Length; i++)
                {
                    rpc.Correlation[offset + i] = _parameterInspectors[i].BeforeCall(Name, rpc.InputParameters);
                    if (WcfEventSource.Instance.ClientParameterInspectorBeforeCallInvokedIsEnabled())
                    {
                        WcfEventSource.Instance.ClientParameterInspectorBeforeCallInvoked(rpc.EventTraceActivity, _parameterInspectors[i].GetType().FullName);
                    }
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                if (ErrorBehavior.ShouldRethrowClientSideExceptionAsIs(e))
                {
                    throw;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(e);
            }

            if (SerializeRequest)
            {
                if (WcfEventSource.Instance.ClientFormatterSerializeRequestStartIsEnabled())
                {
                    WcfEventSource.Instance.ClientFormatterSerializeRequestStart(rpc.EventTraceActivity);
                }

                rpc.Request = _formatter.SerializeRequest(rpc.MessageVersion, rpc.InputParameters);



                if (WcfEventSource.Instance.ClientFormatterSerializeRequestStopIsEnabled())
                {
                    WcfEventSource.Instance.ClientFormatterSerializeRequestStop(rpc.EventTraceActivity);
                }
            }
            else
            {
                if (rpc.InputParameters[0] == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.SFxProxyRuntimeMessageCannotBeNull, Name)));
                }

                rpc.Request = (Message)rpc.InputParameters[0];
                if (!IsValidAction(rpc.Request, Action))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.SFxInvalidRequestAction, Name, rpc.Request.Headers.Action ?? "{NULL}", Action)));
                }
            }
        }

        internal static object GetDefaultParameterValue(Type parameterType)
        {
            return (parameterType.IsValueType() && parameterType != typeof(void)) ? Activator.CreateInstance(parameterType) : null;
        }

        internal bool IsSyncCall(MethodCall methodCall)
        {
            if (_syncMethod == null)
            {
                return false;
            }

            Contract.Assert(methodCall != null);
            Contract.Assert(methodCall.MethodBase != null);
            return methodCall.MethodBase.MethodHandle.Value.Equals(_syncMethod.MethodHandle.Value);
        }

        internal bool IsBeginCall(MethodCall methodCall)
        {
            if (_beginMethod == null)
            {
                return false;
            }

            Contract.Assert(methodCall != null);
            Contract.Assert(methodCall.MethodBase != null);
            return methodCall.MethodBase.MethodHandle.Value.Equals(_beginMethod.MethodHandle.Value);
        }

        internal bool IsTaskCall(MethodCall methodCall)
        {
            if (_taskMethod == null)
            {
                return false;
            }

            Contract.Assert(methodCall != null);
            Contract.Assert(methodCall.MethodBase != null);
            return methodCall.MethodBase.MethodHandle.Value.Equals(_taskMethod.MethodHandle.Value);
        }

        internal object[] MapSyncInputs(MethodCall methodCall, out object[] outs)
        {
            if (_outParams.Length == 0)
            {
                outs = Array.Empty<object>();
            }
            else
            {
                outs = new object[_outParams.Length];
            }
            if (_inParams.Length == 0)
            {
                return Array.Empty<object>();
            }

            return methodCall.InArgs;
        }

        internal object[] MapAsyncBeginInputs(MethodCall methodCall, out AsyncCallback callback, out object asyncState)
        {
            object[] ins;
            if (_inParams.Length == 0)
            {
                ins = Array.Empty<object>();
            }
            else
            {
                ins = new object[_inParams.Length];
            }

            object[] args = methodCall.Args;
            for (int i = 0; i < ins.Length; i++)
            {
                ins[i] = args[_inParams[i].Position];
            }

            callback = args[methodCall.Args.Length - 2] as AsyncCallback;
            asyncState = args[methodCall.Args.Length - 1];
            return ins;
        }

        internal void MapAsyncEndInputs(MethodCall methodCall, out IAsyncResult result, out object[] outs)
        {
            outs = new object[_endOutParams.Length];
            result = methodCall.Args[methodCall.Args.Length - 1] as IAsyncResult;
        }

        internal object[] MapSyncOutputs(MethodCall methodCall, object[] outs, ref object ret)
        {
            return MapOutputs(_outParams, methodCall, outs, ref ret);
        }

        internal object[] MapAsyncOutputs(MethodCall methodCall, object[] outs, ref object ret)
        {
            return MapOutputs(_endOutParams, methodCall, outs, ref ret);
        }

        private object[] MapOutputs(ParameterInfo[] parameters, MethodCall methodCall, object[] outs, ref object ret)
        {
            if (ret == null && _returnParam != null)
            {
                ret = GetDefaultParameterValue(TypeLoader.GetParameterType(_returnParam));
            }

            if (parameters.Length == 0)
            {
                return null;
            }

            object[] args = methodCall.Args;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (outs[i] == null)
                {
                    // the RealProxy infrastructure requires a default value for value types
                    args[parameters[i].Position] = GetDefaultParameterValue(TypeLoader.GetParameterType(parameters[i]));
                }
                else
                {
                    args[parameters[i].Position] = outs[i];
                }
            }

            return args;
        }

        static internal bool IsValidAction(Message message, string action)
        {
            if (message == null)
            {
                return false;
            }

            if (message.IsFault)
            {
                return true;
            }

            if (action == MessageHeaders.WildcardAction)
            {
                return true;
            }

            return (String.CompareOrdinal(message.Headers.Action, action) == 0);
        }
    }
}
