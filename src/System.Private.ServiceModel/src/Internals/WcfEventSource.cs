using System.Diagnostics.Tracing;

namespace System.Runtime
{
    [EventSource(Name = "Microsoft-Windows-Application Server-Applications", Guid = "c651f5f6-1c0d-492e-8ae1-b4efd7c9d503")]
    sealed class WcfEventSource : EventSource
    {
        public static WcfEventSource Instance = new WcfEventSource();

        public bool BufferPoolAllocationIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(EventIds.BufferPoolAllocation, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.Allocate, Task = Tasks.BufferPooling,
            Keywords = Keywords.Infrastructure,
            Message = "Pool allocating {0} Bytes.")]
        public void BufferPoolAllocation(int Size)
        {
            WriteEvent(EventIds.BufferPoolAllocation, Size);
        }

        public bool BufferPoolChangeQuotaIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(EventIds.BufferPoolChangeQuota, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.Tune, Task = Tasks.BufferPooling,
            Keywords = Keywords.Infrastructure,
            Message = "BufferPool of size {0}, changing quota by {1}.")]
        public void BufferPoolChangeQuota(int PoolSize, int Delta)
        {
            WriteEvent(EventIds.BufferPoolChangeQuota, PoolSize, Delta);
        }

        public bool ActionItemScheduledIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Threading, EventChannel.Debug);
        }

        [Event(EventIds.ActionItemScheduled, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ThreadScheduling,
            Keywords = Keywords.Threading,
            Message = "IO Thread scheduler callback invoked.")]
        public void ActionItemScheduled()
        {
            WriteEvent(EventIds.ActionItemScheduled);
        }

        public bool ActionItemCallbackInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Threading, EventChannel.Debug);
        }

        [Event(EventIds.ActionItemCallbackInvoked, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ThreadScheduling,
            Keywords = Keywords.Threading,
            Message = "IO Thread scheduler callback invoked.")]
        public void ActionItemCallbackInvoked()
        {
            WriteEvent(EventIds.ActionItemCallbackInvoked);
        }

        public bool ClientMessageInspectorAfterReceiveInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ClientMessageInspectorAfterReceiveInvoked, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ClientMessageInspectorAfterReceiveInvoked, Task = Tasks.ClientRuntime,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'AfterReceiveReply' on a ClientMessageInspector of type '{0}'.")]
        public void ClientMessageInspectorAfterReceiveInvoked(string TypeName, string HostReference)
        {
            WriteEvent(EventIds.ClientMessageInspectorAfterReceiveInvoked, TypeName, HostReference);
        }

        public bool ClientMessageInspectorBeforeSendInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ClientMessageInspectorBeforeSendInvoked, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.ClientMessageInspectorBeforeSendInvoked, Task = Tasks.ClientRuntime,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'BeforeSendRequest' on a ClientMessageInspector of type  '{0}'.")]
        public void ClientMessageInspectorBeforeSendInvoked(string TypeName, string HostReference)
        {
            WriteEvent(EventIds.ClientMessageInspectorBeforeSendInvoked, TypeName, HostReference);
        }

        public bool ClientParameterInspectorAfterCallInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ClientParameterInspectorAfterCallInvoked, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.ClientParameterInspectorStop, Task = Tasks.ClientRuntime,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'AfterCall' on a ClientParameterInspector of type '{0}'.")]
        public void ClientParameterInspectorAfterCallInvoked(string TypeName, string HostReference)
        {
            WriteEvent(EventIds.ClientParameterInspectorAfterCallInvoked, TypeName, HostReference);
        }

        public bool ClientParameterInspectorBeforeCallInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ClientParameterInspectorBeforeCallInvoked, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.ClientParameterInspectorStart, Task = Tasks.ClientRuntime,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'BeforeCall' on a ClientParameterInspector of type '{0}'.")]
        public void ClientParameterInspectorBeforeCallInvoked(string TypeName, string HostReference)
        {
            WriteEvent(EventIds.ClientParameterInspectorBeforeCallInvoked, TypeName, HostReference);
        }

        public bool OperationInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.OperationInvoked, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.OperationInvokerStart, Task = Tasks.DispatchMessage,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "An OperationInvoker invoked the '{0}' method. Caller information: '{1}'.")]
        public void OperationInvoked(string MethodName, string CallerInfo, string HostReference)
        {
            WriteEvent(EventIds.OperationInvoked, MethodName, CallerInfo, HostReference);
        }

        public bool ErrorHandlerInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ErrorHandlerInvoked, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked an ErrorHandler of type  '{0}' with an exception of type '{2}'.  ErrorHandled == '{1}'.")]
        public void ErrorHandlerInvoked(string TypeName, byte Handled, string ExceptionTypeName, string HostReference)
        {
            WriteEvent(EventIds.ErrorHandlerInvoked, TypeName, Handled, ExceptionTypeName, HostReference);
        }

        public bool FaultProviderInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.FaultProviderInvoked, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked a FaultProvider of type '{0}' with an exception of type '{1}'.")]
        public void FaultProviderInvoked(string TypeName, string ExceptionTypeName, string HostReference)
        {
            WriteEvent(EventIds.FaultProviderInvoked, TypeName, ExceptionTypeName, HostReference);
        }

        public bool MessageInspectorAfterReceiveInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.MessageInspectorAfterReceiveInvoked, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.DispathMessageInspectorAfterReceiveInvoked, Task = Tasks.DispatchMessage,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'AfterReceiveReply' on a MessageInspector of type '{0}'.")]
        public void MessageInspectorAfterReceiveInvoked(string TypeName, string HostReference)
        {
            WriteEvent(EventIds.MessageInspectorAfterReceiveInvoked, TypeName, HostReference);
        }

        public bool MessageInspectorBeforeSendInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.MessageInspectorBeforeSendInvoked, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.DispathMessageInspectorBeforeSendInvoked, Task = Tasks.DispatchMessage,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'BeforeSendRequest' on a MessageInspector of type '{0}'.")]
        public void MessageInspectorBeforeSendInvoked(string TypeName, string HostReference)
        {
            WriteEvent(EventIds.MessageInspectorBeforeSendInvoked, TypeName, HostReference);
        }

        public bool MessageThrottleExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.Quota | Keywords.HealthMonitoring | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.MessageThrottleExceeded, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.Quota | Keywords.HealthMonitoring | Keywords.ServiceModel,
            Message = "The '{0}' throttle limit of '{1}' was hit.")]
        public void MessageThrottleExceeded(string ThrottleName, long Limit, string HostReference)
        {
            WriteEvent(EventIds.MessageThrottleExceeded, ThrottleName, Limit, HostReference);
        }

        public bool ParameterInspectorAfterCallInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ParameterInspectorAfterCallInvoked, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.ParameterInspectorStop, Task = Tasks.DispatchMessage,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'AfterCall' on a ParameterInspector of type '{0}'.")]
        public void ParameterInspectorAfterCallInvoked(string TypeName, string HostReference)
        {
            WriteEvent(EventIds.ParameterInspectorAfterCallInvoked, TypeName, HostReference);
        }

        public bool ParameterInspectorBeforeCallInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ParameterInspectorBeforeCallInvoked, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.ParameterInspectorStart, Task = Tasks.DispatchMessage,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'BeforeCall' on a ParameterInspector of type '{0}'.")]
        public void ParameterInspectorBeforeCallInvoked(string TypeName, string HostReference)
        {
            WriteEvent(EventIds.ParameterInspectorBeforeCallInvoked, TypeName, HostReference);
        }

        public bool ServiceHostStartedIsEnabled()
        {
            return base.IsEnabled(EventLevel.LogAlways, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceHost, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceHostStarted, Level = EventLevel.LogAlways, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.ServiceHostActivation,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceHost,
            Message = "ServiceHost started: '{0}'.")]
        public void ServiceHostStarted(string ServiceTypeName, string HostReference)
        {
            WriteEvent(EventIds.ServiceHostStarted, ServiceTypeName, HostReference);
        }

        public bool OperationCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.HealthMonitoring | Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.OperationCompleted, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.OperationInvokerStop, Task = Tasks.DispatchMessage,
            Keywords = Keywords.HealthMonitoring | Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "An OperationInvoker completed the call to the '{0}' method.  The method call duration was '{1}' ms.")]
        public void OperationCompleted(string MethodName, long Duration, string HostReference)
        {
            WriteEvent(EventIds.OperationCompleted, MethodName, Duration, HostReference);
        }

        public bool MessageReceivedByTransportIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.TransportGeneral, EventChannel.Analytic);
        }

        [Event(EventIds.MessageReceivedByTransport, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.TransportReceive,
            Keywords = Keywords.Troubleshooting | Keywords.TransportGeneral,
            Message = "The transport received a message from '{0}'.")]
        public void MessageReceivedByTransport(string ListenAddress, string HostReference)
        {
            WriteEvent(EventIds.MessageReceivedByTransport, ListenAddress, HostReference);
        }

        public bool MessageSentByTransportIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.TransportGeneral, EventChannel.Analytic);
        }

        [Event(EventIds.MessageSentByTransport, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.TransportSend,
            Keywords = Keywords.Troubleshooting | Keywords.TransportGeneral,
            Message = "The transport sent a message to '{0}'.")]
        public void MessageSentByTransport(string DestinationAddress, string HostReference)
        {
            WriteEvent(EventIds.MessageSentByTransport, DestinationAddress, HostReference);
        }

        public bool ClientOperationPreparedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ClientOperationPrepared, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.OperationPrepared, Task = Tasks.ClientRuntime,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Client is executing Action '{0}' associated with the '{1}' contract. The message will be sent to '{2}'.")]
        public void ClientOperationPrepared(string Action, string ContractName, string Destination, string HostReference)
        {
            WriteEvent(EventIds.ClientOperationPrepared, Action, ContractName, Destination, HostReference);
        }

        public bool ServiceChannelCallStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceChannelCallStop, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.ServiceChannelCall,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Client completed executing Action '{0}' associated with the '{1}' contract. The message was sent to '{2}'.")]
        public void ServiceChannelCallStop(string Action, string ContractName, string Destination, string HostReference)
        {
            WriteEvent(EventIds.ServiceChannelCallStop, Action, ContractName, Destination, HostReference);
        }

        public bool ServiceExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceException, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.DispatchMessage,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceModel,
            Message = "There was an unhandled exception of type '{1}' during message processing.  Full Exception Details: {0}.")]
        public void ServiceException(string ExceptionToString, string ExceptionTypeName, string HostReference)
        {
            WriteEvent(EventIds.ServiceException, ExceptionToString, ExceptionTypeName, HostReference);
        }

        public bool MessageSentToTransportIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.TransportGeneral, EventChannel.Analytic);
        }

        [Event(EventIds.MessageSentToTransport, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.TransportGeneral,
            Message = "The Dispatcher sent a message to the transport. Correlation ID == '{0}'.")]
        public void MessageSentToTransport(Guid CorrelationId, string HostReference)
        {
            WriteEvent(EventIds.MessageSentToTransport, CorrelationId, HostReference);
        }

        public bool MessageReceivedFromTransportIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.TransportGeneral, EventChannel.Analytic);
        }

        [Event(EventIds.MessageReceivedFromTransport, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.TransportGeneral,
            Message = "The Dispatcher received a message from the transport. Correlation ID == '{0}'.")]
        public void MessageReceivedFromTransport(Guid CorrelationId, string HostReference)
        {
            WriteEvent(EventIds.MessageReceivedFromTransport, CorrelationId, HostReference);
        }

        public bool OperationFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.OperationFailed, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.DispatchMessage,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceModel,
            Message = "The '{0}' method threw an unhandled exception when invoked by the OperationInvoker. The method call duration was '{1}' ms.")]
        public void OperationFailed(string MethodName, long Duration, string HostReference)
        {
            WriteEvent(EventIds.OperationFailed, MethodName, Duration, HostReference);
        }

        public bool OperationFaultedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.OperationFaulted, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.DispatchMessage,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceModel,
            Message = "The '{0}' method threw a FaultException when invoked by the OperationInvoker. The method call duration was '{1}' ms.")]
        public void OperationFaulted(string MethodName, long Duration, string HostReference)
        {
            WriteEvent(EventIds.OperationFaulted, MethodName, Duration, HostReference);
        }

        public bool MessageThrottleAtSeventyPercentIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.Quota | Keywords.HealthMonitoring | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.MessageThrottleAtSeventyPercent, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.DispatchMessage,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.Quota | Keywords.HealthMonitoring | Keywords.ServiceModel,
            Message = "The '{0}' throttle limit of '{1}' is at 70%%.")]
        public void MessageThrottleAtSeventyPercent(string ThrottleName, long Limit, string HostReference)
        {
            WriteEvent(EventIds.MessageThrottleAtSeventyPercent, ThrottleName, Limit, HostReference);
        }

        public bool TraceCorrelationKeysIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.TraceCorrelationKeys, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Correlation,
            Keywords = Keywords.Troubleshooting | Keywords.WFServices,
            Message = "Calculated correlation key '{0}' using values '{1}' in parent scope '{2}'.")]
        public void TraceCorrelationKeys(Guid InstanceKey, string Values, string ParentScope, string HostReference)
        {
            WriteEvent(EventIds.TraceCorrelationKeys, InstanceKey, Values, ParentScope, HostReference);
        }

        public bool IdleServicesClosedIsEnabled()
        {
            return base.IsEnabled(EventLevel.LogAlways, Keywords.HealthMonitoring | Keywords.WebHost, EventChannel.Analytic);
        }

        [Event(EventIds.IdleServicesClosed, Level = EventLevel.LogAlways, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.ServiceShutdown,
            Keywords = Keywords.HealthMonitoring | Keywords.WebHost,
            Message = "{0} idle services out of total {1} activated services closed.")]
        public void IdleServicesClosed(int ClosedCount, int TotalCount)
        {
            WriteEvent(EventIds.IdleServicesClosed, ClosedCount, TotalCount);
        }

        public bool UserDefinedErrorOccurredIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.UserEvents | Keywords.ServiceModel | Keywords.EndToEndMonitoring, EventChannel.Analytic);
        }

        [Event(EventIds.UserDefinedErrorOccurred, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.UserEvents | Keywords.ServiceModel | Keywords.EndToEndMonitoring,
            Message = "Name:'{0}', Reference:'{1}', Payload:{2}")]
        public void UserDefinedErrorOccurred(string Name, string HostReference, string Payload)
        {
            WriteEvent(EventIds.UserDefinedErrorOccurred, Name, HostReference, Payload);
        }

        public bool UserDefinedWarningOccurredIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.UserEvents | Keywords.ServiceModel | Keywords.EndToEndMonitoring, EventChannel.Analytic);
        }

        [Event(EventIds.UserDefinedWarningOccurred, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.UserEvents | Keywords.ServiceModel | Keywords.EndToEndMonitoring,
            Message = "Name:'{0}', Reference:'{1}', Payload:{2}")]
        public void UserDefinedWarningOccurred(string Name, string HostReference, string Payload)
        {
            WriteEvent(EventIds.UserDefinedWarningOccurred, Name, HostReference, Payload);
        }

        public bool UserDefinedInformationEventOccuredIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.UserEvents | Keywords.ServiceModel | Keywords.EndToEndMonitoring, EventChannel.Analytic);
        }

        [Event(EventIds.UserDefinedInformationEventOccured, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.UserEvents | Keywords.ServiceModel | Keywords.EndToEndMonitoring,
            Message = "Name:'{0}', Reference:'{1}', Payload:{2}")]
        public void UserDefinedInformationEventOccured(string Name, string HostReference, string Payload)
        {
            WriteEvent(EventIds.UserDefinedInformationEventOccured, Name, HostReference, Payload);
        }

        public bool StopSignpostEventIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.StopSignpostEvent, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.Signpost,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "Activity boundary.")]
        public void StopSignpostEvent(string ExtendedData)
        {
            WriteEvent(EventIds.StopSignpostEvent, ExtendedData);
        }

        public bool StartSignpostEventIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.StartSignpostEvent, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.Signpost,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "Activity boundary.")]
        public void StartSignpostEvent(string ExtendedData)
        {
            WriteEvent(EventIds.StartSignpostEvent, ExtendedData);
        }

        public bool SuspendSignpostEventIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.SuspendSignpostEvent, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Suspend, Task = Tasks.Signpost,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "Activity boundary.")]
        public void SuspendSignpostEvent(string ExtendedData)
        {
            WriteEvent(EventIds.SuspendSignpostEvent, ExtendedData);
        }

        public bool ResumeSignpostEventIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ResumeSignpostEvent, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Resume, Task = Tasks.Signpost,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "Activity boundary.")]
        public void ResumeSignpostEvent(string ExtendedData)
        {
            WriteEvent(EventIds.ResumeSignpostEvent, ExtendedData);
        }

        public bool StartSignpostEvent1IsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.StartSignpostEvent1, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.Signpost,
            Keywords = Keywords.Troubleshooting | Keywords.WFServices,
            Message = "Activity boundary.")]
        public void StartSignpostEvent1(string ExtendedData)
        {
            WriteEvent(EventIds.StartSignpostEvent1, ExtendedData);
        }

        public bool StopSignpostEvent1IsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.StopSignpostEvent1, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.Signpost,
            Keywords = Keywords.Troubleshooting | Keywords.WFServices,
            Message = "Activity boundary.")]
        public void StopSignpostEvent1(string ExtendedData)
        {
            WriteEvent(EventIds.StopSignpostEvent1, ExtendedData);
        }

        public bool MessageLogInfoIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.WCFMessageLogging, EventChannel.Analytic);
        }

        [Event(EventIds.MessageLogInfo, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.WCFMessageLogging,
            Message = "{0}")]
        public void MessageLogInfo(string data1)
        {
            WriteEvent(EventIds.MessageLogInfo, data1);
        }

        public bool MessageLogWarningIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Troubleshooting | Keywords.WCFMessageLogging, EventChannel.Analytic);
        }

        [Event(EventIds.MessageLogWarning, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.WCFMessageLogging,
            Message = "{0}")]
        public void MessageLogWarning(string data1)
        {
            WriteEvent(EventIds.MessageLogWarning, data1);
        }

        public bool TransferEmittedIsEnabled()
        {
            return base.IsEnabled(EventLevel.LogAlways, Keywords.Troubleshooting | Keywords.UserEvents | Keywords.EndToEndMonitoring | Keywords.ServiceModel | Keywords.WFTracking | Keywords.ServiceHost | Keywords.WCFMessageLogging, EventChannel.Analytic);
        }

        [Event(EventIds.TransferEmitted, Level = EventLevel.LogAlways, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.UserEvents | Keywords.EndToEndMonitoring | Keywords.ServiceModel | Keywords.WFTracking | Keywords.ServiceHost | Keywords.WCFMessageLogging,
            Message = "Transfer event emitted.")]
        public void TransferEmitted(string HostReference)
        {
            WriteEvent(EventIds.TransferEmitted, HostReference);
        }

        public bool CompilationStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.CompilationStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ServiceHostCompilation,
            Keywords = Keywords.WebHost,
            Message = "Begin compilation")]
        public void CompilationStart()
        {
            WriteEvent(EventIds.CompilationStart);
        }

        public bool CompilationStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.CompilationStop, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ServiceHostCompilation,
            Keywords = Keywords.WebHost,
            Message = "End compilation")]
        public void CompilationStop()
        {
            WriteEvent(EventIds.CompilationStop);
        }

        public bool ServiceHostFactoryCreationStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.ServiceHostFactoryCreationStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ServiceHostFactoryCreation,
            Keywords = Keywords.WebHost,
            Message = "ServiceHostFactory begin creation")]
        public void ServiceHostFactoryCreationStart()
        {
            WriteEvent(EventIds.ServiceHostFactoryCreationStart);
        }

        public bool ServiceHostFactoryCreationStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.ServiceHostFactoryCreationStop, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ServiceHostFactoryCreation,
            Keywords = Keywords.WebHost,
            Message = "ServiceHostFactory end creation")]
        public void ServiceHostFactoryCreationStop()
        {
            WriteEvent(EventIds.ServiceHostFactoryCreationStop);
        }

        public bool CreateServiceHostStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.CreateServiceHostStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ServiceHostCreate,
            Keywords = Keywords.WebHost,
            Message = "Begin CreateServiceHost")]
        public void CreateServiceHostStart()
        {
            WriteEvent(EventIds.CreateServiceHostStart);
        }

        public bool CreateServiceHostStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.CreateServiceHostStop, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ServiceHostCreate,
            Keywords = Keywords.WebHost,
            Message = "End CreateServiceHost")]
        public void CreateServiceHostStop()
        {
            WriteEvent(EventIds.CreateServiceHostStop);
        }

        public bool HostedTransportConfigurationManagerConfigInitStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.HostedTransportConfigurationManagerConfigInitStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.HostedTransportConfigurationManagerConfigInit,
            Keywords = Keywords.WebHost,
            Message = "HostedTransportConfigurationManager begin configuration initialization")]
        public void HostedTransportConfigurationManagerConfigInitStart()
        {
            WriteEvent(EventIds.HostedTransportConfigurationManagerConfigInitStart);
        }

        public bool HostedTransportConfigurationManagerConfigInitStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.HostedTransportConfigurationManagerConfigInitStop, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.HostedTransportConfigurationManagerConfigInit,
            Keywords = Keywords.WebHost,
            Message = "HostedTransportConfigurationManager end configuration initialization")]
        public void HostedTransportConfigurationManagerConfigInitStop()
        {
            WriteEvent(EventIds.HostedTransportConfigurationManagerConfigInitStop);
        }

        public bool ServiceHostOpenStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceHost, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceHostOpenStart, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.ServiceHostOpen,
            Keywords = Keywords.ServiceHost,
            Message = "ServiceHost Open started.")]
        public void ServiceHostOpenStart()
        {
            WriteEvent(EventIds.ServiceHostOpenStart);
        }

        public bool ServiceHostOpenStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceHost, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceHostOpenStop, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.ServiceHostOpen,
            Keywords = Keywords.ServiceHost,
            Message = "ServiceHost Open completed.")]
        public void ServiceHostOpenStop()
        {
            WriteEvent(EventIds.ServiceHostOpenStop);
        }

        public bool WebHostRequestStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.WebHostRequestStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.WebHostRequest,
            Keywords = Keywords.WebHost,
            Message = "Received request with virtual path '{1}' from the AppDomain '{0}'.")]
        public void WebHostRequestStart(string AppDomainFriendlyName, string VirtualPath)
        {
            WriteEvent(EventIds.WebHostRequestStart, AppDomainFriendlyName, VirtualPath);
        }

        public bool WebHostRequestStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.WebHostRequestStop, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.WebHostRequest,
            Keywords = Keywords.WebHost,
            Message = "WebHostRequest stop.")]
        public void WebHostRequestStop()
        {
            WriteEvent(EventIds.WebHostRequestStop);
        }

        public bool CBAEntryReadIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, EventKeywords.None, EventChannel.Debug);
        }

        [Event(EventIds.CBAEntryRead, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = EventKeywords.None,
            Message = "Processed ServiceActivation Element Relative Address:'{0}', Normalized Relative Address '{1}' .")]
        public void CBAEntryRead(string RelativeAddress, string NormalizedAddress)
        {
            WriteEvent(EventIds.CBAEntryRead, RelativeAddress, NormalizedAddress);
        }

        public bool CBAMatchFoundIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, EventKeywords.None, EventChannel.Debug);
        }

        [Event(EventIds.CBAMatchFound, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = EventKeywords.None,
            Message = "Incoming request matches a ServiceActivation element with address '{0}'. ")]
        public void CBAMatchFound(string IncomingAddress)
        {
            WriteEvent(EventIds.CBAMatchFound, IncomingAddress);
        }

        public bool AspNetRoutingServiceIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.AspNetRoutingService, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.RoutingServices,
            Message = "Incoming request matches a WCF Service defined in Asp.Net route with address {0}.  ")]
        public void AspNetRoutingService(string IncomingAddress)
        {
            WriteEvent(EventIds.AspNetRoutingService, IncomingAddress);
        }

        public bool AspNetRouteIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.AspNetRoute, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.RoutingServices,
            Message = "A new Asp.Net route '{0}' with serviceType '{1}' and serviceHostFactoryType '{2}' is added.")]
        public void AspNetRoute(string AspNetRoutePrefix, string ServiceType, string ServiceHostFactoryType)
        {
            WriteEvent(EventIds.AspNetRoute, AspNetRoutePrefix, ServiceType, ServiceHostFactoryType);
        }

        public bool IncrementBusyCountIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.IncrementBusyCount, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.WebHostRequest,
            Keywords = Keywords.WebHost,
            Message = "IncrementBusyCount called. Source : {0}")]
        public void IncrementBusyCount(string Data)
        {
            WriteEvent(EventIds.IncrementBusyCount, Data);
        }

        public bool DecrementBusyCountIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.DecrementBusyCount, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.WebHostRequest,
            Keywords = Keywords.WebHost,
            Message = "DecrementBusyCount called. Source : {0}")]
        public void DecrementBusyCount(string Data)
        {
            WriteEvent(EventIds.DecrementBusyCount, Data);
        }

        public bool ServiceChannelOpenStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceChannelOpenStart, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.ServiceChannelOpen,
            Keywords = Keywords.ServiceModel,
            Message = "ServiceChannelOpen started.")]
        public void ServiceChannelOpenStart()
        {
            WriteEvent(EventIds.ServiceChannelOpenStart);
        }

        public bool ServiceChannelOpenStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceChannelOpenStop, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.ServiceChannelOpen,
            Keywords = Keywords.ServiceModel,
            Message = "ServiceChannelOpen completed.")]
        public void ServiceChannelOpenStop()
        {
            WriteEvent(EventIds.ServiceChannelOpenStop);
        }

        public bool ServiceChannelCallStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceChannelCallStart, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.ServiceChannelCall,
            Keywords = Keywords.ServiceModel,
            Message = "ServiceChannelCall started.")]
        public void ServiceChannelCallStart()
        {
            WriteEvent(EventIds.ServiceChannelCallStart);
        }

        public bool ServiceChannelBeginCallStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceChannelBeginCallStart, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.ServiceChannelCall,
            Keywords = Keywords.ServiceModel,
            Message = "ServiceChannel asynchronous calls started.")]
        public void ServiceChannelBeginCallStart()
        {
            WriteEvent(EventIds.ServiceChannelBeginCallStart);
        }

        public bool HttpSendMessageStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.HttpSendMessageStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportSend,
            Keywords = Keywords.HTTP,
            Message = "Http Send Request Start.")]
        public void HttpSendMessageStart()
        {
            WriteEvent(EventIds.HttpSendMessageStart);
        }

        public bool HttpSendStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.HttpSendStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.TransportSend,
            Keywords = Keywords.HTTP,
            Message = "Http Send Request Stop.")]
        public void HttpSendStop()
        {
            WriteEvent(EventIds.HttpSendStop);
        }

        public bool HttpMessageReceiveStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.HttpMessageReceiveStart, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "Message received from http transport.")]
        public void HttpMessageReceiveStart()
        {
            WriteEvent(EventIds.HttpMessageReceiveStart);
        }

        public bool DispatchMessageStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.DispatchMessageStart, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.DispatchStart, Task = Tasks.DispatchMessage,
            Keywords = Keywords.ServiceModel,
            Message = "Message dispatching started.")]
        public void DispatchMessageStart(string HostReference)
        {
            WriteEvent(EventIds.DispatchMessageStart, HostReference);
        }

        public bool HttpContextBeforeProcessAuthenticationIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.HttpContextBeforeProcessAuthentication, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.BeforeAuthentication, Task = Tasks.TransportReceive,
            Keywords = Keywords.ServiceModel,
            Message = "Start authentication for message dispatching")]
        public void HttpContextBeforeProcessAuthentication()
        {
            WriteEvent(EventIds.HttpContextBeforeProcessAuthentication);
        }

        public bool DispatchMessageBeforeAuthorizationIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.DispatchMessageBeforeAuthorization, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.BeforeAuthorization, Task = Tasks.DispatchMessage,
            Keywords = Keywords.ServiceModel,
            Message = "Start authorization for message dispatching")]
        public void DispatchMessageBeforeAuthorization()
        {
            WriteEvent(EventIds.DispatchMessageBeforeAuthorization);
        }

        public bool DispatchMessageStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.DispatchMessageStop, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.DispatchStop, Task = Tasks.DispatchMessage,
            Keywords = Keywords.ServiceModel,
            Message = "Message dispatching completed")]
        public void DispatchMessageStop()
        {
            WriteEvent(EventIds.DispatchMessageStop);
        }

        public bool ClientChannelOpenStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ClientChannelOpenStart, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.ClientChannelOpenStart, Task = Tasks.ClientRuntime,
            Keywords = Keywords.ServiceModel,
            Message = "ServiceChannel Open Start.")]
        public void ClientChannelOpenStart()
        {
            WriteEvent(EventIds.ClientChannelOpenStart);
        }

        public bool ClientChannelOpenStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ClientChannelOpenStop, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.ClientChannelOpenStop, Task = Tasks.ClientRuntime,
            Keywords = Keywords.ServiceModel,
            Message = "ServiceChannel Open Stop.")]
        public void ClientChannelOpenStop()
        {
            WriteEvent(EventIds.ClientChannelOpenStop);
        }

        public bool HttpSendStreamedMessageStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.HttpSendStreamedMessageStart, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.TransportSend,
            Keywords = Keywords.HTTP,
            Message = "Http Send streamed message started.")]
        public void HttpSendStreamedMessageStart()
        {
            WriteEvent(EventIds.HttpSendStreamedMessageStart);
        }

        public bool WorkflowApplicationCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowApplicationCompleted, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Completed, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' has completed in the Closed state.")]
        public void WorkflowApplicationCompleted(string data1)
        {
            WriteEvent(EventIds.WorkflowApplicationCompleted, data1);
        }

        public bool WorkflowApplicationTerminatedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowApplicationTerminated, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Terminated, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowApplication Id: '{0}' was terminated. It has completed in the Faulted state with an exception.")]
        public void WorkflowApplicationTerminated(string data1, string SerializedException)
        {
            WriteEvent(EventIds.WorkflowApplicationTerminated, data1, SerializedException);
        }

        public bool WorkflowInstanceCanceledIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowInstanceCanceled, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.InstanceCanceled, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' has completed in the Canceled state.")]
        public void WorkflowInstanceCanceled(string data1)
        {
            WriteEvent(EventIds.WorkflowInstanceCanceled, data1);
        }

        public bool WorkflowInstanceAbortedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowInstanceAborted, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.InstanceAborted, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' was aborted with an exception.")]
        public void WorkflowInstanceAborted(string data1, string SerializedException)
        {
            WriteEvent(EventIds.WorkflowInstanceAborted, data1, SerializedException);
        }

        public bool WorkflowApplicationIdledIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowApplicationIdled, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Idled, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowApplication Id: '{0}' went idle.")]
        public void WorkflowApplicationIdled(string data1)
        {
            WriteEvent(EventIds.WorkflowApplicationIdled, data1);
        }

        public bool WorkflowApplicationUnhandledExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowApplicationUnhandledException, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = Opcodes.UnhandledException, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' has encountered an unhandled exception.  The exception originated from Activity '{1}', DisplayName: '{2}'.  The following action will be taken: {3}.")]
        public void WorkflowApplicationUnhandledException(string data1, string data2, string data3, string data4, string SerializedException)
        {
            WriteEvent(EventIds.WorkflowApplicationUnhandledException, data1, data2, data3, data4, SerializedException);
        }

        public bool WorkflowApplicationPersistedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowApplicationPersisted, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Persisted, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowApplication Id: '{0}' was Persisted.")]
        public void WorkflowApplicationPersisted(string data1)
        {
            WriteEvent(EventIds.WorkflowApplicationPersisted, data1);
        }

        public bool WorkflowApplicationUnloadedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowApplicationUnloaded, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Unloaded, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' was Unloaded.")]
        public void WorkflowApplicationUnloaded(string data1)
        {
            WriteEvent(EventIds.WorkflowApplicationUnloaded, data1);
        }

        public bool ActivityScheduledIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ActivityScheduled, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ScheduleActivity,
            Keywords = Keywords.WFRuntime,
            Message = "Parent Activity '{0}', DisplayName: '{1}', InstanceId: '{2}' scheduled child Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void ActivityScheduled(string data1, string data2, string data3, string data4, string data5, string data6)
        {
            WriteEvent(EventIds.ActivityScheduled, data1, data2, data3, data4, data5, data6);
        }

        public bool ActivityCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ActivityCompleted, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.CompleteActivity,
            Keywords = Keywords.WFRuntime,
            Message = "Activity '{0}', DisplayName: '{1}', InstanceId: '{2}' has completed in the '{3}' state.")]
        public void ActivityCompleted(string data1, string data2, string data3, string data4)
        {
            WriteEvent(EventIds.ActivityCompleted, data1, data2, data3, data4);
        }

        public bool ScheduleExecuteActivityWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ScheduleExecuteActivityWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleExecuteActivity, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "An ExecuteActivityWorkItem has been scheduled for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void ScheduleExecuteActivityWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(EventIds.ScheduleExecuteActivityWorkItem, data1, data2, data3);
        }

        public bool StartExecuteActivityWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.StartExecuteActivityWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.StartExecuteActivity, Task = Tasks.StartWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Starting execution of an ExecuteActivityWorkItem for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void StartExecuteActivityWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(EventIds.StartExecuteActivityWorkItem, data1, data2, data3);
        }

        public bool CompleteExecuteActivityWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CompleteExecuteActivityWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompleteExecuteActivity, Task = Tasks.CompleteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "An ExecuteActivityWorkItem has completed for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void CompleteExecuteActivityWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(EventIds.CompleteExecuteActivityWorkItem, data1, data2, data3);
        }

        public bool ScheduleCompletionWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ScheduleCompletionWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleCompletion, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A CompletionWorkItem has been scheduled for parent Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.  Completed Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void ScheduleCompletionWorkItem(string data1, string data2, string data3, string data4, string data5, string data6)
        {
            WriteEvent(EventIds.ScheduleCompletionWorkItem, data1, data2, data3, data4, data5, data6);
        }

        public bool StartCompletionWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.StartCompletionWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.StartCompletion, Task = Tasks.StartWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Starting execution of a CompletionWorkItem for parent Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'. Completed Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void StartCompletionWorkItem(string data1, string data2, string data3, string data4, string data5, string data6)
        {
            WriteEvent(EventIds.StartCompletionWorkItem, data1, data2, data3, data4, data5, data6);
        }

        public bool CompleteCompletionWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CompleteCompletionWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompleteCompletion, Task = Tasks.CompleteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A CompletionWorkItem has completed for parent Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'. Completed Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void CompleteCompletionWorkItem(string data1, string data2, string data3, string data4, string data5, string data6)
        {
            WriteEvent(EventIds.CompleteCompletionWorkItem, data1, data2, data3, data4, data5, data6);
        }

        public bool ScheduleCancelActivityWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ScheduleCancelActivityWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleCancelActivity, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A CancelActivityWorkItem has been scheduled for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void ScheduleCancelActivityWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(EventIds.ScheduleCancelActivityWorkItem, data1, data2, data3);
        }

        public bool StartCancelActivityWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.StartCancelActivityWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.StartCancelActivity, Task = Tasks.StartWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Starting execution of a CancelActivityWorkItem for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void StartCancelActivityWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(EventIds.StartCancelActivityWorkItem, data1, data2, data3);
        }

        public bool CompleteCancelActivityWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CompleteCancelActivityWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompleteCancelActivity, Task = Tasks.CompleteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A CancelActivityWorkItem has completed for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void CompleteCancelActivityWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(EventIds.CompleteCancelActivityWorkItem, data1, data2, data3);
        }

        public bool CreateBookmarkIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CreateBookmark, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.CreateBookmark,
            Keywords = Keywords.WFRuntime,
            Message = "A Bookmark has been created for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.  BookmarkName: {3}, BookmarkScope: {4}.")]
        public void CreateBookmark(string data1, string data2, string data3, string data4, string data5)
        {
            WriteEvent(EventIds.CreateBookmark, data1, data2, data3, data4, data5);
        }

        public bool ScheduleBookmarkWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ScheduleBookmarkWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleBookmark, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A BookmarkWorkItem has been scheduled for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.  BookmarkName: {3}, BookmarkScope: {4}.")]
        public void ScheduleBookmarkWorkItem(string data1, string data2, string data3, string data4, string data5)
        {
            WriteEvent(EventIds.ScheduleBookmarkWorkItem, data1, data2, data3, data4, data5);
        }

        public bool StartBookmarkWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.StartBookmarkWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.StartBookmark, Task = Tasks.StartWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Starting execution of a BookmarkWorkItem for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.  BookmarkName: {3}, BookmarkScope: {4}.")]
        public void StartBookmarkWorkItem(string data1, string data2, string data3, string data4, string data5)
        {
            WriteEvent(EventIds.StartBookmarkWorkItem, data1, data2, data3, data4, data5);
        }

        public bool CompleteBookmarkWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CompleteBookmarkWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompleteBookmark, Task = Tasks.CompleteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A BookmarkWorkItem has completed for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'. BookmarkName: {3}, BookmarkScope: {4}.")]
        public void CompleteBookmarkWorkItem(string data1, string data2, string data3, string data4, string data5)
        {
            WriteEvent(EventIds.CompleteBookmarkWorkItem, data1, data2, data3, data4, data5);
        }

        public bool CreateBookmarkScopeIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CreateBookmarkScope, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.CreateBookmark,
            Keywords = Keywords.WFRuntime,
            Message = "A BookmarkScope has been created: {0}.")]
        public void CreateBookmarkScope(string data1)
        {
            WriteEvent(EventIds.CreateBookmarkScope, data1);
        }

        public bool BookmarkScopeInitializedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.BookmarkScopeInitialized, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.InitializeBookmarkScope,
            Keywords = Keywords.WFRuntime,
            Message = "The BookmarkScope that had TemporaryId: '{0}' has been initialized with Id: '{1}'.")]
        public void BookmarkScopeInitialized(string data1, string data2)
        {
            WriteEvent(EventIds.BookmarkScopeInitialized, data1, data2);
        }

        public bool ScheduleTransactionContextWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ScheduleTransactionContextWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleTransactionContext, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A TransactionContextWorkItem has been scheduled for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void ScheduleTransactionContextWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(EventIds.ScheduleTransactionContextWorkItem, data1, data2, data3);
        }

        public bool StartTransactionContextWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.StartTransactionContextWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.StartTransactionContext, Task = Tasks.StartWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Starting execution of a TransactionContextWorkItem for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void StartTransactionContextWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(EventIds.StartTransactionContextWorkItem, data1, data2, data3);
        }

        public bool CompleteTransactionContextWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CompleteTransactionContextWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompleteTransactionContext, Task = Tasks.CompleteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A TransactionContextWorkItem has completed for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void CompleteTransactionContextWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(EventIds.CompleteTransactionContextWorkItem, data1, data2, data3);
        }

        public bool ScheduleFaultWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ScheduleFaultWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleFault, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A FaultWorkItem has been scheduled for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.  The exception was propagated from Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void ScheduleFaultWorkItem(string data1, string data2, string data3, string data4, string data5, string data6, string SerializedException)
        {
            WriteEvent(EventIds.ScheduleFaultWorkItem, data1, data2, data3, data4, data5, data6, SerializedException);
        }

        public bool StartFaultWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.StartFaultWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.StartFault, Task = Tasks.StartWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Starting execution of a FaultWorkItem for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.  The exception was propagated from Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void StartFaultWorkItem(string data1, string data2, string data3, string data4, string data5, string data6, string SerializedException)
        {
            WriteEvent(EventIds.StartFaultWorkItem, data1, data2, data3, data4, data5, data6, SerializedException);
        }

        public bool CompleteFaultWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CompleteFaultWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompleteFault, Task = Tasks.CompleteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A FaultWorkItem has completed for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'. The exception was propagated from Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void CompleteFaultWorkItem(string data1, string data2, string data3, string data4, string data5, string data6, string SerializedException)
        {
            WriteEvent(EventIds.CompleteFaultWorkItem, data1, data2, data3, data4, data5, data6, SerializedException);
        }

        public bool ScheduleRuntimeWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ScheduleRuntimeWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleRuntime, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A runtime work item has been scheduled for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void ScheduleRuntimeWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(EventIds.ScheduleRuntimeWorkItem, data1, data2, data3);
        }

        public bool StartRuntimeWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.StartRuntimeWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.StartRuntime, Task = Tasks.StartWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Starting execution of a runtime work item for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void StartRuntimeWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(EventIds.StartRuntimeWorkItem, data1, data2, data3);
        }

        public bool CompleteRuntimeWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CompleteRuntimeWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompleteRuntime, Task = Tasks.CompleteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A runtime work item has completed for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void CompleteRuntimeWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(EventIds.CompleteRuntimeWorkItem, data1, data2, data3);
        }

        public bool RuntimeTransactionSetIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFServices, EventChannel.Debug);
        }

        [Event(EventIds.RuntimeTransactionSet, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.Set, Task = Tasks.RuntimeTransaction,
            Keywords = Keywords.WFServices,
            Message = "The runtime transaction has been set by Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.  Execution isolated to Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void RuntimeTransactionSet(string data1, string data2, string data3, string data4, string data5, string data6)
        {
            WriteEvent(EventIds.RuntimeTransactionSet, data1, data2, data3, data4, data5, data6);
        }

        public bool RuntimeTransactionCompletionRequestedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFServices, EventChannel.Debug);
        }

        [Event(EventIds.RuntimeTransactionCompletionRequested, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompletionRequested, Task = Tasks.RuntimeTransaction,
            Keywords = Keywords.WFServices,
            Message = "Activity '{0}', DisplayName: '{1}', InstanceId: '{2}' has scheduled completion of the runtime transaction.")]
        public void RuntimeTransactionCompletionRequested(string data1, string data2, string data3)
        {
            WriteEvent(EventIds.RuntimeTransactionCompletionRequested, data1, data2, data3);
        }

        public bool RuntimeTransactionCompleteIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFServices, EventChannel.Debug);
        }

        [Event(EventIds.RuntimeTransactionComplete, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.Complete, Task = Tasks.RuntimeTransaction,
            Keywords = Keywords.WFServices,
            Message = "The runtime transaction has completed with the state '{0}'.")]
        public void RuntimeTransactionComplete(string data1)
        {
            WriteEvent(EventIds.RuntimeTransactionComplete, data1);
        }

        public bool EnterNoPersistBlockIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.EnterNoPersistBlock, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.NoPersistBlock,
            Keywords = Keywords.WFRuntime,
            Message = "Entering a no persist block.")]
        public void EnterNoPersistBlock()
        {
            WriteEvent(EventIds.EnterNoPersistBlock);
        }

        public bool ExitNoPersistBlockIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ExitNoPersistBlock, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.NoPersistBlock,
            Keywords = Keywords.WFRuntime,
            Message = "Exiting a no persist block.")]
        public void ExitNoPersistBlock()
        {
            WriteEvent(EventIds.ExitNoPersistBlock);
        }

        public bool InArgumentBoundIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.InArgumentBound, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ExecuteActivity,
            Keywords = Keywords.WFActivities,
            Message = "In argument '{0}' on Activity '{1}', DisplayName: '{2}', InstanceId: '{3}' has been bound with value: {4}.")]
        public void InArgumentBound(string data1, string data2, string data3, string data4, string data5)
        {
            WriteEvent(EventIds.InArgumentBound, data1, data2, data3, data4, data5);
        }

        public bool WorkflowApplicationPersistableIdleIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowApplicationPersistableIdle, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.PersistableIdle, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowApplication Id: '{0}' is idle and persistable.  The following action will be taken: {1}.")]
        public void WorkflowApplicationPersistableIdle(string data1, string data2)
        {
            WriteEvent(EventIds.WorkflowApplicationPersistableIdle, data1, data2);
        }

        public bool WorkflowActivityStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowActivityStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.WorkflowActivity,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' E2E Activity")]
        public void WorkflowActivityStart(Guid Id)
        {
            WriteEvent(EventIds.WorkflowActivityStart, Id);
        }

        public bool WorkflowActivityStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowActivityStop, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.WorkflowActivity,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' E2E Activity")]
        public void WorkflowActivityStop(Guid Id)
        {
            WriteEvent(EventIds.WorkflowActivityStop, Id);
        }

        public bool WorkflowActivitySuspendIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowActivitySuspend, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Suspend, Task = Tasks.WorkflowActivity,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' E2E Activity")]
        public void WorkflowActivitySuspend(Guid Id)
        {
            WriteEvent(EventIds.WorkflowActivitySuspend, Id);
        }

        public bool WorkflowActivityResumeIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowActivityResume, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Resume, Task = Tasks.WorkflowActivity,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' E2E Activity")]
        public void WorkflowActivityResume(Guid Id)
        {
            WriteEvent(EventIds.WorkflowActivityResume, Id);
        }

        public bool InvokeMethodIsStaticIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.InvokeMethodIsStatic, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.IsStatic, Task = Tasks.InvokeMethod,
            Keywords = Keywords.WFRuntime,
            Message = "InvokeMethod '{0}' - method is Static.")]
        public void InvokeMethodIsStatic(string data1)
        {
            WriteEvent(EventIds.InvokeMethodIsStatic, data1);
        }

        public bool InvokeMethodIsNotStaticIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.InvokeMethodIsNotStatic, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.IsNotStatic, Task = Tasks.InvokeMethod,
            Keywords = Keywords.WFRuntime,
            Message = "InvokeMethod '{0}' - method is not Static.")]
        public void InvokeMethodIsNotStatic(string data1)
        {
            WriteEvent(EventIds.InvokeMethodIsNotStatic, data1);
        }

        public bool InvokedMethodThrewExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.InvokedMethodThrewException, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ThrewException, Task = Tasks.InvokeMethod,
            Keywords = Keywords.WFRuntime,
            Message = "An exception was thrown in the method called by the activity '{0}'. {1}")]
        public void InvokedMethodThrewException(string data1, string data2)
        {
            WriteEvent(EventIds.InvokedMethodThrewException, data1, data2);
        }

        public bool InvokeMethodUseAsyncPatternIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.InvokeMethodUseAsyncPattern, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.UseAsyncPattern, Task = Tasks.InvokeMethod,
            Keywords = Keywords.WFRuntime,
            Message = "InvokeMethod '{0}' - method uses asynchronous pattern of '{1}' and '{2}'.")]
        public void InvokeMethodUseAsyncPattern(string data1, string data2, string data3)
        {
            WriteEvent(EventIds.InvokeMethodUseAsyncPattern, data1, data2, data3);
        }

        public bool InvokeMethodDoesNotUseAsyncPatternIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.InvokeMethodDoesNotUseAsyncPattern, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.DoesNotUseAsyncPattern, Task = Tasks.InvokeMethod,
            Keywords = Keywords.WFRuntime,
            Message = "InvokeMethod '{0}' - method does not use asynchronous pattern.")]
        public void InvokeMethodDoesNotUseAsyncPattern(string data1)
        {
            WriteEvent(EventIds.InvokeMethodDoesNotUseAsyncPattern, data1);
        }

        public bool FlowchartStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.FlowchartStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Begin, Task = Tasks.ExecuteFlowchart,
            Keywords = Keywords.WFActivities,
            Message = "Flowchart '{0}' - Start has been scheduled.")]
        public void FlowchartStart(string data1)
        {
            WriteEvent(EventIds.FlowchartStart, data1);
        }

        public bool FlowchartEmptyIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.FlowchartEmpty, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.Empty, Task = Tasks.ExecuteFlowchart,
            Keywords = Keywords.WFActivities,
            Message = "Flowchart '{0}' - was executed with no Nodes.")]
        public void FlowchartEmpty(string data1)
        {
            WriteEvent(EventIds.FlowchartEmpty, data1);
        }

        public bool FlowchartNextNullIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.FlowchartNextNull, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.NextNull, Task = Tasks.ExecuteFlowchart,
            Keywords = Keywords.WFActivities,
            Message = "Flowchart '{0}'/FlowStep - Next node is null. Flowchart execution will end.")]
        public void FlowchartNextNull(string data1)
        {
            WriteEvent(EventIds.FlowchartNextNull, data1);
        }

        public bool FlowchartSwitchCaseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.FlowchartSwitchCase, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.SwitchCase, Task = Tasks.ExecuteFlowchart,
            Keywords = Keywords.WFActivities,
            Message = "Flowchart '{0}'/FlowSwitch - Case '{1}' was selected.")]
        public void FlowchartSwitchCase(string data1, string data2)
        {
            WriteEvent(EventIds.FlowchartSwitchCase, data1, data2);
        }

        public bool FlowchartSwitchDefaultIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.FlowchartSwitchDefault, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.SwitchDefault, Task = Tasks.ExecuteFlowchart,
            Keywords = Keywords.WFActivities,
            Message = "Flowchart '{0}'/FlowSwitch - Default Case was selected.")]
        public void FlowchartSwitchDefault(string data1)
        {
            WriteEvent(EventIds.FlowchartSwitchDefault, data1);
        }

        public bool FlowchartSwitchCaseNotFoundIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.FlowchartSwitchCaseNotFound, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.SwitchCaseNotFound, Task = Tasks.ExecuteFlowchart,
            Keywords = Keywords.WFActivities,
            Message = "Flowchart '{0}'/FlowSwitch - could find neither a Case activity nor a Default Case matching the Expression result. Flowchart execution will end.")]
        public void FlowchartSwitchCaseNotFound(string data1)
        {
            WriteEvent(EventIds.FlowchartSwitchCaseNotFound, data1);
        }

        public bool CompensationStateIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.CompensationState, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.CompensationState,
            Keywords = Keywords.WFActivities,
            Message = "CompensableActivity '{0}' is in the '{1}' state.")]
        public void CompensationState(string data1, string data2)
        {
            WriteEvent(EventIds.CompensationState, data1, data2);
        }

        public bool SwitchCaseNotFoundIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.SwitchCaseNotFound, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ExecuteActivity,
            Keywords = Keywords.WFActivities,
            Message = "The Switch activity '{0}' could not find a Case activity matching the Expression result.")]
        public void SwitchCaseNotFound(string data1)
        {
            WriteEvent(EventIds.SwitchCaseNotFound, data1);
        }

        public bool ChannelInitializationTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ChannelInitializationTimeout, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TimeoutException,
            Keywords = Keywords.ServiceModel,
            Message = "{0}")]
        public void ChannelInitializationTimeout(string data1)
        {
            WriteEvent(EventIds.ChannelInitializationTimeout, data1);
        }

        public bool CloseTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.CloseTimeout, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TimeoutException,
            Keywords = Keywords.ServiceModel,
            Message = "{0}")]
        public void CloseTimeout(string data1)
        {
            WriteEvent(EventIds.CloseTimeout, data1);
        }

        public bool IdleTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.IdleTimeout, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TimeoutException,
            Keywords = Keywords.ServiceModel,
            Message = "{0} Connection pool key: {1}")]
        public void IdleTimeout(string msg, string key)
        {
            WriteEvent(EventIds.IdleTimeout, msg, key);
        }

        public bool LeaseTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.LeaseTimeout, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TimeoutException,
            Keywords = Keywords.ServiceModel,
            Message = "{0} Connection pool key: {1}")]
        public void LeaseTimeout(string msg, string key)
        {
            WriteEvent(EventIds.LeaseTimeout, msg, key);
        }

        public bool OpenTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.OpenTimeout, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TimeoutException,
            Keywords = Keywords.ServiceModel,
            Message = "{0}")]
        public void OpenTimeout(string data1)
        {
            WriteEvent(EventIds.OpenTimeout, data1);
        }

        public bool ReceiveTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ReceiveTimeout, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TimeoutException,
            Keywords = Keywords.ServiceModel,
            Message = "{0}")]
        public void ReceiveTimeout(string data1)
        {
            WriteEvent(EventIds.ReceiveTimeout, data1);
        }

        public bool SendTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.SendTimeout, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TimeoutException,
            Keywords = Keywords.ServiceModel,
            Message = "{0}")]
        public void SendTimeout(string data1)
        {
            WriteEvent(EventIds.SendTimeout, data1);
        }

        public bool InactivityTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.InactivityTimeout, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TimeoutException,
            Keywords = Keywords.ServiceModel,
            Message = "{0}")]
        public void InactivityTimeout(string data1)
        {
            WriteEvent(EventIds.InactivityTimeout, data1);
        }

        public bool MaxReceivedMessageSizeExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.MaxReceivedMessageSizeExceeded, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void MaxReceivedMessageSizeExceeded(string data1)
        {
            WriteEvent(EventIds.MaxReceivedMessageSizeExceeded, data1);
        }

        public bool MaxSentMessageSizeExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.MaxSentMessageSizeExceeded, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void MaxSentMessageSizeExceeded(string data1)
        {
            WriteEvent(EventIds.MaxSentMessageSizeExceeded, data1);
        }

        public bool MaxOutboundConnectionsPerEndpointExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Quota, EventChannel.Debug);
        }

        [Event(EventIds.MaxOutboundConnectionsPerEndpointExceeded, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void MaxOutboundConnectionsPerEndpointExceeded(string data1)
        {
            WriteEvent(EventIds.MaxOutboundConnectionsPerEndpointExceeded, data1);
        }

        public bool MaxPendingConnectionsExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Quota, EventChannel.Debug);
        }

        [Event(EventIds.MaxPendingConnectionsExceeded, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void MaxPendingConnectionsExceeded(string data1)
        {
            WriteEvent(EventIds.MaxPendingConnectionsExceeded, data1);
        }

        public bool ReaderQuotaExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.ReaderQuotaExceeded, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void ReaderQuotaExceeded(string data1)
        {
            WriteEvent(EventIds.ReaderQuotaExceeded, data1);
        }

        public bool NegotiateTokenAuthenticatorStateCacheExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.NegotiateTokenAuthenticatorStateCacheExceeded, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void NegotiateTokenAuthenticatorStateCacheExceeded(string msg)
        {
            WriteEvent(EventIds.NegotiateTokenAuthenticatorStateCacheExceeded, msg);
        }

        public bool NegotiateTokenAuthenticatorStateCacheRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Debug);
        }

        [Event(EventIds.NegotiateTokenAuthenticatorStateCacheRatio, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Negotiate token authenticator state cache ratio: {0}/{1}")]
        public void NegotiateTokenAuthenticatorStateCacheRatio(int cur, int max)
        {
            WriteEvent(EventIds.NegotiateTokenAuthenticatorStateCacheRatio, cur, max);
        }

        public bool SecuritySessionRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Debug);
        }

        [Event(EventIds.SecuritySessionRatio, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Security session ratio: {0}/{1}")]
        public void SecuritySessionRatio(int cur, int max)
        {
            WriteEvent(EventIds.SecuritySessionRatio, cur, max);
        }

        public bool PendingConnectionsRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.PendingConnectionsRatio, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Pending connections ratio: {0}/{1}")]
        public void PendingConnectionsRatio(int cur, int max)
        {
            WriteEvent(EventIds.PendingConnectionsRatio, cur, max);
        }

        public bool ConcurrentCallsRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.ConcurrentCallsRatio, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Concurrent calls ratio: {0}/{1}")]
        public void ConcurrentCallsRatio(int cur, int max)
        {
            WriteEvent(EventIds.ConcurrentCallsRatio, cur, max);
        }

        public bool ConcurrentSessionsRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.ConcurrentSessionsRatio, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Concurrent sessions ratio: {0}/{1}")]
        public void ConcurrentSessionsRatio(int cur, int max)
        {
            WriteEvent(EventIds.ConcurrentSessionsRatio, cur, max);
        }

        public bool OutboundConnectionsPerEndpointRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.OutboundConnectionsPerEndpointRatio, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Outbound connections per endpoint ratio: {0}/{1}")]
        public void OutboundConnectionsPerEndpointRatio(int cur, int max)
        {
            WriteEvent(EventIds.OutboundConnectionsPerEndpointRatio, cur, max);
        }

        public bool PendingMessagesPerChannelRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.PendingMessagesPerChannelRatio, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Pending messages per channel ratio: {0}/{1}")]
        public void PendingMessagesPerChannelRatio(int cur, int max)
        {
            WriteEvent(EventIds.PendingMessagesPerChannelRatio, cur, max);
        }

        public bool ConcurrentInstancesRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.ConcurrentInstancesRatio, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Concurrent instances ratio: {0}/{1}")]
        public void ConcurrentInstancesRatio(int cur, int max)
        {
            WriteEvent(EventIds.ConcurrentInstancesRatio, cur, max);
        }

        public bool PendingAcceptsAtZeroIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Quota, EventChannel.Debug);
        }

        [Event(EventIds.PendingAcceptsAtZero, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Zero pending accepts left")]
        public void PendingAcceptsAtZero()
        {
            WriteEvent(EventIds.PendingAcceptsAtZero);
        }

        public bool MaxSessionSizeReachedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.MaxSessionSizeReached, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void MaxSessionSizeReached(string data1)
        {
            WriteEvent(EventIds.MaxSessionSizeReached, data1);
        }

        public bool ReceiveRetryCountReachedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.ReceiveRetryCountReached, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.MsmqQuotas,
            Keywords = Keywords.Quota,
            Message = "Receive retry count reached on MSMQ message with id '{0}'")]
        public void ReceiveRetryCountReached(string data1)
        {
            WriteEvent(EventIds.ReceiveRetryCountReached, data1);
        }

        public bool MaxRetryCyclesExceededMsmqIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.MaxRetryCyclesExceededMsmq, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.MsmqQuotas,
            Keywords = Keywords.Quota,
            Message = "Max retry cycles exceeded on MSMQ message with id '{0}'")]
        public void MaxRetryCyclesExceededMsmq(string data1)
        {
            WriteEvent(EventIds.MaxRetryCyclesExceededMsmq, data1);
        }

        public bool ReadPoolMissIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.ReadPoolMiss, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Created new '{0}'")]
        public void ReadPoolMiss(string itemTypeName)
        {
            WriteEvent(EventIds.ReadPoolMiss, itemTypeName);
        }

        public bool WritePoolMissIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.WritePoolMiss, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Created new '{0}'")]
        public void WritePoolMiss(string itemTypeName)
        {
            WriteEvent(EventIds.WritePoolMiss, itemTypeName);
        }

        public bool WfMessageReceivedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Debug);
        }

        [Event(EventIds.WfMessageReceived, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Receive, Task = Tasks.WFMessage,
            Keywords = Keywords.WFServices,
            Message = "Message received by workflow")]
        public void WfMessageReceived()
        {
            WriteEvent(EventIds.WfMessageReceived);
        }

        public bool WfMessageSentIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Debug);
        }

        [Event(EventIds.WfMessageSent, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Send, Task = Tasks.WFMessage,
            Keywords = Keywords.WFServices,
            Message = "Message sent from workflow")]
        public void WfMessageSent()
        {
            WriteEvent(EventIds.WfMessageSent);
        }

        public bool MaxRetryCyclesExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.MaxRetryCyclesExceeded, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void MaxRetryCyclesExceeded(string data1)
        {
            WriteEvent(EventIds.MaxRetryCyclesExceeded, data1);
        }

        public bool ExecuteWorkItemStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ExecuteWorkItemStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ExecuteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Execute work item start")]
        public void ExecuteWorkItemStart()
        {
            WriteEvent(EventIds.ExecuteWorkItemStart);
        }

        public bool ExecuteWorkItemStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ExecuteWorkItemStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ExecuteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Execute work item stop")]
        public void ExecuteWorkItemStop()
        {
            WriteEvent(EventIds.ExecuteWorkItemStop);
        }

        public bool SendMessageChannelCacheMissIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.SendMessageChannelCacheMiss, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.Missed, Task = Tasks.MessageChannelCache,
            Keywords = Keywords.WFRuntime,
            Message = "SendMessageChannelCache miss")]
        public void SendMessageChannelCacheMiss()
        {
            WriteEvent(EventIds.SendMessageChannelCacheMiss);
        }

        public bool InternalCacheMetadataStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.InternalCacheMetadataStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.InternalCacheMetadata,
            Keywords = Keywords.WFRuntime,
            Message = "InternalCacheMetadata started on activity '{0}'.")]
        public void InternalCacheMetadataStart(string id)
        {
            WriteEvent(EventIds.InternalCacheMetadataStart, id);
        }

        public bool InternalCacheMetadataStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.InternalCacheMetadataStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.InternalCacheMetadata,
            Keywords = Keywords.WFRuntime,
            Message = "InternalCacheMetadata stopped on activity '{0}'.")]
        public void InternalCacheMetadataStop(string id)
        {
            WriteEvent(EventIds.InternalCacheMetadataStop, id);
        }

        public bool CompileVbExpressionStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CompileVbExpressionStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.VBExpressionCompile,
            Keywords = Keywords.WFRuntime,
            Message = "Compiling VB expression '{0}'")]
        public void CompileVbExpressionStart(string expr)
        {
            WriteEvent(EventIds.CompileVbExpressionStart, expr);
        }

        public bool CacheRootMetadataStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CacheRootMetadataStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.CacheRootMetadata,
            Keywords = Keywords.WFRuntime,
            Message = "CacheRootMetadata started on activity '{0}'")]
        public void CacheRootMetadataStart(string activityName)
        {
            WriteEvent(EventIds.CacheRootMetadataStart, activityName);
        }

        public bool CacheRootMetadataStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CacheRootMetadataStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.CacheRootMetadata,
            Keywords = Keywords.WFRuntime,
            Message = "CacheRootMetadata stopped on activity {0}.")]
        public void CacheRootMetadataStop(string activityName)
        {
            WriteEvent(EventIds.CacheRootMetadataStop, activityName);
        }

        public bool CompileVbExpressionStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CompileVbExpressionStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.VBExpressionCompile,
            Keywords = Keywords.WFRuntime,
            Message = "Finished compiling VB expression.")]
        public void CompileVbExpressionStop()
        {
            WriteEvent(EventIds.CompileVbExpressionStop);
        }

        public bool TryCatchExceptionFromTryIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.TryCatchExceptionFromTry, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.FromTry, Task = Tasks.TryCatchException,
            Keywords = Keywords.WFActivities,
            Message = "The TryCatch activity '{0}' has caught an exception of type '{1}'.")]
        public void TryCatchExceptionFromTry(string data1, string data2)
        {
            WriteEvent(EventIds.TryCatchExceptionFromTry, data1, data2);
        }

        public bool TryCatchExceptionDuringCancelationIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.TryCatchExceptionDuringCancelation, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.DuringCancelation, Task = Tasks.TryCatchException,
            Keywords = Keywords.WFActivities,
            Message = "A child activity of the TryCatch activity '{0}' has thrown an exception during cancelation.")]
        public void TryCatchExceptionDuringCancelation(string data1)
        {
            WriteEvent(EventIds.TryCatchExceptionDuringCancelation, data1);
        }

        public bool TryCatchExceptionFromCatchOrFinallyIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.TryCatchExceptionFromCatchOrFinally, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.FromCatchOrFinally, Task = Tasks.TryCatchException,
            Keywords = Keywords.WFActivities,
            Message = "A Catch or Finally activity that is associated with the TryCatch activity '{0}' has thrown an exception.")]
        public void TryCatchExceptionFromCatchOrFinally(string data1)
        {
            WriteEvent(EventIds.TryCatchExceptionFromCatchOrFinally, data1);
        }

        public bool ReceiveContextCompleteFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Channel, EventChannel.Analytic);
        }

        [Event(EventIds.ReceiveContextCompleteFailed, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Channel,
            Message = "Failed to Complete {0}.")]
        public void ReceiveContextCompleteFailed(string TypeName)
        {
            WriteEvent(EventIds.ReceiveContextCompleteFailed, TypeName);
        }

        public bool ReceiveContextAbandonFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ReceiveContextAbandonFailed, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Channel,
            Message = "Failed to Abandon {0}.")]
        public void ReceiveContextAbandonFailed(string TypeName)
        {
            WriteEvent(EventIds.ReceiveContextAbandonFailed, TypeName);
        }

        public bool ReceiveContextFaultedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ReceiveContextFaulted, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.DispatchMessage,
            Keywords = Keywords.ServiceModel,
            Message = "Receive Context faulted.")]
        public void ReceiveContextFaulted(string EventSource)
        {
            WriteEvent(EventIds.ReceiveContextFaulted, EventSource);
        }

        public bool ReceiveContextAbandonWithExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ReceiveContextAbandonWithException, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Channel,
            Message = "{0} was Abandoned with exception {1}.")]
        public void ReceiveContextAbandonWithException(string TypeName, string ExceptionToString)
        {
            WriteEvent(EventIds.ReceiveContextAbandonWithException, TypeName, ExceptionToString);
        }

        public bool ClientBaseCachedChannelFactoryCountIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ClientBaseCachedChannelFactoryCount, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ChannelFactoryCaching,
            Keywords = Keywords.ServiceModel,
            Message = "Number of cached channel factories is: '{0}'.  At most '{1}' channel factories can be cached.")]
        public void ClientBaseCachedChannelFactoryCount(int Count, int MaxNum, string EventSource)
        {
            WriteEvent(EventIds.ClientBaseCachedChannelFactoryCount, Count, MaxNum, EventSource);
        }

        public bool ClientBaseChannelFactoryAgedOutofCacheIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ClientBaseChannelFactoryAgedOutofCache, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ChannelFactoryCaching,
            Keywords = Keywords.ServiceModel,
            Message = "A channel factory has been aged out of the cache because the cache has reached its limit of '{0}'.")]
        public void ClientBaseChannelFactoryAgedOutofCache(int Count, string EventSource)
        {
            WriteEvent(EventIds.ClientBaseChannelFactoryAgedOutofCache, Count, EventSource);
        }

        public bool ClientBaseChannelFactoryCacheHitIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ClientBaseChannelFactoryCacheHit, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ChannelFactoryCaching,
            Keywords = Keywords.ServiceModel,
            Message = "Used matching channel factory found in cache.")]
        public void ClientBaseChannelFactoryCacheHit(string EventSource)
        {
            WriteEvent(EventIds.ClientBaseChannelFactoryCacheHit, EventSource);
        }

        public bool ClientBaseUsingLocalChannelFactoryIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ClientBaseUsingLocalChannelFactory, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ChannelFactoryCaching,
            Keywords = Keywords.ServiceModel,
            Message = "Not using channel factory from cache, i.e. caching disabled for instance.")]
        public void ClientBaseUsingLocalChannelFactory(string EventSource)
        {
            WriteEvent(EventIds.ClientBaseUsingLocalChannelFactory, EventSource);
        }

        public bool QueryCompositionExecutedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.QueryCompositionExecuted, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.ServiceModel,
            Message = "Query composition using '{0}' was executed on the Request Uri: '{1}'.")]
        public void QueryCompositionExecuted(string TypeName, string Uri, string EventSource)
        {
            WriteEvent(EventIds.QueryCompositionExecuted, TypeName, Uri, EventSource);
        }

        public bool DispatchFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.DispatchFailed, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.DispatchMessage,
            Keywords = Keywords.ServiceModel,
            Message = "The '{0}' operation was dispatched with errors.")]
        public void DispatchFailed(string OperationName, string HostReference)
        {
            WriteEvent(EventIds.DispatchFailed, OperationName, HostReference);
        }

        public bool DispatchSuccessfulIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.DispatchSuccessful, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.DispatchMessage,
            Keywords = Keywords.ServiceModel,
            Message = "The '{0}' operation was dispatched successfully.")]
        public void DispatchSuccessful(string OperationName, string HostReference)
        {
            WriteEvent(EventIds.DispatchSuccessful, OperationName, HostReference);
        }

        public bool MessageReadByEncoderIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.MessageReadByEncoder, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.MessageDecoding,
            Keywords = Keywords.Channel,
            Message = "A message with size '{0}' bytes was read by the encoder.")]
        public void MessageReadByEncoder(int Size, string EventSource)
        {
            WriteEvent(EventIds.MessageReadByEncoder, Size, EventSource);
        }

        public bool MessageWrittenByEncoderIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.MessageWrittenByEncoder, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.MessageEncoding,
            Keywords = Keywords.Channel,
            Message = "A message with size '{0}' bytes was written by the encoder.")]
        public void MessageWrittenByEncoder(int Size, string EventSource)
        {
            WriteEvent(EventIds.MessageWrittenByEncoder, Size, EventSource);
        }

        public bool SessionIdleTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.SessionIdleTimeout, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Timeout,
            Keywords = Keywords.ServiceModel,
            Message = "Session aborting for idle channel to uri:'{0}'.")]
        public void SessionIdleTimeout(string RemoteAddress)
        {
            WriteEvent(EventIds.SessionIdleTimeout, RemoteAddress);
        }

        public bool SocketAcceptEnqueuedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.TCP, EventChannel.Debug);
        }

        [Event(EventIds.SocketAcceptEnqueued, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ConnectionAccept,
            Keywords = Keywords.TCP,
            Message = "Connection accept started.")]
        public void SocketAcceptEnqueued()
        {
            WriteEvent(EventIds.SocketAcceptEnqueued);
        }

        public bool SocketAcceptedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.TCP, EventChannel.Debug);
        }

        [Event(EventIds.SocketAccepted, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ConnectionAccept,
            Keywords = Keywords.TCP,
            Message = "ListenerId:{0} accepted SocketId:{1}.")]
        public void SocketAccepted(int ListenerHashCode, int SocketHashCode)
        {
            WriteEvent(EventIds.SocketAccepted, ListenerHashCode, SocketHashCode);
        }

        public bool ConnectionPoolMissIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ConnectionPoolMiss, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ConnectionPooling,
            Keywords = Keywords.Channel,
            Message = "Pool for {0} has no available connection and {1} busy connections.")]
        public void ConnectionPoolMiss(string PoolKey, int busy)
        {
            WriteEvent(EventIds.ConnectionPoolMiss, PoolKey, busy);
        }

        public bool DispatchFormatterDeserializeRequestStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.DispatchFormatterDeserializeRequestStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.FormatterDeserializeRequest,
            Keywords = Keywords.ServiceModel,
            Message = "Dispatcher started deserialization the request message.")]
        public void DispatchFormatterDeserializeRequestStart()
        {
            WriteEvent(EventIds.DispatchFormatterDeserializeRequestStart);
        }

        public bool DispatchFormatterDeserializeRequestStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.DispatchFormatterDeserializeRequestStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.FormatterDeserializeRequest,
            Keywords = Keywords.ServiceModel,
            Message = "Dispatcher completed deserialization the request message.")]
        public void DispatchFormatterDeserializeRequestStop()
        {
            WriteEvent(EventIds.DispatchFormatterDeserializeRequestStop);
        }

        public bool DispatchFormatterSerializeReplyStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.DispatchFormatterSerializeReplyStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.FormatterSerializeReply,
            Keywords = Keywords.ServiceModel,
            Message = "Dispatcher started serialization of the reply message.")]
        public void DispatchFormatterSerializeReplyStart()
        {
            WriteEvent(EventIds.DispatchFormatterSerializeReplyStart);
        }

        public bool DispatchFormatterSerializeReplyStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.DispatchFormatterSerializeReplyStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.FormatterSerializeReply,
            Keywords = Keywords.ServiceModel,
            Message = "Dispatcher completed serialization of the reply message.")]
        public void DispatchFormatterSerializeReplyStop()
        {
            WriteEvent(EventIds.DispatchFormatterSerializeReplyStop);
        }

        public bool ClientFormatterSerializeRequestStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ClientFormatterSerializeRequestStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.FormatterSerializeRequest,
            Keywords = Keywords.ServiceModel,
            Message = "Client request serialization started.")]
        public void ClientFormatterSerializeRequestStart()
        {
            WriteEvent(EventIds.ClientFormatterSerializeRequestStart);
        }

        public bool ClientFormatterSerializeRequestStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ClientFormatterSerializeRequestStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.FormatterSerializeRequest,
            Keywords = Keywords.ServiceModel,
            Message = "Client completed serialization of the request message.")]
        public void ClientFormatterSerializeRequestStop()
        {
            WriteEvent(EventIds.ClientFormatterSerializeRequestStop);
        }

        public bool ClientFormatterDeserializeReplyStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ClientFormatterDeserializeReplyStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.FormatterDeserializeReply,
            Keywords = Keywords.ServiceModel,
            Message = "Client started deserializing the reply message.")]
        public void ClientFormatterDeserializeReplyStart()
        {
            WriteEvent(EventIds.ClientFormatterDeserializeReplyStart);
        }

        public bool ClientFormatterDeserializeReplyStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ClientFormatterDeserializeReplyStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.FormatterDeserializeReply,
            Keywords = Keywords.ServiceModel,
            Message = "Client completed deserializing the reply message.")]
        public void ClientFormatterDeserializeReplyStop()
        {
            WriteEvent(EventIds.ClientFormatterDeserializeReplyStop);
        }

        public bool SecurityNegotiationStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.SecurityNegotiationStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.SecurityNegotiation,
            Keywords = Keywords.Security,
            Message = "Security negotiation started.")]
        public void SecurityNegotiationStart()
        {
            WriteEvent(EventIds.SecurityNegotiationStart);
        }

        public bool SecurityNegotiationStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.SecurityNegotiationStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.SecurityNegotiation,
            Keywords = Keywords.Security,
            Message = "Security negotiation completed.")]
        public void SecurityNegotiationStop()
        {
            WriteEvent(EventIds.SecurityNegotiationStop);
        }

        public bool SecurityTokenProviderOpenedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.SecurityTokenProviderOpened, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.SecureMessage,
            Keywords = Keywords.Security,
            Message = "SecurityTokenProvider opening completed.")]
        public void SecurityTokenProviderOpened()
        {
            WriteEvent(EventIds.SecurityTokenProviderOpened);
        }

        public bool OutgoingMessageSecuredIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.OutgoingMessageSecured, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.SecureMessage,
            Keywords = Keywords.Security,
            Message = "Outgoing message has been secured.")]
        public void OutgoingMessageSecured()
        {
            WriteEvent(EventIds.OutgoingMessageSecured);
        }

        public bool IncomingMessageVerifiedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security | Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.IncomingMessageVerified, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SecurityVerification,
            Keywords = Keywords.Security | Keywords.ServiceModel,
            Message = "Incoming message has been verified.")]
        public void IncomingMessageVerified()
        {
            WriteEvent(EventIds.IncomingMessageVerified);
        }

        public bool GetServiceInstanceStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.GetServiceInstanceStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ServiceInstance,
            Keywords = Keywords.ServiceModel,
            Message = "Service instance retrieval started.")]
        public void GetServiceInstanceStart()
        {
            WriteEvent(EventIds.GetServiceInstanceStart);
        }

        public bool GetServiceInstanceStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.GetServiceInstanceStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ServiceInstance,
            Keywords = Keywords.ServiceModel,
            Message = "Service instance retrieved.")]
        public void GetServiceInstanceStop()
        {
            WriteEvent(EventIds.GetServiceInstanceStop);
        }

        public bool ChannelReceiveStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ChannelReceiveStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ChannelReceive,
            Keywords = Keywords.Channel,
            Message = "ChannelHandlerId:{0} - Message receive loop started.")]
        public void ChannelReceiveStart(int ChannelId)
        {
            WriteEvent(EventIds.ChannelReceiveStart, ChannelId);
        }

        public bool ChannelReceiveStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ChannelReceiveStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ChannelReceive,
            Keywords = Keywords.Channel,
            Message = "ChannelHandlerId:{0} - Message receive loop stopped.")]
        public void ChannelReceiveStop(int ChannelId)
        {
            WriteEvent(EventIds.ChannelReceiveStop, ChannelId);
        }

        public bool ChannelFactoryCreatedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ChannelFactoryCreated, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ChannelFactoryCreate,
            Keywords = Keywords.ServiceModel,
            Message = "ChannelFactory created .")]
        public void ChannelFactoryCreated(string EventSource)
        {
            WriteEvent(EventIds.ChannelFactoryCreated, EventSource);
        }

        public bool PipeConnectionAcceptStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.PipeConnectionAcceptStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ConnectionAccept,
            Keywords = Keywords.Channel,
            Message = "Pipe connection accept started on {0} .")]
        public void PipeConnectionAcceptStart(string uri)
        {
            WriteEvent(EventIds.PipeConnectionAcceptStart, uri);
        }

        public bool PipeConnectionAcceptStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.PipeConnectionAcceptStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ConnectionAccept,
            Keywords = Keywords.Channel,
            Message = "Pipe connection accepted.")]
        public void PipeConnectionAcceptStop()
        {
            WriteEvent(EventIds.PipeConnectionAcceptStop);
        }

        public bool EstablishConnectionStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.EstablishConnectionStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.Connect,
            Keywords = Keywords.Channel,
            Message = "Connection establishment started for {0}.")]
        public void EstablishConnectionStart(string Key)
        {
            WriteEvent(EventIds.EstablishConnectionStart, Key);
        }

        public bool EstablishConnectionStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.EstablishConnectionStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.Connect,
            Keywords = Keywords.Channel,
            Message = "Connection established.")]
        public void EstablishConnectionStop()
        {
            WriteEvent(EventIds.EstablishConnectionStop);
        }

        public bool SessionPreambleUnderstoodIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.SessionPreambleUnderstood, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.Channel,
            Message = "Session preamble for '{0}' understood.")]
        public void SessionPreambleUnderstood(string Via)
        {
            WriteEvent(EventIds.SessionPreambleUnderstood, Via);
        }

        public bool ConnectionReaderSendFaultIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ConnectionReaderSendFault, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.Channel,
            Message = "Connection reader sending fault '{0}'. ")]
        public void ConnectionReaderSendFault(string FaultString)
        {
            WriteEvent(EventIds.ConnectionReaderSendFault, FaultString);
        }

        public bool SocketAcceptClosedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.TCP, EventChannel.Debug);
        }

        [Event(EventIds.SocketAcceptClosed, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ConnectionAccept,
            Keywords = Keywords.TCP,
            Message = "Socket accept closed.")]
        public void SocketAcceptClosed()
        {
            WriteEvent(EventIds.SocketAcceptClosed);
        }

        public bool ServiceHostFaultedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Critical, Keywords.TCP, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceHostFaulted, Level = EventLevel.Critical, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.ServiceHostFault,
            Keywords = Keywords.TCP,
            Message = "Service host faulted.")]
        public void ServiceHostFaulted(string EventSource)
        {
            WriteEvent(EventIds.ServiceHostFaulted, EventSource);
        }

        public bool ListenerOpenStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ListenerOpenStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ListenerOpen,
            Keywords = Keywords.Channel,
            Message = "Listener opening for '{0}'.")]
        public void ListenerOpenStart(string Uri)
        {
            WriteEvent(EventIds.ListenerOpenStart, Uri);
        }

        public bool ListenerOpenStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ListenerOpenStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ListenerOpen,
            Keywords = Keywords.Channel,
            Message = "Listener open completed.")]
        public void ListenerOpenStop()
        {
            WriteEvent(EventIds.ListenerOpenStop);
        }

        public bool ServerMaxPooledConnectionsQuotaReachedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.ServerMaxPooledConnectionsQuotaReached, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Server max pooled connections quota reached.")]
        public void ServerMaxPooledConnectionsQuotaReached()
        {
            WriteEvent(EventIds.ServerMaxPooledConnectionsQuotaReached);
        }

        public bool TcpConnectionTimedOutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.TCP, EventChannel.Analytic);
        }

        [Event(EventIds.TcpConnectionTimedOut, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.TCP,
            Message = "SocketId:{0} to remote address {1} timed out.")]
        public void TcpConnectionTimedOut(int SocketId, string Uri)
        {
            WriteEvent(EventIds.TcpConnectionTimedOut, SocketId, Uri);
        }

        public bool TcpConnectionResetErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.TCP, EventChannel.Analytic);
        }

        [Event(EventIds.TcpConnectionResetError, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.TCP,
            Message = "SocketId:{0} to remote address {1} had a connection reset error.")]
        public void TcpConnectionResetError(int SocketId, string Uri)
        {
            WriteEvent(EventIds.TcpConnectionResetError, SocketId, Uri);
        }

        public bool ServiceSecurityNegotiationCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.ServiceSecurityNegotiationCompleted, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SecurityNegotiation,
            Keywords = Keywords.Security,
            Message = "Service security negotiation completed.")]
        public void ServiceSecurityNegotiationCompleted()
        {
            WriteEvent(EventIds.ServiceSecurityNegotiationCompleted);
        }

        public bool SecurityNegotiationProcessingFailureIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Security, EventChannel.Analytic);
        }

        [Event(EventIds.SecurityNegotiationProcessingFailure, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.SecurityNegotiation,
            Keywords = Keywords.Security,
            Message = "Security negotiation processing failed.")]
        public void SecurityNegotiationProcessingFailure()
        {
            WriteEvent(EventIds.SecurityNegotiationProcessingFailure);
        }

        public bool SecurityIdentityVerificationSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.SecurityIdentityVerificationSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SecurityVerification,
            Keywords = Keywords.Security,
            Message = "Security verification succeeded.")]
        public void SecurityIdentityVerificationSuccess()
        {
            WriteEvent(EventIds.SecurityIdentityVerificationSuccess);
        }

        public bool SecurityIdentityVerificationFailureIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Security, EventChannel.Analytic);
        }

        [Event(EventIds.SecurityIdentityVerificationFailure, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.SecurityVerification,
            Keywords = Keywords.Security,
            Message = "Security verification failed.")]
        public void SecurityIdentityVerificationFailure()
        {
            WriteEvent(EventIds.SecurityIdentityVerificationFailure);
        }

        public bool PortSharingDuplicatedSocketIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Debug);
        }

        [Event(EventIds.PortSharingDuplicatedSocket, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ActivationDuplicateSocket,
            Keywords = Keywords.ActivationServices,
            Message = "Socket duplicated for {0}.")]
        public void PortSharingDuplicatedSocket(string Uri)
        {
            WriteEvent(EventIds.PortSharingDuplicatedSocket, Uri);
        }

        public bool SecurityImpersonationSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.SecurityImpersonationSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SecurityImpersonation,
            Keywords = Keywords.Security,
            Message = "Security impersonation succeeded.")]
        public void SecurityImpersonationSuccess()
        {
            WriteEvent(EventIds.SecurityImpersonationSuccess);
        }

        public bool SecurityImpersonationFailureIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Security, EventChannel.Analytic);
        }

        [Event(EventIds.SecurityImpersonationFailure, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.SecurityImpersonation,
            Keywords = Keywords.Security,
            Message = "Security impersonation failed.")]
        public void SecurityImpersonationFailure()
        {
            WriteEvent(EventIds.SecurityImpersonationFailure);
        }

        public bool HttpChannelRequestAbortedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.HttpChannelRequestAborted, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "Http channel request aborted.")]
        public void HttpChannelRequestAborted()
        {
            WriteEvent(EventIds.HttpChannelRequestAborted);
        }

        public bool HttpChannelResponseAbortedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.HttpChannelResponseAborted, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TransportSend,
            Keywords = Keywords.HTTP,
            Message = "Http channel response aborted.")]
        public void HttpChannelResponseAborted()
        {
            WriteEvent(EventIds.HttpChannelResponseAborted);
        }

        public bool HttpAuthFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.HttpAuthFailed, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.SecurityVerification,
            Keywords = Keywords.HTTP,
            Message = "Http authentication failed.")]
        public void HttpAuthFailed()
        {
            WriteEvent(EventIds.HttpAuthFailed);
        }

        public bool SharedListenerProxyRegisterStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.SharedListenerProxyRegisterStart, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.ActivationListenerOpen,
            Keywords = Keywords.ActivationServices,
            Message = "SharedListenerProxy registration started for uri '{0}'.")]
        public void SharedListenerProxyRegisterStart(string Uri)
        {
            WriteEvent(EventIds.SharedListenerProxyRegisterStart, Uri);
        }

        public bool SharedListenerProxyRegisterStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.SharedListenerProxyRegisterStop, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.ActivationListenerOpen,
            Keywords = Keywords.ActivationServices,
            Message = "SharedListenerProxy Register Stop.")]
        public void SharedListenerProxyRegisterStop()
        {
            WriteEvent(EventIds.SharedListenerProxyRegisterStop);
        }

        public bool SharedListenerProxyRegisterFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.SharedListenerProxyRegisterFailed, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.ActivationListenerOpen,
            Keywords = Keywords.ActivationServices,
            Message = "SharedListenerProxy register failed with status '{0}'.")]
        public void SharedListenerProxyRegisterFailed(string Status)
        {
            WriteEvent(EventIds.SharedListenerProxyRegisterFailed, Status);
        }

        public bool ConnectionPoolPreambleFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Channel, EventChannel.Analytic);
        }

        [Event(EventIds.ConnectionPoolPreambleFailed, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.SessionStart,
            Keywords = Keywords.Channel,
            Message = "ConnectionPoolPreambleFailed.")]
        public void ConnectionPoolPreambleFailed()
        {
            WriteEvent(EventIds.ConnectionPoolPreambleFailed);
        }

        public bool SslOnInitiateUpgradeIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Analytic);
        }

        [Event(EventIds.SslOnInitiateUpgrade, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.Initiate, Task = Tasks.SessionUpgrade,
            Keywords = Keywords.Security,
            Message = "SslOnAcceptUpgradeStart")]
        public void SslOnInitiateUpgrade()
        {
            WriteEvent(EventIds.SslOnInitiateUpgrade);
        }

        public bool SslOnAcceptUpgradeIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Analytic);
        }

        [Event(EventIds.SslOnAcceptUpgrade, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.Accept, Task = Tasks.SessionUpgrade,
            Keywords = Keywords.Security,
            Message = "SslOnAcceptUpgradeStop")]
        public void SslOnAcceptUpgrade()
        {
            WriteEvent(EventIds.SslOnAcceptUpgrade);
        }

        public bool BinaryMessageEncodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.BinaryMessageEncodingStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.MessageEncoding,
            Keywords = Keywords.Channel,
            Message = "BinaryMessageEncoder started encoding the message.")]
        public void BinaryMessageEncodingStart()
        {
            WriteEvent(EventIds.BinaryMessageEncodingStart);
        }

        public bool MtomMessageEncodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.MtomMessageEncodingStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.MessageEncoding,
            Keywords = Keywords.Channel,
            Message = "MtomMessageEncoder started encoding the message.")]
        public void MtomMessageEncodingStart()
        {
            WriteEvent(EventIds.MtomMessageEncodingStart);
        }

        public bool TextMessageEncodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.TextMessageEncodingStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.MessageEncoding,
            Keywords = Keywords.Channel,
            Message = "TextMessageEncoder started encoding the message.")]
        public void TextMessageEncodingStart()
        {
            WriteEvent(EventIds.TextMessageEncodingStart);
        }

        public bool BinaryMessageDecodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.BinaryMessageDecodingStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.MessageDecoding,
            Keywords = Keywords.Channel,
            Message = "BinaryMessageEncoder started decoding the message.")]
        public void BinaryMessageDecodingStart()
        {
            WriteEvent(EventIds.BinaryMessageDecodingStart);
        }

        public bool MtomMessageDecodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.MtomMessageDecodingStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.MessageDecoding,
            Keywords = Keywords.Channel,
            Message = "MtomMessageEncoder started decoding  the message.")]
        public void MtomMessageDecodingStart()
        {
            WriteEvent(EventIds.MtomMessageDecodingStart);
        }

        public bool TextMessageDecodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.TextMessageDecodingStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.MessageDecoding,
            Keywords = Keywords.Channel,
            Message = "TextMessageEncoder started decoding the message.")]
        public void TextMessageDecodingStart()
        {
            WriteEvent(EventIds.TextMessageDecodingStart);
        }

        public bool HttpResponseReceiveStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.HttpResponseReceiveStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "Http transport started receiving a message.")]
        public void HttpResponseReceiveStart()
        {
            WriteEvent(EventIds.HttpResponseReceiveStart);
        }

        public bool SocketReadStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.TCP, EventChannel.Debug);
        }

        [Event(EventIds.SocketReadStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.TransportReceive,
            Keywords = Keywords.TCP,
            Message = "SocketId:{0} read '{1}' bytes read from '{2}'.")]
        public void SocketReadStop(int SocketId, int Size, string Endpoint)
        {
            WriteEvent(EventIds.SocketReadStop, SocketId, Size, Endpoint);
        }

        public bool SocketAsyncReadStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.TCP, EventChannel.Debug);
        }

        [Event(EventIds.SocketAsyncReadStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.TransportReceive,
            Keywords = Keywords.TCP,
            Message = "SocketId:{0} read '{1}' bytes read from '{2}'.")]
        public void SocketAsyncReadStop(int SocketId, int Size, string Endpoint)
        {
            WriteEvent(EventIds.SocketAsyncReadStop, SocketId, Size, Endpoint);
        }

        public bool SocketWriteStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.TCP, EventChannel.Debug);
        }

        [Event(EventIds.SocketWriteStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportSend,
            Keywords = Keywords.TCP,
            Message = "SocketId:{0} writing '{1}' bytes to '{2}'.")]
        public void SocketWriteStart(int SocketId, int Size, string Endpoint)
        {
            WriteEvent(EventIds.SocketWriteStart, SocketId, Size, Endpoint);
        }

        public bool SocketAsyncWriteStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.TCP, EventChannel.Debug);
        }

        [Event(EventIds.SocketAsyncWriteStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportSend,
            Keywords = Keywords.TCP,
            Message = "SocketId:{0} writing '{1}' bytes to '{2}'.")]
        public void SocketAsyncWriteStart(int SocketId, int Size, string Endpoint)
        {
            WriteEvent(EventIds.SocketAsyncWriteStart, SocketId, Size, Endpoint);
        }

        public bool SequenceAcknowledgementSentIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.SequenceAcknowledgementSent, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.SequenceAck, Task = Tasks.ReliableSession,
            Keywords = Keywords.Channel,
            Message = "SessionId:{0} acknowledgement sent.")]
        public void SequenceAcknowledgementSent(string SessionId)
        {
            WriteEvent(EventIds.SequenceAcknowledgementSent, SessionId);
        }

        public bool ClientReliableSessionReconnectIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ClientReliableSessionReconnect, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Reconnect, Task = Tasks.ReliableSession,
            Keywords = Keywords.Channel,
            Message = "SessionId:{0} reconnecting.")]
        public void ClientReliableSessionReconnect(string SessionId)
        {
            WriteEvent(EventIds.ClientReliableSessionReconnect, SessionId);
        }

        public bool ReliableSessionChannelFaultedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ReliableSessionChannelFaulted, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Faulted, Task = Tasks.ReliableSession,
            Keywords = Keywords.Channel,
            Message = "SessionId:{0} faulted.")]
        public void ReliableSessionChannelFaulted(string SessionId)
        {
            WriteEvent(EventIds.ReliableSessionChannelFaulted, SessionId);
        }

        public bool WindowsStreamSecurityOnInitiateUpgradeIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Analytic);
        }

        [Event(EventIds.WindowsStreamSecurityOnInitiateUpgrade, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.Initiate, Task = Tasks.SessionUpgrade,
            Keywords = Keywords.Security,
            Message = "WindowsStreamSecurity initiating security upgrade.")]
        public void WindowsStreamSecurityOnInitiateUpgrade()
        {
            WriteEvent(EventIds.WindowsStreamSecurityOnInitiateUpgrade);
        }

        public bool WindowsStreamSecurityOnAcceptUpgradeIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Analytic);
        }

        [Event(EventIds.WindowsStreamSecurityOnAcceptUpgrade, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.Accept, Task = Tasks.SessionUpgrade,
            Keywords = Keywords.Security,
            Message = "Windows streaming security on accepting upgrade.")]
        public void WindowsStreamSecurityOnAcceptUpgrade()
        {
            WriteEvent(EventIds.WindowsStreamSecurityOnAcceptUpgrade);
        }

        public bool SocketConnectionAbortIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.TCP, EventChannel.Analytic);
        }

        [Event(EventIds.SocketConnectionAbort, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.ConnectionAbort,
            Keywords = Keywords.TCP,
            Message = "SocketId:{0} is aborting.")]
        public void SocketConnectionAbort(int SocketId)
        {
            WriteEvent(EventIds.SocketConnectionAbort, SocketId);
        }

        public bool HttpGetContextStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.HttpGetContextStart, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "HttpGetContext start.")]
        public void HttpGetContextStart()
        {
            WriteEvent(EventIds.HttpGetContextStart);
        }

        public bool ClientSendPreambleStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ClientSendPreambleStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ClientSendPreamble,
            Keywords = Keywords.Channel,
            Message = "Client sending preamble start.")]
        public void ClientSendPreambleStart()
        {
            WriteEvent(EventIds.ClientSendPreambleStart);
        }

        public bool ClientSendPreambleStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ClientSendPreambleStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ClientSendPreamble,
            Keywords = Keywords.Channel,
            Message = "Client sending preamble stop.")]
        public void ClientSendPreambleStop()
        {
            WriteEvent(EventIds.ClientSendPreambleStop);
        }

        public bool HttpMessageReceiveFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.HttpMessageReceiveFailed, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "Http Message receive failed.")]
        public void HttpMessageReceiveFailed()
        {
            WriteEvent(EventIds.HttpMessageReceiveFailed);
        }

        public bool TransactionScopeCreateIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.TransactionScopeCreate, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.TransactionScopeCreate, Task = Tasks.DispatchMessage,
            Keywords = Keywords.ServiceModel,
            Message = "TransactionScope is being created with LocalIdentifier:'{0}' and DistributedIdentifier:'{1}'.")]
        public void TransactionScopeCreate(string LocalId, Guid Distributed)
        {
            WriteEvent(EventIds.TransactionScopeCreate, LocalId, Distributed);
        }

        public bool StreamedMessageReadByEncoderIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.StreamedMessageReadByEncoder, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.MessageDecoding,
            Keywords = Keywords.Channel,
            Message = "A streamed message was read by the encoder.")]
        public void StreamedMessageReadByEncoder()
        {
            WriteEvent(EventIds.StreamedMessageReadByEncoder);
        }

        public bool StreamedMessageWrittenByEncoderIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.StreamedMessageWrittenByEncoder, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.MessageEncoding,
            Keywords = Keywords.Channel,
            Message = "A streamed message was written by the encoder.")]
        public void StreamedMessageWrittenByEncoder()
        {
            WriteEvent(EventIds.StreamedMessageWrittenByEncoder);
        }

        public bool MessageWrittenAsynchronouslyByEncoderIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.MessageWrittenAsynchronouslyByEncoder, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.MessageEncoding,
            Keywords = Keywords.Channel,
            Message = "A message was written asynchronously by the encoder.")]
        public void MessageWrittenAsynchronouslyByEncoder()
        {
            WriteEvent(EventIds.MessageWrittenAsynchronouslyByEncoder);
        }

        public bool BufferedAsyncWriteStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.BufferedAsyncWriteStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportSend,
            Keywords = Keywords.Channel,
            Message = "BufferId:{0} completed writing '{1}' bytes to underlying stream.")]
        public void BufferedAsyncWriteStart(int BufferId, int Size)
        {
            WriteEvent(EventIds.BufferedAsyncWriteStart, BufferId, Size);
        }

        public bool BufferedAsyncWriteStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.BufferedAsyncWriteStop, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.TransportSend,
            Keywords = Keywords.Channel,
            Message = "A message was written asynchronously by the encoder.")]
        public void BufferedAsyncWriteStop()
        {
            WriteEvent(EventIds.BufferedAsyncWriteStop);
        }

        public bool PipeSharedMemoryCreatedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.PipeSharedMemoryCreated, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ListenerOpen,
            Keywords = Keywords.Channel,
            Message = "Pipe shared memory created at '{0}' .")]
        public void PipeSharedMemoryCreated(string sharedMemoryName)
        {
            WriteEvent(EventIds.PipeSharedMemoryCreated, sharedMemoryName);
        }

        public bool NamedPipeCreatedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.NamedPipeCreated, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ListenerOpen,
            Keywords = Keywords.Channel,
            Message = "NamedPipe '{0}' created.")]
        public void NamedPipeCreated(string pipeName)
        {
            WriteEvent(EventIds.NamedPipeCreated, pipeName);
        }

        public bool SignatureVerificationStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.SignatureVerificationStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.SignatureVerification,
            Keywords = Keywords.Security,
            Message = "Signature verification started.")]
        public void SignatureVerificationStart()
        {
            WriteEvent(EventIds.SignatureVerificationStart);
        }

        public bool SignatureVerificationSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.SignatureVerificationSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SignatureVerification,
            Keywords = Keywords.Security,
            Message = "Signature verification succeeded")]
        public void SignatureVerificationSuccess()
        {
            WriteEvent(EventIds.SignatureVerificationSuccess);
        }

        public bool WrappedKeyDecryptionStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.WrappedKeyDecryptionStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.WrappedKeyDecryption,
            Keywords = Keywords.Security,
            Message = "Wrapped key decryption started.")]
        public void WrappedKeyDecryptionStart()
        {
            WriteEvent(EventIds.WrappedKeyDecryptionStart);
        }

        public bool WrappedKeyDecryptionSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.WrappedKeyDecryptionSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.WrappedKeyDecryption,
            Keywords = Keywords.Security,
            Message = "Wrapped key decryption succeeded.")]
        public void WrappedKeyDecryptionSuccess()
        {
            WriteEvent(EventIds.WrappedKeyDecryptionSuccess);
        }

        public bool EncryptedDataProcessingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.EncryptedDataProcessingStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.EncryptedDataProcessing,
            Keywords = Keywords.Security,
            Message = "Encrypted data processing started.")]
        public void EncryptedDataProcessingStart()
        {
            WriteEvent(EventIds.EncryptedDataProcessingStart);
        }

        public bool EncryptedDataProcessingSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.EncryptedDataProcessingSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.EncryptedDataProcessing,
            Keywords = Keywords.Security,
            Message = "Encrypted data processing succeeded.")]
        public void EncryptedDataProcessingSuccess()
        {
            WriteEvent(EventIds.EncryptedDataProcessingSuccess);
        }

        public bool HttpPipelineProcessInboundRequestStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.HttpPipelineProcessInboundRequestStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "Http message handler started processing the inbound request.")]
        public void HttpPipelineProcessInboundRequestStart()
        {
            WriteEvent(EventIds.HttpPipelineProcessInboundRequestStart);
        }

        public bool HttpPipelineBeginProcessInboundRequestStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.HttpPipelineBeginProcessInboundRequestStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "Http message handler started processing the inbound request asynchronously.")]
        public void HttpPipelineBeginProcessInboundRequestStart()
        {
            WriteEvent(EventIds.HttpPipelineBeginProcessInboundRequestStart);
        }

        public bool HttpPipelineProcessInboundRequestStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.HttpPipelineProcessInboundRequestStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "Http message handler completed processing an inbound request.")]
        public void HttpPipelineProcessInboundRequestStop()
        {
            WriteEvent(EventIds.HttpPipelineProcessInboundRequestStop);
        }

        public bool HttpPipelineFaultedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.HttpPipelineFaulted, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "Http message handler is faulted.")]
        public void HttpPipelineFaulted()
        {
            WriteEvent(EventIds.HttpPipelineFaulted);
        }

        public bool HttpPipelineTimeoutExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.HttpPipelineTimeoutException, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocket connection timed out.")]
        public void HttpPipelineTimeoutException()
        {
            WriteEvent(EventIds.HttpPipelineTimeoutException);
        }

        public bool HttpPipelineProcessResponseStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.HttpPipelineProcessResponseStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportSend,
            Keywords = Keywords.HTTP,
            Message = "Http message handler started processing the response.")]
        public void HttpPipelineProcessResponseStart()
        {
            WriteEvent(EventIds.HttpPipelineProcessResponseStart);
        }

        public bool HttpPipelineBeginProcessResponseStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.HttpPipelineBeginProcessResponseStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportSend,
            Keywords = Keywords.HTTP,
            Message = "Http message handler started processing the response asynchronously.")]
        public void HttpPipelineBeginProcessResponseStart()
        {
            WriteEvent(EventIds.HttpPipelineBeginProcessResponseStart);
        }

        public bool HttpPipelineProcessResponseStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.HttpPipelineProcessResponseStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.TransportSend,
            Keywords = Keywords.HTTP,
            Message = "Http message handler completed processing the response.")]
        public void HttpPipelineProcessResponseStop()
        {
            WriteEvent(EventIds.HttpPipelineProcessResponseStop);
        }

        public bool WebSocketConnectionRequestSendStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketConnectionRequestSendStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocket connection request to '{0}' send start.")]
        public void WebSocketConnectionRequestSendStart(string remoteAddress)
        {
            WriteEvent(EventIds.WebSocketConnectionRequestSendStart, remoteAddress);
        }

        public bool WebSocketConnectionRequestSendStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketConnectionRequestSendStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} connection request sent.")]
        public void WebSocketConnectionRequestSendStop(int websocketId)
        {
            WriteEvent(EventIds.WebSocketConnectionRequestSendStop, websocketId);
        }

        public bool WebSocketConnectionAcceptStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketConnectionAcceptStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocket connection accept start.")]
        public void WebSocketConnectionAcceptStart()
        {
            WriteEvent(EventIds.WebSocketConnectionAcceptStart);
        }

        public bool WebSocketConnectionAcceptedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketConnectionAccepted, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} connection accepted.")]
        public void WebSocketConnectionAccepted(int websocketId)
        {
            WriteEvent(EventIds.WebSocketConnectionAccepted, websocketId);
        }

        public bool WebSocketConnectionDeclinedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.WebSocketConnectionDeclined, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocket connection declined with status code '{0}'")]
        public void WebSocketConnectionDeclined(string errorMessage)
        {
            WriteEvent(EventIds.WebSocketConnectionDeclined, errorMessage);
        }

        public bool WebSocketConnectionFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.WebSocketConnectionFailed, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocket connection request failed: '{0}'")]
        public void WebSocketConnectionFailed(string errorMessage)
        {
            WriteEvent(EventIds.WebSocketConnectionFailed, errorMessage);
        }

        public bool WebSocketConnectionAbortedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.WebSocketConnectionAborted, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} connection is aborted.")]
        public void WebSocketConnectionAborted(int websocketId)
        {
            WriteEvent(EventIds.WebSocketConnectionAborted, websocketId);
        }

        public bool WebSocketAsyncWriteStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketAsyncWriteStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportSend,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} writing '{1}' bytes to '{2}'.")]
        public void WebSocketAsyncWriteStart(int websocketId, int byteCount, string remoteAddress)
        {
            WriteEvent(EventIds.WebSocketAsyncWriteStart, websocketId, byteCount, remoteAddress);
        }

        public bool WebSocketAsyncWriteStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketAsyncWriteStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.TransportSend,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} asynchronous write stop.")]
        public void WebSocketAsyncWriteStop(int websocketId)
        {
            WriteEvent(EventIds.WebSocketAsyncWriteStop, websocketId);
        }

        public bool WebSocketAsyncReadStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketAsyncReadStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} read start.")]
        public void WebSocketAsyncReadStart(int websocketId)
        {
            WriteEvent(EventIds.WebSocketAsyncReadStart, websocketId);
        }

        public bool WebSocketAsyncReadStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketAsyncReadStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} read '{1}' bytes from '{2}'.")]
        public void WebSocketAsyncReadStop(int websocketId, int byteCount, string remoteAddress)
        {
            WriteEvent(EventIds.WebSocketAsyncReadStop, websocketId, byteCount, remoteAddress);
        }

        public bool WebSocketCloseSentIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketCloseSent, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} sending close message to '{1}' with close status '{2}'.")]
        public void WebSocketCloseSent(int websocketId, string remoteAddress, string closeStatus)
        {
            WriteEvent(EventIds.WebSocketCloseSent, websocketId, remoteAddress, closeStatus);
        }

        public bool WebSocketCloseOutputSentIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketCloseOutputSent, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} sending close output message to '{1}' with close status '{2}'.")]
        public void WebSocketCloseOutputSent(int websocketId, string remoteAddress, string closeStatus)
        {
            WriteEvent(EventIds.WebSocketCloseOutputSent, websocketId, remoteAddress, closeStatus);
        }

        public bool WebSocketConnectionClosedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketConnectionClosed, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} connection closed.")]
        public void WebSocketConnectionClosed(int websocketId)
        {
            WriteEvent(EventIds.WebSocketConnectionClosed, websocketId);
        }

        public bool WebSocketCloseStatusReceivedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketCloseStatusReceived, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} connection close message received with status '{1}'.")]
        public void WebSocketCloseStatusReceived(int websocketId, string closeStatus)
        {
            WriteEvent(EventIds.WebSocketCloseStatusReceived, websocketId, closeStatus);
        }

        public bool WebSocketUseVersionFromClientWebSocketFactoryIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketUseVersionFromClientWebSocketFactory, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "Using the WebSocketVersion from a client WebSocket factory of type '{0}'.")]
        public void WebSocketUseVersionFromClientWebSocketFactory(string clientWebSocketFactoryType)
        {
            WriteEvent(EventIds.WebSocketUseVersionFromClientWebSocketFactory, clientWebSocketFactoryType);
        }

        public bool WebSocketCreateClientWebSocketWithFactoryIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketCreateClientWebSocketWithFactory, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "Creating the client WebSocket with a factory of type '{0}'.")]
        public void WebSocketCreateClientWebSocketWithFactory(string clientWebSocketFactoryType)
        {
            WriteEvent(EventIds.WebSocketCreateClientWebSocketWithFactory, clientWebSocketFactoryType);
        }

        public bool InferredContractDescriptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.InferredContractDescription, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.Contract, Task = Tasks.InferDescription,
            Keywords = Keywords.WFServices,
            Message = "ContractDescription with Name='{0}' and Namespace='{1}' has been inferred from WorkflowService.")]
        public void InferredContractDescription(string data1, string data2)
        {
            WriteEvent(EventIds.InferredContractDescription, data1, data2);
        }

        public bool InferredOperationDescriptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.InferredOperationDescription, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.Operation, Task = Tasks.InferDescription,
            Keywords = Keywords.WFServices,
            Message = "OperationDescription with Name='{0}' in contract '{1}' has been inferred from WorkflowService. IsOneWay={2}.")]
        public void InferredOperationDescription(string data1, string data2, string data3)
        {
            WriteEvent(EventIds.InferredOperationDescription, data1, data2, data3);
        }

        public bool DuplicateCorrelationQueryIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.DuplicateCorrelationQuery, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = Opcodes.DuplicateQuery, Task = Tasks.Correlation,
            Keywords = Keywords.WFServices,
            Message = "A duplicate CorrelationQuery was found with Where='{0}'. This duplicate query will not be used when calculating correlation.")]
        public void DuplicateCorrelationQuery(string data1)
        {
            WriteEvent(EventIds.DuplicateCorrelationQuery, data1);
        }

        public bool ServiceEndpointAddedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceEndpointAdded, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.AddServiceEndpoint,
            Keywords = Keywords.WFServices,
            Message = "A service endpoint has been added for address '{0}', binding '{1}', and contract '{2}'.")]
        public void ServiceEndpointAdded(string data1, string data2, string data3)
        {
            WriteEvent(EventIds.ServiceEndpointAdded, data1, data2, data3);
        }

        public bool TrackingProfileNotFoundIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.TrackingProfileNotFound, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.NotFound, Task = Tasks.TrackingProfile,
            Keywords = Keywords.WFServices,
            Message = "TrackingProfile '{0}' for the ActivityDefinitionId '{1}' not found. Either the TrackingProfile is not found in the config file or the ActivityDefinitionId does not match.")]
        public void TrackingProfileNotFound(string TrackingProfile, string ActivityDefinitionId)
        {
            WriteEvent(EventIds.TrackingProfileNotFound, TrackingProfile, ActivityDefinitionId);
        }

        public bool BufferOutOfOrderMessageNoInstanceIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.BufferOutOfOrderMessageNoInstance, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.NoInstance, Task = Tasks.BufferOutOfOrder,
            Keywords = Keywords.WFServices,
            Message = "Operation '{0}' cannot be performed at this time. Another attempt will be made when the service instance is ready to process this particular operation.")]
        public void BufferOutOfOrderMessageNoInstance(string data1)
        {
            WriteEvent(EventIds.BufferOutOfOrderMessageNoInstance, data1);
        }

        public bool BufferOutOfOrderMessageNoBookmarkIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.BufferOutOfOrderMessageNoBookmark, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.NoBookmark, Task = Tasks.BufferOutOfOrder,
            Keywords = Keywords.WFServices,
            Message = "Operation '{1}' on service instance '{0}' cannot be performed at this time. Another attempt will be made when the service instance is ready to process this particular operation.")]
        public void BufferOutOfOrderMessageNoBookmark(string data1, string data2)
        {
            WriteEvent(EventIds.BufferOutOfOrderMessageNoBookmark, data1, data2);
        }

        public bool MaxPendingMessagesPerChannelExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Quota | Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.MaxPendingMessagesPerChannelExceeded, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota | Keywords.WFServices,
            Message = "The throttle 'MaxPendingMessagesPerChannel' limit of  '{0}' was hit. To increase this limit, adjust the MaxPendingMessagesPerChannel property on BufferedReceiveServiceBehavior.")]
        public void MaxPendingMessagesPerChannelExceeded(int limit)
        {
            WriteEvent(EventIds.MaxPendingMessagesPerChannelExceeded, limit);
        }

        public bool XamlServicesLoadStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.XamlServicesLoadStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.XamlServicesLoad,
            Keywords = Keywords.WebHost,
            Message = "XamlServicesLoad start")]
        public void XamlServicesLoadStart()
        {
            WriteEvent(EventIds.XamlServicesLoadStart);
        }

        public bool XamlServicesLoadStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.XamlServicesLoadStop, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.XamlServicesLoad,
            Keywords = Keywords.WebHost,
            Message = "XamlServicesLoad Stop")]
        public void XamlServicesLoadStop()
        {
            WriteEvent(EventIds.XamlServicesLoadStop);
        }

        public bool CreateWorkflowServiceHostStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.CreateWorkflowServiceHostStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.CreateWorkflowServiceHost,
            Keywords = Keywords.WebHost,
            Message = "CreateWorkflowServiceHost start")]
        public void CreateWorkflowServiceHostStart()
        {
            WriteEvent(EventIds.CreateWorkflowServiceHostStart);
        }

        public bool CreateWorkflowServiceHostStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.CreateWorkflowServiceHostStop, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.CreateWorkflowServiceHost,
            Keywords = Keywords.WebHost,
            Message = "CreateWorkflowServiceHost Stop")]
        public void CreateWorkflowServiceHostStop()
        {
            WriteEvent(EventIds.CreateWorkflowServiceHostStop);
        }

        public bool TransactedReceiveScopeEndCommitFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.TransactedReceiveScopeEndCommitFailed, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.RuntimeTransaction,
            Keywords = Keywords.WFServices,
            Message = "The call to EndCommit on the CommittableTransaction with id = '{0}' threw a TransactionException with the following message: '{1}'.")]
        public void TransactedReceiveScopeEndCommitFailed(string data1, string data2)
        {
            WriteEvent(EventIds.TransactedReceiveScopeEndCommitFailed, data1, data2);
        }

        public bool ServiceActivationStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceActivationStart, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.ServiceActivation,
            Keywords = Keywords.WebHost,
            Message = "Service activation start")]
        public void ServiceActivationStart()
        {
            WriteEvent(EventIds.ServiceActivationStart);
        }

        public bool ServiceActivationStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceActivationStop, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.ServiceActivation,
            Keywords = Keywords.WebHost,
            Message = "Service activation Stop")]
        public void ServiceActivationStop()
        {
            WriteEvent(EventIds.ServiceActivationStop);
        }

        public bool ServiceActivationAvailableMemoryIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceActivationAvailableMemory, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Available memory (bytes): {0}")]
        public void ServiceActivationAvailableMemory(ulong availableMemoryBytes)
        {
            WriteEvent(EventIds.ServiceActivationAvailableMemory, availableMemoryBytes);
        }

        public bool ServiceActivationExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WebHost, EventChannel.Operational);
        }

        [Event(EventIds.ServiceActivationException, Level = EventLevel.Error, Channel = EventChannel.Operational, Opcode = EventOpcode.Info,
            Keywords = Keywords.WebHost,
            Message = "The service could not be activated. Exception details: {0}")]
        public void ServiceActivationException(string data1, string SerializedException)
        {
            WriteEvent(EventIds.ServiceActivationException, data1, SerializedException);
        }

        public bool RoutingServiceClosingClientIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceClosingClient, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Closing, Task = Tasks.RoutingServiceClient,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is closing client '{0}'.")]
        public void RoutingServiceClosingClient(string data1)
        {
            WriteEvent(EventIds.RoutingServiceClosingClient, data1);
        }

        public bool RoutingServiceChannelFaultedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceChannelFaulted, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.ChannelFaulted, Task = Tasks.RoutingServiceClient,
            Keywords = Keywords.RoutingServices,
            Message = "Routing Service client '{0}' has faulted.")]
        public void RoutingServiceChannelFaulted(string data1)
        {
            WriteEvent(EventIds.RoutingServiceChannelFaulted, data1);
        }

        public bool RoutingServiceCompletingOneWayIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceCompletingOneWay, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.CompletingOneWay, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "A Routing Service one way message is completing.")]
        public void RoutingServiceCompletingOneWay(string SerializedException)
        {
            WriteEvent(EventIds.RoutingServiceCompletingOneWay, SerializedException);
        }

        public bool RoutingServiceProcessingFailureIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceProcessingFailure, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = Opcodes.ProcessingFailure, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service failed while processing a message on the endpoint with address '{0}'.")]
        public void RoutingServiceProcessingFailure(string data1, string SerializedException)
        {
            WriteEvent(EventIds.RoutingServiceProcessingFailure, data1, SerializedException);
        }

        public bool RoutingServiceCreatingClientForEndpointIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceCreatingClientForEndpoint, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.CreatingForEndpoint, Task = Tasks.RoutingServiceClient,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is creating a client for endpoint: '{0}'.")]
        public void RoutingServiceCreatingClientForEndpoint(string data1)
        {
            WriteEvent(EventIds.RoutingServiceCreatingClientForEndpoint, data1);
        }

        public bool RoutingServiceDisplayConfigIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceDisplayConfig, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is configured with RouteOnHeadersOnly: {0}, SoapProcessingEnabled: {1}, EnsureOrderedDispatch: {2}.")]
        public void RoutingServiceDisplayConfig(string data1, string data2, string data3)
        {
            WriteEvent(EventIds.RoutingServiceDisplayConfig, data1, data2, data3);
        }

        public bool RoutingServiceCompletingTwoWayIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceCompletingTwoWay, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.CompletingTwoWay, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "A Routing Service request reply message is completing.")]
        public void RoutingServiceCompletingTwoWay()
        {
            WriteEvent(EventIds.RoutingServiceCompletingTwoWay);
        }

        public bool RoutingServiceMessageRoutedToEndpointsIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceMessageRoutedToEndpoints, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.RoutedToEndpoints, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service routed message with ID: '{0}' to {1} endpoint lists.")]
        public void RoutingServiceMessageRoutedToEndpoints(string data1, string data2)
        {
            WriteEvent(EventIds.RoutingServiceMessageRoutedToEndpoints, data1, data2);
        }

        public bool RoutingServiceConfigurationAppliedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceConfigurationApplied, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ConfigurationApplied, Task = Tasks.RoutingService,
            Keywords = Keywords.RoutingServices,
            Message = "A new RoutingConfiguration has been applied to the Routing Service.")]
        public void RoutingServiceConfigurationApplied()
        {
            WriteEvent(EventIds.RoutingServiceConfigurationApplied);
        }

        public bool RoutingServiceProcessingMessageIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceProcessingMessage, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ProcessingMessage, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is processing a message with ID: '{0}', Action: '{1}', Inbound URL: '{2}' Received in Transaction: {3}.")]
        public void RoutingServiceProcessingMessage(string data1, string data2, string data3, string data4)
        {
            WriteEvent(EventIds.RoutingServiceProcessingMessage, data1, data2, data3, data4);
        }

        public bool RoutingServiceTransmittingMessageIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceTransmittingMessage, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.TransmittingMessage, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is transmitting the message with ID: '{0}' [operation {1}] to '{2}'.")]
        public void RoutingServiceTransmittingMessage(string data1, string data2, string data3)
        {
            WriteEvent(EventIds.RoutingServiceTransmittingMessage, data1, data2, data3);
        }

        public bool RoutingServiceCommittingTransactionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceCommittingTransaction, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.CommittingTransaction, Task = Tasks.RoutingServiceTransaction,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is committing a transaction with id: '{0}'.")]
        public void RoutingServiceCommittingTransaction(string data1)
        {
            WriteEvent(EventIds.RoutingServiceCommittingTransaction, data1);
        }

        public bool RoutingServiceDuplexCallbackExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceDuplexCallbackException, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = Opcodes.DuplexCallbackException, Task = Tasks.RoutingService,
            Keywords = Keywords.RoutingServices,
            Message = "Routing Service component {0} encountered a duplex callback exception.")]
        public void RoutingServiceDuplexCallbackException(string data1, string SerializedException)
        {
            WriteEvent(EventIds.RoutingServiceDuplexCallbackException, data1, SerializedException);
        }

        public bool RoutingServiceMovedToBackupIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceMovedToBackup, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.MovedToBackup, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "Routing Service message with ID: '{0}' [operation {1}] moved to backup endpoint '{2}'.")]
        public void RoutingServiceMovedToBackup(string data1, string data2, string data3)
        {
            WriteEvent(EventIds.RoutingServiceMovedToBackup, data1, data2, data3);
        }

        public bool RoutingServiceCreatingTransactionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceCreatingTransaction, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Creating, Task = Tasks.RoutingServiceTransaction,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service created a new Transaction with id '{0}' for processing message(s).")]
        public void RoutingServiceCreatingTransaction(string data1)
        {
            WriteEvent(EventIds.RoutingServiceCreatingTransaction, data1);
        }

        public bool RoutingServiceCloseFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceCloseFailed, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.CloseFailed, Task = Tasks.RoutingService,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service failed while closing outbound client '{0}'.")]
        public void RoutingServiceCloseFailed(string data1, string SerializedException)
        {
            WriteEvent(EventIds.RoutingServiceCloseFailed, data1, SerializedException);
        }

        public bool RoutingServiceSendingResponseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceSendingResponse, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.SendingResponse, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is sending back a response message with Action '{0}'.")]
        public void RoutingServiceSendingResponse(string data1)
        {
            WriteEvent(EventIds.RoutingServiceSendingResponse, data1);
        }

        public bool RoutingServiceSendingFaultResponseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceSendingFaultResponse, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.SendingFaultResponse, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is sending back a Fault response message with Action '{0}'.")]
        public void RoutingServiceSendingFaultResponse(string data1)
        {
            WriteEvent(EventIds.RoutingServiceSendingFaultResponse, data1);
        }

        public bool RoutingServiceCompletingReceiveContextIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceCompletingReceiveContext, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.Completing, Task = Tasks.RoutingServiceReceiveContext,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is calling ReceiveContext.Complete for Message with ID: '{0}'.")]
        public void RoutingServiceCompletingReceiveContext(string data1)
        {
            WriteEvent(EventIds.RoutingServiceCompletingReceiveContext, data1);
        }

        public bool RoutingServiceAbandoningReceiveContextIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceAbandoningReceiveContext, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.Abandoning, Task = Tasks.RoutingServiceReceiveContext,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is calling ReceiveContext.Abandon for Message with ID: '{0}'.")]
        public void RoutingServiceAbandoningReceiveContext(string data1)
        {
            WriteEvent(EventIds.RoutingServiceAbandoningReceiveContext, data1);
        }

        public bool RoutingServiceUsingExistingTransactionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceUsingExistingTransaction, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.UsingExisting, Task = Tasks.RoutingServiceTransaction,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service will send messages using existing transaction '{0}'.")]
        public void RoutingServiceUsingExistingTransaction(string data1)
        {
            WriteEvent(EventIds.RoutingServiceUsingExistingTransaction, data1);
        }

        public bool RoutingServiceTransmitFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceTransmitFailed, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.TransmitFailed, Task = Tasks.RoutingService,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service failed while sending to '{0}'.")]
        public void RoutingServiceTransmitFailed(string data1, string SerializedException)
        {
            WriteEvent(EventIds.RoutingServiceTransmitFailed, data1, SerializedException);
        }

        public bool RoutingServiceFilterTableMatchStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Analytic);
        }

        [Event(EventIds.RoutingServiceFilterTableMatchStart, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.RoutingServiceFilterTableMatch,
            Keywords = Keywords.RoutingServices,
            Message = "Routing Service MessageFilterTable Match Start.")]
        public void RoutingServiceFilterTableMatchStart()
        {
            WriteEvent(EventIds.RoutingServiceFilterTableMatchStart);
        }

        public bool RoutingServiceFilterTableMatchStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Analytic);
        }

        [Event(EventIds.RoutingServiceFilterTableMatchStop, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.RoutingServiceFilterTableMatch,
            Keywords = Keywords.RoutingServices,
            Message = "Routing Service MessageFilterTable Match Stop.")]
        public void RoutingServiceFilterTableMatchStop()
        {
            WriteEvent(EventIds.RoutingServiceFilterTableMatchStop);
        }

        public bool RoutingServiceAbortingChannelIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceAbortingChannel, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.AbortingChannel, Task = Tasks.RoutingService,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is calling abort on channel: '{0}'.")]
        public void RoutingServiceAbortingChannel(string data1)
        {
            WriteEvent(EventIds.RoutingServiceAbortingChannel, data1);
        }

        public bool RoutingServiceHandledExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceHandledException, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.HandledException, Task = Tasks.RoutingService,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service has handled an exception.")]
        public void RoutingServiceHandledException(string SerializedException)
        {
            WriteEvent(EventIds.RoutingServiceHandledException, SerializedException);
        }

        public bool RoutingServiceTransmitSucceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceTransmitSucceeded, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.TransmitSucceeded, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service successfully transmitted Message with ID: '{0} [operation {1}] to '{2}'.")]
        public void RoutingServiceTransmitSucceeded(string data1, string data2, string data3)
        {
            WriteEvent(EventIds.RoutingServiceTransmitSucceeded, data1, data2, data3);
        }

        public bool TransportListenerSessionsReceivedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.TransportListenerSessionsReceived, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Receive, Task = Tasks.ActivationDispatchSession,
            Keywords = Keywords.ActivationServices,
            Message = "Transport listener session received with via '{0}'")]
        public void TransportListenerSessionsReceived(string via)
        {
            WriteEvent(EventIds.TransportListenerSessionsReceived, via);
        }

        public bool FailFastExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Critical, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.FailFastException, Level = EventLevel.Critical, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.WASActivation,
            Keywords = Keywords.ActivationServices,
            Message = "FailFastException.")]
        public void FailFastException(string SerializedException)
        {
            WriteEvent(EventIds.FailFastException, SerializedException);
        }

        public bool ServiceStartPipeErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceStartPipeError, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.ActivationServiceStart,
            Keywords = Keywords.ActivationServices,
            Message = "Service start pipe error.")]
        public void ServiceStartPipeError(string Endpoint)
        {
            WriteEvent(EventIds.ServiceStartPipeError, Endpoint);
        }

        public bool DispatchSessionStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.DispatchSessionStart, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.ActivationDispatchSession,
            Keywords = Keywords.ActivationServices,
            Message = "Session dispatch started.")]
        public void DispatchSessionStart()
        {
            WriteEvent(EventIds.DispatchSessionStart);
        }

        public bool PendingSessionQueueFullIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.PendingSessionQueueFull, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.ActivationDispatchSession,
            Keywords = Keywords.ActivationServices,
            Message = "Session dispatch for '{0}' failed since pending session queue is full with '{1}' pending items.")]
        public void PendingSessionQueueFull(string Uri, int count)
        {
            WriteEvent(EventIds.PendingSessionQueueFull, Uri, count);
        }

        public bool MessageQueueRegisterStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.MessageQueueRegisterStart, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.MessageQueueRegister,
            Keywords = Keywords.ActivationServices,
            Message = "Message queue register start.")]
        public void MessageQueueRegisterStart()
        {
            WriteEvent(EventIds.MessageQueueRegisterStart);
        }

        public bool MessageQueueRegisterAbortIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.MessageQueueRegisterAbort, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.MessageQueueRegister,
            Keywords = Keywords.ActivationServices,
            Message = "Message queue registration aborted with status:'{0}' for uri:'{1}'.")]
        public void MessageQueueRegisterAbort(string Status, string Uri)
        {
            WriteEvent(EventIds.MessageQueueRegisterAbort, Status, Uri);
        }

        public bool MessageQueueUnregisterSucceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.MessageQueueUnregisterSucceeded, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.MessageQueueRegister,
            Keywords = Keywords.ActivationServices,
            Message = "Message queue unregister succeeded for uri:'{0}'.")]
        public void MessageQueueUnregisterSucceeded(string Uri)
        {
            WriteEvent(EventIds.MessageQueueUnregisterSucceeded, Uri);
        }

        public bool MessageQueueRegisterFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.MessageQueueRegisterFailed, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.MessageQueueRegister,
            Keywords = Keywords.ActivationServices,
            Message = "Message queue registration for uri:'{0}' failed with status:'{1}'.")]
        public void MessageQueueRegisterFailed(string Uri, string Status)
        {
            WriteEvent(EventIds.MessageQueueRegisterFailed, Uri, Status);
        }

        public bool MessageQueueRegisterCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.MessageQueueRegisterCompleted, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.MessageQueueRegister,
            Keywords = Keywords.ActivationServices,
            Message = "Message queue registration completed for uri '{0}'.")]
        public void MessageQueueRegisterCompleted(string Uri)
        {
            WriteEvent(EventIds.MessageQueueRegisterCompleted, Uri);
        }

        public bool MessageQueueDuplicatedSocketErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.MessageQueueDuplicatedSocketError, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.ActivationDispatchSession,
            Keywords = Keywords.ActivationServices,
            Message = "Message queue failed duplicating socket.")]
        public void MessageQueueDuplicatedSocketError()
        {
            WriteEvent(EventIds.MessageQueueDuplicatedSocketError);
        }

        public bool MessageQueueDuplicatedSocketCompleteIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.MessageQueueDuplicatedSocketComplete, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.ActivationDispatchSession,
            Keywords = Keywords.ActivationServices,
            Message = "MessageQueueDuplicatedSocketComplete")]
        public void MessageQueueDuplicatedSocketComplete()
        {
            WriteEvent(EventIds.MessageQueueDuplicatedSocketComplete);
        }

        public bool TcpTransportListenerListeningStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.TcpTransportListenerListeningStart, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.ActivationTcpListenerListening,
            Keywords = Keywords.ActivationServices,
            Message = "Tcp transport listener starting to listen on uri:'{0}'.")]
        public void TcpTransportListenerListeningStart(string Uri)
        {
            WriteEvent(EventIds.TcpTransportListenerListeningStart, Uri);
        }

        public bool TcpTransportListenerListeningStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.TcpTransportListenerListeningStop, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.ActivationTcpListenerListening,
            Keywords = Keywords.ActivationServices,
            Message = "Tcp transport listener listening.")]
        public void TcpTransportListenerListeningStop()
        {
            WriteEvent(EventIds.TcpTransportListenerListeningStop);
        }

        public bool WebhostUnregisterProtocolFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.WebhostUnregisterProtocolFailed, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.WASActivation,
            Keywords = Keywords.ActivationServices,
            Message = "Error Code:{0}")]
        public void WebhostUnregisterProtocolFailed(string hresult)
        {
            WriteEvent(EventIds.WebhostUnregisterProtocolFailed, hresult);
        }

        public bool WasCloseAllListenerChannelInstancesCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.WasCloseAllListenerChannelInstancesCompleted, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.WASActivation,
            Keywords = Keywords.ActivationServices,
            Message = "Was closing all listener channel instances completed.")]
        public void WasCloseAllListenerChannelInstancesCompleted()
        {
            WriteEvent(EventIds.WasCloseAllListenerChannelInstancesCompleted);
        }

        public bool WasCloseAllListenerChannelInstancesFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.WasCloseAllListenerChannelInstancesFailed, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.WASActivation,
            Keywords = Keywords.ActivationServices,
            Message = "Error Code:{0}")]
        public void WasCloseAllListenerChannelInstancesFailed(string hresult)
        {
            WriteEvent(EventIds.WasCloseAllListenerChannelInstancesFailed, hresult);
        }

        public bool OpenListenerChannelInstanceFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.OpenListenerChannelInstanceFailed, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.WASActivation,
            Keywords = Keywords.ActivationServices,
            Message = "Error Code:{0}")]
        public void OpenListenerChannelInstanceFailed(string hresult)
        {
            WriteEvent(EventIds.OpenListenerChannelInstanceFailed, hresult);
        }

        public bool WasConnectedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.WasConnected, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.Connected, Task = Tasks.WASActivation,
            Keywords = Keywords.ActivationServices,
            Message = "WAS Connected.")]
        public void WasConnected()
        {
            WriteEvent(EventIds.WasConnected);
        }

        public bool WasDisconnectedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.WasDisconnected, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.Disconnect, Task = Tasks.WASActivation,
            Keywords = Keywords.ActivationServices,
            Message = "WAS Disconnected.")]
        public void WasDisconnected()
        {
            WriteEvent(EventIds.WasDisconnected);
        }

        public bool PipeTransportListenerListeningStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.PipeTransportListenerListeningStart, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.ActivationPipeListenerListening,
            Keywords = Keywords.ActivationServices,
            Message = "Pipe transport listener listening start on uri:{0}.")]
        public void PipeTransportListenerListeningStart(string Uri)
        {
            WriteEvent(EventIds.PipeTransportListenerListeningStart, Uri);
        }

        public bool PipeTransportListenerListeningStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.PipeTransportListenerListeningStop, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.ActivationPipeListenerListening,
            Keywords = Keywords.ActivationServices,
            Message = "Pipe transport listener listening stop.")]
        public void PipeTransportListenerListeningStop()
        {
            WriteEvent(EventIds.PipeTransportListenerListeningStop);
        }

        public bool DispatchSessionSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.DispatchSessionSuccess, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.ActivationDispatchSession,
            Keywords = Keywords.ActivationServices,
            Message = "Session dispatch succeeded.")]
        public void DispatchSessionSuccess()
        {
            WriteEvent(EventIds.DispatchSessionSuccess);
        }

        public bool DispatchSessionFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.DispatchSessionFailed, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.ActivationDispatchSession,
            Keywords = Keywords.ActivationServices,
            Message = "Session dispatch failed.")]
        public void DispatchSessionFailed()
        {
            WriteEvent(EventIds.DispatchSessionFailed);
        }

        public bool WasConnectionTimedoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Critical, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.WasConnectionTimedout, Level = EventLevel.Critical, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.WASActivation,
            Keywords = Keywords.ActivationServices,
            Message = "WAS connection timed out.")]
        public void WasConnectionTimedout()
        {
            WriteEvent(EventIds.WasConnectionTimedout);
        }

        public bool RoutingTableLookupStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingTableLookupStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ActivationRoutingTableLookup,
            Keywords = Keywords.ActivationServices,
            Message = "Routing table lookup started.")]
        public void RoutingTableLookupStart()
        {
            WriteEvent(EventIds.RoutingTableLookupStart);
        }

        public bool RoutingTableLookupStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingTableLookupStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ActivationRoutingTableLookup,
            Keywords = Keywords.ActivationServices,
            Message = "Routing table lookup completed.")]
        public void RoutingTableLookupStop()
        {
            WriteEvent(EventIds.RoutingTableLookupStop);
        }

        public bool PendingSessionQueueRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Debug);
        }

        [Event(EventIds.PendingSessionQueueRatio, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Pending session queue ratio: {0}/{1}")]
        public void PendingSessionQueueRatio(int curr, int max)
        {
            WriteEvent(EventIds.PendingSessionQueueRatio, curr, max);
        }

        public bool EndSqlCommandExecuteIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.EndSqlCommandExecute, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.SqlCommandExecute,
            Keywords = Keywords.WFInstanceStore,
            Message = "End SQL command execution: {0}")]
        public void EndSqlCommandExecute(string data1)
        {
            WriteEvent(EventIds.EndSqlCommandExecute, data1);
        }

        public bool StartSqlCommandExecuteIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.StartSqlCommandExecute, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.SqlCommandExecute,
            Keywords = Keywords.WFInstanceStore,
            Message = "Starting SQL command execution: {0}")]
        public void StartSqlCommandExecute(string data1)
        {
            WriteEvent(EventIds.StartSqlCommandExecute, data1);
        }

        public bool RenewLockSystemErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.RenewLockSystemError, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.LockWorkflowInstance,
            Keywords = Keywords.WFInstanceStore,
            Message = "Failed to extend lock expiration, lock expiration already passed or the lock owner was deleted. Aborting SqlWorkflowInstanceStore.")]
        public void RenewLockSystemError()
        {
            WriteEvent(EventIds.RenewLockSystemError);
        }

        public bool FoundProcessingErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.FoundProcessingError, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SqlCommandExecute,
            Keywords = Keywords.WFInstanceStore,
            Message = "Command failed: {0}")]
        public void FoundProcessingError(string data1, string SerializedException)
        {
            WriteEvent(EventIds.FoundProcessingError, data1, SerializedException);
        }

        public bool UnlockInstanceExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.UnlockInstanceException, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.LockWorkflowInstance,
            Keywords = Keywords.WFInstanceStore,
            Message = "Encountered exception {0} while attempting to unlock instance.")]
        public void UnlockInstanceException(string data1)
        {
            WriteEvent(EventIds.UnlockInstanceException, data1);
        }

        public bool MaximumRetriesExceededForSqlCommandIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Quota | Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.MaximumRetriesExceededForSqlCommand, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SqlCommandExecute,
            Keywords = Keywords.Quota | Keywords.WFInstanceStore,
            Message = "Giving up retrying a SQL command as the maximum number of retries have been performed.")]
        public void MaximumRetriesExceededForSqlCommand()
        {
            WriteEvent(EventIds.MaximumRetriesExceededForSqlCommand);
        }

        public bool RetryingSqlCommandDueToSqlErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.RetryingSqlCommandDueToSqlError, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SqlCommandExecute,
            Keywords = Keywords.WFInstanceStore,
            Message = "Retrying a SQL command due to SQL error number {0}.")]
        public void RetryingSqlCommandDueToSqlError(string data1)
        {
            WriteEvent(EventIds.RetryingSqlCommandDueToSqlError, data1);
        }

        public bool TimeoutOpeningSqlConnectionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.TimeoutOpeningSqlConnection, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SqlCommandExecute,
            Keywords = Keywords.WFInstanceStore,
            Message = "Timeout trying to open a SQL connection. The operation did not complete within the allotted timeout of {0}. The time allotted to this operation may have been a portion of a longer timeout.")]
        public void TimeoutOpeningSqlConnection(string data1)
        {
            WriteEvent(EventIds.TimeoutOpeningSqlConnection, data1);
        }

        public bool SqlExceptionCaughtIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.SqlExceptionCaught, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SqlCommandExecute,
            Keywords = Keywords.WFInstanceStore,
            Message = "Caught SQL Exception number {0} message {1}.")]
        public void SqlExceptionCaught(string data1, string data2)
        {
            WriteEvent(EventIds.SqlExceptionCaught, data1, data2);
        }

        public bool QueuingSqlRetryIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.QueuingSqlRetry, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.LockWorkflowInstance,
            Keywords = Keywords.WFInstanceStore,
            Message = "Queuing SQL retry with delay {0} milliseconds.")]
        public void QueuingSqlRetry(string data1)
        {
            WriteEvent(EventIds.QueuingSqlRetry, data1);
        }

        public bool LockRetryTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.LockRetryTimeout, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SqlCommandExecute,
            Keywords = Keywords.WFInstanceStore,
            Message = "Timeout trying to acquire the instance lock.  The operation did not complete within the allotted timeout of {0}. The time allotted to this operation may have been a portion of a longer timeout.")]
        public void LockRetryTimeout(string data1)
        {
            WriteEvent(EventIds.LockRetryTimeout, data1);
        }

        public bool RunnableInstancesDetectionErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.RunnableInstancesDetectionError, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.LockWorkflowInstance,
            Keywords = Keywords.WFInstanceStore,
            Message = "Detection of runnable instances failed due to the following exception")]
        public void RunnableInstancesDetectionError(string SerializedException)
        {
            WriteEvent(EventIds.RunnableInstancesDetectionError, SerializedException);
        }

        public bool InstanceLocksRecoveryErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.InstanceLocksRecoveryError, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.LockWorkflowInstance,
            Keywords = Keywords.WFInstanceStore,
            Message = "Recovering instance locks failed due to the following exception")]
        public void InstanceLocksRecoveryError(string SerializedException)
        {
            WriteEvent(EventIds.InstanceLocksRecoveryError, SerializedException);
        }

        public bool MessageLogEventSizeExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WCFMessageLogging, EventChannel.Debug);
        }

        [Event(EventIds.MessageLogEventSizeExceeded, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WCFMessageLogging,
            Message = "Message could not be logged as it exceeds the ETW event size")]
        public void MessageLogEventSizeExceeded()
        {
            WriteEvent(EventIds.MessageLogEventSizeExceeded);
        }

        public bool DiscoveryClientInClientChannelFailedToCloseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryClientInClientChannelFailedToClose, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.FailedToClose, Task = Tasks.DiscoveryClient,
            Keywords = Keywords.Discovery,
            Message = "The DiscoveryClient created inside DiscoveryClientChannel failed to close and hence has been aborted.")]
        public void DiscoveryClientInClientChannelFailedToClose(string SerializedException)
        {
            WriteEvent(EventIds.DiscoveryClientInClientChannelFailedToClose, SerializedException);
        }

        public bool DiscoveryClientProtocolExceptionSuppressedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryClientProtocolExceptionSuppressed, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ExceptionSuppressed, Task = Tasks.DiscoveryClient,
            Keywords = Keywords.Discovery,
            Message = "A ProtocolException was suppressed while closing the DiscoveryClient. This could be because a DiscoveryService is still trying to send response to the DiscoveryClient.")]
        public void DiscoveryClientProtocolExceptionSuppressed(string SerializedException)
        {
            WriteEvent(EventIds.DiscoveryClientProtocolExceptionSuppressed, SerializedException);
        }

        public bool DiscoveryClientReceivedMulticastSuppressionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryClientReceivedMulticastSuppression, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ReceivedMulticastSuppression, Task = Tasks.DiscoveryClient,
            Keywords = Keywords.Discovery,
            Message = "The DiscoveryClient received a multicast suppression message from a DiscoveryProxy.")]
        public void DiscoveryClientReceivedMulticastSuppression()
        {
            WriteEvent(EventIds.DiscoveryClientReceivedMulticastSuppression);
        }

        public bool DiscoveryMessageReceivedAfterOperationCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryMessageReceivedAfterOperationCompleted, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ReceivedAfterOperationCompleted, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message with messageId='{1}' was dropped by the DiscoveryClient because the corresponding {2} operation was completed.")]
        public void DiscoveryMessageReceivedAfterOperationCompleted(string discoveryMessageName, string messageId, string discoveryOperationName)
        {
            WriteEvent(EventIds.DiscoveryMessageReceivedAfterOperationCompleted, discoveryMessageName, messageId, discoveryOperationName);
        }

        public bool DiscoveryMessageWithInvalidContentIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryMessageWithInvalidContent, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.InvalidContent, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message with messageId='{1}' was dropped because it had invalid content.")]
        public void DiscoveryMessageWithInvalidContent(string messageType, string messageId)
        {
            WriteEvent(EventIds.DiscoveryMessageWithInvalidContent, messageType, messageId);
        }

        public bool DiscoveryMessageWithInvalidRelatesToOrOperationCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryMessageWithInvalidRelatesToOrOperationCompleted, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.InvalidRelatesToOrOperationCompleted, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message with messageId='{1}' and relatesTo='{2}' was dropped by the DiscoveryClient because either the corresponding {3} operation was completed or the relatesTo value is invalid.")]
        public void DiscoveryMessageWithInvalidRelatesToOrOperationCompleted(string discoveryMessageName, string messageId, string relatesTo, string discoveryOperationName)
        {
            WriteEvent(EventIds.DiscoveryMessageWithInvalidRelatesToOrOperationCompleted, discoveryMessageName, messageId, relatesTo, discoveryOperationName);
        }

        public bool DiscoveryMessageWithInvalidReplyToIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryMessageWithInvalidReplyTo, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.InvalidReplyTo, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A discovery request message with messageId='{0}' was dropped because it had an invalid ReplyTo address.")]
        public void DiscoveryMessageWithInvalidReplyTo(string messageId)
        {
            WriteEvent(EventIds.DiscoveryMessageWithInvalidReplyTo, messageId);
        }

        public bool DiscoveryMessageWithNoContentIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryMessageWithNoContent, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.NoContent, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message was dropped because it did not have any content.")]
        public void DiscoveryMessageWithNoContent(string messageType)
        {
            WriteEvent(EventIds.DiscoveryMessageWithNoContent, messageType);
        }

        public bool DiscoveryMessageWithNullMessageIdIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryMessageWithNullMessageId, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.NullMessageId, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message was dropped because the message header did not contain the required MessageId property.")]
        public void DiscoveryMessageWithNullMessageId(string messageType)
        {
            WriteEvent(EventIds.DiscoveryMessageWithNullMessageId, messageType);
        }

        public bool DiscoveryMessageWithNullMessageSequenceIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryMessageWithNullMessageSequence, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.NullMessageSequence, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message with messageId='{1}' was dropped by the DiscoveryClient because it did not have the DiscoveryMessageSequence property.")]
        public void DiscoveryMessageWithNullMessageSequence(string discoveryMessageName, string messageId)
        {
            WriteEvent(EventIds.DiscoveryMessageWithNullMessageSequence, discoveryMessageName, messageId);
        }

        public bool DiscoveryMessageWithNullRelatesToIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryMessageWithNullRelatesTo, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.NullRelatesTo, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message with messageId='{1}' was dropped by the DiscoveryClient because the message header did not contain the required RelatesTo property.")]
        public void DiscoveryMessageWithNullRelatesTo(string discoveryMessageName, string messageId)
        {
            WriteEvent(EventIds.DiscoveryMessageWithNullRelatesTo, discoveryMessageName, messageId);
        }

        public bool DiscoveryMessageWithNullReplyToIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryMessageWithNullReplyTo, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.NullReplyTo, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A discovery request message with messageId='{0}' was dropped because it did not have a ReplyTo address.")]
        public void DiscoveryMessageWithNullReplyTo(string messageId)
        {
            WriteEvent(EventIds.DiscoveryMessageWithNullReplyTo, messageId);
        }

        public bool DuplicateDiscoveryMessageIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DuplicateDiscoveryMessage, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.Duplicate, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message with messageId='{1}' was dropped because it was a duplicate.")]
        public void DuplicateDiscoveryMessage(string messageType, string messageId)
        {
            WriteEvent(EventIds.DuplicateDiscoveryMessage, messageType, messageId);
        }

        public bool EndpointDiscoverabilityDisabledIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.EndpointDiscoverabilityDisabled, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Disabled, Task = Tasks.EndpointDiscoverability,
            Keywords = Keywords.Discovery,
            Message = "The discoverability of endpoint with EndpointAddress='{0}' and ListenUri='{1}' has been disabled.")]
        public void EndpointDiscoverabilityDisabled(string endpointAddress, string listenUri)
        {
            WriteEvent(EventIds.EndpointDiscoverabilityDisabled, endpointAddress, listenUri);
        }

        public bool EndpointDiscoverabilityEnabledIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.EndpointDiscoverabilityEnabled, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Enabled, Task = Tasks.EndpointDiscoverability,
            Keywords = Keywords.Discovery,
            Message = "The discoverability of endpoint with EndpointAddress='{0}' and ListenUri='{1}' has been enabled.")]
        public void EndpointDiscoverabilityEnabled(string endpointAddress, string listenUri)
        {
            WriteEvent(EventIds.EndpointDiscoverabilityEnabled, endpointAddress, listenUri);
        }

        public bool FindInitiatedInDiscoveryClientChannelIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.FindInitiatedInDiscoveryClientChannel, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.FindInitiated, Task = Tasks.DiscoveryClientChannel,
            Keywords = Keywords.Discovery,
            Message = "A Find operation was initiated in the DiscoveryClientChannel to discover endpoint(s).")]
        public void FindInitiatedInDiscoveryClientChannel()
        {
            WriteEvent(EventIds.FindInitiatedInDiscoveryClientChannel);
        }

        public bool InnerChannelCreationFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.InnerChannelCreationFailed, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.CreationFailed, Task = Tasks.DiscoveryClientChannel,
            Keywords = Keywords.Discovery,
            Message = "The DiscoveryClientChannel failed to create the channel with a discovered endpoint with EndpointAddress='{0}' and Via='{1}'. The DiscoveryClientChannel will now attempt to use the next available discovered endpoint.")]
        public void InnerChannelCreationFailed(string endpointAddress, string via, string SerializedException)
        {
            WriteEvent(EventIds.InnerChannelCreationFailed, endpointAddress, via, SerializedException);
        }

        public bool InnerChannelOpenFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.InnerChannelOpenFailed, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.OpenFailed, Task = Tasks.DiscoveryClientChannel,
            Keywords = Keywords.Discovery,
            Message = "The DiscoveryClientChannel failed to open the channel with a discovered endpoint with EndpointAddress='{0}' and Via='{1}'. The DiscoveryClientChannel will now attempt to use the next available discovered endpoint.")]
        public void InnerChannelOpenFailed(string endpointAddress, string via, string SerializedException)
        {
            WriteEvent(EventIds.InnerChannelOpenFailed, endpointAddress, via, SerializedException);
        }

        public bool InnerChannelOpenSucceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.InnerChannelOpenSucceeded, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.OpenSucceeded, Task = Tasks.DiscoveryClientChannel,
            Keywords = Keywords.Discovery,
            Message = "The DiscoveryClientChannel successfully discovered an endpoint and opened the channel using it. The client is connected to a service using EndpointAddress='{0}' and Via='{1}'.")]
        public void InnerChannelOpenSucceeded(string endpointAddress, string via)
        {
            WriteEvent(EventIds.InnerChannelOpenSucceeded, endpointAddress, via);
        }

        public bool SynchronizationContextResetIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.SynchronizationContextReset, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Reset, Task = Tasks.DiscoverySynchronizationContext,
            Keywords = Keywords.Discovery,
            Message = "The SynchronizationContext has been reset to its original value of {0} by DiscoveryClientChannel.")]
        public void SynchronizationContextReset(string synchronizationContextType)
        {
            WriteEvent(EventIds.SynchronizationContextReset, synchronizationContextType);
        }

        public bool SynchronizationContextSetToNullIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.SynchronizationContextSetToNull, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.SetToNull, Task = Tasks.DiscoverySynchronizationContext,
            Keywords = Keywords.Discovery,
            Message = "The SynchronizationContext has been set to null by DiscoveryClientChannel before initiating the Find operation.")]
        public void SynchronizationContextSetToNull()
        {
            WriteEvent(EventIds.SynchronizationContextSetToNull);
        }

        public bool DCSerializeWithSurrogateStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCSerializeWithSurrogateStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.SurrogateSerialize,
            Keywords = Keywords.Serialization,
            Message = "DataContract serialize {0} with surrogates start.")]
        public void DCSerializeWithSurrogateStart(string SurrogateType)
        {
            WriteEvent(EventIds.DCSerializeWithSurrogateStart, SurrogateType);
        }

        public bool DCSerializeWithSurrogateStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCSerializeWithSurrogateStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.SurrogateSerialize,
            Keywords = Keywords.Serialization,
            Message = "DataContract serialize with surrogates stop.")]
        public void DCSerializeWithSurrogateStop()
        {
            WriteEvent(EventIds.DCSerializeWithSurrogateStop);
        }

        public bool DCDeserializeWithSurrogateStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCDeserializeWithSurrogateStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.SurrogateDeserialize,
            Keywords = Keywords.Serialization,
            Message = "DataContract deserialize {0} with surrogates start.")]
        public void DCDeserializeWithSurrogateStart(string SurrogateType)
        {
            WriteEvent(EventIds.DCDeserializeWithSurrogateStart, SurrogateType);
        }

        public bool DCDeserializeWithSurrogateStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCDeserializeWithSurrogateStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.SurrogateDeserialize,
            Keywords = Keywords.Serialization,
            Message = "DataContract deserialize with surrogates stop.")]
        public void DCDeserializeWithSurrogateStop()
        {
            WriteEvent(EventIds.DCDeserializeWithSurrogateStop);
        }

        public bool ImportKnownTypesStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.ImportKnownTypesStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ImportKnownType,
            Keywords = Keywords.Serialization,
            Message = "ImportKnownTypes start.")]
        public void ImportKnownTypesStart()
        {
            WriteEvent(EventIds.ImportKnownTypesStart);
        }

        public bool ImportKnownTypesStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.ImportKnownTypesStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ImportKnownType,
            Keywords = Keywords.Serialization,
            Message = "ImportKnownTypes stop.")]
        public void ImportKnownTypesStop()
        {
            WriteEvent(EventIds.ImportKnownTypesStop);
        }

        public bool DCResolverResolveIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCResolverResolve, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.DataContractResolver,
            Keywords = Keywords.Serialization,
            Message = "DataContract resolver resolving {0} start.")]
        public void DCResolverResolve(string TypeName)
        {
            WriteEvent(EventIds.DCResolverResolve, TypeName);
        }

        public bool DCGenWriterStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCGenWriterStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.GenerateSerializer,
            Keywords = Keywords.Serialization,
            Message = "DataContract generate {0} writer for {1} start.")]
        public void DCGenWriterStart(string Kind, string TypeName)
        {
            WriteEvent(EventIds.DCGenWriterStart, Kind, TypeName);
        }

        public bool DCGenWriterStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCGenWriterStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.GenerateSerializer,
            Keywords = Keywords.Serialization,
            Message = "DataContract generate writer stop.")]
        public void DCGenWriterStop()
        {
            WriteEvent(EventIds.DCGenWriterStop);
        }

        public bool DCGenReaderStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCGenReaderStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.GenerateDeserializer,
            Keywords = Keywords.Serialization,
            Message = "DataContract generate {0} reader for {1} start.")]
        public void DCGenReaderStart(string Kind, string TypeName)
        {
            WriteEvent(EventIds.DCGenReaderStart, Kind, TypeName);
        }

        public bool DCGenReaderStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCGenReaderStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.GenerateDeserializer,
            Keywords = Keywords.Serialization,
            Message = "DataContract generation stop.")]
        public void DCGenReaderStop()
        {
            WriteEvent(EventIds.DCGenReaderStop);
        }

        public bool DCJsonGenReaderStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCJsonGenReaderStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.GenerateDeserializer,
            Keywords = Keywords.Serialization,
            Message = "Json generate {0} reader for {1} start.")]
        public void DCJsonGenReaderStart(string Kind, string TypeName)
        {
            WriteEvent(EventIds.DCJsonGenReaderStart, Kind, TypeName);
        }

        public bool DCJsonGenReaderStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCJsonGenReaderStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.GenerateDeserializer,
            Keywords = Keywords.Serialization,
            Message = "Json reader generation stop.")]
        public void DCJsonGenReaderStop()
        {
            WriteEvent(EventIds.DCJsonGenReaderStop);
        }

        public bool DCJsonGenWriterStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCJsonGenWriterStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.GenerateSerializer,
            Keywords = Keywords.Serialization,
            Message = "Json generate {0} writer for {1} start.")]
        public void DCJsonGenWriterStart(string Kind, string TypeName)
        {
            WriteEvent(EventIds.DCJsonGenWriterStart, Kind, TypeName);
        }

        public bool DCJsonGenWriterStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCJsonGenWriterStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.GenerateSerializer,
            Keywords = Keywords.Serialization,
            Message = "Json generate writer stop.")]
        public void DCJsonGenWriterStop()
        {
            WriteEvent(EventIds.DCJsonGenWriterStop);
        }

        public bool GenXmlSerializableStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.GenXmlSerializableStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.GenerateXmlSerializable,
            Keywords = Keywords.Serialization,
            Message = "Generate Xml serializable for '{0}' start.")]
        public void GenXmlSerializableStart(string DCType)
        {
            WriteEvent(EventIds.GenXmlSerializableStart, DCType);
        }

        public bool GenXmlSerializableStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.GenXmlSerializableStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.GenerateXmlSerializable,
            Keywords = Keywords.Serialization,
            Message = "Generate Xml serializable stop.")]
        public void GenXmlSerializableStop()
        {
            WriteEvent(EventIds.GenXmlSerializableStop);
        }

        public bool JsonMessageDecodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.JsonMessageDecodingStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.MessageDecoding,
            Keywords = Keywords.Channel,
            Message = "JsonMessageEncoder started decoding the message.")]
        public void JsonMessageDecodingStart()
        {
            WriteEvent(EventIds.JsonMessageDecodingStart);
        }

        public bool JsonMessageEncodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.JsonMessageEncodingStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.MessageEncoding,
            Keywords = Keywords.Channel,
            Message = "JsonMessageEncoder started encoding the message.")]
        public void JsonMessageEncodingStart()
        {
            WriteEvent(EventIds.JsonMessageEncodingStart);
        }

        public bool TokenValidationStartedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.TokenValidationStarted, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.TokenValidation,
            Keywords = Keywords.Security,
            Message = "SecurityToken (type '{0}' and id '{1}') validation started.")]
        public void TokenValidationStarted(string tokenType, string tokenID, string HostReference)
        {
            WriteEvent(EventIds.TokenValidationStarted, tokenType, tokenID, HostReference);
        }

        public bool TokenValidationSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.TokenValidationSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.TokenValidation,
            Keywords = Keywords.Security,
            Message = "SecurityToken (type '{0}' and id '{1}') validation succeeded.")]
        public void TokenValidationSuccess(string tokenType, string tokenID, string HostReference)
        {
            WriteEvent(EventIds.TokenValidationSuccess, tokenType, tokenID, HostReference);
        }

        public bool TokenValidationFailureIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.TokenValidationFailure, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.TokenValidation,
            Keywords = Keywords.Security,
            Message = "SecurityToken (type '{0}' and id '{1}') validation failed. {2}")]
        public void TokenValidationFailure(string tokenType, string tokenID, string errorMessage, string HostReference)
        {
            WriteEvent(EventIds.TokenValidationFailure, tokenType, tokenID, errorMessage, HostReference);
        }

        public bool GetIssuerNameSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.GetIssuerNameSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.GetIssuerName,
            Keywords = Keywords.Security,
            Message = "Retrieval of issuer name:{0} from tokenId:{1} succeeded.")]
        public void GetIssuerNameSuccess(string issuerName, string tokenID, string HostReference)
        {
            WriteEvent(EventIds.GetIssuerNameSuccess, issuerName, tokenID, HostReference);
        }

        public bool GetIssuerNameFailureIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.GetIssuerNameFailure, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.GetIssuerName,
            Keywords = Keywords.Security,
            Message = "Retrieval of issuer name from tokenId:{0} failed.")]
        public void GetIssuerNameFailure(string tokenID, string HostReference)
        {
            WriteEvent(EventIds.GetIssuerNameFailure, tokenID, HostReference);
        }

        public bool FederationMessageProcessingStartedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.FederationMessageProcessingStarted, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.FederationMessageProcessing,
            Keywords = Keywords.Security,
            Message = "Federation message processing started.")]
        public void FederationMessageProcessingStarted(string HostReference)
        {
            WriteEvent(EventIds.FederationMessageProcessingStarted, HostReference);
        }

        public bool FederationMessageProcessingSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.FederationMessageProcessingSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.FederationMessageProcessing,
            Keywords = Keywords.Security,
            Message = "Federation message processing succeeded.")]
        public void FederationMessageProcessingSuccess(string HostReference)
        {
            WriteEvent(EventIds.FederationMessageProcessingSuccess, HostReference);
        }

        public bool FederationMessageCreationStartedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.FederationMessageCreationStarted, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.FederationMessageCreation,
            Keywords = Keywords.Security,
            Message = "Creating federation message from form post started.")]
        public void FederationMessageCreationStarted(string HostReference)
        {
            WriteEvent(EventIds.FederationMessageCreationStarted, HostReference);
        }

        public bool FederationMessageCreationSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.FederationMessageCreationSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.FederationMessageCreation,
            Keywords = Keywords.Security,
            Message = "Creating federation message from form post succeeded.")]
        public void FederationMessageCreationSuccess(string HostReference)
        {
            WriteEvent(EventIds.FederationMessageCreationSuccess, HostReference);
        }

        public bool SessionCookieReadingStartedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.SessionCookieReadingStarted, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SessionCookieReading,
            Keywords = Keywords.Security,
            Message = "Reading session token from session cookie started.")]
        public void SessionCookieReadingStarted(string HostReference)
        {
            WriteEvent(EventIds.SessionCookieReadingStarted, HostReference);
        }

        public bool SessionCookieReadingSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.SessionCookieReadingSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SessionCookieReading,
            Keywords = Keywords.Security,
            Message = "Reading session token from session cookie succeeded.")]
        public void SessionCookieReadingSuccess(string HostReference)
        {
            WriteEvent(EventIds.SessionCookieReadingSuccess, HostReference);
        }

        public bool PrincipalSettingFromSessionTokenStartedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.PrincipalSettingFromSessionTokenStarted, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.PrincipalSetting,
            Keywords = Keywords.Security,
            Message = "Principal setting from session token started.")]
        public void PrincipalSettingFromSessionTokenStarted(string HostReference)
        {
            WriteEvent(EventIds.PrincipalSettingFromSessionTokenStarted, HostReference);
        }

        public bool PrincipalSettingFromSessionTokenSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.PrincipalSettingFromSessionTokenSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.PrincipalSetting,
            Keywords = Keywords.Security,
            Message = "Principal setting from session token succeeded.")]
        public void PrincipalSettingFromSessionTokenSuccess(string HostReference)
        {
            WriteEvent(EventIds.PrincipalSettingFromSessionTokenSuccess, HostReference);
        }

        public bool TrackingRecordDroppedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFTracking, EventChannel.Debug);
        }

        [Event(EventIds.TrackingRecordDropped, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.Dropped, Task = Tasks.TrackingRecord,
            Keywords = Keywords.WFTracking,
            Message = "Size of tracking record {0} exceeds maximum allowed by the ETW session for provider {1}")]
        public void TrackingRecordDropped(long RecordNumber, Guid ProviderId)
        {
            WriteEvent(EventIds.TrackingRecordDropped, RecordNumber, ProviderId);
        }

        public bool TrackingRecordRaisedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.TrackingRecordRaised, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Raised, Task = Tasks.TrackingRecord,
            Keywords = Keywords.WFRuntime,
            Message = "Tracking Record {0} raised to {1}.")]
        public void TrackingRecordRaised(string data1, string data2)
        {
            WriteEvent(EventIds.TrackingRecordRaised, data1, data2);
        }

        public bool TrackingRecordTruncatedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFTracking, EventChannel.Debug);
        }

        [Event(EventIds.TrackingRecordTruncated, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.Truncated, Task = Tasks.TrackingRecord,
            Keywords = Keywords.WFTracking,
            Message = "Truncated tracking record {0} written to ETW session with provider {1}. Variables/annotations/user data have been removed")]
        public void TrackingRecordTruncated(long RecordNumber, Guid ProviderId)
        {
            WriteEvent(EventIds.TrackingRecordTruncated, RecordNumber, ProviderId);
        }

        public bool TrackingDataExtractedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.TrackingDataExtracted, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.TrackingProfile,
            Keywords = Keywords.WFRuntime,
            Message = "Tracking data {0} extracted in activity {1}.")]
        public void TrackingDataExtracted(string Data, string Activity)
        {
            WriteEvent(EventIds.TrackingDataExtracted, Data, Activity);
        }

        public bool TrackingValueNotSerializableIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFTracking, EventChannel.Debug);
        }

        [Event(EventIds.TrackingValueNotSerializable, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.TrackingProfile,
            Keywords = Keywords.WFTracking,
            Message = "The extracted argument/variable '{0}' is not serializable.")]
        public void TrackingValueNotSerializable(string name)
        {
            WriteEvent(EventIds.TrackingValueNotSerializable, name);
        }

        public bool AppDomainUnloadIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(EventIds.AppDomainUnload, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "AppDomain unloading. AppDomain.FriendlyName {0}, ProcessName {1}, ProcessId {2}.")]
        public void AppDomainUnload(string appdomainName, string processName, string processId)
        {
            WriteEvent(EventIds.AppDomainUnload, appdomainName, processName, processId);
        }

        public bool HandledExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(EventIds.HandledException, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Handling an exception.  Exception details: {0}")]
        public void HandledException(string data1, string SerializedException)
        {
            WriteEvent(EventIds.HandledException, data1, SerializedException);
        }

        public bool ShipAssertExceptionMessageIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(EventIds.ShipAssertExceptionMessage, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "An unexpected failure occurred. Applications should not attempt to handle this error. For diagnostic purposes, this English message is associated with the failure: {0}.")]
        public void ShipAssertExceptionMessage(string data1)
        {
            WriteEvent(EventIds.ShipAssertExceptionMessage, data1);
        }

        public bool ThrowingExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(EventIds.ThrowingException, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Throwing an exception. Source: {0}. Exception details: {1}")]
        public void ThrowingException(string data1, string data2, string SerializedException)
        {
            WriteEvent(EventIds.ThrowingException, data1, data2, SerializedException);
        }

        public bool UnhandledExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Critical, Keywords.Infrastructure, EventChannel.Operational);
        }

        [Event(EventIds.UnhandledException, Level = EventLevel.Critical, Channel = EventChannel.Operational, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Unhandled exception.  Exception details: {0}")]
        public void UnhandledException(string data1, string SerializedException)
        {
            WriteEvent(EventIds.UnhandledException, data1, SerializedException);
        }

        public bool MaxInstancesExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.MaxInstancesExceeded, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Throttles,
            Keywords = Keywords.WFServices,
            Message = "The system hit the limit set for throttle 'MaxConcurrentInstances'. Limit for this throttle was set to {0}. Throttle value can be changed by modifying attribute 'maxConcurrentInstances' in serviceThrottle element or by modifying 'MaxConcurrentInstances' property on behavior ServiceThrottlingBehavior.")]
        public void MaxInstancesExceeded(int limit)
        {
            WriteEvent(EventIds.MaxInstancesExceeded, limit);
        }

        public bool TraceCodeEventLogCriticalIsEnabled()
        {
            return base.IsEnabled(EventLevel.Critical, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(EventIds.TraceCodeEventLogCritical, Level = EventLevel.Critical, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Wrote to the EventLog.")]
        public void TraceCodeEventLogCritical(string ExtendedData)
        {
            WriteEvent(EventIds.TraceCodeEventLogCritical, ExtendedData);
        }

        public bool TraceCodeEventLogErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(EventIds.TraceCodeEventLogError, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Wrote to the EventLog.")]
        public void TraceCodeEventLogError(string ExtendedData)
        {
            WriteEvent(EventIds.TraceCodeEventLogError, ExtendedData);
        }

        public bool TraceCodeEventLogInfoIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(EventIds.TraceCodeEventLogInfo, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Wrote to the EventLog.")]
        public void TraceCodeEventLogInfo(string ExtendedData)
        {
            WriteEvent(EventIds.TraceCodeEventLogInfo, ExtendedData);
        }

        public bool TraceCodeEventLogVerboseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(EventIds.TraceCodeEventLogVerbose, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Wrote to the EventLog.")]
        public void TraceCodeEventLogVerbose(string ExtendedData)
        {
            WriteEvent(EventIds.TraceCodeEventLogVerbose, ExtendedData);
        }

        public bool TraceCodeEventLogWarningIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(EventIds.TraceCodeEventLogWarning, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Wrote to the EventLog.")]
        public void TraceCodeEventLogWarning(string ExtendedData)
        {
            WriteEvent(EventIds.TraceCodeEventLogWarning, ExtendedData);
        }

        public bool HandledExceptionWarningIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(EventIds.HandledExceptionWarning, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Handling an exception. Exception details: {0}")]
        public void HandledExceptionWarning(string data1, string SerializedException)
        {
            WriteEvent(EventIds.HandledExceptionWarning, data1, SerializedException);
        }

        public bool HandledExceptionErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Infrastructure, EventChannel.Operational);
        }

        [Event(EventIds.HandledExceptionError, Level = EventLevel.Error, Channel = EventChannel.Operational, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Handling an exception. Exception details: {0}")]
        public void HandledExceptionError(string data1, string SerializedException)
        {
            WriteEvent(EventIds.HandledExceptionError, data1, SerializedException);
        }

        public bool HandledExceptionVerboseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(EventIds.HandledExceptionVerbose, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Handling an exception  Exception details: {0}")]
        public void HandledExceptionVerbose(string data1, string SerializedException)
        {
            WriteEvent(EventIds.HandledExceptionVerbose, data1, SerializedException);
        }

        public bool ThrowingExceptionVerboseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(EventIds.ThrowingExceptionVerbose, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Throwing an exception. Source: {0}. Exception details: {1}")]
        public void ThrowingExceptionVerbose(string data1, string data2, string SerializedException)
        {
            WriteEvent(EventIds.ThrowingExceptionVerbose, data1, data2, SerializedException);
        }

        public bool EtwUnhandledExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Critical, Keywords.Infrastructure, EventChannel.Operational);
        }

        [Event(EventIds.EtwUnhandledException, Level = EventLevel.Critical, Channel = EventChannel.Operational, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Unhandled exception. Exception details: {0}")]
        public void EtwUnhandledException(string data1, string SerializedException)
        {
            WriteEvent(EventIds.EtwUnhandledException, data1, SerializedException);
        }

        public bool ThrowingEtwExceptionVerboseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(EventIds.ThrowingEtwExceptionVerbose, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Throwing an exception. Source: {0}. Exception details: {1}")]
        public void ThrowingEtwExceptionVerbose(string data1, string data2, string SerializedException)
        {
            WriteEvent(EventIds.ThrowingEtwExceptionVerbose, data1, data2, SerializedException);
        }

        public bool ThrowingEtwExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(EventIds.ThrowingEtwException, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Throwing an exception. Source: {0}. Exception details: {1}")]
        public void ThrowingEtwException(string data1, string data2, string SerializedException)
        {
            WriteEvent(EventIds.ThrowingEtwException, data1, data2, SerializedException);
        }

        public bool HttpHandlerPickedForUrlIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.HttpHandlerPickedForUrl, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.CreateWorkflowServiceHost,
            Keywords = Keywords.WebHost,
            Message = "The url '{0}' hosts XAML document with root element type '{1}'. The HTTP handler type '{2}' is picked to serve all the requests made to this url.")]
        public void HttpHandlerPickedForUrl(string data1, string data2, string data3)
        {
            WriteEvent(EventIds.HttpHandlerPickedForUrl, data1, data2, data3);
        }


        #region Keywords / Tasks / Opcodes

        public class EventIds
        {
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
            public const EventOpcode NoBookmark = (EventOpcode)10;
            public const EventOpcode Begin = (EventOpcode)11;
            public const EventOpcode NoInstance = (EventOpcode)11;
            public const EventOpcode Allocate = (EventOpcode)12;
            public const EventOpcode Tune = (EventOpcode)13;
            public const EventOpcode ClientChannelOpenStart = (EventOpcode)14;
            public const EventOpcode ClientChannelOpenStop = (EventOpcode)15;
            public const EventOpcode ClientMessageInspectorAfterReceiveInvoked = (EventOpcode)16;
            public const EventOpcode ClientMessageInspectorBeforeSendInvoked = (EventOpcode)17;
            public const EventOpcode ClientParameterInspectorStart = (EventOpcode)18;
            public const EventOpcode ClientParameterInspectorStop = (EventOpcode)19;
            public const EventOpcode OperationPrepared = (EventOpcode)20;
            public const EventOpcode CompleteBookmark = (EventOpcode)21;
            public const EventOpcode CompleteCancelActivity = (EventOpcode)22;
            public const EventOpcode CompleteCompletion = (EventOpcode)23;
            public const EventOpcode CompleteExecuteActivity = (EventOpcode)24;
            public const EventOpcode CompleteFault = (EventOpcode)25;
            public const EventOpcode CompleteRuntime = (EventOpcode)26;
            public const EventOpcode CompleteTransactionContext = (EventOpcode)27;
            public const EventOpcode DuplicateQuery = (EventOpcode)28;
            public const EventOpcode ExceptionSuppressed = (EventOpcode)29;
            public const EventOpcode FailedToClose = (EventOpcode)30;
            public const EventOpcode ReceivedMulticastSuppression = (EventOpcode)31;
            public const EventOpcode CreationFailed = (EventOpcode)32;
            public const EventOpcode FindInitiated = (EventOpcode)33;
            public const EventOpcode OpenFailed = (EventOpcode)34;
            public const EventOpcode OpenSucceeded = (EventOpcode)35;
            public const EventOpcode Duplicate = (EventOpcode)36;
            public const EventOpcode InvalidContent = (EventOpcode)37;
            public const EventOpcode InvalidRelatesToOrOperationCompleted = (EventOpcode)38;
            public const EventOpcode InvalidReplyTo = (EventOpcode)39;
            public const EventOpcode NoContent = (EventOpcode)40;
            public const EventOpcode NullMessageId = (EventOpcode)41;
            public const EventOpcode NullMessageSequence = (EventOpcode)42;
            public const EventOpcode NullRelatesTo = (EventOpcode)43;
            public const EventOpcode NullReplyTo = (EventOpcode)44;
            public const EventOpcode ReceivedAfterOperationCompleted = (EventOpcode)45;
            public const EventOpcode Reset = (EventOpcode)46;
            public const EventOpcode SetToNull = (EventOpcode)47;
            public const EventOpcode BeforeAuthorization = (EventOpcode)48;
            public const EventOpcode DispatchStart = (EventOpcode)49;
            public const EventOpcode DispatchStop = (EventOpcode)50;
            public const EventOpcode DispathMessageInspectorAfterReceiveInvoked = (EventOpcode)51;
            public const EventOpcode DispathMessageInspectorBeforeSendInvoked = (EventOpcode)52;
            public const EventOpcode OperationInvokerStart = (EventOpcode)53;
            public const EventOpcode OperationInvokerStop = (EventOpcode)54;
            public const EventOpcode ParameterInspectorStart = (EventOpcode)55;
            public const EventOpcode ParameterInspectorStop = (EventOpcode)56;
            public const EventOpcode TransactionScopeCreate = (EventOpcode)57;
            public const EventOpcode Disabled = (EventOpcode)58;
            public const EventOpcode Enabled = (EventOpcode)59;
            public const EventOpcode Empty = (EventOpcode)60;
            public const EventOpcode NextNull = (EventOpcode)61;
            public const EventOpcode SwitchCase = (EventOpcode)62;
            public const EventOpcode SwitchCaseNotFound = (EventOpcode)63;
            public const EventOpcode SwitchDefault = (EventOpcode)64;
            public const EventOpcode Contract = (EventOpcode)69;
            public const EventOpcode Operation = (EventOpcode)70;
            public const EventOpcode DoesNotUseAsyncPattern = (EventOpcode)71;
            public const EventOpcode IsNotStatic = (EventOpcode)72;
            public const EventOpcode IsStatic = (EventOpcode)73;
            public const EventOpcode ThrewException = (EventOpcode)74;
            public const EventOpcode UseAsyncPattern = (EventOpcode)75;
            public const EventOpcode Missed = (EventOpcode)76;
            public const EventOpcode Faulted = (EventOpcode)77;
            public const EventOpcode Reconnect = (EventOpcode)78;
            public const EventOpcode SequenceAck = (EventOpcode)79;
            public const EventOpcode AbortingChannel = (EventOpcode)80;
            public const EventOpcode CloseFailed = (EventOpcode)81;
            public const EventOpcode ConfigurationApplied = (EventOpcode)82;
            public const EventOpcode DuplexCallbackException = (EventOpcode)83;
            public const EventOpcode HandledException = (EventOpcode)84;
            public const EventOpcode TransmitFailed = (EventOpcode)85;
            public const EventOpcode ChannelFaulted = (EventOpcode)86;
            public const EventOpcode Closing = (EventOpcode)87;
            public const EventOpcode CreatingForEndpoint = (EventOpcode)88;
            public const EventOpcode CompletingOneWay = (EventOpcode)89;
            public const EventOpcode CompletingTwoWay = (EventOpcode)90;
            public const EventOpcode MovedToBackup = (EventOpcode)91;
            public const EventOpcode ProcessingFailure = (EventOpcode)92;
            public const EventOpcode ProcessingMessage = (EventOpcode)93;
            public const EventOpcode RoutedToEndpoints = (EventOpcode)94;
            public const EventOpcode SendingFaultResponse = (EventOpcode)95;
            public const EventOpcode SendingResponse = (EventOpcode)96;
            public const EventOpcode TransmitSucceeded = (EventOpcode)97;
            public const EventOpcode TransmittingMessage = (EventOpcode)98;
            public const EventOpcode Abandoning = (EventOpcode)99;
            public const EventOpcode Completing = (EventOpcode)100;
            public const EventOpcode CommittingTransaction = (EventOpcode)101;
            public const EventOpcode Creating = (EventOpcode)102;
            public const EventOpcode UsingExisting = (EventOpcode)103;
            public const EventOpcode Complete = (EventOpcode)104;
            public const EventOpcode CompletionRequested = (EventOpcode)105;
            public const EventOpcode Set = (EventOpcode)106;
            public const EventOpcode ScheduleBookmark = (EventOpcode)107;
            public const EventOpcode ScheduleCancelActivity = (EventOpcode)108;
            public const EventOpcode ScheduleCompletion = (EventOpcode)109;
            public const EventOpcode ScheduleExecuteActivity = (EventOpcode)110;
            public const EventOpcode ScheduleFault = (EventOpcode)111;
            public const EventOpcode ScheduleRuntime = (EventOpcode)112;
            public const EventOpcode ScheduleTransactionContext = (EventOpcode)113;
            public const EventOpcode Accept = (EventOpcode)114;
            public const EventOpcode Initiate = (EventOpcode)115;
            public const EventOpcode StartBookmark = (EventOpcode)117;
            public const EventOpcode StartCancelActivity = (EventOpcode)118;
            public const EventOpcode StartCompletion = (EventOpcode)119;
            public const EventOpcode StartExecuteActivity = (EventOpcode)120;
            public const EventOpcode StartFault = (EventOpcode)121;
            public const EventOpcode StartRuntime = (EventOpcode)122;
            public const EventOpcode StartTransactionContext = (EventOpcode)123;
            public const EventOpcode NotFound = (EventOpcode)124;
            public const EventOpcode Dropped = (EventOpcode)125;
            public const EventOpcode Raised = (EventOpcode)126;
            public const EventOpcode Truncated = (EventOpcode)127;
            public const EventOpcode BeforeAuthentication = (EventOpcode)128;
            public const EventOpcode DuringCancelation = (EventOpcode)129;
            public const EventOpcode FromCatchOrFinally = (EventOpcode)130;
            public const EventOpcode FromTry = (EventOpcode)131;
            public const EventOpcode Connected = (EventOpcode)132;
            public const EventOpcode Disconnect = (EventOpcode)133;
            public const EventOpcode Completed = (EventOpcode)134;
            public const EventOpcode Idled = (EventOpcode)135;
            public const EventOpcode InstanceAborted = (EventOpcode)136;
            public const EventOpcode InstanceCanceled = (EventOpcode)137;
            public const EventOpcode PersistableIdle = (EventOpcode)138;
            public const EventOpcode Persisted = (EventOpcode)139;
            public const EventOpcode Terminated = (EventOpcode)140;
            public const EventOpcode UnhandledException = (EventOpcode)141;
            public const EventOpcode Unloaded = (EventOpcode)142;
            public const EventOpcode suspend = (EventOpcode)143;
            public const EventOpcode AbortedRecord = (EventOpcode)144;
            public const EventOpcode AbortedWithId = (EventOpcode)145;
            public const EventOpcode SuspendedRecord = (EventOpcode)146;
            public const EventOpcode SuspendedWithId = (EventOpcode)147;
            public const EventOpcode TerminatedRecord = (EventOpcode)148;
            public const EventOpcode TerminatedWithId = (EventOpcode)149;
            public const EventOpcode UnhandledExceptionRecord = (EventOpcode)150;
            public const EventOpcode UnhandledExceptionWithId = (EventOpcode)151;
            public const EventOpcode UpdatedRecord = (EventOpcode)152;
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
