// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel.Dispatcher
{
    internal class ErrorBehavior
    {
        private IErrorHandler[] _handlers;
        private bool _debug;
        private bool _isOnServer;
        private MessageVersion _messageVersion;

        internal ErrorBehavior(ChannelDispatcher channelDispatcher)
        {
            _handlers = EmptyArray<IErrorHandler>.ToArray(channelDispatcher.ErrorHandlers);
            _debug = channelDispatcher.IncludeExceptionDetailInFaults;
            _isOnServer = channelDispatcher.IsOnServer;
            _messageVersion = channelDispatcher.MessageVersion;
        }

        private void InitializeFault(ref MessageRpc rpc)
        {
            Exception error = rpc.Error;
            FaultException fault = error as FaultException;
            if (fault != null)
            {
                string action;
                MessageFault messageFault = rpc.Operation.FaultFormatter.Serialize(fault, out action);
                if (action == null)
                    action = rpc.RequestVersion.Addressing.DefaultFaultAction;
                if (messageFault != null)
                    rpc.FaultInfo.Fault = Message.CreateMessage(rpc.RequestVersion, messageFault, action);
            }
        }

        internal void ProvideMessageFault(ref MessageRpc rpc)
        {
            if (rpc.Error != null)
            {
                ProvideMessageFaultCore(ref rpc);
            }
        }

        private void ProvideMessageFaultCore(ref MessageRpc rpc)
        {
            if (_messageVersion != rpc.RequestVersion)
            {
                Fx.Assert("System.ServiceModel.Dispatcher.ErrorBehavior.ProvideMessageFaultCore(): (this.messageVersion != rpc.RequestVersion)");
            }

            this.InitializeFault(ref rpc);

            this.ProvideFault(rpc.Error, rpc.Channel.GetProperty<FaultConverter>(), ref rpc.FaultInfo);

            this.ProvideMessageFaultCoreCoda(ref rpc);
        }

        private void ProvideMessageFaultCoreCoda(ref MessageRpc rpc)
        {
            if (rpc.FaultInfo.Fault.Headers.Action == null)
            {
                rpc.FaultInfo.Fault.Headers.Action = rpc.RequestVersion.Addressing.DefaultFaultAction;
            }

            rpc.Reply = rpc.FaultInfo.Fault;
        }

        internal void ProvideOnlyFaultOfLastResort(ref MessageRpc rpc)
        {
            ProvideFaultOfLastResort(rpc.Error, ref rpc.FaultInfo);
            ProvideMessageFaultCoreCoda(ref rpc);
        }

        private void ProvideFaultOfLastResort(Exception error, ref ErrorHandlerFaultInfo faultInfo)
        {
            if (faultInfo.Fault == null)
            {
                FaultCode code = new FaultCode(FaultCodeConstants.Codes.InternalServiceFault, FaultCodeConstants.Namespaces.NetDispatch);
                code = FaultCode.CreateReceiverFaultCode(code);
                string action = FaultCodeConstants.Actions.NetDispatcher;
                MessageFault fault;
                if (_debug)
                {
                    faultInfo.DefaultFaultAction = action;
                    fault = MessageFault.CreateFault(code, new FaultReason(error.Message), new ExceptionDetail(error));
                }
                else
                {
                    string reason = _isOnServer ? SRServiceModel.SFxInternalServerError : SRServiceModel.SFxInternalCallbackError;
                    fault = MessageFault.CreateFault(code, new FaultReason(reason));
                }
                faultInfo.IsConsideredUnhandled = true;
                faultInfo.Fault = Message.CreateMessage(_messageVersion, fault, action);
            }
            //if this is an InternalServiceFault coming from another service dispatcher we should treat it as unhandled so that the channels are cleaned up
            else if (error != null)
            {
                FaultException e = error as FaultException;
                if (e != null && e.Fault != null && e.Fault.Code != null && e.Fault.Code.SubCode != null &&
                    string.Compare(e.Fault.Code.SubCode.Namespace, FaultCodeConstants.Namespaces.NetDispatch, StringComparison.Ordinal) == 0 &&
                    string.Compare(e.Fault.Code.SubCode.Name, FaultCodeConstants.Codes.InternalServiceFault, StringComparison.Ordinal) == 0)
                {
                    faultInfo.IsConsideredUnhandled = true;
                }
            }
        }

        internal void ProvideFault(Exception e, FaultConverter faultConverter, ref ErrorHandlerFaultInfo faultInfo)
        {
            ProvideWellKnownFault(e, faultConverter, ref faultInfo);
            for (int i = 0; i < _handlers.Length; i++)
            {
                Message m = faultInfo.Fault;
                _handlers[i].ProvideFault(e, _messageVersion, ref m);
                faultInfo.Fault = m;
                if (WcfEventSource.Instance.FaultProviderInvokedIsEnabled())
                {
                    WcfEventSource.Instance.FaultProviderInvoked(_handlers[i].GetType().FullName, e.Message);
                }
            }
            this.ProvideFaultOfLastResort(e, ref faultInfo);
        }

        private void ProvideWellKnownFault(Exception e, FaultConverter faultConverter, ref ErrorHandlerFaultInfo faultInfo)
        {
            Message faultMessage;
            if (faultConverter != null && faultConverter.TryCreateFaultMessage(e, out faultMessage))
            {
                faultInfo.Fault = faultMessage;
                return;
            }
            else if (e is NetDispatcherFaultException)
            {
                NetDispatcherFaultException ndfe = e as NetDispatcherFaultException;
                if (_debug)
                {
                    ExceptionDetail detail = new ExceptionDetail(ndfe);
                    faultInfo.Fault = Message.CreateMessage(_messageVersion, MessageFault.CreateFault(ndfe.Code, ndfe.Reason, detail), ndfe.Action);
                }
                else
                {
                    faultInfo.Fault = Message.CreateMessage(_messageVersion, ndfe.CreateMessageFault(), ndfe.Action);
                }
            }
        }

        internal void HandleError(ref MessageRpc rpc)
        {
            if (rpc.Error != null)
            {
                HandleErrorCore(ref rpc);
            }
        }

        private void HandleErrorCore(ref MessageRpc rpc)
        {
            bool handled = HandleErrorCommon(rpc.Error, ref rpc.FaultInfo);
            if (handled)
            {
                rpc.Error = null;
            }
        }

        private bool HandleErrorCommon(Exception error, ref ErrorHandlerFaultInfo faultInfo)
        {
            bool handled;
            if (faultInfo.Fault != null   // there is a message
                && !faultInfo.IsConsideredUnhandled) // and it's not the internal-server-error one
            {
                handled = true;
            }
            else
            {
                handled = false;
            }

            try
            {
                if (WcfEventSource.Instance.ServiceExceptionIsEnabled())
                {
                    WcfEventSource.Instance.ServiceException(error.ToString(), error.GetType().FullName);
                }
                for (int i = 0; i < _handlers.Length; i++)
                {
                    bool handledByThis = _handlers[i].HandleError(error);
                    handled = handledByThis || handled;
                    if (WcfEventSource.Instance.ErrorHandlerInvokedIsEnabled())
                    {
                        WcfEventSource.Instance.ErrorHandlerInvoked(_handlers[i].GetType().FullName, handledByThis, error.GetType().FullName);
                    }
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(e);
            }
            return handled;
        }

        internal bool HandleError(Exception error)
        {
            ErrorHandlerFaultInfo faultInfo = new ErrorHandlerFaultInfo(_messageVersion.Addressing.DefaultFaultAction);
            return HandleError(error, ref faultInfo);
        }

        internal bool HandleError(Exception error, ref ErrorHandlerFaultInfo faultInfo)
        {
            return HandleErrorCommon(error, ref faultInfo);
        }

        internal static bool ShouldRethrowClientSideExceptionAsIs(Exception e)
        {
            return true;
        }

        // This ensures that people debugging first-chance Exceptions see this Exception,
        // and that the Exception shows up in the trace logs.
        internal static void ThrowAndCatch(Exception e, Message message)
        {
            try
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    if (message == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(e);
                    }
                    else
                    {
                        throw TraceUtility.ThrowHelperError(e, message);
                    }
                }
                else
                {
                    if (message == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(e);
                    }
                    else
                    {
                        TraceUtility.ThrowHelperError(e, message);
                    }
                }
            }
            catch (Exception e2)
            {
                if (!object.ReferenceEquals(e, e2))
                {
                    throw;
                }
            }
        }

        internal static void ThrowAndCatch(Exception e)
        {
            ThrowAndCatch(e, null);
        }
    }
}
