// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Tracing;
using System.Runtime.Diagnostics;

namespace System.Runtime
{
    [EventSource(Name = "Microsoft-Windows-Application Server-Applications", Guid = "c651f5f6-1c0d-492e-8ae1-b4efd7c9d503")]
    internal sealed class WcfEventSource : EventSource
    {
        public static WcfEventSource Instance = new WcfEventSource();

        public bool BufferPoolAllocationIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(EventIds.BufferPoolAllocation, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.BufferPoolingAllocate, Task = Tasks.BufferPooling,
            Keywords = Keywords.Infrastructure,
            Message = "Pool allocating {0} Bytes.")]
        public void BufferPoolAllocation(int Size, string AppDomain)
        {
            WriteEvent(EventIds.BufferPoolAllocation, Size, AppDomain);
        }

        [NonEvent]
        public void BufferPoolAllocation(int Size)
        {
            BufferPoolAllocation(Size, "");
        }

        public bool BufferPoolChangeQuotaIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(EventIds.BufferPoolChangeQuota, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.BufferPoolingTune, Task = Tasks.BufferPooling,
            Keywords = Keywords.Infrastructure,
            Message = "BufferPool of size {0}, changing quota by {1}.")]
        public void BufferPoolChangeQuota(int PoolSize, int Delta, string AppDomain)
        {
            WriteEvent(EventIds.BufferPoolChangeQuota, PoolSize, Delta, AppDomain);
        }

        [NonEvent]
        public void BufferPoolChangeQuota(int PoolSize, int Delta)
        {
            BufferPoolChangeQuota(PoolSize, Delta, "");
        }

        public bool ActionItemScheduledIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Threading, EventChannel.Debug);
        }

        [Event(EventIds.ActionItemScheduled, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ThreadScheduling,
            Keywords = Keywords.Threading,
            Message = "IO Thread scheduler callback invoked.")]
        public void ActionItemScheduled(string AppDomain)
        {
            WriteEvent(EventIds.ActionItemScheduled, AppDomain);
        }

        [NonEvent]
        public void ActionItemScheduled()
        {
            ActionItemScheduled("");
        }

        public bool ActionItemCallbackInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Threading, EventChannel.Debug);
        }

        [Event(EventIds.ActionItemCallbackInvoked, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ThreadScheduling,
            Keywords = Keywords.Threading,
            Message = "IO Thread scheduler callback invoked.")]
        public void ActionItemCallbackInvoked(string AppDomain)
        {
            WriteEvent(EventIds.ActionItemCallbackInvoked, AppDomain);
        }

        [NonEvent]
        public void ActionItemCallbackInvoked()
        {
            ActionItemCallbackInvoked("");
        }

        public bool ClientMessageInspectorAfterReceiveInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ClientMessageInspectorAfterReceiveInvoked, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ClientRuntimeClientMessageInspectorAfterReceiveInvoked, Task = Tasks.ClientRuntime,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'AfterReceiveReply' on a ClientMessageInspector of type '{0}'.")]
        public void ClientMessageInspectorAfterReceiveInvoked(string TypeName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.ClientMessageInspectorAfterReceiveInvoked, TypeName, HostReference, AppDomain);
        }

        [NonEvent]
        public void ClientMessageInspectorAfterReceiveInvoked(EventTraceActivity eventTraceActivity, string TypeName)
        {
            SetActivityId(eventTraceActivity);
            ClientMessageInspectorAfterReceiveInvoked(TypeName, "", "");
        }

        public bool ClientMessageInspectorBeforeSendInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ClientMessageInspectorBeforeSendInvoked, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.ClientRuntimeClientMessageInspectorBeforeSendInvoked, Task = Tasks.ClientRuntime,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'BeforeSendRequest' on a ClientMessageInspector of type  '{0}'.")]
        public void ClientMessageInspectorBeforeSendInvoked(string TypeName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.ClientMessageInspectorBeforeSendInvoked, TypeName, HostReference, AppDomain);
        }

        [NonEvent]
        public void ClientMessageInspectorBeforeSendInvoked(EventTraceActivity eventTraceActivity, string TypeName)
        {
            SetActivityId(eventTraceActivity);
            ClientMessageInspectorBeforeSendInvoked(TypeName, "", "");
        }

        public bool ClientParameterInspectorAfterCallInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ClientParameterInspectorAfterCallInvoked, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.ClientRuntimeClientParameterInspectorStop, Task = Tasks.ClientRuntime,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'AfterCall' on a ClientParameterInspector of type '{0}'.")]
        public void ClientParameterInspectorAfterCallInvoked(string TypeName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.ClientParameterInspectorAfterCallInvoked, TypeName, HostReference, AppDomain);
        }

        [NonEvent]
        public void ClientParameterInspectorAfterCallInvoked(EventTraceActivity eventTraceActivity, string TypeName)
        {
            SetActivityId(eventTraceActivity);
            ClientParameterInspectorAfterCallInvoked(TypeName, "", "");
        }

        public bool ClientParameterInspectorBeforeCallInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ClientParameterInspectorBeforeCallInvoked, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.ClientRuntimeClientParameterInspectorStart, Task = Tasks.ClientRuntime,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'BeforeCall' on a ClientParameterInspector of type '{0}'.")]
        public void ClientParameterInspectorBeforeCallInvoked(string TypeName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.ClientParameterInspectorBeforeCallInvoked, TypeName, HostReference, AppDomain);
        }

        [NonEvent]
        public void ClientParameterInspectorBeforeCallInvoked(EventTraceActivity eventTraceActivity, string TypeName)
        {
            SetActivityId(eventTraceActivity);
            ClientParameterInspectorBeforeCallInvoked(TypeName, "", "");
        }

        public bool OperationInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.OperationInvoked, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.DispatchMessageOperationInvokerStart, Task = Tasks.DispatchMessage,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "An OperationInvoker invoked the '{0}' method. Caller information: '{1}'.")]
        public void OperationInvoked(string MethodName, string CallerInfo, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.OperationInvoked, MethodName, CallerInfo, HostReference, AppDomain);
        }

        [NonEvent]
        public void OperationInvoked(EventTraceActivity eventTraceActivity, string MethodName, string CallerInfo)
        {
            SetActivityId(eventTraceActivity);
            OperationInvoked(MethodName, CallerInfo, "", "");
        }

        public bool ErrorHandlerInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ErrorHandlerInvoked, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked an ErrorHandler of type  '{0}' with an exception of type '{2}'.  ErrorHandled == '{1}'.")]
        public void ErrorHandlerInvoked(string TypeName, bool Handled, string ExceptionTypeName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.ErrorHandlerInvoked, TypeName, Handled, ExceptionTypeName, HostReference, AppDomain);
        }

        [NonEvent]
        public void ErrorHandlerInvoked(string TypeName, bool Handled, string ExceptionTypeName)
        {
            ErrorHandlerInvoked(TypeName, Handled, ExceptionTypeName, "", "");
        }

        public bool FaultProviderInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.FaultProviderInvoked, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked a FaultProvider of type '{0}' with an exception of type '{1}'.")]
        public void FaultProviderInvoked(string TypeName, string ExceptionTypeName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.FaultProviderInvoked, TypeName, ExceptionTypeName, HostReference, AppDomain);
        }

        [NonEvent]
        public void FaultProviderInvoked(string TypeName, string ExceptionTypeName)
        {
            FaultProviderInvoked(TypeName, ExceptionTypeName, "", "");
        }

        public bool ParameterInspectorAfterCallInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ParameterInspectorAfterCallInvoked, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.DispatchMessageParameterInspectorStop, Task = Tasks.DispatchMessage,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'AfterCall' on a ParameterInspector of type '{0}'.")]
        public void ParameterInspectorAfterCallInvoked(string TypeName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.ParameterInspectorAfterCallInvoked, TypeName, HostReference, AppDomain);
        }

        [NonEvent]
        public void ParameterInspectorAfterCallInvoked(EventTraceActivity eventTraceActivity, string TypeName)
        {
            SetActivityId(eventTraceActivity);
            ParameterInspectorAfterCallInvoked(TypeName, "", "");
        }

        public bool ParameterInspectorBeforeCallInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ParameterInspectorBeforeCallInvoked, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.DispatchMessageParameterInspectorStart, Task = Tasks.DispatchMessage,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'BeforeCall' on a ParameterInspector of type '{0}'.")]
        public void ParameterInspectorBeforeCallInvoked(string TypeName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.ParameterInspectorBeforeCallInvoked, TypeName, HostReference, AppDomain);
        }

        [NonEvent]
        public void ParameterInspectorBeforeCallInvoked(EventTraceActivity eventTraceActivity, string TypeName)
        {
            SetActivityId(eventTraceActivity);
            ParameterInspectorBeforeCallInvoked(TypeName, "", "");
        }

        public bool OperationCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.HealthMonitoring | Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.OperationCompleted, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.DispatchMessageOperationInvokerStop, Task = Tasks.DispatchMessage,
            Keywords = Keywords.HealthMonitoring | Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "An OperationInvoker completed the call to the '{0}' method.  The method call duration was '{1}' ms.")]
        public void OperationCompleted(string MethodName, long Duration, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.OperationCompleted, MethodName, Duration, HostReference, AppDomain);
        }

        [NonEvent]
        public void OperationCompleted(EventTraceActivity eventTraceActivity, string MethodName, long Duration)
        {
            SetActivityId(eventTraceActivity);
            OperationCompleted(MethodName, Duration, "", "");
        }

        public bool MessageReceivedByTransportIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.TransportGeneral, EventChannel.Analytic);
        }

        [Event(EventIds.MessageReceivedByTransport, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.TransportReceive,
            Keywords = Keywords.Troubleshooting | Keywords.TransportGeneral,
            Message = "The transport received a message from '{0}'.")]
        public void MessageReceivedByTransport(string ListenAddress, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.MessageReceivedByTransport, ListenAddress, HostReference, AppDomain);
        }

        [NonEvent]
        public void MessageReceivedByTransport(EventTraceActivity eventTraceActivity, string ListenAddress, Guid relatedActivityId)
        {
            TransferActivityId(eventTraceActivity);
            MessageReceivedByTransport(ListenAddress, "", "");
        }

        public bool MessageSentByTransportIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.TransportGeneral, EventChannel.Analytic);
        }

        [Event(EventIds.MessageSentByTransport, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.TransportSend,
            Keywords = Keywords.Troubleshooting | Keywords.TransportGeneral,
            Message = "The transport sent a message to '{0}'.")]
        public void MessageSentByTransport(string DestinationAddress, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.MessageSentByTransport, DestinationAddress, HostReference, AppDomain);
        }

        [NonEvent]
        public void MessageSentByTransport(EventTraceActivity eventTraceActivity, string DestinationAddress)
        {
            SetActivityId(eventTraceActivity);
            MessageSentByTransport(DestinationAddress, "", "");
        }

        public bool ClientOperationPreparedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ClientOperationPrepared, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ClientRuntimeOperationPrepared, Task = Tasks.ClientRuntime,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Client is executing Action '{0}' associated with the '{1}' contract. The message will be sent to '{2}'.")]
        public void ClientOperationPrepared(string Action, string ContractName, string Destination, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.ClientOperationPrepared, Action, ContractName, Destination, HostReference, AppDomain);
        }

        [NonEvent]
        public void ClientOperationPrepared(EventTraceActivity eventTraceActivity, string Action, string ContractName, string Destination, Guid relatedActivityId)
        {
            TransferActivityId(eventTraceActivity);
            ClientOperationPrepared(Action, ContractName, Destination, "", "");
        }

        public bool ServiceChannelCallStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceChannelCallStop, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.ServiceChannelCall,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Client completed executing Action '{0}' associated with the '{1}' contract. The message was sent to '{2}'.")]
        public void ServiceChannelCallStop(string Action, string ContractName, string Destination, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.ServiceChannelCallStop, Action, ContractName, Destination, HostReference, AppDomain);
        }

        [NonEvent]
        public void ServiceChannelCallStop(EventTraceActivity eventTraceActivity, string Action, string ContractName, string Destination)
        {
            SetActivityId(eventTraceActivity);
            ServiceChannelCallStop(Action, ContractName, Destination, "", "");
        }

        public bool ServiceExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceException, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.DispatchMessage,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceModel,
            Message = "There was an unhandled exception of type '{1}' during message processing.  Full Exception Details: {0}.")]
        public void ServiceException(string ExceptionToString, string ExceptionTypeName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.ServiceException, ExceptionToString, ExceptionTypeName, HostReference, AppDomain);
        }

        [NonEvent]
        public void ServiceException(string ExceptionToString, string ExceptionTypeName)
        {
            ServiceException(ExceptionToString, ExceptionTypeName, "", "");
        }

        public bool MessageSentToTransportIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.TransportGeneral, EventChannel.Analytic);
        }

        [Event(EventIds.MessageSentToTransport, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.TransportGeneral,
            Message = "The Dispatcher sent a message to the transport. Correlation ID == '{0}'.")]
        public void MessageSentToTransport(Guid CorrelationId, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.MessageSentToTransport, CorrelationId, HostReference, AppDomain);
        }

        [NonEvent]
        public void MessageSentToTransport(EventTraceActivity eventTraceActivity, Guid CorrelationId)
        {
            SetActivityId(eventTraceActivity);
            MessageSentToTransport(CorrelationId, "", "");
        }

        public bool MessageReceivedFromTransportIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.TransportGeneral, EventChannel.Analytic);
        }

        [Event(EventIds.MessageReceivedFromTransport, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.TransportGeneral,
            Message = "The Dispatcher received a message from the transport. Correlation ID == '{0}'.")]
        public void MessageReceivedFromTransport(Guid CorrelationId, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.MessageReceivedFromTransport, CorrelationId, HostReference, AppDomain);
        }

        [NonEvent]
        public void MessageReceivedFromTransport(EventTraceActivity eventTraceActivity, Guid CorrelationId, string HostReference)
        {
            SetActivityId(eventTraceActivity);
            MessageReceivedFromTransport(CorrelationId, HostReference, "");
        }

        public bool OperationFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.OperationFailed, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.DispatchMessage,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceModel,
            Message = "The '{0}' method threw an unhandled exception when invoked by the OperationInvoker. The method call duration was '{1}' ms.")]
        public void OperationFailed(string MethodName, long Duration, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.OperationFailed, MethodName, Duration, HostReference, AppDomain);
        }

        [NonEvent]
        public void OperationFailed(EventTraceActivity eventTraceActivity, string MethodName, long Duration)
        {
            SetActivityId(eventTraceActivity);
            OperationFailed(MethodName, Duration, "", "");
        }

        public bool OperationFaultedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.OperationFaulted, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.DispatchMessage,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceModel,
            Message = "The '{0}' method threw a FaultException when invoked by the OperationInvoker. The method call duration was '{1}' ms.")]
        public void OperationFaulted(string MethodName, long Duration, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.OperationFaulted, MethodName, Duration, HostReference, AppDomain);
        }

        [NonEvent]
        public void OperationFaulted(EventTraceActivity eventTraceActivity, string MethodName, long Duration)
        {
            SetActivityId(eventTraceActivity);
            OperationFaulted(MethodName, Duration, "", "");
        }

        public bool ServiceChannelOpenStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceChannelOpenStart, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.ServiceChannelOpen,
            Keywords = Keywords.ServiceModel,
            Message = "ServiceChannelOpen started.")]
        public void ServiceChannelOpenStart(string AppDomain)
        {
            WriteEvent(EventIds.ServiceChannelOpenStart, AppDomain);
        }

        [NonEvent]
        public void ServiceChannelOpenStart(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            ServiceChannelOpenStart("");
        }

        public bool ServiceChannelOpenStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceChannelOpenStop, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.ServiceChannelOpen,
            Keywords = Keywords.ServiceModel,
            Message = "ServiceChannelOpen completed.")]
        public void ServiceChannelOpenStop(string AppDomain)
        {
            WriteEvent(EventIds.ServiceChannelOpenStop, AppDomain);
        }

        [NonEvent]
        public void ServiceChannelOpenStop(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            ServiceChannelOpenStop("");
        }

        public bool ServiceChannelCallStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceChannelCallStart, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.ServiceChannelCall,
            Keywords = Keywords.ServiceModel,
            Message = "ServiceChannelCall started.")]
        public void ServiceChannelCallStart(string AppDomain)
        {
            WriteEvent(EventIds.ServiceChannelCallStart, AppDomain);
        }

        [NonEvent]
        public void ServiceChannelCallStart(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            ServiceChannelCallStart("");
        }

        [Event(EventIds.ServiceChannelBeginCallStart, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.ServiceChannelCall,
            Keywords = Keywords.ServiceModel,
            Message = "ServiceChannel asynchronous calls started.")]
        public void ServiceChannelBeginCallStart(string AppDomain)
        {
            WriteEvent(EventIds.ServiceChannelBeginCallStart, AppDomain);
        }

        [NonEvent]
        public void ServiceChannelBeginCallStart(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            ServiceChannelBeginCallStart("");
        }

        public bool DispatchMessageStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.DispatchMessageStart, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.DispatchMessageDispatchStart, Task = Tasks.DispatchMessage,
            Keywords = Keywords.ServiceModel,
            Message = "Message dispatching started.")]
        public void DispatchMessageStart(string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.DispatchMessageStart, HostReference, AppDomain);
        }

        [NonEvent]
        public void DispatchMessageStart(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            DispatchMessageStart("", "");
        }

        public bool DispatchMessageStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.DispatchMessageStop, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.DispatchMessageDispatchStop, Task = Tasks.DispatchMessage,
            Keywords = Keywords.ServiceModel,
            Message = "Message dispatching completed")]
        public void DispatchMessageStop(string AppDomain)
        {
            WriteEvent(EventIds.DispatchMessageStop, AppDomain);
        }

        [NonEvent]
        public void DispatchMessageStop(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            DispatchMessageStop("");
        }

        public bool ClientChannelOpenStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ClientChannelOpenStart, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.ClientRuntimeClientChannelOpenStart, Task = Tasks.ClientRuntime,
            Keywords = Keywords.ServiceModel,
            Message = "ServiceChannel Open Start.")]
        public void ClientChannelOpenStart(string AppDomain)
        {
            WriteEvent(EventIds.ClientChannelOpenStart, AppDomain);
        }

        [NonEvent]
        public void ClientChannelOpenStart(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            ClientChannelOpenStart("");
        }

        public bool ClientChannelOpenStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ClientChannelOpenStop, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.ClientRuntimeClientChannelOpenStop, Task = Tasks.ClientRuntime,
            Keywords = Keywords.ServiceModel,
            Message = "ServiceChannel Open Stop.")]
        public void ClientChannelOpenStop(string AppDomain)
        {
            WriteEvent(EventIds.ClientChannelOpenStop, AppDomain);
        }

        [NonEvent]
        public void ClientChannelOpenStop(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            ClientChannelOpenStop("");
        }

        [Event(EventIds.ScheduleRuntimeWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleWorkItemScheduleRuntime, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A runtime work item has been scheduled for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void ScheduleRuntimeWorkItem(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.ScheduleRuntimeWorkItem, data1, data2, data3, AppDomain);
        }

        public bool CloseTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.CloseTimeout, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TimeoutException,
            Keywords = Keywords.ServiceModel,
            Message = "{0}")]
        public void CloseTimeout(string data1, string AppDomain)
        {
            WriteEvent(EventIds.CloseTimeout, data1, AppDomain);
        }

        [NonEvent]
        public void CloseTimeout(string data1)
        {
            CloseTimeout(data1, "");
        }

        public bool IdleTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.IdleTimeout, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TimeoutException,
            Keywords = Keywords.ServiceModel,
            Message = "{0} Connection pool key: {1}")]
        public void IdleTimeout(string msg, string key, string AppDomain)
        {
            WriteEvent(EventIds.IdleTimeout, msg, key, AppDomain);
        }

        [NonEvent]
        public void IdleTimeout(string msg, string key)
        {
            IdleTimeout(msg, key, "");
        }

        public bool LeaseTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.LeaseTimeout, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TimeoutException,
            Keywords = Keywords.ServiceModel,
            Message = "{0} Connection pool key: {1}")]
        public void LeaseTimeout(string msg, string key, string AppDomain)
        {
            WriteEvent(EventIds.LeaseTimeout, msg, key, AppDomain);
        }

        [NonEvent]
        public void LeaseTimeout(string msg, string key)
        {
            LeaseTimeout(msg, key, "");
        }

        public bool ReceiveTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ReceiveTimeout, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TimeoutException,
            Keywords = Keywords.ServiceModel,
            Message = "{0}")]
        public void ReceiveTimeout(string data1, string AppDomain)
        {
            WriteEvent(EventIds.ReceiveTimeout, data1, AppDomain);
        }

        [NonEvent]
        public void ReceiveTimeout(string data1)
        {
            ReceiveTimeout(data1, "");
        }

        public bool MaxReceivedMessageSizeExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.MaxReceivedMessageSizeExceeded, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void MaxReceivedMessageSizeExceeded(string data1, string AppDomain)
        {
            WriteEvent(EventIds.MaxReceivedMessageSizeExceeded, data1, AppDomain);
        }

        [NonEvent]
        public void MaxReceivedMessageSizeExceeded(string data1)
        {
            MaxReceivedMessageSizeExceeded(data1, "");
        }

        public bool MaxSentMessageSizeExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.MaxSentMessageSizeExceeded, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void MaxSentMessageSizeExceeded(string data1, string AppDomain)
        {
            WriteEvent(EventIds.MaxSentMessageSizeExceeded, data1, AppDomain);
        }

        [NonEvent]
        public void MaxSentMessageSizeExceeded(string data1)
        {
            MaxSentMessageSizeExceeded(data1, "");
        }

        public bool MaxOutboundConnectionsPerEndpointExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Quota, EventChannel.Debug);
        }

        [Event(EventIds.MaxOutboundConnectionsPerEndpointExceeded, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void MaxOutboundConnectionsPerEndpointExceeded(string data1, string AppDomain)
        {
            WriteEvent(EventIds.MaxOutboundConnectionsPerEndpointExceeded, data1, AppDomain);
        }

        [NonEvent]
        public void MaxOutboundConnectionsPerEndpointExceeded(string data1)
        {
            MaxOutboundConnectionsPerEndpointExceeded(data1, "");
        }

        public bool OutboundConnectionsPerEndpointRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.OutboundConnectionsPerEndpointRatio, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Outbound connections per endpoint ratio: {0}/{1}")]
        public void OutboundConnectionsPerEndpointRatio(int cur, int max, string AppDomain)
        {
            WriteEvent(EventIds.OutboundConnectionsPerEndpointRatio, cur, max, AppDomain);
        }

        [NonEvent]
        public void OutboundConnectionsPerEndpointRatio(int cur, int max)
        {
            OutboundConnectionsPerEndpointRatio(cur, max, "");
        }

        public bool MaxSessionSizeReachedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.MaxSessionSizeReached, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void MaxSessionSizeReached(string data1, string AppDomain)
        {
            WriteEvent(EventIds.MaxSessionSizeReached, data1, AppDomain);
        }

        [NonEvent]
        public void MaxSessionSizeReached(string data1)
        {
            MaxSessionSizeReached(data1, "");
        }

        public bool ReadPoolMissIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.ReadPoolMiss, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Created new '{0}'")]
        public void ReadPoolMiss(string itemTypeName, string AppDomain)
        {
            WriteEvent(EventIds.ReadPoolMiss, itemTypeName, AppDomain);
        }

        [NonEvent]
        public void ReadPoolMiss(string itemTypeName)
        {
            ReadPoolMiss(itemTypeName, "");
        }

        public bool WritePoolMissIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.WritePoolMiss, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Created new '{0}'")]
        public void WritePoolMiss(string itemTypeName, string AppDomain)
        {
            WriteEvent(EventIds.WritePoolMiss, itemTypeName, AppDomain);
        }

        [NonEvent]
        public void WritePoolMiss(string itemTypeName)
        {
            WritePoolMiss(itemTypeName, "");
        }

        public bool MessageReadByEncoderIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.MessageReadByEncoder, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.MessageDecoding,
            Keywords = Keywords.Channel,
            Message = "A message with size '{0}' bytes was read by the encoder.")]
        public void MessageReadByEncoder(int Size, string EventSource, string AppDomain)
        {
            WriteEvent(EventIds.MessageReadByEncoder, Size, EventSource, AppDomain);
        }

        [NonEvent]
        public void MessageReadByEncoder(EventTraceActivity eventTraceActivity, int Size, object source)
        {
            TracePayload payload = FxTrace.Trace.GetSerializedPayload(source, null, null);
            SetActivityId(eventTraceActivity);
            MessageReadByEncoder(Size, payload.EventSource, "");
        }

        public bool MessageWrittenByEncoderIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.MessageWrittenByEncoder, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.MessageEncoding,
            Keywords = Keywords.Channel,
            Message = "A message with size '{0}' bytes was written by the encoder.")]
        public void MessageWrittenByEncoder(int Size, string EventSource, string AppDomain)
        {
            WriteEvent(EventIds.MessageWrittenByEncoder, Size, EventSource, AppDomain);
        }

        [NonEvent]
        public void MessageWrittenByEncoder(EventTraceActivity eventTraceActivity, int Size, object source)
        {
            TracePayload payload = FxTrace.Trace.GetSerializedPayload(source, null, null);
            SetActivityId(eventTraceActivity);
            MessageWrittenByEncoder(Size, payload.EventSource, "");
        }

        public bool SessionIdleTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.SessionIdleTimeout, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Timeout,
            Keywords = Keywords.ServiceModel,
            Message = "Session aborting for idle channel to uri:'{0}'.")]
        public void SessionIdleTimeout(string RemoteAddress, string AppDomain)
        {
            WriteEvent(EventIds.SessionIdleTimeout, RemoteAddress, AppDomain);
        }

        [NonEvent]
        public void SessionIdleTimeout(string RemoteAddress)
        {
            SessionIdleTimeout(RemoteAddress, "");
        }

        public bool ConnectionPoolMissIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ConnectionPoolMiss, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ConnectionPooling,
            Keywords = Keywords.Channel,
            Message = "Pool for {0} has no available connection and {1} busy connections.")]
        public void ConnectionPoolMiss(string PoolKey, int busy, string AppDomain)
        {
            WriteEvent(EventIds.ConnectionPoolMiss, PoolKey, busy, AppDomain);
        }

        [NonEvent]
        public void ConnectionPoolMiss(string PoolKey, int busy)
        {
            ConnectionPoolMiss(PoolKey, busy, "");
        }

        public bool DispatchFormatterDeserializeRequestStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.DispatchFormatterDeserializeRequestStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.FormatterDeserializeRequest,
            Keywords = Keywords.ServiceModel,
            Message = "Dispatcher started deserialization the request message.")]
        public void DispatchFormatterDeserializeRequestStart(string AppDomain)
        {
            WriteEvent(EventIds.DispatchFormatterDeserializeRequestStart, AppDomain);
        }

        [NonEvent]
        public void DispatchFormatterDeserializeRequestStart(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            DispatchFormatterDeserializeRequestStart("");
        }

        public bool DispatchFormatterDeserializeRequestStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.DispatchFormatterDeserializeRequestStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.FormatterDeserializeRequest,
            Keywords = Keywords.ServiceModel,
            Message = "Dispatcher completed deserialization the request message.")]
        public void DispatchFormatterDeserializeRequestStop(string AppDomain)
        {
            WriteEvent(EventIds.DispatchFormatterDeserializeRequestStop, AppDomain);
        }

        [NonEvent]
        public void DispatchFormatterDeserializeRequestStop(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            DispatchFormatterDeserializeRequestStop("");
        }

        public bool DispatchFormatterSerializeReplyStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.DispatchFormatterSerializeReplyStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.FormatterSerializeReply,
            Keywords = Keywords.ServiceModel,
            Message = "Dispatcher started serialization of the reply message.")]
        public void DispatchFormatterSerializeReplyStart(string AppDomain)
        {
            WriteEvent(EventIds.DispatchFormatterSerializeReplyStart, AppDomain);
        }

        [NonEvent]
        public void DispatchFormatterSerializeReplyStart(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            DispatchFormatterSerializeReplyStart("");
        }

        public bool DispatchFormatterSerializeReplyStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.DispatchFormatterSerializeReplyStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.FormatterSerializeReply,
            Keywords = Keywords.ServiceModel,
            Message = "Dispatcher completed serialization of the reply message.")]
        public void DispatchFormatterSerializeReplyStop(string AppDomain)
        {
            WriteEvent(EventIds.DispatchFormatterSerializeReplyStop, AppDomain);
        }

        [NonEvent]
        public void DispatchFormatterSerializeReplyStop(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            DispatchFormatterSerializeReplyStop("");
        }

        public bool ClientFormatterSerializeRequestStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ClientFormatterSerializeRequestStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.FormatterSerializeRequest,
            Keywords = Keywords.ServiceModel,
            Message = "Client request serialization started.")]
        public void ClientFormatterSerializeRequestStart(string AppDomain)
        {
            WriteEvent(EventIds.ClientFormatterSerializeRequestStart, AppDomain);
        }

        [NonEvent]
        public void ClientFormatterSerializeRequestStart(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            ClientFormatterSerializeRequestStart("");
        }

        public bool ClientFormatterSerializeRequestStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ClientFormatterSerializeRequestStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.FormatterSerializeRequest,
            Keywords = Keywords.ServiceModel,
            Message = "Client completed serialization of the request message.")]
        public void ClientFormatterSerializeRequestStop(string AppDomain)
        {
            WriteEvent(EventIds.ClientFormatterSerializeRequestStop, AppDomain);
        }

        [NonEvent]
        public void ClientFormatterSerializeRequestStop(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            ClientFormatterSerializeRequestStop("");
        }

        public bool ClientFormatterDeserializeReplyStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ClientFormatterDeserializeReplyStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.FormatterDeserializeReply,
            Keywords = Keywords.ServiceModel,
            Message = "Client started deserializing the reply message.")]
        public void ClientFormatterDeserializeReplyStart(string AppDomain)
        {
            WriteEvent(EventIds.ClientFormatterDeserializeReplyStart, AppDomain);
        }

        [NonEvent]
        public void ClientFormatterDeserializeReplyStart(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            ClientFormatterDeserializeReplyStart("");
        }

        public bool ClientFormatterDeserializeReplyStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ClientFormatterDeserializeReplyStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.FormatterDeserializeReply,
            Keywords = Keywords.ServiceModel,
            Message = "Client completed deserializing the reply message.")]
        public void ClientFormatterDeserializeReplyStop(string AppDomain)
        {
            WriteEvent(EventIds.ClientFormatterDeserializeReplyStop, AppDomain);
        }

        [NonEvent]
        public void ClientFormatterDeserializeReplyStop(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            ClientFormatterDeserializeReplyStop("");
        }

        public bool GetServiceInstanceStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.GetServiceInstanceStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ServiceInstance,
            Keywords = Keywords.ServiceModel,
            Message = "Service instance retrieval started.")]
        public void GetServiceInstanceStart(string AppDomain)
        {
            WriteEvent(EventIds.GetServiceInstanceStart, AppDomain);
        }

        [NonEvent]
        public void GetServiceInstanceStart(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            GetServiceInstanceStart("");
        }

        public bool GetServiceInstanceStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.GetServiceInstanceStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ServiceInstance,
            Keywords = Keywords.ServiceModel,
            Message = "Service instance retrieved.")]
        public void GetServiceInstanceStop(string AppDomain)
        {
            WriteEvent(EventIds.GetServiceInstanceStop, AppDomain);
        }

        [NonEvent]
        public void GetServiceInstanceStop(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            GetServiceInstanceStop("");
        }

        public bool ChannelReceiveStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ChannelReceiveStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ChannelReceive,
            Keywords = Keywords.Channel,
            Message = "ChannelHandlerId:{0} - Message receive loop started.")]
        public void ChannelReceiveStart(int ChannelId, string AppDomain)
        {
            WriteEvent(EventIds.ChannelReceiveStart, ChannelId, AppDomain);
        }

        [NonEvent]
        public void ChannelReceiveStart(EventTraceActivity eventTraceActivity, int ChannelId)
        {
            SetActivityId(eventTraceActivity);
            ChannelReceiveStart(ChannelId, "");
        }

        public bool ChannelReceiveStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ChannelReceiveStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ChannelReceive,
            Keywords = Keywords.Channel,
            Message = "ChannelHandlerId:{0} - Message receive loop stopped.")]
        public void ChannelReceiveStop(int ChannelId, string AppDomain)
        {
            WriteEvent(EventIds.ChannelReceiveStop, ChannelId, AppDomain);
        }

        [NonEvent]
        public void ChannelReceiveStop(EventTraceActivity eventTraceActivity, int ChannelId)
        {
            SetActivityId(eventTraceActivity);
            ChannelReceiveStop(ChannelId, "");
        }

        public bool ChannelFactoryCreatedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ChannelFactoryCreated, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ChannelFactoryCreate,
            Keywords = Keywords.ServiceModel,
            Message = "ChannelFactory created .")]
        public void ChannelFactoryCreated(string EventSource, string AppDomain)
        {
            WriteEvent(EventIds.ChannelFactoryCreated, EventSource, AppDomain);
        }

        [NonEvent]
        public void ChannelFactoryCreated(object source)
        {
            TracePayload payload = FxTrace.Trace.GetSerializedPayload(source, null, null);
            ChannelFactoryCreated(payload.EventSource, "");
        }

        public bool ListenerOpenStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ListenerOpenStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ListenerOpen,
            Keywords = Keywords.Channel,
            Message = "Listener opening for '{0}'.")]
        public void ListenerOpenStart(string Uri, string AppDomain)
        {
            WriteEvent(EventIds.ListenerOpenStart, Uri, AppDomain);
        }

        [NonEvent]
        public void ListenerOpenStart(EventTraceActivity eventTraceActivity, string Uri, Guid relatedActivityId)
        {
            TransferActivityId(eventTraceActivity);
            ListenerOpenStart(Uri, "");
        }

        public bool ListenerOpenStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ListenerOpenStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ListenerOpen,
            Keywords = Keywords.Channel,
            Message = "Listener open completed.")]
        public void ListenerOpenStop(string AppDomain)
        {
            WriteEvent(EventIds.ListenerOpenStop, AppDomain);
        }

        [NonEvent]
        public void ListenerOpenStop(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            ListenerOpenStop("");
        }

        public bool SecurityIdentityVerificationFailureIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Security, EventChannel.Analytic);
        }

        [Event(EventIds.SecurityIdentityVerificationFailure, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.SecurityVerification,
            Keywords = Keywords.Security,
            Message = "Security verification failed.")]
        public void SecurityIdentityVerificationFailure(string AppDomain)
        {
            WriteEvent(EventIds.SecurityIdentityVerificationFailure, AppDomain);
        }

        [NonEvent]
        public void SecurityIdentityVerificationFailure(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            SecurityIdentityVerificationFailure("");
        }

        public bool SslOnInitiateUpgradeIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Analytic);
        }

        [Event(EventIds.SslOnInitiateUpgrade, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.SessionUpgradeInitiate, Task = Tasks.SessionUpgrade,
            Keywords = Keywords.Security,
            Message = "SslOnAcceptUpgradeStart")]
        public void SslOnInitiateUpgrade(string AppDomain)
        {
            WriteEvent(EventIds.SslOnInitiateUpgrade, AppDomain);
        }

        [NonEvent]
        public void SslOnInitiateUpgrade()
        {
            SslOnInitiateUpgrade("");
        }

        public bool SslOnAcceptUpgradeIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Analytic);
        }

        [Event(EventIds.SslOnAcceptUpgrade, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.SessionUpgradeAccept, Task = Tasks.SessionUpgrade,
            Keywords = Keywords.Security,
            Message = "SslOnAcceptUpgradeStop")]
        public void SslOnAcceptUpgrade(string AppDomain)
        {
            WriteEvent(EventIds.SslOnAcceptUpgrade, AppDomain);
        }

        [NonEvent]
        public void SslOnAcceptUpgrade(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            SslOnAcceptUpgrade("");
        }

        public bool BinaryMessageEncodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.BinaryMessageEncodingStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.MessageEncoding,
            Keywords = Keywords.Channel,
            Message = "BinaryMessageEncoder started encoding the message.")]
        public void BinaryMessageEncodingStart(string AppDomain)
        {
            WriteEvent(EventIds.BinaryMessageEncodingStart, AppDomain);
        }

        [NonEvent]
        public void BinaryMessageEncodingStart(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            BinaryMessageEncodingStart("");
        }

        public bool TextMessageEncodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.TextMessageEncodingStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.MessageEncoding,
            Keywords = Keywords.Channel,
            Message = "TextMessageEncoder started encoding the message.")]
        public void TextMessageEncodingStart(string AppDomain)
        {
            WriteEvent(EventIds.TextMessageEncodingStart, AppDomain);
        }

        [NonEvent]
        public void TextMessageEncodingStart(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            TextMessageEncodingStart("");
        }

        public bool BinaryMessageDecodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.BinaryMessageDecodingStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.MessageDecoding,
            Keywords = Keywords.Channel,
            Message = "BinaryMessageEncoder started decoding the message.")]
        public void BinaryMessageDecodingStart(string AppDomain)
        {
            WriteEvent(EventIds.BinaryMessageDecodingStart, AppDomain);
        }

        [NonEvent]
        public void BinaryMessageDecodingStart()
        {
            BinaryMessageDecodingStart("");
        }

        public bool TextMessageDecodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.TextMessageDecodingStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.MessageDecoding,
            Keywords = Keywords.Channel,
            Message = "TextMessageEncoder started decoding the message.")]
        public void TextMessageDecodingStart(string AppDomain)
        {
            WriteEvent(EventIds.TextMessageDecodingStart, AppDomain);
        }

        [NonEvent]
        public void TextMessageDecodingStart()
        {
            TextMessageDecodingStart("");
        }

        public bool SocketReadStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.TCP, EventChannel.Debug);
        }

        [Event(EventIds.SocketReadStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.TransportReceive,
            Keywords = Keywords.TCP,
            Message = "SocketId:{0} read '{1}' bytes read from '{2}'.")]
        public void SocketReadStop(int SocketId, int Size, string Endpoint, string AppDomain)
        {
            WriteEvent(EventIds.SocketReadStop, SocketId, Size, Endpoint, AppDomain);
        }

        [NonEvent]
        public void SocketReadStop(int SocketId, int Size, string Endpoint)
        {
            SocketReadStop(SocketId, Size, Endpoint, "");
        }

        public bool SocketAsyncReadStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.TCP, EventChannel.Debug);
        }

        [Event(EventIds.SocketAsyncReadStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.TransportReceive,
            Keywords = Keywords.TCP,
            Message = "SocketId:{0} read '{1}' bytes read from '{2}'.")]
        public void SocketAsyncReadStop(int SocketId, int Size, string Endpoint, string AppDomain)
        {
            WriteEvent(EventIds.SocketAsyncReadStop, SocketId, Size, Endpoint, AppDomain);
        }

        [NonEvent]
        public void SocketAsyncReadStop(int SocketId, int Size, string Endpoint)
        {
            SocketAsyncReadStop(SocketId, Size, Endpoint, "");
        }

        public bool SocketWriteStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.TCP, EventChannel.Debug);
        }

        [Event(EventIds.SocketWriteStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportSend,
            Keywords = Keywords.TCP,
            Message = "SocketId:{0} writing '{1}' bytes to '{2}'.")]
        public void SocketWriteStart(int SocketId, int Size, string Endpoint, string AppDomain)
        {
            WriteEvent(EventIds.SocketWriteStart, SocketId, Size, Endpoint, AppDomain);
        }

        [NonEvent]
        public void SocketWriteStart(int SocketId, int Size, string Endpoint)
        {
            SocketWriteStart(SocketId, Size, Endpoint, "");
        }

        public bool SocketAsyncWriteStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.TCP, EventChannel.Debug);
        }

        [Event(EventIds.SocketAsyncWriteStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportSend,
            Keywords = Keywords.TCP,
            Message = "SocketId:{0} writing '{1}' bytes to '{2}'.")]
        public void SocketAsyncWriteStart(int SocketId, int Size, string Endpoint, string AppDomain)
        {
            WriteEvent(EventIds.SocketAsyncWriteStart, SocketId, Size, Endpoint, AppDomain);
        }

        [NonEvent]
        public void SocketAsyncWriteStart(int SocketId, int Size, string Endpoint)
        {
            SocketAsyncWriteStart(SocketId, Size, Endpoint, "");
        }

        public bool WindowsStreamSecurityOnInitiateUpgradeIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Analytic);
        }

        [Event(EventIds.WindowsStreamSecurityOnInitiateUpgrade, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.SessionUpgradeInitiate, Task = Tasks.SessionUpgrade,
            Keywords = Keywords.Security,
            Message = "WindowsStreamSecurity initiating security upgrade.")]
        public void WindowsStreamSecurityOnInitiateUpgrade(string AppDomain)
        {
            WriteEvent(EventIds.WindowsStreamSecurityOnInitiateUpgrade, AppDomain);
        }

        [NonEvent]
        public void WindowsStreamSecurityOnInitiateUpgrade()
        {
            WindowsStreamSecurityOnInitiateUpgrade("");
        }

        public bool WindowsStreamSecurityOnAcceptUpgradeIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Analytic);
        }

        [Event(EventIds.WindowsStreamSecurityOnAcceptUpgrade, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.SessionUpgradeAccept, Task = Tasks.SessionUpgrade,
            Keywords = Keywords.Security,
            Message = "Windows streaming security on accepting upgrade.")]
        public void WindowsStreamSecurityOnAcceptUpgrade(string AppDomain)
        {
            WriteEvent(EventIds.WindowsStreamSecurityOnAcceptUpgrade, AppDomain);
        }

        [NonEvent]
        public void WindowsStreamSecurityOnAcceptUpgrade(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            WindowsStreamSecurityOnAcceptUpgrade("");
        }

        public bool StreamedMessageReadByEncoderIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.StreamedMessageReadByEncoder, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.MessageDecoding,
            Keywords = Keywords.Channel,
            Message = "A streamed message was read by the encoder.")]
        public void StreamedMessageReadByEncoder(string AppDomain)
        {
            WriteEvent(EventIds.StreamedMessageReadByEncoder, AppDomain);
        }

        [NonEvent]
        public void StreamedMessageReadByEncoder(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            StreamedMessageReadByEncoder("");
        }

        public bool StreamedMessageWrittenByEncoderIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.StreamedMessageWrittenByEncoder, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.MessageEncoding,
            Keywords = Keywords.Channel,
            Message = "A streamed message was written by the encoder.")]
        public void StreamedMessageWrittenByEncoder(string AppDomain)
        {
            WriteEvent(EventIds.StreamedMessageWrittenByEncoder, AppDomain);
        }

        [NonEvent]
        public void StreamedMessageWrittenByEncoder(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            StreamedMessageWrittenByEncoder("");
        }

        public bool WebSocketConnectionRequestSendStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketConnectionRequestSendStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocket connection request to '{0}' send start.")]
        public void WebSocketConnectionRequestSendStart(string remoteAddress, string AppDomain)
        {
            WriteEvent(EventIds.WebSocketConnectionRequestSendStart, remoteAddress, AppDomain);
        }

        [NonEvent]
        public void WebSocketConnectionRequestSendStart(EventTraceActivity eventTraceActivity, string remoteAddress)
        {
            SetActivityId(eventTraceActivity);
            WebSocketConnectionRequestSendStart(remoteAddress, "");
        }

        public bool WebSocketConnectionRequestSendStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketConnectionRequestSendStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} connection request sent.")]
        public void WebSocketConnectionRequestSendStop(int websocketId, string AppDomain)
        {
            WriteEvent(EventIds.WebSocketConnectionRequestSendStop, websocketId, AppDomain);
        }

        [NonEvent]
        public void WebSocketConnectionRequestSendStop(EventTraceActivity eventTraceActivity, int websocketId)
        {
            SetActivityId(eventTraceActivity);
            WebSocketConnectionRequestSendStop(websocketId, "");
        }

        public bool WebSocketConnectionFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.WebSocketConnectionFailed, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocket connection request failed: '{0}'")]
        public void WebSocketConnectionFailed(string errorMessage, string AppDomain)
        {
            WriteEvent(EventIds.WebSocketConnectionFailed, errorMessage, AppDomain);
        }

        [NonEvent]
        public void WebSocketConnectionFailed(EventTraceActivity eventTraceActivity, string errorMessage)
        {
            SetActivityId(eventTraceActivity);
            WebSocketConnectionFailed(errorMessage, "");
        }

        public bool WebSocketConnectionAbortedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.WebSocketConnectionAborted, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} connection is aborted.")]
        public void WebSocketConnectionAborted(int websocketId, string AppDomain)
        {
            WriteEvent(EventIds.WebSocketConnectionAborted, websocketId, AppDomain);
        }

        [NonEvent]
        public void WebSocketConnectionAborted(EventTraceActivity eventTraceActivity, int websocketId)
        {
            SetActivityId(eventTraceActivity);
            WebSocketConnectionAborted(websocketId, "");
        }

        public bool WebSocketAsyncWriteStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketAsyncWriteStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportSend,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} writing '{1}' bytes to '{2}'.")]
        public void WebSocketAsyncWriteStart(int websocketId, int byteCount, string remoteAddress, string AppDomain)
        {
            WriteEvent(EventIds.WebSocketAsyncWriteStart, websocketId, byteCount, remoteAddress, AppDomain);
        }

        [NonEvent]
        public void WebSocketAsyncWriteStart(int websocketId, int byteCount, string remoteAddress)
        {
            WebSocketAsyncWriteStart(websocketId, byteCount, remoteAddress, "");
        }

        public bool WebSocketAsyncWriteStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketAsyncWriteStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.TransportSend,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} asynchronous write stop.")]
        public void WebSocketAsyncWriteStop(int websocketId, string AppDomain)
        {
            WriteEvent(EventIds.WebSocketAsyncWriteStop, websocketId, AppDomain);
        }

        [NonEvent]
        public void WebSocketAsyncWriteStop(int websocketId)
        {
            WebSocketAsyncWriteStop(websocketId, "");
        }

        public bool WebSocketAsyncReadStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketAsyncReadStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} read start.")]
        public void WebSocketAsyncReadStart(int websocketId, string AppDomain)
        {
            WriteEvent(EventIds.WebSocketAsyncReadStart, websocketId, AppDomain);
        }

        [NonEvent]
        public void WebSocketAsyncReadStart(int websocketId)
        {
            WebSocketAsyncReadStart(websocketId, "");
        }

        public bool WebSocketAsyncReadStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketAsyncReadStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} read '{1}' bytes from '{2}'.")]
        public void WebSocketAsyncReadStop(int websocketId, int byteCount, string remoteAddress, string AppDomain)
        {
            WriteEvent(EventIds.WebSocketAsyncReadStop, websocketId, byteCount, remoteAddress, AppDomain);
        }

        [NonEvent]
        public void WebSocketAsyncReadStop(int websocketId, int byteCount, string remoteAddress)
        {
            WebSocketAsyncReadStop(websocketId, byteCount, remoteAddress, "");
        }

        public bool WebSocketCloseSentIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketCloseSent, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} sending close message to '{1}' with close status '{2}'.")]
        public void WebSocketCloseSent(int websocketId, string remoteAddress, string closeStatus, string AppDomain)
        {
            WriteEvent(EventIds.WebSocketCloseSent, websocketId, remoteAddress, closeStatus, AppDomain);
        }

        [NonEvent]
        public void WebSocketCloseSent(int websocketId, string remoteAddress, string closeStatus)
        {
            WebSocketCloseSent(websocketId, remoteAddress, closeStatus, "");
        }

        public bool WebSocketCloseOutputSentIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketCloseOutputSent, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} sending close output message to '{1}' with close status '{2}'.")]
        public void WebSocketCloseOutputSent(int websocketId, string remoteAddress, string closeStatus, string AppDomain)
        {
            WriteEvent(EventIds.WebSocketCloseOutputSent, websocketId, remoteAddress, closeStatus, AppDomain);
        }

        [NonEvent]
        public void WebSocketCloseOutputSent(int websocketId, string remoteAddress, string closeStatus)
        {
            WebSocketCloseOutputSent(websocketId, remoteAddress, closeStatus, "");
        }

        public bool WebSocketConnectionClosedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketConnectionClosed, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} connection closed.")]
        public void WebSocketConnectionClosed(int websocketId, string AppDomain)
        {
            WriteEvent(EventIds.WebSocketConnectionClosed, websocketId, AppDomain);
        }

        [NonEvent]
        public void WebSocketConnectionClosed(int websocketId)
        {
            WebSocketConnectionClosed(websocketId, "");
        }

        public bool WebSocketCloseStatusReceivedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketCloseStatusReceived, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} connection close message received with status '{1}'.")]
        public void WebSocketCloseStatusReceived(int websocketId, string closeStatus, string AppDomain)
        {
            WriteEvent(EventIds.WebSocketCloseStatusReceived, websocketId, closeStatus, AppDomain);
        }

        [NonEvent]
        public void WebSocketCloseStatusReceived(int websocketId, string closeStatus)
        {
            WebSocketCloseStatusReceived(websocketId, closeStatus, "");
        }

        public bool WebSocketCreateClientWebSocketWithFactoryIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketCreateClientWebSocketWithFactory, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "Creating the client WebSocket with a factory of type '{0}'.")]
        public void WebSocketCreateClientWebSocketWithFactory(string clientWebSocketFactoryType, string AppDomain)
        {
            WriteEvent(EventIds.WebSocketCreateClientWebSocketWithFactory, clientWebSocketFactoryType, AppDomain);
        }

        [NonEvent]
        public void WebSocketCreateClientWebSocketWithFactory(EventTraceActivity eventTraceActivity, string clientWebSocketFactoryType)
        {
            SetActivityId(eventTraceActivity);
            WebSocketCreateClientWebSocketWithFactory(clientWebSocketFactoryType, "");
        }

        public bool TokenValidationStartedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.TokenValidationStarted, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.TokenValidation,
            Keywords = Keywords.Security,
            Message = "SecurityToken (type '{0}' and id '{1}') validation started.")]
        public void TokenValidationStarted(string tokenType, string tokenID, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.TokenValidationStarted, tokenType, tokenID, HostReference, AppDomain);
        }

        [NonEvent]
        public void TokenValidationStarted(EventTraceActivity eventTraceActivity, string tokenType, string tokenID)
        {
            SetActivityId(eventTraceActivity);
            TokenValidationStarted(tokenType, tokenID, "", "");
        }

        public bool TokenValidationSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.TokenValidationSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.TokenValidation,
            Keywords = Keywords.Security,
            Message = "SecurityToken (type '{0}' and id '{1}') validation succeeded.")]
        public void TokenValidationSuccess(string tokenType, string tokenID, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.TokenValidationSuccess, tokenType, tokenID, HostReference, AppDomain);
        }

        [NonEvent]
        public void TokenValidationSuccess(EventTraceActivity eventTraceActivity, string tokenType, string tokenID)
        {
            SetActivityId(eventTraceActivity);
            TokenValidationSuccess(tokenType, tokenID, "", "");
        }

        public bool TokenValidationFailureIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.TokenValidationFailure, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.TokenValidation,
            Keywords = Keywords.Security,
            Message = "SecurityToken (type '{0}' and id '{1}') validation failed. {2}")]
        public void TokenValidationFailure(string tokenType, string tokenID, string errorMessage, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.TokenValidationFailure, tokenType, tokenID, errorMessage, HostReference, AppDomain);
        }

        [NonEvent]
        public void TokenValidationFailure(EventTraceActivity eventTraceActivity, string tokenType, string tokenID, string errorMessage)
        {
            SetActivityId(eventTraceActivity);
            TokenValidationFailure(tokenType, tokenID, errorMessage, "", "");
        }

        public bool HandledExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(EventIds.HandledException, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Handling an exception.  Exception details: {0}")]
        public void HandledException(string data1, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.HandledException, data1, SerializedException, AppDomain);
        }

        [NonEvent]
        public void HandledException(string data1, string SerializedException)
        {
            HandledException(data1, SerializedException, "");
        }

        public bool ShipAssertExceptionMessageIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(EventIds.ShipAssertExceptionMessage, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "An unexpected failure occurred. Applications should not attempt to handle this error. For diagnostic purposes, this English message is associated with the failure: {0}.")]
        public void ShipAssertExceptionMessage(string data1, string AppDomain)
        {
            WriteEvent(EventIds.ShipAssertExceptionMessage, data1, AppDomain);
        }

        [NonEvent]
        public void ShipAssertExceptionMessage(string data1)
        {
            ShipAssertExceptionMessage(data1, "");
        }

        public bool ThrowingExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(EventIds.ThrowingException, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Throwing an exception. Source: {0}. Exception details: {1}")]
        public void ThrowingException(string data1, string data2, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.ThrowingException, data1, data2, SerializedException, AppDomain);
        }

        [NonEvent]
        public void ThrowingException(string data1, string data2, string SerializedException)
        {
            ThrowingException(data1, data2, SerializedException, "");
        }

        public bool UnhandledExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Critical, Keywords.Infrastructure, EventChannel.Operational);
        }

        [Event(EventIds.UnhandledException, Level = EventLevel.Critical, Channel = EventChannel.Operational, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Unhandled exception.  Exception details: {0}")]
        public void UnhandledException(string data1, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.UnhandledException, data1, SerializedException, AppDomain);
        }

        [NonEvent]
        public void UnhandledException(string data1, string SerializedException)
        {
            UnhandledException(data1, SerializedException, "");
        }

        public bool TraceCodeEventLogCriticalIsEnabled()
        {
            return base.IsEnabled(EventLevel.Critical, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(EventIds.TraceCodeEventLogCritical, Level = EventLevel.Critical, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Wrote to the EventLog.")]
        public void TraceCodeEventLogCritical(string ExtendedData, string AppDomain)
        {
            WriteEvent(EventIds.TraceCodeEventLogCritical, ExtendedData, AppDomain);
        }

        [NonEvent]
        public void TraceCodeEventLogCritical(string ExtendedData)
        {
            TraceCodeEventLogCritical(ExtendedData, "");
        }

        public bool TraceCodeEventLogErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(EventIds.TraceCodeEventLogError, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Wrote to the EventLog.")]
        public void TraceCodeEventLogError(string ExtendedData, string AppDomain)
        {
            WriteEvent(EventIds.TraceCodeEventLogError, ExtendedData, AppDomain);
        }

        [NonEvent]
        public void TraceCodeEventLogError(string ExtendedData)
        {
            TraceCodeEventLogError(ExtendedData, "");
        }

        public bool TraceCodeEventLogInfoIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(EventIds.TraceCodeEventLogInfo, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Wrote to the EventLog.")]
        public void TraceCodeEventLogInfo(string ExtendedData, string AppDomain)
        {
            WriteEvent(EventIds.TraceCodeEventLogInfo, ExtendedData, AppDomain);
        }

        [NonEvent]
        public void TraceCodeEventLogInfo(string ExtendedData)
        {
            TraceCodeEventLogInfo(ExtendedData, "");
        }

        public bool TraceCodeEventLogVerboseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(EventIds.TraceCodeEventLogVerbose, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Wrote to the EventLog.")]
        public void TraceCodeEventLogVerbose(string ExtendedData, string AppDomain)
        {
            WriteEvent(EventIds.TraceCodeEventLogVerbose, ExtendedData, AppDomain);
        }

        [NonEvent]
        public void TraceCodeEventLogVerbose(string ExtendedData)
        {
            TraceCodeEventLogVerbose(ExtendedData, "");
        }

        public bool TraceCodeEventLogWarningIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(EventIds.TraceCodeEventLogWarning, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Wrote to the EventLog.")]
        public void TraceCodeEventLogWarning(string ExtendedData, string AppDomain)
        {
            WriteEvent(EventIds.TraceCodeEventLogWarning, ExtendedData, AppDomain);
        }

        [NonEvent]
        public void TraceCodeEventLogWarning(string ExtendedData)
        {
            TraceCodeEventLogWarning(ExtendedData, "");
        }

        public bool HandledExceptionWarningIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(EventIds.HandledExceptionWarning, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Handling an exception. Exception details: {0}")]
        public void HandledExceptionWarning(string data1, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.HandledExceptionWarning, data1, SerializedException, AppDomain);
        }

        [NonEvent]
        public void HandledExceptionWarning(string data1, string SerializedException)
        {
            HandledExceptionWarning(data1, SerializedException, "");
        }

        public bool HandledExceptionErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Infrastructure, EventChannel.Operational);
        }

        [Event(EventIds.HandledExceptionError, Level = EventLevel.Error, Channel = EventChannel.Operational, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Handling an exception. Exception details: {0}")]
        public void HandledExceptionError(string data1, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.HandledExceptionError, data1, SerializedException, AppDomain);
        }

        [NonEvent]
        public void HandledExceptionError(string data1, string SerializedException)
        {
            HandledExceptionError(data1, SerializedException, "");
        }

        public bool HandledExceptionVerboseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(EventIds.HandledExceptionVerbose, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Handling an exception  Exception details: {0}")]
        public void HandledExceptionVerbose(string data1, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.HandledExceptionVerbose, data1, SerializedException, AppDomain);
        }

        [NonEvent]
        public void HandledExceptionVerbose(string data1, string SerializedException)
        {
            HandledExceptionVerbose(data1, SerializedException, "");
        }

        public bool ThrowingExceptionVerboseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(EventIds.ThrowingExceptionVerbose, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Throwing an exception. Source: {0}. Exception details: {1}")]
        public void ThrowingExceptionVerbose(string data1, string data2, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.ThrowingExceptionVerbose, data1, data2, SerializedException, AppDomain);
        }

        [NonEvent]
        public void ThrowingExceptionVerbose(string data1, string data2, string SerializedException)
        {
            ThrowingExceptionVerbose(data1, data2, SerializedException, "");
        }

        public bool EtwUnhandledExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Critical, Keywords.Infrastructure, EventChannel.Operational);
        }

        [Event(EventIds.EtwUnhandledException, Level = EventLevel.Critical, Channel = EventChannel.Operational, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Unhandled exception. Exception details: {0}")]
        public void EtwUnhandledException(string data1, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.EtwUnhandledException, data1, SerializedException, AppDomain);
        }

        [NonEvent]
        public void EtwUnhandledException(string data1, string SerializedException)
        {
            EtwUnhandledException(data1, SerializedException, "");
        }

        public bool ThrowingEtwExceptionVerboseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(EventIds.ThrowingEtwExceptionVerbose, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Throwing an exception. Source: {0}. Exception details: {1}")]
        public void ThrowingEtwExceptionVerbose(string data1, string data2, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.ThrowingEtwExceptionVerbose, data1, data2, SerializedException, AppDomain);
        }

        [NonEvent]
        public void ThrowingEtwExceptionVerbose(string data1, string data2, string SerializedException)
        {
            ThrowingEtwExceptionVerbose(data1, data2, SerializedException, "");
        }

        public bool ThrowingEtwExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(EventIds.ThrowingEtwException, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Throwing an exception. Source: {0}. Exception details: {1}")]
        public void ThrowingEtwException(string data1, string data2, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.ThrowingEtwException, data1, data2, SerializedException, AppDomain);
        }

        [NonEvent]
        public void ThrowingEtwException(string data1, string data2, string SerializedException)
        {
            ThrowingEtwException(data1, data2, SerializedException, "");
        }

        [NonEvent]
        private void SetActivityId(EventTraceActivity eventTraceActivity)
        {
            if (eventTraceActivity != null)
            {
                SetCurrentThreadActivityId(eventTraceActivity.ActivityId);
            }
        }

        [NonEvent]
        private void TransferActivityId(EventTraceActivity eventTraceActivity)
        {
            if (eventTraceActivity != null)
            {
                Guid oldGuid;
                SetCurrentThreadActivityId(eventTraceActivity.ActivityId, out oldGuid);
            }
        }

        #region Keywords / Tasks / Opcodes

        public class EventIds
        {
            public const int WorkflowInstanceRecord = 100;
            public const int WorkflowInstanceUnhandledExceptionRecord = 101;
            public const int WorkflowInstanceAbortedRecord = 102;
            public const int ActivityStateRecord = 103;
            public const int ActivityScheduledRecord = 104;
            public const int FaultPropagationRecord = 105;
            public const int CancelRequestedRecord = 106;
            public const int BookmarkResumptionRecord = 107;
            public const int CustomTrackingRecordInfo = 108;
            public const int CustomTrackingRecordWarning = 110;
            public const int CustomTrackingRecordError = 111;
            public const int WorkflowInstanceSuspendedRecord = 112;
            public const int WorkflowInstanceTerminatedRecord = 113;
            public const int WorkflowInstanceRecordWithId = 114;
            public const int WorkflowInstanceAbortedRecordWithId = 115;
            public const int WorkflowInstanceSuspendedRecordWithId = 116;
            public const int WorkflowInstanceTerminatedRecordWithId = 117;
            public const int WorkflowInstanceUnhandledExceptionRecordWithId = 118;
            public const int WorkflowInstanceUpdatedRecord = 119;
            public const int BufferPoolAllocation = 131;
            public const int BufferPoolChangeQuota = 132;
            public const int ActionItemScheduled = 133;
            public const int ActionItemCallbackInvoked = 134;
            public const int ClientMessageInspectorAfterReceiveInvoked = 201;
            public const int ClientMessageInspectorBeforeSendInvoked = 202;
            public const int ClientParameterInspectorAfterCallInvoked = 203;
            public const int ClientParameterInspectorBeforeCallInvoked = 204;
            public const int OperationInvoked = 205;
            public const int ErrorHandlerInvoked = 206;
            public const int FaultProviderInvoked = 207;
            public const int MessageInspectorAfterReceiveInvoked = 208;
            public const int MessageInspectorBeforeSendInvoked = 209;
            public const int MessageThrottleExceeded = 210;
            public const int ParameterInspectorAfterCallInvoked = 211;
            public const int ParameterInspectorBeforeCallInvoked = 212;
            public const int ServiceHostStarted = 213;
            public const int OperationCompleted = 214;
            public const int MessageReceivedByTransport = 215;
            public const int MessageSentByTransport = 216;
            public const int ClientOperationPrepared = 217;
            public const int ServiceChannelCallStop = 218;
            public const int ServiceException = 219;
            public const int MessageSentToTransport = 220;
            public const int MessageReceivedFromTransport = 221;
            public const int OperationFailed = 222;
            public const int OperationFaulted = 223;
            public const int MessageThrottleAtSeventyPercent = 224;
            public const int TraceCorrelationKeys = 225;
            public const int IdleServicesClosed = 226;
            public const int UserDefinedErrorOccurred = 301;
            public const int UserDefinedWarningOccurred = 302;
            public const int UserDefinedInformationEventOccured = 303;
            public const int StopSignpostEvent = 401;
            public const int StartSignpostEvent = 402;
            public const int SuspendSignpostEvent = 403;
            public const int ResumeSignpostEvent = 404;
            public const int StartSignpostEvent1 = 440;
            public const int StopSignpostEvent1 = 441;
            public const int MessageLogInfo = 451;
            public const int MessageLogWarning = 452;
            public const int TransferEmitted = 499;
            public const int CompilationStart = 501;
            public const int CompilationStop = 502;
            public const int ServiceHostFactoryCreationStart = 503;
            public const int ServiceHostFactoryCreationStop = 504;
            public const int CreateServiceHostStart = 505;
            public const int CreateServiceHostStop = 506;
            public const int HostedTransportConfigurationManagerConfigInitStart = 507;
            public const int HostedTransportConfigurationManagerConfigInitStop = 508;
            public const int ServiceHostOpenStart = 509;
            public const int ServiceHostOpenStop = 510;
            public const int WebHostRequestStart = 513;
            public const int WebHostRequestStop = 514;
            public const int CBAEntryRead = 601;
            public const int CBAMatchFound = 602;
            public const int AspNetRoutingService = 603;
            public const int AspNetRoute = 604;
            public const int IncrementBusyCount = 605;
            public const int DecrementBusyCount = 606;
            public const int ServiceChannelOpenStart = 701;
            public const int ServiceChannelOpenStop = 702;
            public const int ServiceChannelCallStart = 703;
            public const int ServiceChannelBeginCallStart = 704;
            public const int HttpSendMessageStart = 706;
            public const int HttpSendStop = 707;
            public const int HttpMessageReceiveStart = 708;
            public const int DispatchMessageStart = 709;
            public const int HttpContextBeforeProcessAuthentication = 710;
            public const int DispatchMessageBeforeAuthorization = 711;
            public const int DispatchMessageStop = 712;
            public const int ClientChannelOpenStart = 715;
            public const int ClientChannelOpenStop = 716;
            public const int HttpSendStreamedMessageStart = 717;
            public const int WorkflowApplicationCompleted = 1001;
            public const int WorkflowApplicationTerminated = 1002;
            public const int WorkflowInstanceCanceled = 1003;
            public const int WorkflowInstanceAborted = 1004;
            public const int WorkflowApplicationIdled = 1005;
            public const int WorkflowApplicationUnhandledException = 1006;
            public const int WorkflowApplicationPersisted = 1007;
            public const int WorkflowApplicationUnloaded = 1008;
            public const int ActivityScheduled = 1009;
            public const int ActivityCompleted = 1010;
            public const int ScheduleExecuteActivityWorkItem = 1011;
            public const int StartExecuteActivityWorkItem = 1012;
            public const int CompleteExecuteActivityWorkItem = 1013;
            public const int ScheduleCompletionWorkItem = 1014;
            public const int StartCompletionWorkItem = 1015;
            public const int CompleteCompletionWorkItem = 1016;
            public const int ScheduleCancelActivityWorkItem = 1017;
            public const int StartCancelActivityWorkItem = 1018;
            public const int CompleteCancelActivityWorkItem = 1019;
            public const int CreateBookmark = 1020;
            public const int ScheduleBookmarkWorkItem = 1021;
            public const int StartBookmarkWorkItem = 1022;
            public const int CompleteBookmarkWorkItem = 1023;
            public const int CreateBookmarkScope = 1024;
            public const int BookmarkScopeInitialized = 1025;
            public const int ScheduleTransactionContextWorkItem = 1026;
            public const int StartTransactionContextWorkItem = 1027;
            public const int CompleteTransactionContextWorkItem = 1028;
            public const int ScheduleFaultWorkItem = 1029;
            public const int StartFaultWorkItem = 1030;
            public const int CompleteFaultWorkItem = 1031;
            public const int ScheduleRuntimeWorkItem = 1032;
            public const int StartRuntimeWorkItem = 1033;
            public const int CompleteRuntimeWorkItem = 1034;
            public const int RuntimeTransactionSet = 1035;
            public const int RuntimeTransactionCompletionRequested = 1036;
            public const int RuntimeTransactionComplete = 1037;
            public const int EnterNoPersistBlock = 1038;
            public const int ExitNoPersistBlock = 1039;
            public const int InArgumentBound = 1040;
            public const int WorkflowApplicationPersistableIdle = 1041;
            public const int WorkflowActivityStart = 1101;
            public const int WorkflowActivityStop = 1102;
            public const int WorkflowActivitySuspend = 1103;
            public const int WorkflowActivityResume = 1104;
            public const int InvokeMethodIsStatic = 1124;
            public const int InvokeMethodIsNotStatic = 1125;
            public const int InvokedMethodThrewException = 1126;
            public const int InvokeMethodUseAsyncPattern = 1131;
            public const int InvokeMethodDoesNotUseAsyncPattern = 1132;
            public const int FlowchartStart = 1140;
            public const int FlowchartEmpty = 1141;
            public const int FlowchartNextNull = 1143;
            public const int FlowchartSwitchCase = 1146;
            public const int FlowchartSwitchDefault = 1147;
            public const int FlowchartSwitchCaseNotFound = 1148;
            public const int CompensationState = 1150;
            public const int SwitchCaseNotFound = 1223;
            public const int ChannelInitializationTimeout = 1400;
            public const int CloseTimeout = 1401;
            public const int IdleTimeout = 1402;
            public const int LeaseTimeout = 1403;
            public const int OpenTimeout = 1405;
            public const int ReceiveTimeout = 1406;
            public const int SendTimeout = 1407;
            public const int InactivityTimeout = 1409;
            public const int MaxReceivedMessageSizeExceeded = 1416;
            public const int MaxSentMessageSizeExceeded = 1417;
            public const int MaxOutboundConnectionsPerEndpointExceeded = 1418;
            public const int MaxPendingConnectionsExceeded = 1419;
            public const int ReaderQuotaExceeded = 1420;
            public const int NegotiateTokenAuthenticatorStateCacheExceeded = 1422;
            public const int NegotiateTokenAuthenticatorStateCacheRatio = 1423;
            public const int SecuritySessionRatio = 1424;
            public const int PendingConnectionsRatio = 1430;
            public const int ConcurrentCallsRatio = 1431;
            public const int ConcurrentSessionsRatio = 1432;
            public const int OutboundConnectionsPerEndpointRatio = 1433;
            public const int PendingMessagesPerChannelRatio = 1436;
            public const int ConcurrentInstancesRatio = 1438;
            public const int PendingAcceptsAtZero = 1439;
            public const int MaxSessionSizeReached = 1441;
            public const int ReceiveRetryCountReached = 1442;
            public const int MaxRetryCyclesExceededMsmq = 1443;
            public const int ReadPoolMiss = 1445;
            public const int WritePoolMiss = 1446;
            public const int WfMessageReceived = 1449;
            public const int WfMessageSent = 1450;
            public const int MaxRetryCyclesExceeded = 1451;
            public const int ExecuteWorkItemStart = 2021;
            public const int ExecuteWorkItemStop = 2022;
            public const int SendMessageChannelCacheMiss = 2023;
            public const int InternalCacheMetadataStart = 2024;
            public const int InternalCacheMetadataStop = 2025;
            public const int CompileVbExpressionStart = 2026;
            public const int CacheRootMetadataStart = 2027;
            public const int CacheRootMetadataStop = 2028;
            public const int CompileVbExpressionStop = 2029;
            public const int TryCatchExceptionFromTry = 2576;
            public const int TryCatchExceptionDuringCancelation = 2577;
            public const int TryCatchExceptionFromCatchOrFinally = 2578;
            public const int ReceiveContextCompleteFailed = 3300;
            public const int ReceiveContextAbandonFailed = 3301;
            public const int ReceiveContextFaulted = 3302;
            public const int ReceiveContextAbandonWithException = 3303;
            public const int ClientBaseCachedChannelFactoryCount = 3305;
            public const int ClientBaseChannelFactoryAgedOutofCache = 3306;
            public const int ClientBaseChannelFactoryCacheHit = 3307;
            public const int ClientBaseUsingLocalChannelFactory = 3308;
            public const int QueryCompositionExecuted = 3309;
            public const int DispatchFailed = 3310;
            public const int DispatchSuccessful = 3311;
            public const int MessageReadByEncoder = 3312;
            public const int MessageWrittenByEncoder = 3313;
            public const int SessionIdleTimeout = 3314;
            public const int SocketAcceptEnqueued = 3319;
            public const int SocketAccepted = 3320;
            public const int ConnectionPoolMiss = 3321;
            public const int DispatchFormatterDeserializeRequestStart = 3322;
            public const int DispatchFormatterDeserializeRequestStop = 3323;
            public const int DispatchFormatterSerializeReplyStart = 3324;
            public const int DispatchFormatterSerializeReplyStop = 3325;
            public const int ClientFormatterSerializeRequestStart = 3326;
            public const int ClientFormatterSerializeRequestStop = 3327;
            public const int ClientFormatterDeserializeReplyStart = 3328;
            public const int ClientFormatterDeserializeReplyStop = 3329;
            public const int SecurityNegotiationStart = 3330;
            public const int SecurityNegotiationStop = 3331;
            public const int SecurityTokenProviderOpened = 3332;
            public const int OutgoingMessageSecured = 3333;
            public const int IncomingMessageVerified = 3334;
            public const int GetServiceInstanceStart = 3335;
            public const int GetServiceInstanceStop = 3336;
            public const int ChannelReceiveStart = 3337;
            public const int ChannelReceiveStop = 3338;
            public const int ChannelFactoryCreated = 3339;
            public const int PipeConnectionAcceptStart = 3340;
            public const int PipeConnectionAcceptStop = 3341;
            public const int EstablishConnectionStart = 3342;
            public const int EstablishConnectionStop = 3343;
            public const int SessionPreambleUnderstood = 3345;
            public const int ConnectionReaderSendFault = 3346;
            public const int SocketAcceptClosed = 3347;
            public const int ServiceHostFaulted = 3348;
            public const int ListenerOpenStart = 3349;
            public const int ListenerOpenStop = 3350;
            public const int ServerMaxPooledConnectionsQuotaReached = 3351;
            public const int TcpConnectionTimedOut = 3352;
            public const int TcpConnectionResetError = 3353;
            public const int ServiceSecurityNegotiationCompleted = 3354;
            public const int SecurityNegotiationProcessingFailure = 3355;
            public const int SecurityIdentityVerificationSuccess = 3356;
            public const int SecurityIdentityVerificationFailure = 3357;
            public const int PortSharingDuplicatedSocket = 3358;
            public const int SecurityImpersonationSuccess = 3359;
            public const int SecurityImpersonationFailure = 3360;
            public const int HttpChannelRequestAborted = 3361;
            public const int HttpChannelResponseAborted = 3362;
            public const int HttpAuthFailed = 3363;
            public const int SharedListenerProxyRegisterStart = 3364;
            public const int SharedListenerProxyRegisterStop = 3365;
            public const int SharedListenerProxyRegisterFailed = 3366;
            public const int ConnectionPoolPreambleFailed = 3367;
            public const int SslOnInitiateUpgrade = 3368;
            public const int SslOnAcceptUpgrade = 3369;
            public const int BinaryMessageEncodingStart = 3370;
            public const int MtomMessageEncodingStart = 3371;
            public const int TextMessageEncodingStart = 3372;
            public const int BinaryMessageDecodingStart = 3373;
            public const int MtomMessageDecodingStart = 3374;
            public const int TextMessageDecodingStart = 3375;
            public const int HttpResponseReceiveStart = 3376;
            public const int SocketReadStop = 3377;
            public const int SocketAsyncReadStop = 3378;
            public const int SocketWriteStart = 3379;
            public const int SocketAsyncWriteStart = 3380;
            public const int SequenceAcknowledgementSent = 3381;
            public const int ClientReliableSessionReconnect = 3382;
            public const int ReliableSessionChannelFaulted = 3383;
            public const int WindowsStreamSecurityOnInitiateUpgrade = 3384;
            public const int WindowsStreamSecurityOnAcceptUpgrade = 3385;
            public const int SocketConnectionAbort = 3386;
            public const int HttpGetContextStart = 3388;
            public const int ClientSendPreambleStart = 3389;
            public const int ClientSendPreambleStop = 3390;
            public const int HttpMessageReceiveFailed = 3391;
            public const int TransactionScopeCreate = 3392;
            public const int StreamedMessageReadByEncoder = 3393;
            public const int StreamedMessageWrittenByEncoder = 3394;
            public const int MessageWrittenAsynchronouslyByEncoder = 3395;
            public const int BufferedAsyncWriteStart = 3396;
            public const int BufferedAsyncWriteStop = 3397;
            public const int PipeSharedMemoryCreated = 3398;
            public const int NamedPipeCreated = 3399;
            public const int SignatureVerificationStart = 3401;
            public const int SignatureVerificationSuccess = 3402;
            public const int WrappedKeyDecryptionStart = 3403;
            public const int WrappedKeyDecryptionSuccess = 3404;
            public const int EncryptedDataProcessingStart = 3405;
            public const int EncryptedDataProcessingSuccess = 3406;
            public const int HttpPipelineProcessInboundRequestStart = 3407;
            public const int HttpPipelineBeginProcessInboundRequestStart = 3408;
            public const int HttpPipelineProcessInboundRequestStop = 3409;
            public const int HttpPipelineFaulted = 3410;
            public const int HttpPipelineTimeoutException = 3411;
            public const int HttpPipelineProcessResponseStart = 3412;
            public const int HttpPipelineBeginProcessResponseStart = 3413;
            public const int HttpPipelineProcessResponseStop = 3414;
            public const int WebSocketConnectionRequestSendStart = 3415;
            public const int WebSocketConnectionRequestSendStop = 3416;
            public const int WebSocketConnectionAcceptStart = 3417;
            public const int WebSocketConnectionAccepted = 3418;
            public const int WebSocketConnectionDeclined = 3419;
            public const int WebSocketConnectionFailed = 3420;
            public const int WebSocketConnectionAborted = 3421;
            public const int WebSocketAsyncWriteStart = 3422;
            public const int WebSocketAsyncWriteStop = 3423;
            public const int WebSocketAsyncReadStart = 3424;
            public const int WebSocketAsyncReadStop = 3425;
            public const int WebSocketCloseSent = 3426;
            public const int WebSocketCloseOutputSent = 3427;
            public const int WebSocketConnectionClosed = 3428;
            public const int WebSocketCloseStatusReceived = 3429;
            public const int WebSocketUseVersionFromClientWebSocketFactory = 3430;
            public const int WebSocketCreateClientWebSocketWithFactory = 3431;
            public const int InferredContractDescription = 3501;
            public const int InferredOperationDescription = 3502;
            public const int DuplicateCorrelationQuery = 3503;
            public const int ServiceEndpointAdded = 3507;
            public const int TrackingProfileNotFound = 3508;
            public const int BufferOutOfOrderMessageNoInstance = 3550;
            public const int BufferOutOfOrderMessageNoBookmark = 3551;
            public const int MaxPendingMessagesPerChannelExceeded = 3552;
            public const int XamlServicesLoadStart = 3553;
            public const int XamlServicesLoadStop = 3554;
            public const int CreateWorkflowServiceHostStart = 3555;
            public const int CreateWorkflowServiceHostStop = 3556;
            public const int TransactedReceiveScopeEndCommitFailed = 3557;
            public const int ServiceActivationStart = 3558;
            public const int ServiceActivationStop = 3559;
            public const int ServiceActivationAvailableMemory = 3560;
            public const int ServiceActivationException = 3561;
            public const int RoutingServiceClosingClient = 3800;
            public const int RoutingServiceChannelFaulted = 3801;
            public const int RoutingServiceCompletingOneWay = 3802;
            public const int RoutingServiceProcessingFailure = 3803;
            public const int RoutingServiceCreatingClientForEndpoint = 3804;
            public const int RoutingServiceDisplayConfig = 3805;
            public const int RoutingServiceCompletingTwoWay = 3807;
            public const int RoutingServiceMessageRoutedToEndpoints = 3809;
            public const int RoutingServiceConfigurationApplied = 3810;
            public const int RoutingServiceProcessingMessage = 3815;
            public const int RoutingServiceTransmittingMessage = 3816;
            public const int RoutingServiceCommittingTransaction = 3817;
            public const int RoutingServiceDuplexCallbackException = 3818;
            public const int RoutingServiceMovedToBackup = 3819;
            public const int RoutingServiceCreatingTransaction = 3820;
            public const int RoutingServiceCloseFailed = 3821;
            public const int RoutingServiceSendingResponse = 3822;
            public const int RoutingServiceSendingFaultResponse = 3823;
            public const int RoutingServiceCompletingReceiveContext = 3824;
            public const int RoutingServiceAbandoningReceiveContext = 3825;
            public const int RoutingServiceUsingExistingTransaction = 3826;
            public const int RoutingServiceTransmitFailed = 3827;
            public const int RoutingServiceFilterTableMatchStart = 3828;
            public const int RoutingServiceFilterTableMatchStop = 3829;
            public const int RoutingServiceAbortingChannel = 3830;
            public const int RoutingServiceHandledException = 3831;
            public const int RoutingServiceTransmitSucceeded = 3832;
            public const int TransportListenerSessionsReceived = 4001;
            public const int FailFastException = 4002;
            public const int ServiceStartPipeError = 4003;
            public const int DispatchSessionStart = 4008;
            public const int PendingSessionQueueFull = 4010;
            public const int MessageQueueRegisterStart = 4011;
            public const int MessageQueueRegisterAbort = 4012;
            public const int MessageQueueUnregisterSucceeded = 4013;
            public const int MessageQueueRegisterFailed = 4014;
            public const int MessageQueueRegisterCompleted = 4015;
            public const int MessageQueueDuplicatedSocketError = 4016;
            public const int MessageQueueDuplicatedSocketComplete = 4019;
            public const int TcpTransportListenerListeningStart = 4020;
            public const int TcpTransportListenerListeningStop = 4021;
            public const int WebhostUnregisterProtocolFailed = 4022;
            public const int WasCloseAllListenerChannelInstancesCompleted = 4023;
            public const int WasCloseAllListenerChannelInstancesFailed = 4024;
            public const int OpenListenerChannelInstanceFailed = 4025;
            public const int WasConnected = 4026;
            public const int WasDisconnected = 4027;
            public const int PipeTransportListenerListeningStart = 4028;
            public const int PipeTransportListenerListeningStop = 4029;
            public const int DispatchSessionSuccess = 4030;
            public const int DispatchSessionFailed = 4031;
            public const int WasConnectionTimedout = 4032;
            public const int RoutingTableLookupStart = 4033;
            public const int RoutingTableLookupStop = 4034;
            public const int PendingSessionQueueRatio = 4035;
            public const int EndSqlCommandExecute = 4201;
            public const int StartSqlCommandExecute = 4202;
            public const int RenewLockSystemError = 4203;
            public const int FoundProcessingError = 4205;
            public const int UnlockInstanceException = 4206;
            public const int MaximumRetriesExceededForSqlCommand = 4207;
            public const int RetryingSqlCommandDueToSqlError = 4208;
            public const int TimeoutOpeningSqlConnection = 4209;
            public const int SqlExceptionCaught = 4210;
            public const int QueuingSqlRetry = 4211;
            public const int LockRetryTimeout = 4212;
            public const int RunnableInstancesDetectionError = 4213;
            public const int InstanceLocksRecoveryError = 4214;
            public const int MessageLogEventSizeExceeded = 4600;
            public const int DiscoveryClientInClientChannelFailedToClose = 4801;
            public const int DiscoveryClientProtocolExceptionSuppressed = 4802;
            public const int DiscoveryClientReceivedMulticastSuppression = 4803;
            public const int DiscoveryMessageReceivedAfterOperationCompleted = 4804;
            public const int DiscoveryMessageWithInvalidContent = 4805;
            public const int DiscoveryMessageWithInvalidRelatesToOrOperationCompleted = 4806;
            public const int DiscoveryMessageWithInvalidReplyTo = 4807;
            public const int DiscoveryMessageWithNoContent = 4808;
            public const int DiscoveryMessageWithNullMessageId = 4809;
            public const int DiscoveryMessageWithNullMessageSequence = 4810;
            public const int DiscoveryMessageWithNullRelatesTo = 4811;
            public const int DiscoveryMessageWithNullReplyTo = 4812;
            public const int DuplicateDiscoveryMessage = 4813;
            public const int EndpointDiscoverabilityDisabled = 4814;
            public const int EndpointDiscoverabilityEnabled = 4815;
            public const int FindInitiatedInDiscoveryClientChannel = 4816;
            public const int InnerChannelCreationFailed = 4817;
            public const int InnerChannelOpenFailed = 4818;
            public const int InnerChannelOpenSucceeded = 4819;
            public const int SynchronizationContextReset = 4820;
            public const int SynchronizationContextSetToNull = 4821;
            public const int DCSerializeWithSurrogateStart = 5001;
            public const int DCSerializeWithSurrogateStop = 5002;
            public const int DCDeserializeWithSurrogateStart = 5003;
            public const int DCDeserializeWithSurrogateStop = 5004;
            public const int ImportKnownTypesStart = 5005;
            public const int ImportKnownTypesStop = 5006;
            public const int DCResolverResolve = 5007;
            public const int DCGenWriterStart = 5008;
            public const int DCGenWriterStop = 5009;
            public const int DCGenReaderStart = 5010;
            public const int DCGenReaderStop = 5011;
            public const int DCJsonGenReaderStart = 5012;
            public const int DCJsonGenReaderStop = 5013;
            public const int DCJsonGenWriterStart = 5014;
            public const int DCJsonGenWriterStop = 5015;
            public const int GenXmlSerializableStart = 5016;
            public const int GenXmlSerializableStop = 5017;
            public const int JsonMessageDecodingStart = 5203;
            public const int JsonMessageEncodingStart = 5204;
            public const int TokenValidationStarted = 5402;
            public const int TokenValidationSuccess = 5403;
            public const int TokenValidationFailure = 5404;
            public const int GetIssuerNameSuccess = 5405;
            public const int GetIssuerNameFailure = 5406;
            public const int FederationMessageProcessingStarted = 5600;
            public const int FederationMessageProcessingSuccess = 5601;
            public const int FederationMessageCreationStarted = 5602;
            public const int FederationMessageCreationSuccess = 5603;
            public const int SessionCookieReadingStarted = 5604;
            public const int SessionCookieReadingSuccess = 5605;
            public const int PrincipalSettingFromSessionTokenStarted = 5606;
            public const int PrincipalSettingFromSessionTokenSuccess = 5607;
            public const int TrackingRecordDropped = 39456;
            public const int TrackingRecordRaised = 39457;
            public const int TrackingRecordTruncated = 39458;
            public const int TrackingDataExtracted = 39459;
            public const int TrackingValueNotSerializable = 39460;
            public const int AppDomainUnload = 57393;
            public const int HandledException = 57394;
            public const int ShipAssertExceptionMessage = 57395;
            public const int ThrowingException = 57396;
            public const int UnhandledException = 57397;
            public const int MaxInstancesExceeded = 57398;
            public const int TraceCodeEventLogCritical = 57399;
            public const int TraceCodeEventLogError = 57400;
            public const int TraceCodeEventLogInfo = 57401;
            public const int TraceCodeEventLogVerbose = 57402;
            public const int TraceCodeEventLogWarning = 57403;
            public const int HandledExceptionWarning = 57404;
            public const int HandledExceptionError = 57405;
            public const int HandledExceptionVerbose = 57406;
            public const int ThrowingExceptionVerbose = 57407;
            public const int EtwUnhandledException = 57408;
            public const int ThrowingEtwExceptionVerbose = 57409;
            public const int ThrowingEtwException = 57410;
            public const int HttpHandlerPickedForUrl = 62326;
        }

        public class Tasks
        {
            public const EventTask ActivationDispatchSession = (EventTask)2500;
            public const EventTask ActivationDuplicateSocket = (EventTask)2501;
            public const EventTask ActivationListenerOpen = (EventTask)2502;
            public const EventTask ActivationPipeListenerListening = (EventTask)2503;
            public const EventTask ActivationRoutingTableLookup = (EventTask)2504;
            public const EventTask ActivationServiceStart = (EventTask)2505;
            public const EventTask ActivationTcpListenerListening = (EventTask)2506;
            public const EventTask AddServiceEndpoint = (EventTask)2507;
            public const EventTask BufferOutOfOrder = (EventTask)2508;
            public const EventTask BufferPooling = (EventTask)2509;
            public const EventTask CacheRootMetadata = (EventTask)2510;
            public const EventTask ChannelFactoryCaching = (EventTask)2511;
            public const EventTask ChannelFactoryCreate = (EventTask)2512;
            public const EventTask ChannelReceive = (EventTask)2513;
            public const EventTask ClientRuntime = (EventTask)2514;
            public const EventTask ClientSendPreamble = (EventTask)2515;
            public const EventTask CompensationState = (EventTask)2516;
            public const EventTask CompleteActivity = (EventTask)2517;
            public const EventTask CompleteWorkItem = (EventTask)2518;
            public const EventTask Connect = (EventTask)2519;
            public const EventTask ConnectionAbort = (EventTask)2520;
            public const EventTask ConnectionAccept = (EventTask)2521;
            public const EventTask ConnectionPooling = (EventTask)2522;
            public const EventTask Correlation = (EventTask)2523;
            public const EventTask CreateBookmark = (EventTask)2524;
            public const EventTask CreateWorkflowServiceHost = (EventTask)2526;
            public const EventTask CustomTrackingRecord = (EventTask)2527;
            public const EventTask DataContractResolver = (EventTask)2528;
            public const EventTask DiscoveryClient = (EventTask)2529;
            public const EventTask DiscoveryClientChannel = (EventTask)2530;
            public const EventTask DiscoveryMessage = (EventTask)2531;
            public const EventTask DiscoverySynchronizationContext = (EventTask)2532;
            public const EventTask DispatchMessage = (EventTask)2533;
            public const EventTask EndpointDiscoverability = (EventTask)2534;
            public const EventTask ExecuteActivity = (EventTask)2535;
            public const EventTask ExecuteFlowchart = (EventTask)2536;
            public const EventTask ExecuteWorkItem = (EventTask)2537;
            public const EventTask FormatterDeserializeReply = (EventTask)2539;
            public const EventTask FormatterDeserializeRequest = (EventTask)2540;
            public const EventTask FormatterSerializeReply = (EventTask)2541;
            public const EventTask FormatterSerializeRequest = (EventTask)2542;
            public const EventTask GenerateDeserializer = (EventTask)2543;
            public const EventTask GenerateSerializer = (EventTask)2544;
            public const EventTask GenerateXmlSerializable = (EventTask)2545;
            public const EventTask HostedTransportConfigurationManagerConfigInit = (EventTask)2546;
            public const EventTask ImportKnownType = (EventTask)2547;
            public const EventTask InferDescription = (EventTask)2548;
            public const EventTask InitializeBookmarkScope = (EventTask)2549;
            public const EventTask InternalCacheMetadata = (EventTask)2550;
            public const EventTask InvokeMethod = (EventTask)2551;
            public const EventTask ListenerOpen = (EventTask)2552;
            public const EventTask LockWorkflowInstance = (EventTask)2553;
            public const EventTask MessageChannelCache = (EventTask)2554;
            public const EventTask MessageDecoding = (EventTask)2555;
            public const EventTask MessageEncoding = (EventTask)2556;
            public const EventTask MessageQueueRegister = (EventTask)2557;
            public const EventTask MsmqQuotas = (EventTask)2558;
            public const EventTask NoPersistBlock = (EventTask)2559;
            public const EventTask Quotas = (EventTask)2560;
            public const EventTask ReliableSession = (EventTask)2561;
            public const EventTask RoutingService = (EventTask)2562;
            public const EventTask RoutingServiceClient = (EventTask)2563;
            public const EventTask RoutingServiceFilterTableMatch = (EventTask)2564;
            public const EventTask RoutingServiceMessage = (EventTask)2565;
            public const EventTask RoutingServiceReceiveContext = (EventTask)2566;
            public const EventTask RoutingServiceTransaction = (EventTask)2567;
            public const EventTask RuntimeTransaction = (EventTask)2568;
            public const EventTask ScheduleActivity = (EventTask)2569;
            public const EventTask ScheduleWorkItem = (EventTask)2570;
            public const EventTask SecureMessage = (EventTask)2571;
            public const EventTask SecurityImpersonation = (EventTask)2572;
            public const EventTask SecurityNegotiation = (EventTask)2573;
            public const EventTask SecurityVerification = (EventTask)2574;
            public const EventTask ServiceActivation = (EventTask)2575;
            public const EventTask ServiceChannelCall = (EventTask)2576;
            public const EventTask ServiceChannelOpen = (EventTask)2577;
            public const EventTask ServiceHostActivation = (EventTask)2578;
            public const EventTask ServiceHostCompilation = (EventTask)2579;
            public const EventTask ServiceHostCreate = (EventTask)2580;
            public const EventTask ServiceHostFactoryCreation = (EventTask)2581;
            public const EventTask ServiceHostFault = (EventTask)2582;
            public const EventTask ServiceHostOpen = (EventTask)2583;
            public const EventTask ServiceInstance = (EventTask)2584;
            public const EventTask ServiceShutdown = (EventTask)2585;
            public const EventTask SessionStart = (EventTask)2586;
            public const EventTask SessionUpgrade = (EventTask)2587;
            public const EventTask Signpost = (EventTask)2588;
            public const EventTask SqlCommandExecute = (EventTask)2589;
            public const EventTask StartWorkItem = (EventTask)2590;
            public const EventTask SurrogateDeserialize = (EventTask)2591;
            public const EventTask SurrogateSerialize = (EventTask)2592;
            public const EventTask ThreadScheduling = (EventTask)2593;
            public const EventTask Throttles = (EventTask)2594;
            public const EventTask Timeout = (EventTask)2595;
            public const EventTask TimeoutException = (EventTask)2596;
            public const EventTask TrackingProfile = (EventTask)2597;
            public const EventTask TrackingRecord = (EventTask)2598;
            public const EventTask TransportReceive = (EventTask)2599;
            public const EventTask TransportSend = (EventTask)2600;
            public const EventTask TryCatchException = (EventTask)2601;
            public const EventTask VBExpressionCompile = (EventTask)2602;
            public const EventTask WASActivation = (EventTask)2603;
            public const EventTask WebHostRequest = (EventTask)2604;
            public const EventTask WFApplicationStateChange = (EventTask)2605;
            public const EventTask WFMessage = (EventTask)2606;
            public const EventTask WorkflowActivity = (EventTask)2607;
            public const EventTask WorkflowInstanceRecord = (EventTask)2608;
            public const EventTask WorkflowTracking = (EventTask)2609;
            public const EventTask XamlServicesLoad = (EventTask)2610;
            public const EventTask SignatureVerification = (EventTask)2611;
            public const EventTask TokenValidation = (EventTask)2612;
            public const EventTask GetIssuerName = (EventTask)2613;
            public const EventTask WrappedKeyDecryption = (EventTask)2614;
            public const EventTask EncryptedDataProcessing = (EventTask)2615;
            public const EventTask FederationMessageProcessing = (EventTask)2616;
            public const EventTask FederationMessageCreation = (EventTask)2617;
            public const EventTask SessionCookieReading = (EventTask)2618;
            public const EventTask PrincipalSetting = (EventTask)2619;
        }

        public class Opcodes
        {
            public const EventOpcode BufferOutOfOrderNoBookmark = (EventOpcode)10;
            public const EventOpcode RoutingServiceReceiveContextCompleting = (EventOpcode)100;
            public const EventOpcode RoutingServiceTransactionCommittingTransaction = (EventOpcode)101;
            public const EventOpcode RoutingServiceTransactionCreating = (EventOpcode)102;
            public const EventOpcode RoutingServiceTransactionUsingExisting = (EventOpcode)103;
            public const EventOpcode RuntimeTransactionComplete = (EventOpcode)104;
            public const EventOpcode RuntimeTransactionCompletionRequested = (EventOpcode)105;
            public const EventOpcode RuntimeTransactionSet = (EventOpcode)106;
            public const EventOpcode ScheduleWorkItemScheduleBookmark = (EventOpcode)107;
            public const EventOpcode ScheduleWorkItemScheduleCancelActivity = (EventOpcode)108;
            public const EventOpcode ScheduleWorkItemScheduleCompletion = (EventOpcode)109;
            public const EventOpcode ExecuteFlowchartBegin = (EventOpcode)11;
            public const EventOpcode BufferOutOfOrderNoInstance = (EventOpcode)11;
            public const EventOpcode ScheduleWorkItemScheduleExecuteActivity = (EventOpcode)110;
            public const EventOpcode ScheduleWorkItemScheduleFault = (EventOpcode)111;
            public const EventOpcode ScheduleWorkItemScheduleRuntime = (EventOpcode)112;
            public const EventOpcode ScheduleWorkItemScheduleTransactionContext = (EventOpcode)113;
            public const EventOpcode SessionUpgradeAccept = (EventOpcode)114;
            public const EventOpcode SessionUpgradeInitiate = (EventOpcode)115;
            public const EventOpcode Signpostsuspend = (EventOpcode)116;
            public const EventOpcode StartWorkItemStartBookmark = (EventOpcode)117;
            public const EventOpcode StartWorkItemStartCancelActivity = (EventOpcode)118;
            public const EventOpcode StartWorkItemStartCompletion = (EventOpcode)119;
            public const EventOpcode BufferPoolingAllocate = (EventOpcode)12;
            public const EventOpcode StartWorkItemStartExecuteActivity = (EventOpcode)120;
            public const EventOpcode StartWorkItemStartFault = (EventOpcode)121;
            public const EventOpcode StartWorkItemStartRuntime = (EventOpcode)122;
            public const EventOpcode StartWorkItemStartTransactionContext = (EventOpcode)123;
            public const EventOpcode TrackingProfileNotFound = (EventOpcode)124;
            public const EventOpcode TrackingRecordDropped = (EventOpcode)125;
            public const EventOpcode TrackingRecordRaised = (EventOpcode)126;
            public const EventOpcode TrackingRecordTruncated = (EventOpcode)127;
            public const EventOpcode TransportReceiveBeforeAuthentication = (EventOpcode)128;
            public const EventOpcode TryCatchExceptionDuringCancelation = (EventOpcode)129;
            public const EventOpcode BufferPoolingTune = (EventOpcode)13;
            public const EventOpcode TryCatchExceptionFromCatchOrFinally = (EventOpcode)130;
            public const EventOpcode TryCatchExceptionFromTry = (EventOpcode)131;
            public const EventOpcode WASActivationConnected = (EventOpcode)132;
            public const EventOpcode WASActivationDisconnect = (EventOpcode)133;
            public const EventOpcode WFApplicationStateChangeCompleted = (EventOpcode)134;
            public const EventOpcode WFApplicationStateChangeIdled = (EventOpcode)135;
            public const EventOpcode WFApplicationStateChangeInstanceAborted = (EventOpcode)136;
            public const EventOpcode WFApplicationStateChangeInstanceCanceled = (EventOpcode)137;
            public const EventOpcode WFApplicationStateChangePersistableIdle = (EventOpcode)138;
            public const EventOpcode WFApplicationStateChangePersisted = (EventOpcode)139;
            public const EventOpcode ClientRuntimeClientChannelOpenStart = (EventOpcode)14;
            public const EventOpcode WFApplicationStateChangeTerminated = (EventOpcode)140;
            public const EventOpcode WFApplicationStateChangeUnhandledException = (EventOpcode)141;
            public const EventOpcode WFApplicationStateChangeUnloaded = (EventOpcode)142;
            public const EventOpcode WorkflowActivitysuspend = (EventOpcode)143;
            public const EventOpcode WorkflowInstanceRecordAbortedRecord = (EventOpcode)144;
            public const EventOpcode WorkflowInstanceRecordAbortedWithId = (EventOpcode)145;
            public const EventOpcode WorkflowInstanceRecordSuspendedRecord = (EventOpcode)146;
            public const EventOpcode WorkflowInstanceRecordSuspendedWithId = (EventOpcode)147;
            public const EventOpcode WorkflowInstanceRecordTerminatedRecord = (EventOpcode)148;
            public const EventOpcode WorkflowInstanceRecordTerminatedWithId = (EventOpcode)149;
            public const EventOpcode ClientRuntimeClientChannelOpenStop = (EventOpcode)15;
            public const EventOpcode WorkflowInstanceRecordUnhandledExceptionRecord = (EventOpcode)150;
            public const EventOpcode WorkflowInstanceRecordUnhandledExceptionWithId = (EventOpcode)151;
            public const EventOpcode WorkflowInstanceRecordUpdatedRecord = (EventOpcode)152;
            public const EventOpcode ClientRuntimeClientMessageInspectorAfterReceiveInvoked = (EventOpcode)16;
            public const EventOpcode ClientRuntimeClientMessageInspectorBeforeSendInvoked = (EventOpcode)17;
            public const EventOpcode ClientRuntimeClientParameterInspectorStart = (EventOpcode)18;
            public const EventOpcode ClientRuntimeClientParameterInspectorStop = (EventOpcode)19;
            public const EventOpcode ClientRuntimeOperationPrepared = (EventOpcode)20;
            public const EventOpcode CompleteWorkItemCompleteBookmark = (EventOpcode)21;
            public const EventOpcode CompleteWorkItemCompleteCancelActivity = (EventOpcode)22;
            public const EventOpcode CompleteWorkItemCompleteCompletion = (EventOpcode)23;
            public const EventOpcode CompleteWorkItemCompleteExecuteActivity = (EventOpcode)24;
            public const EventOpcode CompleteWorkItemCompleteFault = (EventOpcode)25;
            public const EventOpcode CompleteWorkItemCompleteRuntime = (EventOpcode)26;
            public const EventOpcode CompleteWorkItemCompleteTransactionContext = (EventOpcode)27;
            public const EventOpcode CorrelationDuplicateQuery = (EventOpcode)28;
            public const EventOpcode DiscoveryClientExceptionSuppressed = (EventOpcode)29;
            public const EventOpcode DiscoveryClientFailedToClose = (EventOpcode)30;
            public const EventOpcode DiscoveryClientReceivedMulticastSuppression = (EventOpcode)31;
            public const EventOpcode DiscoveryClientChannelCreationFailed = (EventOpcode)32;
            public const EventOpcode DiscoveryClientChannelFindInitiated = (EventOpcode)33;
            public const EventOpcode DiscoveryClientChannelOpenFailed = (EventOpcode)34;
            public const EventOpcode DiscoveryClientChannelOpenSucceeded = (EventOpcode)35;
            public const EventOpcode DiscoveryMessageDuplicate = (EventOpcode)36;
            public const EventOpcode DiscoveryMessageInvalidContent = (EventOpcode)37;
            public const EventOpcode DiscoveryMessageInvalidRelatesToOrOperationCompleted = (EventOpcode)38;
            public const EventOpcode DiscoveryMessageInvalidReplyTo = (EventOpcode)39;
            public const EventOpcode DiscoveryMessageNoContent = (EventOpcode)40;
            public const EventOpcode DiscoveryMessageNullMessageId = (EventOpcode)41;
            public const EventOpcode DiscoveryMessageNullMessageSequence = (EventOpcode)42;
            public const EventOpcode DiscoveryMessageNullRelatesTo = (EventOpcode)43;
            public const EventOpcode DiscoveryMessageNullReplyTo = (EventOpcode)44;
            public const EventOpcode DiscoveryMessageReceivedAfterOperationCompleted = (EventOpcode)45;
            public const EventOpcode DiscoverySynchronizationContextReset = (EventOpcode)46;
            public const EventOpcode DiscoverySynchronizationContextSetToNull = (EventOpcode)47;
            public const EventOpcode DispatchMessageBeforeAuthorization = (EventOpcode)48;
            public const EventOpcode DispatchMessageDispatchStart = (EventOpcode)49;
            public const EventOpcode DispatchMessageDispatchStop = (EventOpcode)50;
            public const EventOpcode DispatchMessageDispathMessageInspectorAfterReceiveInvoked = (EventOpcode)51;
            public const EventOpcode DispatchMessageDispathMessageInspectorBeforeSendInvoked = (EventOpcode)52;
            public const EventOpcode DispatchMessageOperationInvokerStart = (EventOpcode)53;
            public const EventOpcode DispatchMessageOperationInvokerStop = (EventOpcode)54;
            public const EventOpcode DispatchMessageParameterInspectorStart = (EventOpcode)55;
            public const EventOpcode DispatchMessageParameterInspectorStop = (EventOpcode)56;
            public const EventOpcode DispatchMessageTransactionScopeCreate = (EventOpcode)57;
            public const EventOpcode EndpointDiscoverabilityDisabled = (EventOpcode)58;
            public const EventOpcode EndpointDiscoverabilityEnabled = (EventOpcode)59;
            public const EventOpcode ExecuteFlowchartEmpty = (EventOpcode)60;
            public const EventOpcode ExecuteFlowchartNextNull = (EventOpcode)61;
            public const EventOpcode ExecuteFlowchartSwitchCase = (EventOpcode)62;
            public const EventOpcode ExecuteFlowchartSwitchCaseNotFound = (EventOpcode)63;
            public const EventOpcode ExecuteFlowchartSwitchDefault = (EventOpcode)64;
            public const EventOpcode InferDescriptionContract = (EventOpcode)69;
            public const EventOpcode InferDescriptionOperation = (EventOpcode)70;
            public const EventOpcode InvokeMethodDoesNotUseAsyncPattern = (EventOpcode)71;
            public const EventOpcode InvokeMethodIsNotStatic = (EventOpcode)72;
            public const EventOpcode InvokeMethodIsStatic = (EventOpcode)73;
            public const EventOpcode InvokeMethodThrewException = (EventOpcode)74;
            public const EventOpcode InvokeMethodUseAsyncPattern = (EventOpcode)75;
            public const EventOpcode MessageChannelCacheMissed = (EventOpcode)76;
            public const EventOpcode ReliableSessionFaulted = (EventOpcode)77;
            public const EventOpcode ReliableSessionReconnect = (EventOpcode)78;
            public const EventOpcode ReliableSessionSequenceAck = (EventOpcode)79;
            public const EventOpcode RoutingServiceAbortingChannel = (EventOpcode)80;
            public const EventOpcode RoutingServiceCloseFailed = (EventOpcode)81;
            public const EventOpcode RoutingServiceConfigurationApplied = (EventOpcode)82;
            public const EventOpcode RoutingServiceDuplexCallbackException = (EventOpcode)83;
            public const EventOpcode RoutingServiceHandledException = (EventOpcode)84;
            public const EventOpcode RoutingServiceTransmitFailed = (EventOpcode)85;
            public const EventOpcode RoutingServiceClientChannelFaulted = (EventOpcode)86;
            public const EventOpcode RoutingServiceClientClosing = (EventOpcode)87;
            public const EventOpcode RoutingServiceClientCreatingForEndpoint = (EventOpcode)88;
            public const EventOpcode RoutingServiceMessageCompletingOneWay = (EventOpcode)89;
            public const EventOpcode RoutingServiceMessageCompletingTwoWay = (EventOpcode)90;
            public const EventOpcode RoutingServiceMessageMovedToBackup = (EventOpcode)91;
            public const EventOpcode RoutingServiceMessageProcessingFailure = (EventOpcode)92;
            public const EventOpcode RoutingServiceMessageProcessingMessage = (EventOpcode)93;
            public const EventOpcode RoutingServiceMessageRoutedToEndpoints = (EventOpcode)94;
            public const EventOpcode RoutingServiceMessageSendingFaultResponse = (EventOpcode)95;
            public const EventOpcode RoutingServiceMessageSendingResponse = (EventOpcode)96;
            public const EventOpcode RoutingServiceMessageTransmitSucceeded = (EventOpcode)97;
            public const EventOpcode RoutingServiceMessageTransmittingMessage = (EventOpcode)98;
            public const EventOpcode RoutingServiceReceiveContextAbandoning = (EventOpcode)99;
        }

        public class Keywords
        {
            public const EventKeywords ServiceHost = (EventKeywords)0x1;
            public const EventKeywords Serialization = (EventKeywords)0x2;
            public const EventKeywords ServiceModel = (EventKeywords)0x4;
            public const EventKeywords Transaction = (EventKeywords)0x8;
            public const EventKeywords Security = (EventKeywords)0x10;
            public const EventKeywords WCFMessageLogging = (EventKeywords)0x20;
            public const EventKeywords WFTracking = (EventKeywords)0x40;
            public const EventKeywords WebHost = (EventKeywords)0x80;
            public const EventKeywords HTTP = (EventKeywords)0x100;
            public const EventKeywords TCP = (EventKeywords)0x200;
            public const EventKeywords TransportGeneral = (EventKeywords)0x400;
            public const EventKeywords ActivationServices = (EventKeywords)0x800;
            public const EventKeywords Channel = (EventKeywords)0x1000;
            public const EventKeywords WebHTTP = (EventKeywords)0x2000;
            public const EventKeywords Discovery = (EventKeywords)0x4000;
            public const EventKeywords RoutingServices = (EventKeywords)0x8000;
            public const EventKeywords Infrastructure = (EventKeywords)0x10000;
            public const EventKeywords EndToEndMonitoring = (EventKeywords)0x20000;
            public const EventKeywords HealthMonitoring = (EventKeywords)0x40000;
            public const EventKeywords Troubleshooting = (EventKeywords)0x80000;
            public const EventKeywords UserEvents = (EventKeywords)0x100000;
            public const EventKeywords Threading = (EventKeywords)0x200000;
            public const EventKeywords Quota = (EventKeywords)0x400000;
            public const EventKeywords WFRuntime = (EventKeywords)0x1000000;
            public const EventKeywords WFActivities = (EventKeywords)0x2000000;
            public const EventKeywords WFServices = (EventKeywords)0x4000000;
            public const EventKeywords WFInstanceStore = (EventKeywords)0x8000000;
        }

        #endregion
    }
}
