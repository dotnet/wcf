using System.Diagnostics.Tracing;

namespace System.Runtime
{
    [EventSource(Name = "Microsoft-Windows-Application Server-Applications", Guid = "c651f5f6-1c0d-492e-8ae1-b4efd7c9d503")]
    sealed class WcfEventSource : EventSource
    {
        public static WcfEventSource Instance = new WcfEventSource();

        public bool WorkflowInstanceRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(100, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord= WorkflowInstanceRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, State = {4}, Annotations = {5}, ProfileName = {6}")]
        public void WorkflowInstanceRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string State, string Annotations, string ProfileName, string HostReference)
        {
            WriteEvent(100, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, State, Annotations, ProfileName, HostReference);
        }

        public bool WorkflowInstanceUnhandledExceptionRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(101, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = Opcodes.UnhandledExceptionRecord, Task = Tasks.WorkflowInstanceRecord,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = WorkflowInstanceUnhandledExceptionRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, SourceName = {4}, SourceId = {5}, SourceInstanceId = {6}, SourceTypeName={7}, Exception={8}, Annotations= {9}, ProfileName = {10}")]
        public void WorkflowInstanceUnhandledExceptionRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string SourceName, string SourceId, string SourceInstanceId, string SourceTypeName, string Exception, string Annotations, string ProfileName, string HostReference)
        {
            WriteEvent(101, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, SourceName, SourceId, SourceInstanceId, SourceTypeName, Exception, Annotations, ProfileName, HostReference);
        }

        public bool WorkflowInstanceAbortedRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(102, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = Opcodes.AbortedRecord, Task = Tasks.WorkflowInstanceRecord,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = WorkflowInstanceAbortedRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, Reason = {4}, Annotations = {5}, ProfileName = {6}")]
        public void WorkflowInstanceAbortedRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string Reason, string Annotations, string ProfileName, string HostReference)
        {
            WriteEvent(102, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, Reason, Annotations, ProfileName, HostReference);
        }

        public bool ActivityStateRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(103, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = ActivityStateRecord, InstanceID = {0}, RecordNumber={1}, EventTime={2}, State = {3}, Name={4}, ActivityId={5}, ActivityInstanceId={6}, ActivityTypeName={7}, Arguments={8}, Variables={9}, Annotations={10}, ProfileName = {11}")]
        public void ActivityStateRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string State, string Name, string ActivityId, string ActivityInstanceId, string ActivityTypeName, string Arguments, string Variables, string Annotations, string ProfileName, string HostReference)
        {
            WriteEvent(103, InstanceId, RecordNumber, EventTime, State, Name, ActivityId, ActivityInstanceId, ActivityTypeName, Arguments, Variables, Annotations, ProfileName, HostReference);
        }

        public bool ActivityScheduledRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(104, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = ActivityScheduledRecord, InstanceID = {0},  RecordNumber = {1}, EventTime = {2}, Name = {3}, ActivityId = {4}, ActivityInstanceId = {5}, ActivityTypeName = {6}, ChildActivityName = {7}, ChildActivityId = {8}, ChildActivityInstanceId = {9}, ChildActivityTypeName ={10}, Annotations={11}, ProfileName = {12}")]
        public void ActivityScheduledRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string Name, string ActivityId, string ActivityInstanceId, string ActivityTypeName, string ChildActivityName, string ChildActivityId, string ChildActivityInstanceId, string ChildActivityTypeName, string Annotations, string ProfileName, string HostReference)
        {
            WriteEvent(104, InstanceId, RecordNumber, EventTime, Name, ActivityId, ActivityInstanceId, ActivityTypeName, ChildActivityName, ChildActivityId, ChildActivityInstanceId, ChildActivityTypeName, Annotations, ProfileName, HostReference);
        }

        public bool FaultPropagationRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(105, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = FaultPropagationRecord, InstanceID={0}, RecordNumber={1}, EventTime={2}, FaultSourceActivityName={3}, FaultSourceActivityId={4}, FaultSourceActivityInstanceId={5}, FaultSourceActivityTypeName={6}, FaultHandlerActivityName={7},  FaultHandlerActivityId = {8}, FaultHandlerActivityInstanceId ={9}, FaultHandlerActivityTypeName={10}, Fault={11}, IsFaultSource={12}, Annotations={13}, ProfileName = {14}")]
        public void FaultPropagationRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string FaultSourceActivityName, string FaultSourceActivityId, string FaultSourceActivityInstanceId, string FaultSourceActivityTypeName, string FaultHandlerActivityName, string FaultHandlerActivityId, string FaultHandlerActivityInstanceId, string FaultHandlerActivityTypeName, string Fault, byte IsFaultSource, string Annotations, string ProfileName, string HostReference)
        {
            WriteEvent(105, InstanceId, RecordNumber, EventTime, FaultSourceActivityName, FaultSourceActivityId, FaultSourceActivityInstanceId, FaultSourceActivityTypeName, FaultHandlerActivityName, FaultHandlerActivityId, FaultHandlerActivityInstanceId, FaultHandlerActivityTypeName, Fault, IsFaultSource, Annotations, ProfileName, HostReference);
        }

        public bool CancelRequestedRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(106, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = CancelRequestedRecord, InstanceID={0}, RecordNumber={1}, EventTime={2}, Name={3}, ActivityId={4}, ActivityInstanceId={5}, ActivityTypeName = {6}, ChildActivityName = {7}, ChildActivityId = {8}, ChildActivityInstanceId = {9}, ChildActivityTypeName ={10}, Annotations={11}, ProfileName = {12}")]
        public void CancelRequestedRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string Name, string ActivityId, string ActivityInstanceId, string ActivityTypeName, string ChildActivityName, string ChildActivityId, string ChildActivityInstanceId, string ChildActivityTypeName, string Annotations, string ProfileName, string HostReference)
        {
            WriteEvent(106, InstanceId, RecordNumber, EventTime, Name, ActivityId, ActivityInstanceId, ActivityTypeName, ChildActivityName, ChildActivityId, ChildActivityInstanceId, ChildActivityTypeName, Annotations, ProfileName, HostReference);
        }

        public bool BookmarkResumptionRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(107, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = BookmarkResumptionRecord, InstanceID={0}, RecordNumber={1},EventTime={2}, Name={3}, SubInstanceID={4},  OwnerActivityName={5}, OwnerActivityId ={6}, OwnerActivityInstanceId={7}, OwnerActivityTypeName={8}, Annotations={9}, ProfileName = {10}")]
        public void BookmarkResumptionRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string Name, Guid SubInstanceID, string OwnerActivityName, string OwnerActivityId, string OwnerActivityInstanceId, string OwnerActivityTypeName, string Annotations, string ProfileName, string HostReference)
        {
            WriteEvent(107, InstanceId, RecordNumber, EventTime, Name, SubInstanceID, OwnerActivityName, OwnerActivityId, OwnerActivityInstanceId, OwnerActivityTypeName, Annotations, ProfileName, HostReference);
        }

        public bool CustomTrackingRecordInfoIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.UserEvents | Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(108, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.UserEvents | Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = CustomTrackingRecord, InstanceID = {0}, RecordNumber={1}, EventTime={2},  Name={3}, ActivityName={4}, ActivityId={5}, ActivityInstanceId={6}, ActivityTypeName={7}, Data={8}, Annotations={9}, ProfileName = {10}")]
        public void CustomTrackingRecordInfo(Guid InstanceId, long RecordNumber, DateTime EventTime, string Name, string ActivityName, string ActivityId, string ActivityInstanceId, string ActivityTypeName, string Data, string Annotations, string ProfileName, string HostReference)
        {
            WriteEvent(108, InstanceId, RecordNumber, EventTime, Name, ActivityName, ActivityId, ActivityInstanceId, ActivityTypeName, Data, Annotations, ProfileName, HostReference);
        }

        public bool CustomTrackingRecordWarningIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.UserEvents | Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(110, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.UserEvents | Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = CustomTrackingRecord, InstanceID = {0}, RecordNumber={1}, EventTime={2}, Name={3}, ActivityName={4}, ActivityId={5}, ActivityInstanceId={6}, ActivityTypeName={7}, Data={8}, Annotations={9}, ProfileName = {10}")]
        public void CustomTrackingRecordWarning(Guid InstanceId, long RecordNumber, DateTime EventTime, string Name, string ActivityName, string ActivityId, string ActivityInstanceId, string ActivityTypeName, string Data, string Annotations, string ProfileName, string HostReference)
        {
            WriteEvent(110, InstanceId, RecordNumber, EventTime, Name, ActivityName, ActivityId, ActivityInstanceId, ActivityTypeName, Data, Annotations, ProfileName, HostReference);
        }

        public bool CustomTrackingRecordErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.UserEvents | Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(111, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.UserEvents | Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = CustomTrackingRecord, InstanceID = {0}, RecordNumber={1}, EventTime={2}, Name={3}, ActivityName={4}, ActivityId={5}, ActivityInstanceId={6}, ActivityTypeName={7}, Data={8}, Annotations={9}, ProfileName = {10}")]
        public void CustomTrackingRecordError(Guid InstanceId, long RecordNumber, DateTime EventTime, string Name, string ActivityName, string ActivityId, string ActivityInstanceId, string ActivityTypeName, string Data, string Annotations, string ProfileName, string HostReference)
        {
            WriteEvent(111, InstanceId, RecordNumber, EventTime, Name, ActivityName, ActivityId, ActivityInstanceId, ActivityTypeName, Data, Annotations, ProfileName, HostReference);
        }

        public bool WorkflowInstanceSuspendedRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(112, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.SuspendedRecord, Task = Tasks.WorkflowInstanceRecord,
            Keywords = Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = WorkflowInstanceSuspendedRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, Reason = {4}, Annotations = {5}, ProfileName = {6}")]
        public void WorkflowInstanceSuspendedRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string Reason, string Annotations, string ProfileName, string HostReference)
        {
            WriteEvent(112, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, Reason, Annotations, ProfileName, HostReference);
        }

        public bool WorkflowInstanceTerminatedRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(113, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = Opcodes.TerminatedRecord, Task = Tasks.WorkflowInstanceRecord,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = WorkflowInstanceTerminatedRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, Reason = {4}, Annotations = {5}, ProfileName = {6}")]
        public void WorkflowInstanceTerminatedRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string Reason, string Annotations, string ProfileName, string HostReference)
        {
            WriteEvent(113, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, Reason, Annotations, ProfileName, HostReference);
        }

        public bool WorkflowInstanceRecordWithIdIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(114, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord= WorkflowInstanceRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, State = {4}, Annotations = {5}, ProfileName = {6}, WorkflowDefinitionIdentity = {7}")]
        public void WorkflowInstanceRecordWithId(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string State, string Annotations, string ProfileName, string WorkflowDefinitionIdentity, string HostReference)
        {
            WriteEvent(114, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, State, Annotations, ProfileName, WorkflowDefinitionIdentity, HostReference);
        }

        public bool WorkflowInstanceAbortedRecordWithIdIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(115, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = Opcodes.AbortedWithId, Task = Tasks.WorkflowInstanceRecord,
            Keywords = Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = WorkflowInstanceAbortedRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, Reason = {4},  Annotations = {5}, ProfileName = {6}, WorkflowDefinitionIdentity = {7}")]
        public void WorkflowInstanceAbortedRecordWithId(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string Reason, string Annotations, string ProfileName, string WorkflowDefinitionIdentity, string HostReference)
        {
            WriteEvent(115, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, Reason, Annotations, ProfileName, WorkflowDefinitionIdentity, HostReference);
        }

        public bool WorkflowInstanceSuspendedRecordWithIdIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(116, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.SuspendedWithId, Task = Tasks.WorkflowInstanceRecord,
            Keywords = Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = WorkflowInstanceSuspendedRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, Reason = {4}, Annotations = {5}, ProfileName = {6}, WorkflowDefinitionIdentity = {7}")]
        public void WorkflowInstanceSuspendedRecordWithId(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string Reason, string Annotations, string ProfileName, string WorkflowDefinitionIdentity, string HostReference)
        {
            WriteEvent(116, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, Reason, Annotations, ProfileName, WorkflowDefinitionIdentity, HostReference);
        }

        public bool WorkflowInstanceTerminatedRecordWithIdIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(117, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = Opcodes.TerminatedWithId, Task = Tasks.WorkflowInstanceRecord,
            Keywords = Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = WorkflowInstanceTerminatedRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, Reason = {4},  Annotations = {5}, ProfileName = {6}, WorkflowDefinitionIdentity = {7}")]
        public void WorkflowInstanceTerminatedRecordWithId(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string Reason, string Annotations, string ProfileName, string WorkflowDefinitionIdentity, string HostReference)
        {
            WriteEvent(117, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, Reason, Annotations, ProfileName, WorkflowDefinitionIdentity, HostReference);
        }

        public bool WorkflowInstanceUnhandledExceptionRecordWithIdIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(118, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = Opcodes.UnhandledExceptionWithId, Task = Tasks.WorkflowInstanceRecord,
            Keywords = Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = WorkflowInstanceUnhandledExceptionRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, SourceName = {4}, SourceId = {5}, SourceInstanceId = {6}, SourceTypeName={7}, Exception={8},  Annotations= {9}, ProfileName = {10}, WorkflowDefinitionIdentity = {11}")]
        public void WorkflowInstanceUnhandledExceptionRecordWithId(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string SourceName, string SourceId, string SourceInstanceId, string SourceTypeName, string Exception, string Annotations, string ProfileName, string WorkflowDefinitionIdentity, string HostReference)
        {
            WriteEvent(118, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, SourceName, SourceId, SourceInstanceId, SourceTypeName, Exception, Annotations, ProfileName, WorkflowDefinitionIdentity, HostReference);
        }

        public bool WorkflowInstanceUpdatedRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(119, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.UpdatedRecord, Task = Tasks.WorkflowInstanceRecord,
            Keywords = Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord= WorkflowInstanceUpdatedRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, State = {4}, OriginalDefinitionIdentity = {5}, UpdatedDefinitionIdentity = {6}, Annotations = {7}, ProfileName = {8}")]
        public void WorkflowInstanceUpdatedRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string State, string OriginalDefinitionIdentity, string UpdatedDefinitionIdentity, string Annotations, string ProfileName, string HostReference)
        {
            WriteEvent(119, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, State, OriginalDefinitionIdentity, UpdatedDefinitionIdentity, Annotations, ProfileName, HostReference);
        }

        public bool BufferPoolAllocationIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(131, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.Allocate, Task = Tasks.BufferPooling,
            Keywords = Keywords.Infrastructure,
            Message = "Pool allocating {0} Bytes.")]
        public void BufferPoolAllocation(int Size)
        {
            WriteEvent(131, Size);
        }

        public bool BufferPoolChangeQuotaIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(132, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.Tune, Task = Tasks.BufferPooling,
            Keywords = Keywords.Infrastructure,
            Message = "BufferPool of size {0}, changing quota by {1}.")]
        public void BufferPoolChangeQuota(int PoolSize, int Delta)
        {
            WriteEvent(132, PoolSize, Delta);
        }

        public bool ActionItemScheduledIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Threading, EventChannel.Debug);
        }

        [Event(133, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Threading,
            Message = "IO Thread scheduler callback invoked.")]
        public void ActionItemScheduled()
        {
            WriteEvent(133);
        }

        public bool ActionItemCallbackInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Threading, EventChannel.Debug);
        }

        [Event(134, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Threading,
            Message = "IO Thread scheduler callback invoked.")]
        public void ActionItemCallbackInvoked()
        {
            WriteEvent(134);
        }

        public bool ClientMessageInspectorAfterReceiveInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(201, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ClientMessageInspectorAfterReceiveInvoked, Task = Tasks.ClientRuntime,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'AfterReceiveReply' on a ClientMessageInspector of type '{0}'.")]
        public void ClientMessageInspectorAfterReceiveInvoked(string TypeName, string HostReference)
        {
            WriteEvent(201, TypeName, HostReference);
        }

        public bool ClientMessageInspectorBeforeSendInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(202, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.ClientMessageInspectorBeforeSendInvoked, Task = Tasks.ClientRuntime,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'BeforeSendRequest' on a ClientMessageInspector of type  '{0}'.")]
        public void ClientMessageInspectorBeforeSendInvoked(string TypeName, string HostReference)
        {
            WriteEvent(202, TypeName, HostReference);
        }

        public bool ClientParameterInspectorAfterCallInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(203, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.ClientParameterInspectorStop, Task = Tasks.ClientRuntime,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'AfterCall' on a ClientParameterInspector of type '{0}'.")]
        public void ClientParameterInspectorAfterCallInvoked(string TypeName, string HostReference)
        {
            WriteEvent(203, TypeName, HostReference);
        }

        public bool ClientParameterInspectorBeforeCallInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(204, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.ClientParameterInspectorStart, Task = Tasks.ClientRuntime,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'BeforeCall' on a ClientParameterInspector of type '{0}'.")]
        public void ClientParameterInspectorBeforeCallInvoked(string TypeName, string HostReference)
        {
            WriteEvent(204, TypeName, HostReference);
        }

        public bool OperationInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(205, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.OperationInvokerStart, Task = Tasks.DispatchMessage,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "An OperationInvoker invoked the '{0}' method. Caller information: '{1}'.")]
        public void OperationInvoked(string MethodName, string CallerInfo, string HostReference)
        {
            WriteEvent(205, MethodName, CallerInfo, HostReference);
        }

        public bool ErrorHandlerInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(206, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked an ErrorHandler of type  '{0}' with an exception of type '{2}'.  ErrorHandled == '{1}'.")]
        public void ErrorHandlerInvoked(string TypeName, byte Handled, string ExceptionTypeName, string HostReference)
        {
            WriteEvent(206, TypeName, Handled, ExceptionTypeName, HostReference);
        }

        public bool FaultProviderInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(207, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked a FaultProvider of type '{0}' with an exception of type '{1}'.")]
        public void FaultProviderInvoked(string TypeName, string ExceptionTypeName, string HostReference)
        {
            WriteEvent(207, TypeName, ExceptionTypeName, HostReference);
        }

        public bool MessageInspectorAfterReceiveInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(208, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.DispathMessageInspectorAfterReceiveInvoked, Task = Tasks.DispatchMessage,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'AfterReceiveReply' on a MessageInspector of type '{0}'.")]
        public void MessageInspectorAfterReceiveInvoked(string TypeName, string HostReference)
        {
            WriteEvent(208, TypeName, HostReference);
        }

        public bool MessageInspectorBeforeSendInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(209, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.DispathMessageInspectorBeforeSendInvoked, Task = Tasks.DispatchMessage,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'BeforeSendRequest' on a MessageInspector of type '{0}'.")]
        public void MessageInspectorBeforeSendInvoked(string TypeName, string HostReference)
        {
            WriteEvent(209, TypeName, HostReference);
        }

        public bool MessageThrottleExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.Quota | Keywords.HealthMonitoring | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(210, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.Quota | Keywords.HealthMonitoring | Keywords.ServiceModel,
            Message = "The '{0}' throttle limit of '{1}' was hit.")]
        public void MessageThrottleExceeded(string ThrottleName, long Limit, string HostReference)
        {
            WriteEvent(210, ThrottleName, Limit, HostReference);
        }

        public bool ParameterInspectorAfterCallInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(211, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.ParameterInspectorStop, Task = Tasks.DispatchMessage,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'AfterCall' on a ParameterInspector of type '{0}'.")]
        public void ParameterInspectorAfterCallInvoked(string TypeName, string HostReference)
        {
            WriteEvent(211, TypeName, HostReference);
        }

        public bool ParameterInspectorBeforeCallInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(212, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.ParameterInspectorStart, Task = Tasks.DispatchMessage,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'BeforeCall' on a ParameterInspector of type '{0}'.")]
        public void ParameterInspectorBeforeCallInvoked(string TypeName, string HostReference)
        {
            WriteEvent(212, TypeName, HostReference);
        }

        public bool ServiceHostStartedIsEnabled()
        {
            return base.IsEnabled(EventLevel.LogAlways, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceHost, EventChannel.Analytic);
        }

        [Event(213, Level = EventLevel.LogAlways, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceHost,
            Message = "ServiceHost started: '{0}'.")]
        public void ServiceHostStarted(string ServiceTypeName, string HostReference)
        {
            WriteEvent(213, ServiceTypeName, HostReference);
        }

        public bool OperationCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.HealthMonitoring | Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(214, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.OperationInvokerStop, Task = Tasks.DispatchMessage,
            Keywords = Keywords.HealthMonitoring | Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "An OperationInvoker completed the call to the '{0}' method.  The method call duration was '{1}' ms.")]
        public void OperationCompleted(string MethodName, long Duration, string HostReference)
        {
            WriteEvent(214, MethodName, Duration, HostReference);
        }

        public bool MessageReceivedByTransportIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.TransportGeneral, EventChannel.Analytic);
        }

        [Event(215, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Troubleshooting | Keywords.TransportGeneral,
            Message = "The transport received a message from '{0}'.")]
        public void MessageReceivedByTransport(string ListenAddress, string HostReference)
        {
            WriteEvent(215, ListenAddress, HostReference);
        }

        public bool MessageSentByTransportIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.TransportGeneral, EventChannel.Analytic);
        }

        [Event(216, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Troubleshooting | Keywords.TransportGeneral,
            Message = "The transport sent a message to '{0}'.")]
        public void MessageSentByTransport(string DestinationAddress, string HostReference)
        {
            WriteEvent(216, DestinationAddress, HostReference);
        }

        public bool ClientOperationPreparedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(217, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.OperationPrepared, Task = Tasks.ClientRuntime,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Client is executing Action '{0}' associated with the '{1}' contract. The message will be sent to '{2}'.")]
        public void ClientOperationPrepared(string Action, string ContractName, string Destination, string HostReference)
        {
            WriteEvent(217, Action, ContractName, Destination, HostReference);
        }

        public bool ServiceChannelCallStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(218, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Client completed executing Action '{0}' associated with the '{1}' contract. The message was sent to '{2}'.")]
        public void ServiceChannelCallStop(string Action, string ContractName, string Destination, string HostReference)
        {
            WriteEvent(218, Action, ContractName, Destination, HostReference);
        }

        public bool ServiceExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(219, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceModel,
            Message = "There was an unhandled exception of type '{1}' during message processing.  Full Exception Details: {0}.")]
        public void ServiceException(string ExceptionToString, string ExceptionTypeName, string HostReference)
        {
            WriteEvent(219, ExceptionToString, ExceptionTypeName, HostReference);
        }

        public bool MessageSentToTransportIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.TransportGeneral, EventChannel.Analytic);
        }

        [Event(220, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.TransportGeneral,
            Message = "The Dispatcher sent a message to the transport. Correlation ID == '{0}'.")]
        public void MessageSentToTransport(Guid CorrelationId, string HostReference)
        {
            WriteEvent(220, CorrelationId, HostReference);
        }

        public bool MessageReceivedFromTransportIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.TransportGeneral, EventChannel.Analytic);
        }

        [Event(221, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.TransportGeneral,
            Message = "The Dispatcher received a message from the transport. Correlation ID == '{0}'.")]
        public void MessageReceivedFromTransport(Guid CorrelationId, string HostReference)
        {
            WriteEvent(221, CorrelationId, HostReference);
        }

        public bool OperationFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(222, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceModel,
            Message = "The '{0}' method threw an unhandled exception when invoked by the OperationInvoker. The method call duration was '{1}' ms.")]
        public void OperationFailed(string MethodName, long Duration, string HostReference)
        {
            WriteEvent(222, MethodName, Duration, HostReference);
        }

        public bool OperationFaultedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(223, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceModel,
            Message = "The '{0}' method threw a FaultException when invoked by the OperationInvoker. The method call duration was '{1}' ms.")]
        public void OperationFaulted(string MethodName, long Duration, string HostReference)
        {
            WriteEvent(223, MethodName, Duration, HostReference);
        }

        public bool MessageThrottleAtSeventyPercentIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.Quota | Keywords.HealthMonitoring | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(224, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.Quota | Keywords.HealthMonitoring | Keywords.ServiceModel,
            Message = "The '{0}' throttle limit of '{1}' is at 70%%.")]
        public void MessageThrottleAtSeventyPercent(string ThrottleName, long Limit, string HostReference)
        {
            WriteEvent(224, ThrottleName, Limit, HostReference);
        }

        public bool TraceCorrelationKeysIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(225, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.WFServices,
            Message = "Calculated correlation key '{0}' using values '{1}' in parent scope '{2}'.")]
        public void TraceCorrelationKeys(Guid InstanceKey, string Values, string ParentScope, string HostReference)
        {
            WriteEvent(225, InstanceKey, Values, ParentScope, HostReference);
        }

        public bool IdleServicesClosedIsEnabled()
        {
            return base.IsEnabled(EventLevel.LogAlways, Keywords.HealthMonitoring | Keywords.WebHost, EventChannel.Analytic);
        }

        [Event(226, Level = EventLevel.LogAlways, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.HealthMonitoring | Keywords.WebHost,
            Message = "{0} idle services out of total {1} activated services closed.")]
        public void IdleServicesClosed(int ClosedCount, int TotalCount)
        {
            WriteEvent(226, ClosedCount, TotalCount);
        }

        public bool UserDefinedErrorOccurredIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.UserEvents | Keywords.ServiceModel | Keywords.EndToEndMonitoring, EventChannel.Analytic);
        }

        [Event(301, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.UserEvents | Keywords.ServiceModel | Keywords.EndToEndMonitoring,
            Message = "Name:'{0}', Reference:'{1}', Payload:{2}")]
        public void UserDefinedErrorOccurred(string Name, string HostReference, string Payload)
        {
            WriteEvent(301, Name, HostReference, Payload);
        }

        public bool UserDefinedWarningOccurredIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.UserEvents | Keywords.ServiceModel | Keywords.EndToEndMonitoring, EventChannel.Analytic);
        }

        [Event(302, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.UserEvents | Keywords.ServiceModel | Keywords.EndToEndMonitoring,
            Message = "Name:'{0}', Reference:'{1}', Payload:{2}")]
        public void UserDefinedWarningOccurred(string Name, string HostReference, string Payload)
        {
            WriteEvent(302, Name, HostReference, Payload);
        }

        public bool UserDefinedInformationEventOccuredIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.UserEvents | Keywords.ServiceModel | Keywords.EndToEndMonitoring, EventChannel.Analytic);
        }

        [Event(303, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.UserEvents | Keywords.ServiceModel | Keywords.EndToEndMonitoring,
            Message = "Name:'{0}', Reference:'{1}', Payload:{2}")]
        public void UserDefinedInformationEventOccured(string Name, string HostReference, string Payload)
        {
            WriteEvent(303, Name, HostReference, Payload);
        }

        public bool StopSignpostEventIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(401, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "Activity boundary.")]
        public void StopSignpostEvent(string ExtendedData)
        {
            WriteEvent(401, ExtendedData);
        }

        public bool StartSignpostEventIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(402, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "Activity boundary.")]
        public void StartSignpostEvent(string ExtendedData)
        {
            WriteEvent(402, ExtendedData);
        }

        public bool SuspendSignpostEventIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(403, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Suspend,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "Activity boundary.")]
        public void SuspendSignpostEvent(string ExtendedData)
        {
            WriteEvent(403, ExtendedData);
        }

        public bool ResumeSignpostEventIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(404, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Resume,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "Activity boundary.")]
        public void ResumeSignpostEvent(string ExtendedData)
        {
            WriteEvent(404, ExtendedData);
        }

        public bool StartSignpostEvent1IsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(440, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start,
            Keywords = Keywords.Troubleshooting | Keywords.WFServices,
            Message = "Activity boundary.")]
        public void StartSignpostEvent1(string ExtendedData)
        {
            WriteEvent(440, ExtendedData);
        }

        public bool StopSignpostEvent1IsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(441, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Troubleshooting | Keywords.WFServices,
            Message = "Activity boundary.")]
        public void StopSignpostEvent1(string ExtendedData)
        {
            WriteEvent(441, ExtendedData);
        }

        public bool MessageLogInfoIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.WCFMessageLogging, EventChannel.Analytic);
        }

        [Event(451, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.WCFMessageLogging,
            Message = "{0}")]
        public void MessageLogInfo(string data1)
        {
            WriteEvent(451, data1);
        }

        public bool MessageLogWarningIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Troubleshooting | Keywords.WCFMessageLogging, EventChannel.Analytic);
        }

        [Event(452, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.WCFMessageLogging,
            Message = "{0}")]
        public void MessageLogWarning(string data1)
        {
            WriteEvent(452, data1);
        }

        public bool TransferEmittedIsEnabled()
        {
            return base.IsEnabled(EventLevel.LogAlways, Keywords.Troubleshooting | Keywords.UserEvents | Keywords.EndToEndMonitoring | Keywords.ServiceModel | Keywords.WFTracking | Keywords.ServiceHost | Keywords.WCFMessageLogging, EventChannel.Analytic);
        }

        [Event(499, Level = EventLevel.LogAlways, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.UserEvents | Keywords.EndToEndMonitoring | Keywords.ServiceModel | Keywords.WFTracking | Keywords.ServiceHost | Keywords.WCFMessageLogging,
            Message = "Transfer event emitted.")]
        public void TransferEmitted(Guid newId)
        {
            WriteEvent(499, newId);
        }

        public bool CompilationStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(501, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.WebHost,
            Message = "Begin compilation")]
        public void CompilationStart()
        {
            WriteEvent(501);
        }

        public bool CompilationStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(502, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.WebHost,
            Message = "End compilation")]
        public void CompilationStop()
        {
            WriteEvent(502);
        }

        public bool ServiceHostFactoryCreationStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(503, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.WebHost,
            Message = "ServiceHostFactory begin creation")]
        public void ServiceHostFactoryCreationStart()
        {
            WriteEvent(503);
        }

        public bool ServiceHostFactoryCreationStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(504, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.WebHost,
            Message = "ServiceHostFactory end creation")]
        public void ServiceHostFactoryCreationStop()
        {
            WriteEvent(504);
        }

        public bool CreateServiceHostStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(505, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.WebHost,
            Message = "Begin CreateServiceHost")]
        public void CreateServiceHostStart()
        {
            WriteEvent(505);
        }

        public bool CreateServiceHostStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(506, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.WebHost,
            Message = "End CreateServiceHost")]
        public void CreateServiceHostStop()
        {
            WriteEvent(506);
        }

        public bool HostedTransportConfigurationManagerConfigInitStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(507, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.WebHost,
            Message = "HostedTransportConfigurationManager begin configuration initialization")]
        public void HostedTransportConfigurationManagerConfigInitStart()
        {
            WriteEvent(507);
        }

        public bool HostedTransportConfigurationManagerConfigInitStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(508, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.WebHost,
            Message = "HostedTransportConfigurationManager end configuration initialization")]
        public void HostedTransportConfigurationManagerConfigInitStop()
        {
            WriteEvent(508);
        }

        public bool ServiceHostOpenStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceHost, EventChannel.Analytic);
        }

        [Event(509, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start,
            Keywords = Keywords.ServiceHost,
            Message = "ServiceHost Open started.")]
        public void ServiceHostOpenStart()
        {
            WriteEvent(509);
        }

        public bool ServiceHostOpenStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceHost, EventChannel.Analytic);
        }

        [Event(510, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop,
            Keywords = Keywords.ServiceHost,
            Message = "ServiceHost Open completed.")]
        public void ServiceHostOpenStop()
        {
            WriteEvent(510);
        }

        public bool WebHostRequestStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(513, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.WebHost,
            Message = "Received request with virtual path '{1}' from the AppDomain '{0}'.")]
        public void WebHostRequestStart(string VirtualPath)
        {
            WriteEvent(513, VirtualPath);
        }

        public bool WebHostRequestStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(514, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.WebHost,
            Message = "WebHostRequest stop.")]
        public void WebHostRequestStop()
        {
            WriteEvent(514);
        }

        public bool CBAEntryReadIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, EventKeywords.None, EventChannel.Debug);
        }

        [Event(601, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = EventKeywords.None,
            Message = "Processed ServiceActivation Element Relative Address:'{0}', Normalized Relative Address '{1}' .")]
        public void CBAEntryRead(string RelativeAddress, string NormalizedAddress)
        {
            WriteEvent(601, RelativeAddress, NormalizedAddress);
        }

        public bool CBAMatchFoundIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, EventKeywords.None, EventChannel.Debug);
        }

        [Event(602, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = EventKeywords.None,
            Message = "Incoming request matches a ServiceActivation element with address '{0}'. ")]
        public void CBAMatchFound(string IncomingAddress)
        {
            WriteEvent(602, IncomingAddress);
        }

        public bool AspNetRoutingServiceIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(603, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.RoutingServices,
            Message = "Incoming request matches a WCF Service defined in Asp.Net route with address {0}.  ")]
        public void AspNetRoutingService(string IncomingAddress)
        {
            WriteEvent(603, IncomingAddress);
        }

        public bool AspNetRouteIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(604, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.RoutingServices,
            Message = "A new Asp.Net route '{0}' with serviceType '{1}' and serviceHostFactoryType '{2}' is added.")]
        public void AspNetRoute(string AspNetRoutePrefix, string ServiceType, string ServiceHostFactoryType)
        {
            WriteEvent(604, AspNetRoutePrefix, ServiceType, ServiceHostFactoryType);
        }

        public bool IncrementBusyCountIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(605, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WebHost,
            Message = "IncrementBusyCount called. Source : {0}")]
        public void IncrementBusyCount(string Data)
        {
            WriteEvent(605, Data);
        }

        public bool DecrementBusyCountIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(606, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.WebHost,
            Message = "DecrementBusyCount called. Source : {0}")]
        public void DecrementBusyCount(string Data)
        {
            WriteEvent(606, Data);
        }

        public bool ServiceChannelOpenStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(701, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start,
            Keywords = Keywords.ServiceModel,
            Message = "ServiceChannelOpen started.")]
        public void ServiceChannelOpenStart()
        {
            WriteEvent(701);
        }

        public bool ServiceChannelOpenStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(702, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop,
            Keywords = Keywords.ServiceModel,
            Message = "ServiceChannelOpen completed.")]
        public void ServiceChannelOpenStop()
        {
            WriteEvent(702);
        }

        public bool ServiceChannelCallStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(703, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start,
            Keywords = Keywords.ServiceModel,
            Message = "ServiceChannelCall started.")]
        public void ServiceChannelCallStart()
        {
            WriteEvent(703);
        }

        public bool ServiceChannelBeginCallStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(704, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start,
            Keywords = Keywords.ServiceModel,
            Message = "ServiceChannel asynchronous calls started.")]
        public void ServiceChannelBeginCallStart()
        {
            WriteEvent(704);
        }

        public bool HttpSendMessageStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(706, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.HTTP,
            Message = "Http Send Request Start.")]
        public void HttpSendMessageStart()
        {
            WriteEvent(706);
        }

        public bool HttpSendStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(707, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.HTTP,
            Message = "Http Send Request Stop.")]
        public void HttpSendStop()
        {
            WriteEvent(707);
        }

        public bool HttpMessageReceiveStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(708, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start,
            Keywords = Keywords.HTTP,
            Message = "Message received from http transport.")]
        public void HttpMessageReceiveStart()
        {
            WriteEvent(708);
        }

        public bool DispatchMessageStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(709, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.DispatchStart, Task = Tasks.DispatchMessage,
            Keywords = Keywords.ServiceModel,
            Message = "Message dispatching started.")]
        public void DispatchMessageStart(string HostReference)
        {
            WriteEvent(709, HostReference);
        }

        public bool HttpContextBeforeProcessAuthenticationIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(710, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.BeforeAuthentication, Task = Tasks.TransportReceive,
            Keywords = Keywords.ServiceModel,
            Message = "Start authentication for message dispatching")]
        public void HttpContextBeforeProcessAuthentication()
        {
            WriteEvent(710);
        }

        public bool DispatchMessageBeforeAuthorizationIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(711, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.BeforeAuthorization, Task = Tasks.DispatchMessage,
            Keywords = Keywords.ServiceModel,
            Message = "Start authorization for message dispatching")]
        public void DispatchMessageBeforeAuthorization()
        {
            WriteEvent(711);
        }

        public bool DispatchMessageStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(712, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.DispatchStop, Task = Tasks.DispatchMessage,
            Keywords = Keywords.ServiceModel,
            Message = "Message dispatching completed")]
        public void DispatchMessageStop()
        {
            WriteEvent(712);
        }

        public bool ClientChannelOpenStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(715, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.ClientChannelOpenStart, Task = Tasks.ClientRuntime,
            Keywords = Keywords.ServiceModel,
            Message = "ServiceChannel Open Start.")]
        public void ClientChannelOpenStart()
        {
            WriteEvent(715);
        }

        public bool ClientChannelOpenStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(716, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.ClientChannelOpenStop, Task = Tasks.ClientRuntime,
            Keywords = Keywords.ServiceModel,
            Message = "ServiceChannel Open Stop.")]
        public void ClientChannelOpenStop()
        {
            WriteEvent(716);
        }

        public bool HttpSendStreamedMessageStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(717, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start,
            Keywords = Keywords.HTTP,
            Message = "Http Send streamed message started.")]
        public void HttpSendStreamedMessageStart()
        {
            WriteEvent(717);
        }

        public bool WorkflowApplicationCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1001, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Completed, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' has completed in the Closed state.")]
        public void WorkflowApplicationCompleted(string data1)
        {
            WriteEvent(1001, data1);
        }

        public bool WorkflowApplicationTerminatedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1002, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Terminated, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowApplication Id: '{0}' was terminated. It has completed in the Faulted state with an exception.")]
        public void WorkflowApplicationTerminated(string data1, string SerializedException)
        {
            WriteEvent(1002, data1, SerializedException);
        }

        public bool WorkflowInstanceCanceledIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1003, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.InstanceCanceled, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' has completed in the Canceled state.")]
        public void WorkflowInstanceCanceled(string data1)
        {
            WriteEvent(1003, data1);
        }

        public bool WorkflowInstanceAbortedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1004, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.InstanceAborted, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' was aborted with an exception.")]
        public void WorkflowInstanceAborted(string data1, string SerializedException)
        {
            WriteEvent(1004, data1, SerializedException);
        }

        public bool WorkflowApplicationIdledIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1005, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Idled, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowApplication Id: '{0}' went idle.")]
        public void WorkflowApplicationIdled(string data1)
        {
            WriteEvent(1005, data1);
        }

        public bool WorkflowApplicationUnhandledExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1006, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = Opcodes.UnhandledException, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' has encountered an unhandled exception.  The exception originated from Activity '{1}', DisplayName: '{2}'.  The following action will be taken: {3}.")]
        public void WorkflowApplicationUnhandledException(string data1, string data2, string data3, string data4, string SerializedException)
        {
            WriteEvent(1006, data1, data2, data3, data4, SerializedException);
        }

        public bool WorkflowApplicationPersistedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1007, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Persisted, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowApplication Id: '{0}' was Persisted.")]
        public void WorkflowApplicationPersisted(string data1)
        {
            WriteEvent(1007, data1);
        }

        public bool WorkflowApplicationUnloadedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1008, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Unloaded, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' was Unloaded.")]
        public void WorkflowApplicationUnloaded(string data1)
        {
            WriteEvent(1008, data1);
        }

        public bool ActivityScheduledIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1009, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFRuntime,
            Message = "Parent Activity '{0}', DisplayName: '{1}', InstanceId: '{2}' scheduled child Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void ActivityScheduled(string data1, string data2, string data3, string data4, string data5, string data6)
        {
            WriteEvent(1009, data1, data2, data3, data4, data5, data6);
        }

        public bool ActivityCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1010, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFRuntime,
            Message = "Activity '{0}', DisplayName: '{1}', InstanceId: '{2}' has completed in the '{3}' state.")]
        public void ActivityCompleted(string data1, string data2, string data3, string data4)
        {
            WriteEvent(1010, data1, data2, data3, data4);
        }

        public bool ScheduleExecuteActivityWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1011, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleExecuteActivity, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "An ExecuteActivityWorkItem has been scheduled for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void ScheduleExecuteActivityWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(1011, data1, data2, data3);
        }

        public bool StartExecuteActivityWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1012, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.StartExecuteActivity, Task = Tasks.StartWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Starting execution of an ExecuteActivityWorkItem for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void StartExecuteActivityWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(1012, data1, data2, data3);
        }

        public bool CompleteExecuteActivityWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1013, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompleteExecuteActivity, Task = Tasks.CompleteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "An ExecuteActivityWorkItem has completed for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void CompleteExecuteActivityWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(1013, data1, data2, data3);
        }

        public bool ScheduleCompletionWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1014, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleCompletion, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A CompletionWorkItem has been scheduled for parent Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.  Completed Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void ScheduleCompletionWorkItem(string data1, string data2, string data3, string data4, string data5, string data6)
        {
            WriteEvent(1014, data1, data2, data3, data4, data5, data6);
        }

        public bool StartCompletionWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1015, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.StartCompletion, Task = Tasks.StartWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Starting execution of a CompletionWorkItem for parent Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'. Completed Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void StartCompletionWorkItem(string data1, string data2, string data3, string data4, string data5, string data6)
        {
            WriteEvent(1015, data1, data2, data3, data4, data5, data6);
        }

        public bool CompleteCompletionWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1016, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompleteCompletion, Task = Tasks.CompleteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A CompletionWorkItem has completed for parent Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'. Completed Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void CompleteCompletionWorkItem(string data1, string data2, string data3, string data4, string data5, string data6)
        {
            WriteEvent(1016, data1, data2, data3, data4, data5, data6);
        }

        public bool ScheduleCancelActivityWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1017, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleCancelActivity, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A CancelActivityWorkItem has been scheduled for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void ScheduleCancelActivityWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(1017, data1, data2, data3);
        }

        public bool StartCancelActivityWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1018, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.StartCancelActivity, Task = Tasks.StartWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Starting execution of a CancelActivityWorkItem for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void StartCancelActivityWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(1018, data1, data2, data3);
        }

        public bool CompleteCancelActivityWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1019, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompleteCancelActivity, Task = Tasks.CompleteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A CancelActivityWorkItem has completed for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void CompleteCancelActivityWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(1019, data1, data2, data3);
        }

        public bool CreateBookmarkIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1020, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFRuntime,
            Message = "A Bookmark has been created for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.  BookmarkName: {3}, BookmarkScope: {4}.")]
        public void CreateBookmark(string data1, string data2, string data3, string data4, string data5)
        {
            WriteEvent(1020, data1, data2, data3, data4, data5);
        }

        public bool ScheduleBookmarkWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1021, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleBookmark, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A BookmarkWorkItem has been scheduled for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.  BookmarkName: {3}, BookmarkScope: {4}.")]
        public void ScheduleBookmarkWorkItem(string data1, string data2, string data3, string data4, string data5)
        {
            WriteEvent(1021, data1, data2, data3, data4, data5);
        }

        public bool StartBookmarkWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1022, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.StartBookmark, Task = Tasks.StartWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Starting execution of a BookmarkWorkItem for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.  BookmarkName: {3}, BookmarkScope: {4}.")]
        public void StartBookmarkWorkItem(string data1, string data2, string data3, string data4, string data5)
        {
            WriteEvent(1022, data1, data2, data3, data4, data5);
        }

        public bool CompleteBookmarkWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1023, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompleteBookmark, Task = Tasks.CompleteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A BookmarkWorkItem has completed for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'. BookmarkName: {3}, BookmarkScope: {4}.")]
        public void CompleteBookmarkWorkItem(string data1, string data2, string data3, string data4, string data5)
        {
            WriteEvent(1023, data1, data2, data3, data4, data5);
        }

        public bool CreateBookmarkScopeIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1024, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFRuntime,
            Message = "A BookmarkScope has been created: {0}.")]
        public void CreateBookmarkScope(string data1)
        {
            WriteEvent(1024, data1);
        }

        public bool BookmarkScopeInitializedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1025, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFRuntime,
            Message = "The BookmarkScope that had TemporaryId: '{0}' has been initialized with Id: '{1}'.")]
        public void BookmarkScopeInitialized(string data1, string data2)
        {
            WriteEvent(1025, data1, data2);
        }

        public bool ScheduleTransactionContextWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1026, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleTransactionContext, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A TransactionContextWorkItem has been scheduled for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void ScheduleTransactionContextWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(1026, data1, data2, data3);
        }

        public bool StartTransactionContextWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1027, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.StartTransactionContext, Task = Tasks.StartWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Starting execution of a TransactionContextWorkItem for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void StartTransactionContextWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(1027, data1, data2, data3);
        }

        public bool CompleteTransactionContextWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1028, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompleteTransactionContext, Task = Tasks.CompleteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A TransactionContextWorkItem has completed for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void CompleteTransactionContextWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(1028, data1, data2, data3);
        }

        public bool ScheduleFaultWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1029, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleFault, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A FaultWorkItem has been scheduled for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.  The exception was propagated from Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void ScheduleFaultWorkItem(string data1, string data2, string data3, string data4, string data5, string data6, string SerializedException)
        {
            WriteEvent(1029, data1, data2, data3, data4, data5, data6, SerializedException);
        }

        public bool StartFaultWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1030, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.StartFault, Task = Tasks.StartWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Starting execution of a FaultWorkItem for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.  The exception was propagated from Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void StartFaultWorkItem(string data1, string data2, string data3, string data4, string data5, string data6, string SerializedException)
        {
            WriteEvent(1030, data1, data2, data3, data4, data5, data6, SerializedException);
        }

        public bool CompleteFaultWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1031, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompleteFault, Task = Tasks.CompleteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A FaultWorkItem has completed for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'. The exception was propagated from Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void CompleteFaultWorkItem(string data1, string data2, string data3, string data4, string data5, string data6, string SerializedException)
        {
            WriteEvent(1031, data1, data2, data3, data4, data5, data6, SerializedException);
        }

        public bool ScheduleRuntimeWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1032, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleRuntime, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A runtime work item has been scheduled for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void ScheduleRuntimeWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(1032, data1, data2, data3);
        }

        public bool StartRuntimeWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1033, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.StartRuntime, Task = Tasks.StartWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Starting execution of a runtime work item for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void StartRuntimeWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(1033, data1, data2, data3);
        }

        public bool CompleteRuntimeWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1034, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompleteRuntime, Task = Tasks.CompleteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A runtime work item has completed for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void CompleteRuntimeWorkItem(string data1, string data2, string data3)
        {
            WriteEvent(1034, data1, data2, data3);
        }

        public bool RuntimeTransactionSetIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFServices, EventChannel.Debug);
        }

        [Event(1035, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.Set, Task = Tasks.RuntimeTransaction,
            Keywords = Keywords.WFServices,
            Message = "The runtime transaction has been set by Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.  Execution isolated to Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void RuntimeTransactionSet(string data1, string data2, string data3, string data4, string data5, string data6)
        {
            WriteEvent(1035, data1, data2, data3, data4, data5, data6);
        }

        public bool RuntimeTransactionCompletionRequestedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFServices, EventChannel.Debug);
        }

        [Event(1036, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompletionRequested, Task = Tasks.RuntimeTransaction,
            Keywords = Keywords.WFServices,
            Message = "Activity '{0}', DisplayName: '{1}', InstanceId: '{2}' has scheduled completion of the runtime transaction.")]
        public void RuntimeTransactionCompletionRequested(string data1, string data2, string data3)
        {
            WriteEvent(1036, data1, data2, data3);
        }

        public bool RuntimeTransactionCompleteIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFServices, EventChannel.Debug);
        }

        [Event(1037, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.Complete, Task = Tasks.RuntimeTransaction,
            Keywords = Keywords.WFServices,
            Message = "The runtime transaction has completed with the state '{0}'.")]
        public void RuntimeTransactionComplete(string data1)
        {
            WriteEvent(1037, data1);
        }

        public bool EnterNoPersistBlockIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1038, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFRuntime,
            Message = "Entering a no persist block.")]
        public void EnterNoPersistBlock()
        {
            WriteEvent(1038);
        }

        public bool ExitNoPersistBlockIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1039, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFRuntime,
            Message = "Exiting a no persist block.")]
        public void ExitNoPersistBlock()
        {
            WriteEvent(1039);
        }

        public bool InArgumentBoundIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(1040, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFActivities,
            Message = "In argument '{0}' on Activity '{1}', DisplayName: '{2}', InstanceId: '{3}' has been bound with value: {4}.")]
        public void InArgumentBound(string data1, string data2, string data3, string data4, string data5)
        {
            WriteEvent(1040, data1, data2, data3, data4, data5);
        }

        public bool WorkflowApplicationPersistableIdleIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1041, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.PersistableIdle, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowApplication Id: '{0}' is idle and persistable.  The following action will be taken: {1}.")]
        public void WorkflowApplicationPersistableIdle(string data1, string data2)
        {
            WriteEvent(1041, data1, data2);
        }

        public bool WorkflowActivityStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1101, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' E2E Activity")]
        public void WorkflowActivityStart(Guid Id)
        {
            WriteEvent(1101, Id);
        }

        public bool WorkflowActivityStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1102, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' E2E Activity")]
        public void WorkflowActivityStop(Guid Id)
        {
            WriteEvent(1102, Id);
        }

        public bool WorkflowActivitySuspendIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1103, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Suspend,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' E2E Activity")]
        public void WorkflowActivitySuspend(Guid Id)
        {
            WriteEvent(1103, Id);
        }

        public bool WorkflowActivityResumeIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1104, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Resume,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' E2E Activity")]
        public void WorkflowActivityResume(Guid Id)
        {
            WriteEvent(1104, Id);
        }

        public bool InvokeMethodIsStaticIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1124, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.IsStatic, Task = Tasks.InvokeMethod,
            Keywords = Keywords.WFRuntime,
            Message = "InvokeMethod '{0}' - method is Static.")]
        public void InvokeMethodIsStatic(string data1)
        {
            WriteEvent(1124, data1);
        }

        public bool InvokeMethodIsNotStaticIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1125, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.IsNotStatic, Task = Tasks.InvokeMethod,
            Keywords = Keywords.WFRuntime,
            Message = "InvokeMethod '{0}' - method is not Static.")]
        public void InvokeMethodIsNotStatic(string data1)
        {
            WriteEvent(1125, data1);
        }

        public bool InvokedMethodThrewExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1126, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ThrewException, Task = Tasks.InvokeMethod,
            Keywords = Keywords.WFRuntime,
            Message = "An exception was thrown in the method called by the activity '{0}'. {1}")]
        public void InvokedMethodThrewException(string data1, string data2)
        {
            WriteEvent(1126, data1, data2);
        }

        public bool InvokeMethodUseAsyncPatternIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1131, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.UseAsyncPattern, Task = Tasks.InvokeMethod,
            Keywords = Keywords.WFRuntime,
            Message = "InvokeMethod '{0}' - method uses asynchronous pattern of '{1}' and '{2}'.")]
        public void InvokeMethodUseAsyncPattern(string data1, string data2, string data3)
        {
            WriteEvent(1131, data1, data2, data3);
        }

        public bool InvokeMethodDoesNotUseAsyncPatternIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(1132, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.DoesNotUseAsyncPattern, Task = Tasks.InvokeMethod,
            Keywords = Keywords.WFRuntime,
            Message = "InvokeMethod '{0}' - method does not use asynchronous pattern.")]
        public void InvokeMethodDoesNotUseAsyncPattern(string data1)
        {
            WriteEvent(1132, data1);
        }

        public bool FlowchartStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(1140, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Begin, Task = Tasks.ExecuteFlowchart,
            Keywords = Keywords.WFActivities,
            Message = "Flowchart '{0}' - Start has been scheduled.")]
        public void FlowchartStart(string data1)
        {
            WriteEvent(1140, data1);
        }

        public bool FlowchartEmptyIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(1141, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.Empty, Task = Tasks.ExecuteFlowchart,
            Keywords = Keywords.WFActivities,
            Message = "Flowchart '{0}' - was executed with no Nodes.")]
        public void FlowchartEmpty(string data1)
        {
            WriteEvent(1141, data1);
        }

        public bool FlowchartNextNullIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(1143, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.NextNull, Task = Tasks.ExecuteFlowchart,
            Keywords = Keywords.WFActivities,
            Message = "Flowchart '{0}'/FlowStep - Next node is null. Flowchart execution will end.")]
        public void FlowchartNextNull(string data1)
        {
            WriteEvent(1143, data1);
        }

        public bool FlowchartSwitchCaseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(1146, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.SwitchCase, Task = Tasks.ExecuteFlowchart,
            Keywords = Keywords.WFActivities,
            Message = "Flowchart '{0}'/FlowSwitch - Case '{1}' was selected.")]
        public void FlowchartSwitchCase(string data1, string data2)
        {
            WriteEvent(1146, data1, data2);
        }

        public bool FlowchartSwitchDefaultIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(1147, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.SwitchDefault, Task = Tasks.ExecuteFlowchart,
            Keywords = Keywords.WFActivities,
            Message = "Flowchart '{0}'/FlowSwitch - Default Case was selected.")]
        public void FlowchartSwitchDefault(string data1)
        {
            WriteEvent(1147, data1);
        }

        public bool FlowchartSwitchCaseNotFoundIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(1148, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.SwitchCaseNotFound, Task = Tasks.ExecuteFlowchart,
            Keywords = Keywords.WFActivities,
            Message = "Flowchart '{0}'/FlowSwitch - could find neither a Case activity nor a Default Case matching the Expression result. Flowchart execution will end.")]
        public void FlowchartSwitchCaseNotFound(string data1)
        {
            WriteEvent(1148, data1);
        }

        public bool CompensationStateIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(1150, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFActivities,
            Message = "CompensableActivity '{0}' is in the '{1}' state.")]
        public void CompensationState(string data1, string data2)
        {
            WriteEvent(1150, data1, data2);
        }

        public bool SwitchCaseNotFoundIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(1223, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFActivities,
            Message = "The Switch activity '{0}' could not find a Case activity matching the Expression result.")]
        public void SwitchCaseNotFound(string data1)
        {
            WriteEvent(1223, data1);
        }

        public bool ChannelInitializationTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(1400, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ServiceModel,
            Message = "{0}")]
        public void ChannelInitializationTimeout(string data1)
        {
            WriteEvent(1400, data1);
        }

        public bool CloseTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(1401, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ServiceModel,
            Message = "{0}")]
        public void CloseTimeout(string data1)
        {
            WriteEvent(1401, data1);
        }

        public bool IdleTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(1402, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ServiceModel,
            Message = "{0} Connection pool key: {1}")]
        public void IdleTimeout(string msg, string key)
        {
            WriteEvent(1402, msg, key);
        }

        public bool LeaseTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(1403, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ServiceModel,
            Message = "{0} Connection pool key: {1}")]
        public void LeaseTimeout(string msg, string key)
        {
            WriteEvent(1403, msg, key);
        }

        public bool OpenTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(1405, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ServiceModel,
            Message = "{0}")]
        public void OpenTimeout(string data1)
        {
            WriteEvent(1405, data1);
        }

        public bool ReceiveTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(1406, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ServiceModel,
            Message = "{0}")]
        public void ReceiveTimeout(string data1)
        {
            WriteEvent(1406, data1);
        }

        public bool SendTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(1407, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ServiceModel,
            Message = "{0}")]
        public void SendTimeout(string data1)
        {
            WriteEvent(1407, data1);
        }

        public bool InactivityTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(1409, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ServiceModel,
            Message = "{0}")]
        public void InactivityTimeout(string data1)
        {
            WriteEvent(1409, data1);
        }

        public bool MaxReceivedMessageSizeExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(1416, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void MaxReceivedMessageSizeExceeded(string data1)
        {
            WriteEvent(1416, data1);
        }

        public bool MaxSentMessageSizeExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(1417, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void MaxSentMessageSizeExceeded(string data1)
        {
            WriteEvent(1417, data1);
        }

        public bool MaxOutboundConnectionsPerEndpointExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Quota, EventChannel.Debug);
        }

        [Event(1418, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void MaxOutboundConnectionsPerEndpointExceeded(string data1)
        {
            WriteEvent(1418, data1);
        }

        public bool MaxPendingConnectionsExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Quota, EventChannel.Debug);
        }

        [Event(1419, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void MaxPendingConnectionsExceeded(string data1)
        {
            WriteEvent(1419, data1);
        }

        public bool ReaderQuotaExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(1420, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void ReaderQuotaExceeded(string data1)
        {
            WriteEvent(1420, data1);
        }

        public bool NegotiateTokenAuthenticatorStateCacheExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(1422, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void NegotiateTokenAuthenticatorStateCacheExceeded(string msg)
        {
            WriteEvent(1422, msg);
        }

        public bool NegotiateTokenAuthenticatorStateCacheRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Debug);
        }

        [Event(1423, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "Negotiate token authenticator state cache ratio: {0}/{1}")]
        public void NegotiateTokenAuthenticatorStateCacheRatio(int cur, int max)
        {
            WriteEvent(1423, cur, max);
        }

        public bool SecuritySessionRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Debug);
        }

        [Event(1424, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "Security session ratio: {0}/{1}")]
        public void SecuritySessionRatio(int cur, int max)
        {
            WriteEvent(1424, cur, max);
        }

        public bool PendingConnectionsRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(1430, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "Pending connections ratio: {0}/{1}")]
        public void PendingConnectionsRatio(int cur, int max)
        {
            WriteEvent(1430, cur, max);
        }

        public bool ConcurrentCallsRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(1431, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "Concurrent calls ratio: {0}/{1}")]
        public void ConcurrentCallsRatio(int cur, int max)
        {
            WriteEvent(1431, cur, max);
        }

        public bool ConcurrentSessionsRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(1432, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "Concurrent sessions ratio: {0}/{1}")]
        public void ConcurrentSessionsRatio(int cur, int max)
        {
            WriteEvent(1432, cur, max);
        }

        public bool OutboundConnectionsPerEndpointRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(1433, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "Outbound connections per endpoint ratio: {0}/{1}")]
        public void OutboundConnectionsPerEndpointRatio(int cur, int max)
        {
            WriteEvent(1433, cur, max);
        }

        public bool PendingMessagesPerChannelRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(1436, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "Pending messages per channel ratio: {0}/{1}")]
        public void PendingMessagesPerChannelRatio(int cur, int max)
        {
            WriteEvent(1436, cur, max);
        }

        public bool ConcurrentInstancesRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(1438, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "Concurrent instances ratio: {0}/{1}")]
        public void ConcurrentInstancesRatio(int cur, int max)
        {
            WriteEvent(1438, cur, max);
        }

        public bool PendingAcceptsAtZeroIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Quota, EventChannel.Debug);
        }

        [Event(1439, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "Zero pending accepts left")]
        public void PendingAcceptsAtZero()
        {
            WriteEvent(1439);
        }

        public bool MaxSessionSizeReachedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(1441, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void MaxSessionSizeReached(string data1)
        {
            WriteEvent(1441, data1);
        }

        public bool ReceiveRetryCountReachedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(1442, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "Receive retry count reached on MSMQ message with id '{0}'")]
        public void ReceiveRetryCountReached(string data1)
        {
            WriteEvent(1442, data1);
        }

        public bool MaxRetryCyclesExceededMsmqIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(1443, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "Max retry cycles exceeded on MSMQ message with id '{0}'")]
        public void MaxRetryCyclesExceededMsmq(string data1)
        {
            WriteEvent(1443, data1);
        }

        public bool ReadPoolMissIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(1445, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "Created new '{0}'")]
        public void ReadPoolMiss(string itemTypeName)
        {
            WriteEvent(1445, itemTypeName);
        }

        public bool WritePoolMissIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(1446, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "Created new '{0}'")]
        public void WritePoolMiss(string itemTypeName)
        {
            WriteEvent(1446, itemTypeName);
        }

        public bool WfMessageReceivedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Debug);
        }

        [Event(1449, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Receive,
            Keywords = Keywords.WFServices,
            Message = "Message received by workflow")]
        public void WfMessageReceived()
        {
            WriteEvent(1449);
        }

        public bool WfMessageSentIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Debug);
        }

        [Event(1450, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Send,
            Keywords = Keywords.WFServices,
            Message = "Message sent from workflow")]
        public void WfMessageSent()
        {
            WriteEvent(1450);
        }

        public bool MaxRetryCyclesExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(1451, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void MaxRetryCyclesExceeded(string data1)
        {
            WriteEvent(1451, data1);
        }

        public bool ExecuteWorkItemStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(2021, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.WFRuntime,
            Message = "Execute work item start")]
        public void ExecuteWorkItemStart()
        {
            WriteEvent(2021);
        }

        public bool ExecuteWorkItemStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(2022, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.WFRuntime,
            Message = "Execute work item stop")]
        public void ExecuteWorkItemStop()
        {
            WriteEvent(2022);
        }

        public bool SendMessageChannelCacheMissIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(2023, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.Missed, Task = Tasks.MessageChannelCache,
            Keywords = Keywords.WFRuntime,
            Message = "SendMessageChannelCache miss")]
        public void SendMessageChannelCacheMiss()
        {
            WriteEvent(2023);
        }

        public bool InternalCacheMetadataStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(2024, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.WFRuntime,
            Message = "InternalCacheMetadata started on activity '{0}'.")]
        public void InternalCacheMetadataStart(string id)
        {
            WriteEvent(2024, id);
        }

        public bool InternalCacheMetadataStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(2025, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.WFRuntime,
            Message = "InternalCacheMetadata stopped on activity '{0}'.")]
        public void InternalCacheMetadataStop(string id)
        {
            WriteEvent(2025, id);
        }

        public bool CompileVbExpressionStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(2026, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.WFRuntime,
            Message = "Compiling VB expression '{0}'")]
        public void CompileVbExpressionStart(string expr)
        {
            WriteEvent(2026, expr);
        }

        public bool CacheRootMetadataStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(2027, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.WFRuntime,
            Message = "CacheRootMetadata started on activity '{0}'")]
        public void CacheRootMetadataStart(string activityName)
        {
            WriteEvent(2027, activityName);
        }

        public bool CacheRootMetadataStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(2028, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.WFRuntime,
            Message = "CacheRootMetadata stopped on activity {0}.")]
        public void CacheRootMetadataStop(string activityName)
        {
            WriteEvent(2028, activityName);
        }

        public bool CompileVbExpressionStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(2029, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.WFRuntime,
            Message = "Finished compiling VB expression.")]
        public void CompileVbExpressionStop()
        {
            WriteEvent(2029);
        }

        public bool TryCatchExceptionFromTryIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(2576, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.FromTry, Task = Tasks.TryCatchException,
            Keywords = Keywords.WFActivities,
            Message = "The TryCatch activity '{0}' has caught an exception of type '{1}'.")]
        public void TryCatchExceptionFromTry(string data1, string data2)
        {
            WriteEvent(2576, data1, data2);
        }

        public bool TryCatchExceptionDuringCancelationIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(2577, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.DuringCancelation, Task = Tasks.TryCatchException,
            Keywords = Keywords.WFActivities,
            Message = "A child activity of the TryCatch activity '{0}' has thrown an exception during cancelation.")]
        public void TryCatchExceptionDuringCancelation(string data1)
        {
            WriteEvent(2577, data1);
        }

        public bool TryCatchExceptionFromCatchOrFinallyIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(2578, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.FromCatchOrFinally, Task = Tasks.TryCatchException,
            Keywords = Keywords.WFActivities,
            Message = "A Catch or Finally activity that is associated with the TryCatch activity '{0}' has thrown an exception.")]
        public void TryCatchExceptionFromCatchOrFinally(string data1)
        {
            WriteEvent(2578, data1);
        }

        public bool ReceiveContextCompleteFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Channel, EventChannel.Analytic);
        }

        [Event(3300, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Channel,
            Message = "Failed to Complete {0}.")]
        public void ReceiveContextCompleteFailed(string TypeName)
        {
            WriteEvent(3300, TypeName);
        }

        public bool ReceiveContextAbandonFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3301, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Channel,
            Message = "Failed to Abandon {0}.")]
        public void ReceiveContextAbandonFailed(string TypeName)
        {
            WriteEvent(3301, TypeName);
        }

        public bool ReceiveContextFaultedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(3302, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ServiceModel,
            Message = "Receive Context faulted.")]
        public void ReceiveContextFaulted(string EventSource)
        {
            WriteEvent(3302, EventSource);
        }

        public bool ReceiveContextAbandonWithExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3303, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Channel,
            Message = "{0} was Abandoned with exception {1}.")]
        public void ReceiveContextAbandonWithException(string TypeName, string ExceptionToString)
        {
            WriteEvent(3303, TypeName, ExceptionToString);
        }

        public bool ClientBaseCachedChannelFactoryCountIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(3305, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.ServiceModel,
            Message = "Number of cached channel factories is: '{0}'.  At most '{1}' channel factories can be cached.")]
        public void ClientBaseCachedChannelFactoryCount(int Count, int MaxNum, string EventSource)
        {
            WriteEvent(3305, Count, MaxNum, EventSource);
        }

        public bool ClientBaseChannelFactoryAgedOutofCacheIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(3306, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.ServiceModel,
            Message = "A channel factory has been aged out of the cache because the cache has reached its limit of '{0}'.")]
        public void ClientBaseChannelFactoryAgedOutofCache(int Count, string EventSource)
        {
            WriteEvent(3306, Count, EventSource);
        }

        public bool ClientBaseChannelFactoryCacheHitIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(3307, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.ServiceModel,
            Message = "Used matching channel factory found in cache.")]
        public void ClientBaseChannelFactoryCacheHit(string EventSource)
        {
            WriteEvent(3307, EventSource);
        }

        public bool ClientBaseUsingLocalChannelFactoryIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(3308, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.ServiceModel,
            Message = "Not using channel factory from cache, i.e. caching disabled for instance.")]
        public void ClientBaseUsingLocalChannelFactory(string EventSource)
        {
            WriteEvent(3308, EventSource);
        }

        public bool QueryCompositionExecutedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(3309, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.ServiceModel,
            Message = "Query composition using '{0}' was executed on the Request Uri: '{1}'.")]
        public void QueryCompositionExecuted(string TypeName, string Uri, string EventSource)
        {
            WriteEvent(3309, TypeName, Uri, EventSource);
        }

        public bool DispatchFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(3310, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ServiceModel,
            Message = "The '{0}' operation was dispatched with errors.")]
        public void DispatchFailed(string OperationName, string HostReference)
        {
            WriteEvent(3310, OperationName, HostReference);
        }

        public bool DispatchSuccessfulIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(3311, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop,
            Keywords = Keywords.ServiceModel,
            Message = "The '{0}' operation was dispatched successfully.")]
        public void DispatchSuccessful(string OperationName, string HostReference)
        {
            WriteEvent(3311, OperationName, HostReference);
        }

        public bool MessageReadByEncoderIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3312, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Channel,
            Message = "A message with size '{0}' bytes was read by the encoder.")]
        public void MessageReadByEncoder(int Size, string EventSource)
        {
            WriteEvent(3312, Size, EventSource);
        }

        public bool MessageWrittenByEncoderIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3313, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Channel,
            Message = "A message with size '{0}' bytes was written by the encoder.")]
        public void MessageWrittenByEncoder(int Size, string EventSource)
        {
            WriteEvent(3313, Size, EventSource);
        }

        public bool SessionIdleTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(3314, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ServiceModel,
            Message = "Session aborting for idle channel to uri:'{0}'.")]
        public void SessionIdleTimeout(string RemoteAddress)
        {
            WriteEvent(3314, RemoteAddress);
        }

        public bool SocketAcceptEnqueuedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.TCP, EventChannel.Debug);
        }

        [Event(3319, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.TCP,
            Message = "Connection accept started.")]
        public void SocketAcceptEnqueued()
        {
            WriteEvent(3319);
        }

        public bool SocketAcceptedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.TCP, EventChannel.Debug);
        }

        [Event(3320, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.TCP,
            Message = "ListenerId:{0} accepted SocketId:{1}.")]
        public void SocketAccepted(int ListenerHashCode, int SocketHashCode)
        {
            WriteEvent(3320, ListenerHashCode, SocketHashCode);
        }

        public bool ConnectionPoolMissIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3321, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Channel,
            Message = "Pool for {0} has no available connection and {1} busy connections.")]
        public void ConnectionPoolMiss(string PoolKey, int busy)
        {
            WriteEvent(3321, PoolKey, busy);
        }

        public bool DispatchFormatterDeserializeRequestStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(3322, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.ServiceModel,
            Message = "Dispatcher started deserialization the request message.")]
        public void DispatchFormatterDeserializeRequestStart()
        {
            WriteEvent(3322);
        }

        public bool DispatchFormatterDeserializeRequestStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(3323, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.ServiceModel,
            Message = "Dispatcher completed deserialization the request message.")]
        public void DispatchFormatterDeserializeRequestStop()
        {
            WriteEvent(3323);
        }

        public bool DispatchFormatterSerializeReplyStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(3324, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.ServiceModel,
            Message = "Dispatcher started serialization of the reply message.")]
        public void DispatchFormatterSerializeReplyStart()
        {
            WriteEvent(3324);
        }

        public bool DispatchFormatterSerializeReplyStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(3325, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.ServiceModel,
            Message = "Dispatcher completed serialization of the reply message.")]
        public void DispatchFormatterSerializeReplyStop()
        {
            WriteEvent(3325);
        }

        public bool ClientFormatterSerializeRequestStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(3326, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.ServiceModel,
            Message = "Client request serialization started.")]
        public void ClientFormatterSerializeRequestStart()
        {
            WriteEvent(3326);
        }

        public bool ClientFormatterSerializeRequestStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(3327, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.ServiceModel,
            Message = "Client completed serialization of the request message.")]
        public void ClientFormatterSerializeRequestStop()
        {
            WriteEvent(3327);
        }

        public bool ClientFormatterDeserializeReplyStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(3328, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.ServiceModel,
            Message = "Client started deserializing the reply message.")]
        public void ClientFormatterDeserializeReplyStart()
        {
            WriteEvent(3328);
        }

        public bool ClientFormatterDeserializeReplyStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(3329, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.ServiceModel,
            Message = "Client completed deserializing the reply message.")]
        public void ClientFormatterDeserializeReplyStop()
        {
            WriteEvent(3329);
        }

        public bool SecurityNegotiationStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(3330, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Security,
            Message = "Security negotiation started.")]
        public void SecurityNegotiationStart()
        {
            WriteEvent(3330);
        }

        public bool SecurityNegotiationStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(3331, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Security,
            Message = "Security negotiation completed.")]
        public void SecurityNegotiationStop()
        {
            WriteEvent(3331);
        }

        public bool SecurityTokenProviderOpenedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(3332, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Security,
            Message = "SecurityTokenProvider opening completed.")]
        public void SecurityTokenProviderOpened()
        {
            WriteEvent(3332);
        }

        public bool OutgoingMessageSecuredIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(3333, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Security,
            Message = "Outgoing message has been secured.")]
        public void OutgoingMessageSecured()
        {
            WriteEvent(3333);
        }

        public bool IncomingMessageVerifiedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security | Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(3334, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security | Keywords.ServiceModel,
            Message = "Incoming message has been verified.")]
        public void IncomingMessageVerified()
        {
            WriteEvent(3334);
        }

        public bool GetServiceInstanceStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(3335, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.ServiceModel,
            Message = "Service instance retrieval started.")]
        public void GetServiceInstanceStart()
        {
            WriteEvent(3335);
        }

        public bool GetServiceInstanceStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(3336, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.ServiceModel,
            Message = "Service instance retrieved.")]
        public void GetServiceInstanceStop()
        {
            WriteEvent(3336);
        }

        public bool ChannelReceiveStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3337, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Channel,
            Message = "ChannelHandlerId:{0} - Message receive loop started.")]
        public void ChannelReceiveStart(int ChannelId)
        {
            WriteEvent(3337, ChannelId);
        }

        public bool ChannelReceiveStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3338, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Channel,
            Message = "ChannelHandlerId:{0} - Message receive loop stopped.")]
        public void ChannelReceiveStop(int ChannelId)
        {
            WriteEvent(3338, ChannelId);
        }

        public bool ChannelFactoryCreatedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(3339, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.ServiceModel,
            Message = "ChannelFactory created .")]
        public void ChannelFactoryCreated(string EventSource)
        {
            WriteEvent(3339, EventSource);
        }

        public bool PipeConnectionAcceptStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3340, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Channel,
            Message = "Pipe connection accept started on {0} .")]
        public void PipeConnectionAcceptStart(string uri)
        {
            WriteEvent(3340, uri);
        }

        public bool PipeConnectionAcceptStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3341, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Channel,
            Message = "Pipe connection accepted.")]
        public void PipeConnectionAcceptStop()
        {
            WriteEvent(3341);
        }

        public bool EstablishConnectionStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3342, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Channel,
            Message = "Connection establishment started for {0}.")]
        public void EstablishConnectionStart(string Key)
        {
            WriteEvent(3342, Key);
        }

        public bool EstablishConnectionStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3343, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Channel,
            Message = "Connection established.")]
        public void EstablishConnectionStop()
        {
            WriteEvent(3343);
        }

        public bool SessionPreambleUnderstoodIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3345, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Channel,
            Message = "Session preamble for '{0}' understood.")]
        public void SessionPreambleUnderstood(string Via)
        {
            WriteEvent(3345, Via);
        }

        public bool ConnectionReaderSendFaultIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3346, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Channel,
            Message = "Connection reader sending fault '{0}'. ")]
        public void ConnectionReaderSendFault(string FaultString)
        {
            WriteEvent(3346, FaultString);
        }

        public bool SocketAcceptClosedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.TCP, EventChannel.Debug);
        }

        [Event(3347, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.TCP,
            Message = "Socket accept closed.")]
        public void SocketAcceptClosed()
        {
            WriteEvent(3347);
        }

        public bool ServiceHostFaultedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Critical, Keywords.TCP, EventChannel.Analytic);
        }

        [Event(3348, Level = EventLevel.Critical, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.TCP,
            Message = "Service host faulted.")]
        public void ServiceHostFaulted(string EventSource)
        {
            WriteEvent(3348, EventSource);
        }

        public bool ListenerOpenStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3349, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Channel,
            Message = "Listener opening for '{0}'.")]
        public void ListenerOpenStart(string Uri)
        {
            WriteEvent(3349, Uri);
        }

        public bool ListenerOpenStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3350, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Channel,
            Message = "Listener open completed.")]
        public void ListenerOpenStop()
        {
            WriteEvent(3350);
        }

        public bool ServerMaxPooledConnectionsQuotaReachedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(3351, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "Server max pooled connections quota reached.")]
        public void ServerMaxPooledConnectionsQuotaReached()
        {
            WriteEvent(3351);
        }

        public bool TcpConnectionTimedOutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.TCP, EventChannel.Analytic);
        }

        [Event(3352, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.TCP,
            Message = "SocketId:{0} to remote address {1} timed out.")]
        public void TcpConnectionTimedOut(int SocketId, string Uri)
        {
            WriteEvent(3352, SocketId, Uri);
        }

        public bool TcpConnectionResetErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.TCP, EventChannel.Analytic);
        }

        [Event(3353, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.TCP,
            Message = "SocketId:{0} to remote address {1} had a connection reset error.")]
        public void TcpConnectionResetError(int SocketId, string Uri)
        {
            WriteEvent(3353, SocketId, Uri);
        }

        public bool ServiceSecurityNegotiationCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(3354, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "Service security negotiation completed.")]
        public void ServiceSecurityNegotiationCompleted()
        {
            WriteEvent(3354);
        }

        public bool SecurityNegotiationProcessingFailureIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Security, EventChannel.Analytic);
        }

        [Event(3355, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "Security negotiation processing failed.")]
        public void SecurityNegotiationProcessingFailure()
        {
            WriteEvent(3355);
        }

        public bool SecurityIdentityVerificationSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(3356, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "Security verification succeeded.")]
        public void SecurityIdentityVerificationSuccess()
        {
            WriteEvent(3356);
        }

        public bool SecurityIdentityVerificationFailureIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Security, EventChannel.Analytic);
        }

        [Event(3357, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "Security verification failed.")]
        public void SecurityIdentityVerificationFailure()
        {
            WriteEvent(3357);
        }

        public bool PortSharingDuplicatedSocketIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Debug);
        }

        [Event(3358, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.ActivationServices,
            Message = "Socket duplicated for {0}.")]
        public void PortSharingDuplicatedSocket(string Uri)
        {
            WriteEvent(3358, Uri);
        }

        public bool SecurityImpersonationSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(3359, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "Security impersonation succeeded.")]
        public void SecurityImpersonationSuccess()
        {
            WriteEvent(3359);
        }

        public bool SecurityImpersonationFailureIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Security, EventChannel.Analytic);
        }

        [Event(3360, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "Security impersonation failed.")]
        public void SecurityImpersonationFailure()
        {
            WriteEvent(3360);
        }

        public bool HttpChannelRequestAbortedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(3361, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.HTTP,
            Message = "Http channel request aborted.")]
        public void HttpChannelRequestAborted()
        {
            WriteEvent(3361);
        }

        public bool HttpChannelResponseAbortedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(3362, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.HTTP,
            Message = "Http channel response aborted.")]
        public void HttpChannelResponseAborted()
        {
            WriteEvent(3362);
        }

        public bool HttpAuthFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(3363, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.HTTP,
            Message = "Http authentication failed.")]
        public void HttpAuthFailed()
        {
            WriteEvent(3363);
        }

        public bool SharedListenerProxyRegisterStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(3364, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start,
            Keywords = Keywords.ActivationServices,
            Message = "SharedListenerProxy registration started for uri '{0}'.")]
        public void SharedListenerProxyRegisterStart(string Uri)
        {
            WriteEvent(3364, Uri);
        }

        public bool SharedListenerProxyRegisterStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(3365, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop,
            Keywords = Keywords.ActivationServices,
            Message = "SharedListenerProxy Register Stop.")]
        public void SharedListenerProxyRegisterStop()
        {
            WriteEvent(3365);
        }

        public bool SharedListenerProxyRegisterFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(3366, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ActivationServices,
            Message = "SharedListenerProxy register failed with status '{0}'.")]
        public void SharedListenerProxyRegisterFailed(string Status)
        {
            WriteEvent(3366, Status);
        }

        public bool ConnectionPoolPreambleFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Channel, EventChannel.Analytic);
        }

        [Event(3367, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Channel,
            Message = "ConnectionPoolPreambleFailed.")]
        public void ConnectionPoolPreambleFailed()
        {
            WriteEvent(3367);
        }

        public bool SslOnInitiateUpgradeIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Analytic);
        }

        [Event(3368, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.Initiate, Task = Tasks.SessionUpgrade,
            Keywords = Keywords.Security,
            Message = "SslOnAcceptUpgradeStart")]
        public void SslOnInitiateUpgrade()
        {
            WriteEvent(3368);
        }

        public bool SslOnAcceptUpgradeIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Analytic);
        }

        [Event(3369, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.Accept, Task = Tasks.SessionUpgrade,
            Keywords = Keywords.Security,
            Message = "SslOnAcceptUpgradeStop")]
        public void SslOnAcceptUpgrade()
        {
            WriteEvent(3369);
        }

        public bool BinaryMessageEncodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3370, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Channel,
            Message = "BinaryMessageEncoder started encoding the message.")]
        public void BinaryMessageEncodingStart()
        {
            WriteEvent(3370);
        }

        public bool MtomMessageEncodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3371, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Channel,
            Message = "MtomMessageEncoder started encoding the message.")]
        public void MtomMessageEncodingStart()
        {
            WriteEvent(3371);
        }

        public bool TextMessageEncodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3372, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Channel,
            Message = "TextMessageEncoder started encoding the message.")]
        public void TextMessageEncodingStart()
        {
            WriteEvent(3372);
        }

        public bool BinaryMessageDecodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3373, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Channel,
            Message = "BinaryMessageEncoder started decoding the message.")]
        public void BinaryMessageDecodingStart()
        {
            WriteEvent(3373);
        }

        public bool MtomMessageDecodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3374, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Channel,
            Message = "MtomMessageEncoder started decoding  the message.")]
        public void MtomMessageDecodingStart()
        {
            WriteEvent(3374);
        }

        public bool TextMessageDecodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3375, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Channel,
            Message = "TextMessageEncoder started decoding the message.")]
        public void TextMessageDecodingStart()
        {
            WriteEvent(3375);
        }

        public bool HttpResponseReceiveStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(3376, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.HTTP,
            Message = "Http transport started receiving a message.")]
        public void HttpResponseReceiveStart()
        {
            WriteEvent(3376);
        }

        public bool SocketReadStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.TCP, EventChannel.Debug);
        }

        [Event(3377, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.TCP,
            Message = "SocketId:{0} read '{1}' bytes read from '{2}'.")]
        public void SocketReadStop(int SocketId, int Size, string Endpoint)
        {
            WriteEvent(3377, SocketId, Size, Endpoint);
        }

        public bool SocketAsyncReadStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.TCP, EventChannel.Debug);
        }

        [Event(3378, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.TCP,
            Message = "SocketId:{0} read '{1}' bytes read from '{2}'.")]
        public void SocketAsyncReadStop(int SocketId, int Size, string Endpoint)
        {
            WriteEvent(3378, SocketId, Size, Endpoint);
        }

        public bool SocketWriteStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.TCP, EventChannel.Debug);
        }

        [Event(3379, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.TCP,
            Message = "SocketId:{0} writing '{1}' bytes to '{2}'.")]
        public void SocketWriteStart(int SocketId, int Size, string Endpoint)
        {
            WriteEvent(3379, SocketId, Size, Endpoint);
        }

        public bool SocketAsyncWriteStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.TCP, EventChannel.Debug);
        }

        [Event(3380, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.TCP,
            Message = "SocketId:{0} writing '{1}' bytes to '{2}'.")]
        public void SocketAsyncWriteStart(int SocketId, int Size, string Endpoint)
        {
            WriteEvent(3380, SocketId, Size, Endpoint);
        }

        public bool SequenceAcknowledgementSentIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3381, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.SequenceAck, Task = Tasks.ReliableSession,
            Keywords = Keywords.Channel,
            Message = "SessionId:{0} acknowledgement sent.")]
        public void SequenceAcknowledgementSent(string SessionId)
        {
            WriteEvent(3381, SessionId);
        }

        public bool ClientReliableSessionReconnectIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3382, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Reconnect, Task = Tasks.ReliableSession,
            Keywords = Keywords.Channel,
            Message = "SessionId:{0} reconnecting.")]
        public void ClientReliableSessionReconnect(string SessionId)
        {
            WriteEvent(3382, SessionId);
        }

        public bool ReliableSessionChannelFaultedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3383, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Faulted, Task = Tasks.ReliableSession,
            Keywords = Keywords.Channel,
            Message = "SessionId:{0} faulted.")]
        public void ReliableSessionChannelFaulted(string SessionId)
        {
            WriteEvent(3383, SessionId);
        }

        public bool WindowsStreamSecurityOnInitiateUpgradeIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Analytic);
        }

        [Event(3384, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.Initiate, Task = Tasks.SessionUpgrade,
            Keywords = Keywords.Security,
            Message = "WindowsStreamSecurity initiating security upgrade.")]
        public void WindowsStreamSecurityOnInitiateUpgrade()
        {
            WriteEvent(3384);
        }

        public bool WindowsStreamSecurityOnAcceptUpgradeIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Analytic);
        }

        [Event(3385, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.Accept, Task = Tasks.SessionUpgrade,
            Keywords = Keywords.Security,
            Message = "Windows streaming security on accepting upgrade.")]
        public void WindowsStreamSecurityOnAcceptUpgrade()
        {
            WriteEvent(3385);
        }

        public bool SocketConnectionAbortIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.TCP, EventChannel.Analytic);
        }

        [Event(3386, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.TCP,
            Message = "SocketId:{0} is aborting.")]
        public void SocketConnectionAbort(int SocketId)
        {
            WriteEvent(3386, SocketId);
        }

        public bool HttpGetContextStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(3388, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start,
            Keywords = Keywords.HTTP,
            Message = "HttpGetContext start.")]
        public void HttpGetContextStart()
        {
            WriteEvent(3388);
        }

        public bool ClientSendPreambleStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3389, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Channel,
            Message = "Client sending preamble start.")]
        public void ClientSendPreambleStart()
        {
            WriteEvent(3389);
        }

        public bool ClientSendPreambleStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3390, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Channel,
            Message = "Client sending preamble stop.")]
        public void ClientSendPreambleStop()
        {
            WriteEvent(3390);
        }

        public bool HttpMessageReceiveFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(3391, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start,
            Keywords = Keywords.HTTP,
            Message = "Http Message receive failed.")]
        public void HttpMessageReceiveFailed()
        {
            WriteEvent(3391);
        }

        public bool TransactionScopeCreateIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(3392, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.TransactionScopeCreate, Task = Tasks.DispatchMessage,
            Keywords = Keywords.ServiceModel,
            Message = "TransactionScope is being created with LocalIdentifier:'{0}' and DistributedIdentifier:'{1}'.")]
        public void TransactionScopeCreate(string LocalId, Guid Distributed)
        {
            WriteEvent(3392, LocalId, Distributed);
        }

        public bool StreamedMessageReadByEncoderIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3393, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Channel,
            Message = "A streamed message was read by the encoder.")]
        public void StreamedMessageReadByEncoder()
        {
            WriteEvent(3393);
        }

        public bool StreamedMessageWrittenByEncoderIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3394, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Channel,
            Message = "A streamed message was written by the encoder.")]
        public void StreamedMessageWrittenByEncoder()
        {
            WriteEvent(3394);
        }

        public bool MessageWrittenAsynchronouslyByEncoderIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3395, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Channel,
            Message = "A message was written asynchronously by the encoder.")]
        public void MessageWrittenAsynchronouslyByEncoder()
        {
            WriteEvent(3395);
        }

        public bool BufferedAsyncWriteStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3396, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Channel,
            Message = "BufferId:{0} completed writing '{1}' bytes to underlying stream.")]
        public void BufferedAsyncWriteStart(int BufferId, int Size)
        {
            WriteEvent(3396, BufferId, Size);
        }

        public bool BufferedAsyncWriteStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3397, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Channel,
            Message = "A message was written asynchronously by the encoder.")]
        public void BufferedAsyncWriteStop()
        {
            WriteEvent(3397);
        }

        public bool PipeSharedMemoryCreatedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3398, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Channel,
            Message = "Pipe shared memory created at '{0}' .")]
        public void PipeSharedMemoryCreated(string sharedMemoryName)
        {
            WriteEvent(3398, sharedMemoryName);
        }

        public bool NamedPipeCreatedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(3399, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Channel,
            Message = "NamedPipe '{0}' created.")]
        public void NamedPipeCreated(string pipeName)
        {
            WriteEvent(3399, pipeName);
        }

        public bool SignatureVerificationStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(3401, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Security,
            Message = "Signature verification started.")]
        public void SignatureVerificationStart()
        {
            WriteEvent(3401);
        }

        public bool SignatureVerificationSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(3402, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "Signature verification succeeded")]
        public void SignatureVerificationSuccess()
        {
            WriteEvent(3402);
        }

        public bool WrappedKeyDecryptionStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(3403, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "Wrapped key decryption started.")]
        public void WrappedKeyDecryptionStart()
        {
            WriteEvent(3403);
        }

        public bool WrappedKeyDecryptionSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(3404, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "Wrapped key decryption succeeded.")]
        public void WrappedKeyDecryptionSuccess()
        {
            WriteEvent(3404);
        }

        public bool EncryptedDataProcessingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(3405, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "Encrypted data processing started.")]
        public void EncryptedDataProcessingStart()
        {
            WriteEvent(3405);
        }

        public bool EncryptedDataProcessingSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(3406, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "Encrypted data processing succeeded.")]
        public void EncryptedDataProcessingSuccess()
        {
            WriteEvent(3406);
        }

        public bool HttpPipelineProcessInboundRequestStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(3407, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.HTTP,
            Message = "Http message handler started processing the inbound request.")]
        public void HttpPipelineProcessInboundRequestStart()
        {
            WriteEvent(3407);
        }

        public bool HttpPipelineBeginProcessInboundRequestStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(3408, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.HTTP,
            Message = "Http message handler started processing the inbound request asynchronously.")]
        public void HttpPipelineBeginProcessInboundRequestStart()
        {
            WriteEvent(3408);
        }

        public bool HttpPipelineProcessInboundRequestStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(3409, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.HTTP,
            Message = "Http message handler completed processing an inbound request.")]
        public void HttpPipelineProcessInboundRequestStop()
        {
            WriteEvent(3409);
        }

        public bool HttpPipelineFaultedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(3410, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.HTTP,
            Message = "Http message handler is faulted.")]
        public void HttpPipelineFaulted()
        {
            WriteEvent(3410);
        }

        public bool HttpPipelineTimeoutExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(3411, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.HTTP,
            Message = "WebSocket connection timed out.")]
        public void HttpPipelineTimeoutException()
        {
            WriteEvent(3411);
        }

        public bool HttpPipelineProcessResponseStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(3412, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.HTTP,
            Message = "Http message handler started processing the response.")]
        public void HttpPipelineProcessResponseStart()
        {
            WriteEvent(3412);
        }

        public bool HttpPipelineBeginProcessResponseStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(3413, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.HTTP,
            Message = "Http message handler started processing the response asynchronously.")]
        public void HttpPipelineBeginProcessResponseStart()
        {
            WriteEvent(3413);
        }

        public bool HttpPipelineProcessResponseStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(3414, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.HTTP,
            Message = "Http message handler completed processing the response.")]
        public void HttpPipelineProcessResponseStop()
        {
            WriteEvent(3414);
        }

        public bool WebSocketConnectionRequestSendStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(3415, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.HTTP,
            Message = "WebSocket connection request to '{0}' send start.")]
        public void WebSocketConnectionRequestSendStart(string remoteAddress)
        {
            WriteEvent(3415, remoteAddress);
        }

        public bool WebSocketConnectionRequestSendStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(3416, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} connection request sent.")]
        public void WebSocketConnectionRequestSendStop(int websocketId)
        {
            WriteEvent(3416, websocketId);
        }

        public bool WebSocketConnectionAcceptStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(3417, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.HTTP,
            Message = "WebSocket connection accept start.")]
        public void WebSocketConnectionAcceptStart()
        {
            WriteEvent(3417);
        }

        public bool WebSocketConnectionAcceptedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(3418, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} connection accepted.")]
        public void WebSocketConnectionAccepted(int websocketId)
        {
            WriteEvent(3418, websocketId);
        }

        public bool WebSocketConnectionDeclinedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(3419, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.HTTP,
            Message = "WebSocket connection declined with status code '{0}'")]
        public void WebSocketConnectionDeclined(string errorMessage)
        {
            WriteEvent(3419, errorMessage);
        }

        public bool WebSocketConnectionFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(3420, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.HTTP,
            Message = "WebSocket connection request failed: '{0}'")]
        public void WebSocketConnectionFailed(string errorMessage)
        {
            WriteEvent(3420, errorMessage);
        }

        public bool WebSocketConnectionAbortedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(3421, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} connection is aborted.")]
        public void WebSocketConnectionAborted(int websocketId)
        {
            WriteEvent(3421, websocketId);
        }

        public bool WebSocketAsyncWriteStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(3422, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} writing '{1}' bytes to '{2}'.")]
        public void WebSocketAsyncWriteStart(int websocketId, int byteCount, string remoteAddress)
        {
            WriteEvent(3422, websocketId, byteCount, remoteAddress);
        }

        public bool WebSocketAsyncWriteStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(3423, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} asynchronous write stop.")]
        public void WebSocketAsyncWriteStop(int websocketId)
        {
            WriteEvent(3423, websocketId);
        }

        public bool WebSocketAsyncReadStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(3424, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} read start.")]
        public void WebSocketAsyncReadStart(int websocketId)
        {
            WriteEvent(3424, websocketId);
        }

        public bool WebSocketAsyncReadStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(3425, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} read '{1}' bytes from '{2}'.")]
        public void WebSocketAsyncReadStop(int websocketId, int byteCount, string remoteAddress)
        {
            WriteEvent(3425, websocketId, byteCount, remoteAddress);
        }

        public bool WebSocketCloseSentIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(3426, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} sending close message to '{1}' with close status '{2}'.")]
        public void WebSocketCloseSent(int websocketId, string remoteAddress, string closeStatus)
        {
            WriteEvent(3426, websocketId, remoteAddress, closeStatus);
        }

        public bool WebSocketCloseOutputSentIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(3427, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} sending close output message to '{1}' with close status '{2}'.")]
        public void WebSocketCloseOutputSent(int websocketId, string remoteAddress, string closeStatus)
        {
            WriteEvent(3427, websocketId, remoteAddress, closeStatus);
        }

        public bool WebSocketConnectionClosedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(3428, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} connection closed.")]
        public void WebSocketConnectionClosed(int websocketId)
        {
            WriteEvent(3428, websocketId);
        }

        public bool WebSocketCloseStatusReceivedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(3429, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} connection close message received with status '{1}'.")]
        public void WebSocketCloseStatusReceived(int websocketId, string closeStatus)
        {
            WriteEvent(3429, websocketId, closeStatus);
        }

        public bool WebSocketUseVersionFromClientWebSocketFactoryIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(3430, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.HTTP,
            Message = "Using the WebSocketVersion from a client WebSocket factory of type '{0}'.")]
        public void WebSocketUseVersionFromClientWebSocketFactory(string clientWebSocketFactoryType)
        {
            WriteEvent(3430, clientWebSocketFactoryType);
        }

        public bool WebSocketCreateClientWebSocketWithFactoryIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(3431, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.HTTP,
            Message = "Creating the client WebSocket with a factory of type '{0}'.")]
        public void WebSocketCreateClientWebSocketWithFactory(string clientWebSocketFactoryType)
        {
            WriteEvent(3431, clientWebSocketFactoryType);
        }

        public bool InferredContractDescriptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(3501, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.Contract, Task = Tasks.InferDescription,
            Keywords = Keywords.WFServices,
            Message = "ContractDescription with Name='{0}' and Namespace='{1}' has been inferred from WorkflowService.")]
        public void InferredContractDescription(string data1, string data2)
        {
            WriteEvent(3501, data1, data2);
        }

        public bool InferredOperationDescriptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(3502, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.Operation, Task = Tasks.InferDescription,
            Keywords = Keywords.WFServices,
            Message = "OperationDescription with Name='{0}' in contract '{1}' has been inferred from WorkflowService. IsOneWay={2}.")]
        public void InferredOperationDescription(string data1, string data2, string data3)
        {
            WriteEvent(3502, data1, data2, data3);
        }

        public bool DuplicateCorrelationQueryIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(3503, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = Opcodes.DuplicateQuery, Task = Tasks.Correlation,
            Keywords = Keywords.WFServices,
            Message = "A duplicate CorrelationQuery was found with Where='{0}'. This duplicate query will not be used when calculating correlation.")]
        public void DuplicateCorrelationQuery(string data1)
        {
            WriteEvent(3503, data1);
        }

        public bool ServiceEndpointAddedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(3507, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFServices,
            Message = "A service endpoint has been added for address '{0}', binding '{1}', and contract '{2}'.")]
        public void ServiceEndpointAdded(string data1, string data2, string data3)
        {
            WriteEvent(3507, data1, data2, data3);
        }

        public bool TrackingProfileNotFoundIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(3508, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.NotFound, Task = Tasks.TrackingProfile,
            Keywords = Keywords.WFServices,
            Message = "TrackingProfile '{0}' for the ActivityDefinitionId '{1}' not found. Either the TrackingProfile is not found in the config file or the ActivityDefinitionId does not match.")]
        public void TrackingProfileNotFound(string TrackingProfile, string ActivityDefinitionId)
        {
            WriteEvent(3508, TrackingProfile, ActivityDefinitionId);
        }

        public bool BufferOutOfOrderMessageNoInstanceIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(3550, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.NoInstance, Task = Tasks.BufferOutOfOrder,
            Keywords = Keywords.WFServices,
            Message = "Operation '{0}' cannot be performed at this time. Another attempt will be made when the service instance is ready to process this particular operation.")]
        public void BufferOutOfOrderMessageNoInstance(string data1)
        {
            WriteEvent(3550, data1);
        }

        public bool BufferOutOfOrderMessageNoBookmarkIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(3551, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.NoBookmark, Task = Tasks.BufferOutOfOrder,
            Keywords = Keywords.WFServices,
            Message = "Operation '{1}' on service instance '{0}' cannot be performed at this time. Another attempt will be made when the service instance is ready to process this particular operation.")]
        public void BufferOutOfOrderMessageNoBookmark(string data1, string data2)
        {
            WriteEvent(3551, data1, data2);
        }

        public bool MaxPendingMessagesPerChannelExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Quota | Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(3552, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota | Keywords.WFServices,
            Message = "The throttle 'MaxPendingMessagesPerChannel' limit of  '{0}' was hit. To increase this limit, adjust the MaxPendingMessagesPerChannel property on BufferedReceiveServiceBehavior.")]
        public void MaxPendingMessagesPerChannelExceeded(int limit)
        {
            WriteEvent(3552, limit);
        }

        public bool XamlServicesLoadStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(3553, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.WebHost,
            Message = "XamlServicesLoad start")]
        public void XamlServicesLoadStart()
        {
            WriteEvent(3553);
        }

        public bool XamlServicesLoadStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(3554, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.WebHost,
            Message = "XamlServicesLoad Stop")]
        public void XamlServicesLoadStop()
        {
            WriteEvent(3554);
        }

        public bool CreateWorkflowServiceHostStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(3555, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.WebHost,
            Message = "CreateWorkflowServiceHost start")]
        public void CreateWorkflowServiceHostStart()
        {
            WriteEvent(3555);
        }

        public bool CreateWorkflowServiceHostStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(3556, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.WebHost,
            Message = "CreateWorkflowServiceHost Stop")]
        public void CreateWorkflowServiceHostStop()
        {
            WriteEvent(3556);
        }

        public bool TransactedReceiveScopeEndCommitFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(3557, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFServices,
            Message = "The call to EndCommit on the CommittableTransaction with id = '{0}' threw a TransactionException with the following message: '{1}'.")]
        public void TransactedReceiveScopeEndCommitFailed(string data1, string data2)
        {
            WriteEvent(3557, data1, data2);
        }

        public bool ServiceActivationStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Analytic);
        }

        [Event(3558, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start,
            Keywords = Keywords.WebHost,
            Message = "Service activation start")]
        public void ServiceActivationStart()
        {
            WriteEvent(3558);
        }

        public bool ServiceActivationStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Analytic);
        }

        [Event(3559, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop,
            Keywords = Keywords.WebHost,
            Message = "Service activation Stop")]
        public void ServiceActivationStop()
        {
            WriteEvent(3559);
        }

        public bool ServiceActivationAvailableMemoryIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(3560, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "Available memory (bytes): {0}")]
        public void ServiceActivationAvailableMemory(ulong availableMemoryBytes)
        {
            WriteEvent(3560, availableMemoryBytes);
        }

        public bool ServiceActivationExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WebHost, EventChannel.Operational);
        }

        [Event(3561, Level = EventLevel.Error, Channel = EventChannel.Operational, Opcode = EventOpcode.Info,
            Keywords = Keywords.WebHost,
            Message = "The service could not be activated. Exception details: {0}")]
        public void ServiceActivationException(string data1, string SerializedException)
        {
            WriteEvent(3561, data1, SerializedException);
        }

        public bool RoutingServiceClosingClientIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3800, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Closing, Task = Tasks.RoutingServiceClient,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is closing client '{0}'.")]
        public void RoutingServiceClosingClient(string data1)
        {
            WriteEvent(3800, data1);
        }

        public bool RoutingServiceChannelFaultedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3801, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.ChannelFaulted, Task = Tasks.RoutingServiceClient,
            Keywords = Keywords.RoutingServices,
            Message = "Routing Service client '{0}' has faulted.")]
        public void RoutingServiceChannelFaulted(string data1)
        {
            WriteEvent(3801, data1);
        }

        public bool RoutingServiceCompletingOneWayIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3802, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.CompletingOneWay, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "A Routing Service one way message is completing.")]
        public void RoutingServiceCompletingOneWay(string SerializedException)
        {
            WriteEvent(3802, SerializedException);
        }

        public bool RoutingServiceProcessingFailureIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3803, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = Opcodes.ProcessingFailure, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service failed while processing a message on the endpoint with address '{0}'.")]
        public void RoutingServiceProcessingFailure(string data1, string SerializedException)
        {
            WriteEvent(3803, data1, SerializedException);
        }

        public bool RoutingServiceCreatingClientForEndpointIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3804, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.CreatingForEndpoint, Task = Tasks.RoutingServiceClient,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is creating a client for endpoint: '{0}'.")]
        public void RoutingServiceCreatingClientForEndpoint(string data1)
        {
            WriteEvent(3804, data1);
        }

        public bool RoutingServiceDisplayConfigIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3805, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is configured with RouteOnHeadersOnly: {0}, SoapProcessingEnabled: {1}, EnsureOrderedDispatch: {2}.")]
        public void RoutingServiceDisplayConfig(string data1, string data2, string data3)
        {
            WriteEvent(3805, data1, data2, data3);
        }

        public bool RoutingServiceCompletingTwoWayIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3807, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.CompletingTwoWay, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "A Routing Service request reply message is completing.")]
        public void RoutingServiceCompletingTwoWay()
        {
            WriteEvent(3807);
        }

        public bool RoutingServiceMessageRoutedToEndpointsIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3809, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.RoutedToEndpoints, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service routed message with ID: '{0}' to {1} endpoint lists.")]
        public void RoutingServiceMessageRoutedToEndpoints(string data1, string data2)
        {
            WriteEvent(3809, data1, data2);
        }

        public bool RoutingServiceConfigurationAppliedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3810, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ConfigurationApplied, Task = Tasks.RoutingService,
            Keywords = Keywords.RoutingServices,
            Message = "A new RoutingConfiguration has been applied to the Routing Service.")]
        public void RoutingServiceConfigurationApplied()
        {
            WriteEvent(3810);
        }

        public bool RoutingServiceProcessingMessageIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3815, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ProcessingMessage, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is processing a message with ID: '{0}', Action: '{1}', Inbound URL: '{2}' Received in Transaction: {3}.")]
        public void RoutingServiceProcessingMessage(string data1, string data2, string data3, string data4)
        {
            WriteEvent(3815, data1, data2, data3, data4);
        }

        public bool RoutingServiceTransmittingMessageIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3816, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.TransmittingMessage, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is transmitting the message with ID: '{0}' [operation {1}] to '{2}'.")]
        public void RoutingServiceTransmittingMessage(string data1, string data2, string data3)
        {
            WriteEvent(3816, data1, data2, data3);
        }

        public bool RoutingServiceCommittingTransactionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3817, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.CommittingTransaction, Task = Tasks.RoutingServiceTransaction,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is committing a transaction with id: '{0}'.")]
        public void RoutingServiceCommittingTransaction(string data1)
        {
            WriteEvent(3817, data1);
        }

        public bool RoutingServiceDuplexCallbackExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3818, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = Opcodes.DuplexCallbackException, Task = Tasks.RoutingService,
            Keywords = Keywords.RoutingServices,
            Message = "Routing Service component {0} encountered a duplex callback exception.")]
        public void RoutingServiceDuplexCallbackException(string data1, string SerializedException)
        {
            WriteEvent(3818, data1, SerializedException);
        }

        public bool RoutingServiceMovedToBackupIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3819, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.MovedToBackup, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "Routing Service message with ID: '{0}' [operation {1}] moved to backup endpoint '{2}'.")]
        public void RoutingServiceMovedToBackup(string data1, string data2, string data3)
        {
            WriteEvent(3819, data1, data2, data3);
        }

        public bool RoutingServiceCreatingTransactionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3820, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Creating, Task = Tasks.RoutingServiceTransaction,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service created a new Transaction with id '{0}' for processing message(s).")]
        public void RoutingServiceCreatingTransaction(string data1)
        {
            WriteEvent(3820, data1);
        }

        public bool RoutingServiceCloseFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3821, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.CloseFailed, Task = Tasks.RoutingService,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service failed while closing outbound client '{0}'.")]
        public void RoutingServiceCloseFailed(string data1, string SerializedException)
        {
            WriteEvent(3821, data1, SerializedException);
        }

        public bool RoutingServiceSendingResponseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3822, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.SendingResponse, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is sending back a response message with Action '{0}'.")]
        public void RoutingServiceSendingResponse(string data1)
        {
            WriteEvent(3822, data1);
        }

        public bool RoutingServiceSendingFaultResponseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3823, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.SendingFaultResponse, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is sending back a Fault response message with Action '{0}'.")]
        public void RoutingServiceSendingFaultResponse(string data1)
        {
            WriteEvent(3823, data1);
        }

        public bool RoutingServiceCompletingReceiveContextIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3824, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.Completing, Task = Tasks.RoutingServiceReceiveContext,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is calling ReceiveContext.Complete for Message with ID: '{0}'.")]
        public void RoutingServiceCompletingReceiveContext(string data1)
        {
            WriteEvent(3824, data1);
        }

        public bool RoutingServiceAbandoningReceiveContextIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3825, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.Abandoning, Task = Tasks.RoutingServiceReceiveContext,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is calling ReceiveContext.Abandon for Message with ID: '{0}'.")]
        public void RoutingServiceAbandoningReceiveContext(string data1)
        {
            WriteEvent(3825, data1);
        }

        public bool RoutingServiceUsingExistingTransactionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3826, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.UsingExisting, Task = Tasks.RoutingServiceTransaction,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service will send messages using existing transaction '{0}'.")]
        public void RoutingServiceUsingExistingTransaction(string data1)
        {
            WriteEvent(3826, data1);
        }

        public bool RoutingServiceTransmitFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3827, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.TransmitFailed, Task = Tasks.RoutingService,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service failed while sending to '{0}'.")]
        public void RoutingServiceTransmitFailed(string data1, string SerializedException)
        {
            WriteEvent(3827, data1, SerializedException);
        }

        public bool RoutingServiceFilterTableMatchStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Analytic);
        }

        [Event(3828, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start,
            Keywords = Keywords.RoutingServices,
            Message = "Routing Service MessageFilterTable Match Start.")]
        public void RoutingServiceFilterTableMatchStart()
        {
            WriteEvent(3828);
        }

        public bool RoutingServiceFilterTableMatchStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Analytic);
        }

        [Event(3829, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop,
            Keywords = Keywords.RoutingServices,
            Message = "Routing Service MessageFilterTable Match Stop.")]
        public void RoutingServiceFilterTableMatchStop()
        {
            WriteEvent(3829);
        }

        public bool RoutingServiceAbortingChannelIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3830, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.AbortingChannel, Task = Tasks.RoutingService,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is calling abort on channel: '{0}'.")]
        public void RoutingServiceAbortingChannel(string data1)
        {
            WriteEvent(3830, data1);
        }

        public bool RoutingServiceHandledExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3831, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.HandledException, Task = Tasks.RoutingService,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service has handled an exception.")]
        public void RoutingServiceHandledException(string SerializedException)
        {
            WriteEvent(3831, SerializedException);
        }

        public bool RoutingServiceTransmitSucceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(3832, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.TransmitSucceeded, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service successfully transmitted Message with ID: '{0} [operation {1}] to '{2}'.")]
        public void RoutingServiceTransmitSucceeded(string data1, string data2, string data3)
        {
            WriteEvent(3832, data1, data2, data3);
        }

        public bool TransportListenerSessionsReceivedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4001, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Receive,
            Keywords = Keywords.ActivationServices,
            Message = "Transport listener session received with via '{0}'")]
        public void TransportListenerSessionsReceived(string via)
        {
            WriteEvent(4001, via);
        }

        public bool FailFastExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Critical, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4002, Level = EventLevel.Critical, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ActivationServices,
            Message = "FailFastException.")]
        public void FailFastException(string SerializedException)
        {
            WriteEvent(4002, SerializedException);
        }

        public bool ServiceStartPipeErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4003, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ActivationServices,
            Message = "Service start pipe error.")]
        public void ServiceStartPipeError(string Endpoint)
        {
            WriteEvent(4003, Endpoint);
        }

        public bool DispatchSessionStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4008, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start,
            Keywords = Keywords.ActivationServices,
            Message = "Session dispatch started.")]
        public void DispatchSessionStart()
        {
            WriteEvent(4008);
        }

        public bool PendingSessionQueueFullIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4010, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ActivationServices,
            Message = "Session dispatch for '{0}' failed since pending session queue is full with '{1}' pending items.")]
        public void PendingSessionQueueFull(string Uri, int count)
        {
            WriteEvent(4010, Uri, count);
        }

        public bool MessageQueueRegisterStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4011, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start,
            Keywords = Keywords.ActivationServices,
            Message = "Message queue register start.")]
        public void MessageQueueRegisterStart()
        {
            WriteEvent(4011);
        }

        public bool MessageQueueRegisterAbortIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4012, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ActivationServices,
            Message = "Message queue registration aborted with status:'{0}' for uri:'{1}'.")]
        public void MessageQueueRegisterAbort(string Status, string Uri)
        {
            WriteEvent(4012, Status, Uri);
        }

        public bool MessageQueueUnregisterSucceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4013, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop,
            Keywords = Keywords.ActivationServices,
            Message = "Message queue unregister succeeded for uri:'{0}'.")]
        public void MessageQueueUnregisterSucceeded(string Uri)
        {
            WriteEvent(4013, Uri);
        }

        public bool MessageQueueRegisterFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4014, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ActivationServices,
            Message = "Message queue registration for uri:'{0}' failed with status:'{1}'.")]
        public void MessageQueueRegisterFailed(string Uri, string Status)
        {
            WriteEvent(4014, Uri, Status);
        }

        public bool MessageQueueRegisterCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4015, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ActivationServices,
            Message = "Message queue registration completed for uri '{0}'.")]
        public void MessageQueueRegisterCompleted(string Uri)
        {
            WriteEvent(4015, Uri);
        }

        public bool MessageQueueDuplicatedSocketErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4016, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ActivationServices,
            Message = "Message queue failed duplicating socket.")]
        public void MessageQueueDuplicatedSocketError()
        {
            WriteEvent(4016);
        }

        public bool MessageQueueDuplicatedSocketCompleteIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4019, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop,
            Keywords = Keywords.ActivationServices,
            Message = "MessageQueueDuplicatedSocketComplete")]
        public void MessageQueueDuplicatedSocketComplete()
        {
            WriteEvent(4019);
        }

        public bool TcpTransportListenerListeningStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4020, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start,
            Keywords = Keywords.ActivationServices,
            Message = "Tcp transport listener starting to listen on uri:'{0}'.")]
        public void TcpTransportListenerListeningStart(string Uri)
        {
            WriteEvent(4020, Uri);
        }

        public bool TcpTransportListenerListeningStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4021, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop,
            Keywords = Keywords.ActivationServices,
            Message = "Tcp transport listener listening.")]
        public void TcpTransportListenerListeningStop()
        {
            WriteEvent(4021);
        }

        public bool WebhostUnregisterProtocolFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4022, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ActivationServices,
            Message = "Error Code:{0}")]
        public void WebhostUnregisterProtocolFailed(string hresult)
        {
            WriteEvent(4022, hresult);
        }

        public bool WasCloseAllListenerChannelInstancesCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4023, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop,
            Keywords = Keywords.ActivationServices,
            Message = "Was closing all listener channel instances completed.")]
        public void WasCloseAllListenerChannelInstancesCompleted()
        {
            WriteEvent(4023);
        }

        public bool WasCloseAllListenerChannelInstancesFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4024, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop,
            Keywords = Keywords.ActivationServices,
            Message = "Error Code:{0}")]
        public void WasCloseAllListenerChannelInstancesFailed(string hresult)
        {
            WriteEvent(4024, hresult);
        }

        public bool OpenListenerChannelInstanceFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4025, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ActivationServices,
            Message = "Error Code:{0}")]
        public void OpenListenerChannelInstanceFailed(string hresult)
        {
            WriteEvent(4025, hresult);
        }

        public bool WasConnectedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4026, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.Connected, Task = Tasks.WASActivation,
            Keywords = Keywords.ActivationServices,
            Message = "WAS Connected.")]
        public void WasConnected()
        {
            WriteEvent(4026);
        }

        public bool WasDisconnectedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4027, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.Disconnect, Task = Tasks.WASActivation,
            Keywords = Keywords.ActivationServices,
            Message = "WAS Disconnected.")]
        public void WasDisconnected()
        {
            WriteEvent(4027);
        }

        public bool PipeTransportListenerListeningStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4028, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start,
            Keywords = Keywords.ActivationServices,
            Message = "Pipe transport listener listening start on uri:{0}.")]
        public void PipeTransportListenerListeningStart(string Uri)
        {
            WriteEvent(4028, Uri);
        }

        public bool PipeTransportListenerListeningStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4029, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop,
            Keywords = Keywords.ActivationServices,
            Message = "Pipe transport listener listening stop.")]
        public void PipeTransportListenerListeningStop()
        {
            WriteEvent(4029);
        }

        public bool DispatchSessionSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4030, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop,
            Keywords = Keywords.ActivationServices,
            Message = "Session dispatch succeeded.")]
        public void DispatchSessionSuccess()
        {
            WriteEvent(4030);
        }

        public bool DispatchSessionFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4031, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ActivationServices,
            Message = "Session dispatch failed.")]
        public void DispatchSessionFailed()
        {
            WriteEvent(4031);
        }

        public bool WasConnectionTimedoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Critical, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(4032, Level = EventLevel.Critical, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.ActivationServices,
            Message = "WAS connection timed out.")]
        public void WasConnectionTimedout()
        {
            WriteEvent(4032);
        }

        public bool RoutingTableLookupStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Debug);
        }

        [Event(4033, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.ActivationServices,
            Message = "Routing table lookup started.")]
        public void RoutingTableLookupStart()
        {
            WriteEvent(4033);
        }

        public bool RoutingTableLookupStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Debug);
        }

        [Event(4034, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.ActivationServices,
            Message = "Routing table lookup completed.")]
        public void RoutingTableLookupStop()
        {
            WriteEvent(4034);
        }

        public bool PendingSessionQueueRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Debug);
        }

        [Event(4035, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota,
            Message = "Pending session queue ratio: {0}/{1}")]
        public void PendingSessionQueueRatio(int curr, int max)
        {
            WriteEvent(4035, curr, max);
        }

        public bool EndSqlCommandExecuteIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(4201, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.WFInstanceStore,
            Message = "End SQL command execution: {0}")]
        public void EndSqlCommandExecute(string data1)
        {
            WriteEvent(4201, data1);
        }

        public bool StartSqlCommandExecuteIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(4202, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.WFInstanceStore,
            Message = "Starting SQL command execution: {0}")]
        public void StartSqlCommandExecute(string data1)
        {
            WriteEvent(4202, data1);
        }

        public bool RenewLockSystemErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(4203, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFInstanceStore,
            Message = "Failed to extend lock expiration, lock expiration already passed or the lock owner was deleted. Aborting SqlWorkflowInstanceStore.")]
        public void RenewLockSystemError()
        {
            WriteEvent(4203);
        }

        public bool FoundProcessingErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(4205, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFInstanceStore,
            Message = "Command failed: {0}")]
        public void FoundProcessingError(string data1, string SerializedException)
        {
            WriteEvent(4205, data1, SerializedException);
        }

        public bool UnlockInstanceExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(4206, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFInstanceStore,
            Message = "Encountered exception {0} while attempting to unlock instance.")]
        public void UnlockInstanceException(string data1)
        {
            WriteEvent(4206, data1);
        }

        public bool MaximumRetriesExceededForSqlCommandIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Quota | Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(4207, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Quota | Keywords.WFInstanceStore,
            Message = "Giving up retrying a SQL command as the maximum number of retries have been performed.")]
        public void MaximumRetriesExceededForSqlCommand()
        {
            WriteEvent(4207);
        }

        public bool RetryingSqlCommandDueToSqlErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(4208, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFInstanceStore,
            Message = "Retrying a SQL command due to SQL error number {0}.")]
        public void RetryingSqlCommandDueToSqlError(string data1)
        {
            WriteEvent(4208, data1);
        }

        public bool TimeoutOpeningSqlConnectionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(4209, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFInstanceStore,
            Message = "Timeout trying to open a SQL connection. The operation did not complete within the allotted timeout of {0}. The time allotted to this operation may have been a portion of a longer timeout.")]
        public void TimeoutOpeningSqlConnection(string data1)
        {
            WriteEvent(4209, data1);
        }

        public bool SqlExceptionCaughtIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(4210, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFInstanceStore,
            Message = "Caught SQL Exception number {0} message {1}.")]
        public void SqlExceptionCaught(string data1, string data2)
        {
            WriteEvent(4210, data1, data2);
        }

        public bool QueuingSqlRetryIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(4211, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFInstanceStore,
            Message = "Queuing SQL retry with delay {0} milliseconds.")]
        public void QueuingSqlRetry(string data1)
        {
            WriteEvent(4211, data1);
        }

        public bool LockRetryTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(4212, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFInstanceStore,
            Message = "Timeout trying to acquire the instance lock.  The operation did not complete within the allotted timeout of {0}. The time allotted to this operation may have been a portion of a longer timeout.")]
        public void LockRetryTimeout(string data1)
        {
            WriteEvent(4212, data1);
        }

        public bool RunnableInstancesDetectionErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(4213, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFInstanceStore,
            Message = "Detection of runnable instances failed due to the following exception")]
        public void RunnableInstancesDetectionError(string SerializedException)
        {
            WriteEvent(4213, SerializedException);
        }

        public bool InstanceLocksRecoveryErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(4214, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFInstanceStore,
            Message = "Recovering instance locks failed due to the following exception")]
        public void InstanceLocksRecoveryError(string SerializedException)
        {
            WriteEvent(4214, SerializedException);
        }

        public bool MessageLogEventSizeExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WCFMessageLogging, EventChannel.Debug);
        }

        [Event(4600, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WCFMessageLogging,
            Message = "Message could not be logged as it exceeds the ETW event size")]
        public void MessageLogEventSizeExceeded()
        {
            WriteEvent(4600);
        }

        public bool DiscoveryClientInClientChannelFailedToCloseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(4801, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.FailedToClose, Task = Tasks.DiscoveryClient,
            Keywords = Keywords.Discovery,
            Message = "The DiscoveryClient created inside DiscoveryClientChannel failed to close and hence has been aborted.")]
        public void DiscoveryClientInClientChannelFailedToClose(string SerializedException)
        {
            WriteEvent(4801, SerializedException);
        }

        public bool DiscoveryClientProtocolExceptionSuppressedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(4802, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ExceptionSuppressed, Task = Tasks.DiscoveryClient,
            Keywords = Keywords.Discovery,
            Message = "A ProtocolException was suppressed while closing the DiscoveryClient. This could be because a DiscoveryService is still trying to send response to the DiscoveryClient.")]
        public void DiscoveryClientProtocolExceptionSuppressed(string SerializedException)
        {
            WriteEvent(4802, SerializedException);
        }

        public bool DiscoveryClientReceivedMulticastSuppressionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(4803, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ReceivedMulticastSuppression, Task = Tasks.DiscoveryClient,
            Keywords = Keywords.Discovery,
            Message = "The DiscoveryClient received a multicast suppression message from a DiscoveryProxy.")]
        public void DiscoveryClientReceivedMulticastSuppression()
        {
            WriteEvent(4803);
        }

        public bool DiscoveryMessageReceivedAfterOperationCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(4804, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ReceivedAfterOperationCompleted, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message with messageId='{1}' was dropped by the DiscoveryClient because the corresponding {2} operation was completed.")]
        public void DiscoveryMessageReceivedAfterOperationCompleted(string discoveryMessageName, string messageId, string discoveryOperationName)
        {
            WriteEvent(4804, discoveryMessageName, messageId, discoveryOperationName);
        }

        public bool DiscoveryMessageWithInvalidContentIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(4805, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.InvalidContent, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message with messageId='{1}' was dropped because it had invalid content.")]
        public void DiscoveryMessageWithInvalidContent(string messageType, string messageId)
        {
            WriteEvent(4805, messageType, messageId);
        }

        public bool DiscoveryMessageWithInvalidRelatesToOrOperationCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(4806, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.InvalidRelatesToOrOperationCompleted, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message with messageId='{1}' and relatesTo='{2}' was dropped by the DiscoveryClient because either the corresponding {3} operation was completed or the relatesTo value is invalid.")]
        public void DiscoveryMessageWithInvalidRelatesToOrOperationCompleted(string discoveryMessageName, string messageId, string relatesTo, string discoveryOperationName)
        {
            WriteEvent(4806, discoveryMessageName, messageId, relatesTo, discoveryOperationName);
        }

        public bool DiscoveryMessageWithInvalidReplyToIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(4807, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.InvalidReplyTo, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A discovery request message with messageId='{0}' was dropped because it had an invalid ReplyTo address.")]
        public void DiscoveryMessageWithInvalidReplyTo(string messageId)
        {
            WriteEvent(4807, messageId);
        }

        public bool DiscoveryMessageWithNoContentIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(4808, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.NoContent, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message was dropped because it did not have any content.")]
        public void DiscoveryMessageWithNoContent(string messageType)
        {
            WriteEvent(4808, messageType);
        }

        public bool DiscoveryMessageWithNullMessageIdIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(4809, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.NullMessageId, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message was dropped because the message header did not contain the required MessageId property.")]
        public void DiscoveryMessageWithNullMessageId(string messageType)
        {
            WriteEvent(4809, messageType);
        }

        public bool DiscoveryMessageWithNullMessageSequenceIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(4810, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.NullMessageSequence, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message with messageId='{1}' was dropped by the DiscoveryClient because it did not have the DiscoveryMessageSequence property.")]
        public void DiscoveryMessageWithNullMessageSequence(string discoveryMessageName, string messageId)
        {
            WriteEvent(4810, discoveryMessageName, messageId);
        }

        public bool DiscoveryMessageWithNullRelatesToIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(4811, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.NullRelatesTo, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message with messageId='{1}' was dropped by the DiscoveryClient because the message header did not contain the required RelatesTo property.")]
        public void DiscoveryMessageWithNullRelatesTo(string discoveryMessageName, string messageId)
        {
            WriteEvent(4811, discoveryMessageName, messageId);
        }

        public bool DiscoveryMessageWithNullReplyToIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(4812, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.NullReplyTo, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A discovery request message with messageId='{0}' was dropped because it did not have a ReplyTo address.")]
        public void DiscoveryMessageWithNullReplyTo(string messageId)
        {
            WriteEvent(4812, messageId);
        }

        public bool DuplicateDiscoveryMessageIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(4813, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.Duplicate, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message with messageId='{1}' was dropped because it was a duplicate.")]
        public void DuplicateDiscoveryMessage(string messageType, string messageId)
        {
            WriteEvent(4813, messageType, messageId);
        }

        public bool EndpointDiscoverabilityDisabledIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(4814, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Disabled, Task = Tasks.EndpointDiscoverability,
            Keywords = Keywords.Discovery,
            Message = "The discoverability of endpoint with EndpointAddress='{0}' and ListenUri='{1}' has been disabled.")]
        public void EndpointDiscoverabilityDisabled(string endpointAddress, string listenUri)
        {
            WriteEvent(4814, endpointAddress, listenUri);
        }

        public bool EndpointDiscoverabilityEnabledIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(4815, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Enabled, Task = Tasks.EndpointDiscoverability,
            Keywords = Keywords.Discovery,
            Message = "The discoverability of endpoint with EndpointAddress='{0}' and ListenUri='{1}' has been enabled.")]
        public void EndpointDiscoverabilityEnabled(string endpointAddress, string listenUri)
        {
            WriteEvent(4815, endpointAddress, listenUri);
        }

        public bool FindInitiatedInDiscoveryClientChannelIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(4816, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.FindInitiated, Task = Tasks.DiscoveryClientChannel,
            Keywords = Keywords.Discovery,
            Message = "A Find operation was initiated in the DiscoveryClientChannel to discover endpoint(s).")]
        public void FindInitiatedInDiscoveryClientChannel()
        {
            WriteEvent(4816);
        }

        public bool InnerChannelCreationFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(4817, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.CreationFailed, Task = Tasks.DiscoveryClientChannel,
            Keywords = Keywords.Discovery,
            Message = "The DiscoveryClientChannel failed to create the channel with a discovered endpoint with EndpointAddress='{0}' and Via='{1}'. The DiscoveryClientChannel will now attempt to use the next available discovered endpoint.")]
        public void InnerChannelCreationFailed(string endpointAddress, string via, string SerializedException)
        {
            WriteEvent(4817, endpointAddress, via, SerializedException);
        }

        public bool InnerChannelOpenFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(4818, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.OpenFailed, Task = Tasks.DiscoveryClientChannel,
            Keywords = Keywords.Discovery,
            Message = "The DiscoveryClientChannel failed to open the channel with a discovered endpoint with EndpointAddress='{0}' and Via='{1}'. The DiscoveryClientChannel will now attempt to use the next available discovered endpoint.")]
        public void InnerChannelOpenFailed(string endpointAddress, string via, string SerializedException)
        {
            WriteEvent(4818, endpointAddress, via, SerializedException);
        }

        public bool InnerChannelOpenSucceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(4819, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.OpenSucceeded, Task = Tasks.DiscoveryClientChannel,
            Keywords = Keywords.Discovery,
            Message = "The DiscoveryClientChannel successfully discovered an endpoint and opened the channel using it. The client is connected to a service using EndpointAddress='{0}' and Via='{1}'.")]
        public void InnerChannelOpenSucceeded(string endpointAddress, string via)
        {
            WriteEvent(4819, endpointAddress, via);
        }

        public bool SynchronizationContextResetIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(4820, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Reset, Task = Tasks.DiscoverySynchronizationContext,
            Keywords = Keywords.Discovery,
            Message = "The SynchronizationContext has been reset to its original value of {0} by DiscoveryClientChannel.")]
        public void SynchronizationContextReset(string synchronizationContextType)
        {
            WriteEvent(4820, synchronizationContextType);
        }

        public bool SynchronizationContextSetToNullIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(4821, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.SetToNull, Task = Tasks.DiscoverySynchronizationContext,
            Keywords = Keywords.Discovery,
            Message = "The SynchronizationContext has been set to null by DiscoveryClientChannel before initiating the Find operation.")]
        public void SynchronizationContextSetToNull()
        {
            WriteEvent(4821);
        }

        public bool DCSerializeWithSurrogateStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(5001, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Serialization,
            Message = "DataContract serialize {0} with surrogates start.")]
        public void DCSerializeWithSurrogateStart(string SurrogateType)
        {
            WriteEvent(5001, SurrogateType);
        }

        public bool DCSerializeWithSurrogateStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(5002, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Serialization,
            Message = "DataContract serialize with surrogates stop.")]
        public void DCSerializeWithSurrogateStop()
        {
            WriteEvent(5002);
        }

        public bool DCDeserializeWithSurrogateStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(5003, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Serialization,
            Message = "DataContract deserialize {0} with surrogates start.")]
        public void DCDeserializeWithSurrogateStart(string SurrogateType)
        {
            WriteEvent(5003, SurrogateType);
        }

        public bool DCDeserializeWithSurrogateStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(5004, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Serialization,
            Message = "DataContract deserialize with surrogates stop.")]
        public void DCDeserializeWithSurrogateStop()
        {
            WriteEvent(5004);
        }

        public bool ImportKnownTypesStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(5005, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Serialization,
            Message = "ImportKnownTypes start.")]
        public void ImportKnownTypesStart()
        {
            WriteEvent(5005);
        }

        public bool ImportKnownTypesStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(5006, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Serialization,
            Message = "ImportKnownTypes stop.")]
        public void ImportKnownTypesStop()
        {
            WriteEvent(5006);
        }

        public bool DCResolverResolveIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(5007, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Serialization,
            Message = "DataContract resolver resolving {0} start.")]
        public void DCResolverResolve(string TypeName)
        {
            WriteEvent(5007, TypeName);
        }

        public bool DCGenWriterStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(5008, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Serialization,
            Message = "DataContract generate {0} writer for {1} start.")]
        public void DCGenWriterStart(string Kind, string TypeName)
        {
            WriteEvent(5008, Kind, TypeName);
        }

        public bool DCGenWriterStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(5009, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Serialization,
            Message = "DataContract generate writer stop.")]
        public void DCGenWriterStop()
        {
            WriteEvent(5009);
        }

        public bool DCGenReaderStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(5010, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Serialization,
            Message = "DataContract generate {0} reader for {1} start.")]
        public void DCGenReaderStart(string Kind, string TypeName)
        {
            WriteEvent(5010, Kind, TypeName);
        }

        public bool DCGenReaderStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(5011, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Serialization,
            Message = "DataContract generation stop.")]
        public void DCGenReaderStop()
        {
            WriteEvent(5011);
        }

        public bool DCJsonGenReaderStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(5012, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Serialization,
            Message = "Json generate {0} reader for {1} start.")]
        public void DCJsonGenReaderStart(string Kind, string TypeName)
        {
            WriteEvent(5012, Kind, TypeName);
        }

        public bool DCJsonGenReaderStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(5013, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Serialization,
            Message = "Json reader generation stop.")]
        public void DCJsonGenReaderStop()
        {
            WriteEvent(5013);
        }

        public bool DCJsonGenWriterStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(5014, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Serialization,
            Message = "Json generate {0} writer for {1} start.")]
        public void DCJsonGenWriterStart(string Kind, string TypeName)
        {
            WriteEvent(5014, Kind, TypeName);
        }

        public bool DCJsonGenWriterStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(5015, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Serialization,
            Message = "Json generate writer stop.")]
        public void DCJsonGenWriterStop()
        {
            WriteEvent(5015);
        }

        public bool GenXmlSerializableStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(5016, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Serialization,
            Message = "Generate Xml serializable for '{0}' start.")]
        public void GenXmlSerializableStart(string DCType)
        {
            WriteEvent(5016, DCType);
        }

        public bool GenXmlSerializableStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(5017, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop,
            Keywords = Keywords.Serialization,
            Message = "Generate Xml serializable stop.")]
        public void GenXmlSerializableStop()
        {
            WriteEvent(5017);
        }

        public bool JsonMessageDecodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(5203, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Channel,
            Message = "JsonMessageEncoder started decoding the message.")]
        public void JsonMessageDecodingStart()
        {
            WriteEvent(5203);
        }

        public bool JsonMessageEncodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(5204, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start,
            Keywords = Keywords.Channel,
            Message = "JsonMessageEncoder started encoding the message.")]
        public void JsonMessageEncodingStart()
        {
            WriteEvent(5204);
        }

        public bool TokenValidationStartedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(5402, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "SecurityToken (type '{0}' and id '{1}') validation started.")]
        public void TokenValidationStarted(string tokenType, string tokenID, string HostReference)
        {
            WriteEvent(5402, tokenType, tokenID, HostReference);
        }

        public bool TokenValidationSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(5403, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "SecurityToken (type '{0}' and id '{1}') validation succeeded.")]
        public void TokenValidationSuccess(string tokenType, string tokenID, string HostReference)
        {
            WriteEvent(5403, tokenType, tokenID, HostReference);
        }

        public bool TokenValidationFailureIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Security, EventChannel.Debug);
        }

        [Event(5404, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "SecurityToken (type '{0}' and id '{1}') validation failed. {2}")]
        public void TokenValidationFailure(string tokenType, string tokenID, string errorMessage, string HostReference)
        {
            WriteEvent(5404, tokenType, tokenID, errorMessage, HostReference);
        }

        public bool GetIssuerNameSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(5405, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "Retrieval of issuer name:{0} from tokenId:{1} succeeded.")]
        public void GetIssuerNameSuccess(string issuerName, string tokenID, string HostReference)
        {
            WriteEvent(5405, issuerName, tokenID, HostReference);
        }

        public bool GetIssuerNameFailureIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Security, EventChannel.Debug);
        }

        [Event(5406, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "Retrieval of issuer name from tokenId:{0} failed.")]
        public void GetIssuerNameFailure(string tokenID, string HostReference)
        {
            WriteEvent(5406, tokenID, HostReference);
        }

        public bool FederationMessageProcessingStartedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(5600, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "Federation message processing started.")]
        public void FederationMessageProcessingStarted(string HostReference)
        {
            WriteEvent(5600, HostReference);
        }

        public bool FederationMessageProcessingSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(5601, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "Federation message processing succeeded.")]
        public void FederationMessageProcessingSuccess(string HostReference)
        {
            WriteEvent(5601, HostReference);
        }

        public bool FederationMessageCreationStartedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(5602, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "Creating federation message from form post started.")]
        public void FederationMessageCreationStarted(string HostReference)
        {
            WriteEvent(5602, HostReference);
        }

        public bool FederationMessageCreationSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(5603, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "Creating federation message from form post succeeded.")]
        public void FederationMessageCreationSuccess(string HostReference)
        {
            WriteEvent(5603, HostReference);
        }

        public bool SessionCookieReadingStartedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(5604, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "Reading session token from session cookie started.")]
        public void SessionCookieReadingStarted(string HostReference)
        {
            WriteEvent(5604, HostReference);
        }

        public bool SessionCookieReadingSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(5605, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "Reading session token from session cookie succeeded.")]
        public void SessionCookieReadingSuccess(string HostReference)
        {
            WriteEvent(5605, HostReference);
        }

        public bool PrincipalSettingFromSessionTokenStartedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(5606, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "Principal setting from session token started.")]
        public void PrincipalSettingFromSessionTokenStarted(string HostReference)
        {
            WriteEvent(5606, HostReference);
        }

        public bool PrincipalSettingFromSessionTokenSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(5607, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Security,
            Message = "Principal setting from session token succeeded.")]
        public void PrincipalSettingFromSessionTokenSuccess(string HostReference)
        {
            WriteEvent(5607, HostReference);
        }

        public bool TrackingRecordDroppedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFTracking, EventChannel.Debug);
        }

        [Event(39456, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.Dropped, Task = Tasks.TrackingRecord,
            Keywords = Keywords.WFTracking,
            Message = "Size of tracking record {0} exceeds maximum allowed by the ETW session for provider {1}")]
        public void TrackingRecordDropped(long RecordNumber, Guid ProviderId)
        {
            WriteEvent(39456, RecordNumber, ProviderId);
        }

        public bool TrackingRecordRaisedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(39457, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.Raised, Task = Tasks.TrackingRecord,
            Keywords = Keywords.WFRuntime,
            Message = "Tracking Record {0} raised to {1}.")]
        public void TrackingRecordRaised(string data1, string data2)
        {
            WriteEvent(39457, data1, data2);
        }

        public bool TrackingRecordTruncatedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFTracking, EventChannel.Debug);
        }

        [Event(39458, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.Truncated, Task = Tasks.TrackingRecord,
            Keywords = Keywords.WFTracking,
            Message = "Truncated tracking record {0} written to ETW session with provider {1}. Variables/annotations/user data have been removed")]
        public void TrackingRecordTruncated(long RecordNumber, Guid ProviderId)
        {
            WriteEvent(39458, RecordNumber, ProviderId);
        }

        public bool TrackingDataExtractedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(39459, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFRuntime,
            Message = "Tracking data {0} extracted in activity {1}.")]
        public void TrackingDataExtracted(string Data, string Activity)
        {
            WriteEvent(39459, Data, Activity);
        }

        public bool TrackingValueNotSerializableIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFTracking, EventChannel.Debug);
        }

        [Event(39460, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFTracking,
            Message = "The extracted argument/variable '{0}' is not serializable.")]
        public void TrackingValueNotSerializable(string name)
        {
            WriteEvent(39460, name);
        }

        public bool AppDomainUnloadIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(57393, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "AppDomain unloading. AppDomain.FriendlyName {0}, ProcessName {1}, ProcessId {2}.")]
        public void AppDomainUnload(string AppDomainName, string processName, string processId)
        {
            WriteEvent(57393, AppDomainName, processName, processId);
        }

        public bool HandledExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(57394, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Handling an exception.  Exception details: {0}")]
        public void HandledException(string data1, string SerializedException)
        {
            WriteEvent(57394, data1, SerializedException);
        }

        public bool ShipAssertExceptionMessageIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(57395, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "An unexpected failure occurred. Applications should not attempt to handle this error. For diagnostic purposes, this English message is associated with the failure: {0}.")]
        public void ShipAssertExceptionMessage(string data1)
        {
            WriteEvent(57395, data1);
        }

        public bool ThrowingExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(57396, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Throwing an exception. Source: {0}. Exception details: {1}")]
        public void ThrowingException(string data1, string data2, string SerializedException)
        {
            WriteEvent(57396, data1, data2, SerializedException);
        }

        public bool UnhandledExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Critical, Keywords.Infrastructure, EventChannel.Operational);
        }

        [Event(57397, Level = EventLevel.Critical, Channel = EventChannel.Operational, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Unhandled exception.  Exception details: {0}")]
        public void UnhandledException(string data1, string SerializedException)
        {
            WriteEvent(57397, data1, SerializedException);
        }

        public bool MaxInstancesExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(57398, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.WFServices,
            Message = "The system hit the limit set for throttle 'MaxConcurrentInstances'. Limit for this throttle was set to {0}. Throttle value can be changed by modifying attribute 'maxConcurrentInstances' in serviceThrottle element or by modifying 'MaxConcurrentInstances' property on behavior ServiceThrottlingBehavior.")]
        public void MaxInstancesExceeded(int limit)
        {
            WriteEvent(57398, limit);
        }

        public bool TraceCodeEventLogCriticalIsEnabled()
        {
            return base.IsEnabled(EventLevel.Critical, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(57399, Level = EventLevel.Critical, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Wrote to the EventLog.")]
        public void TraceCodeEventLogCritical(string ExtendedData)
        {
            WriteEvent(57399, ExtendedData);
        }

        public bool TraceCodeEventLogErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(57400, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Wrote to the EventLog.")]
        public void TraceCodeEventLogError(string ExtendedData)
        {
            WriteEvent(57400, ExtendedData);
        }

        public bool TraceCodeEventLogInfoIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(57401, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Wrote to the EventLog.")]
        public void TraceCodeEventLogInfo(string ExtendedData)
        {
            WriteEvent(57401, ExtendedData);
        }

        public bool TraceCodeEventLogVerboseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(57402, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Wrote to the EventLog.")]
        public void TraceCodeEventLogVerbose(string ExtendedData)
        {
            WriteEvent(57402, ExtendedData);
        }

        public bool TraceCodeEventLogWarningIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(57403, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Wrote to the EventLog.")]
        public void TraceCodeEventLogWarning(string ExtendedData)
        {
            WriteEvent(57403, ExtendedData);
        }

        public bool HandledExceptionWarningIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(57404, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Handling an exception. Exception details: {0}")]
        public void HandledExceptionWarning(string data1, string SerializedException)
        {
            WriteEvent(57404, data1, SerializedException);
        }

        public bool HandledExceptionErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Infrastructure, EventChannel.Operational);
        }

        [Event(57405, Level = EventLevel.Error, Channel = EventChannel.Operational, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Handling an exception. Exception details: {0}")]
        public void HandledExceptionError(string data1, string SerializedException)
        {
            WriteEvent(57405, data1, SerializedException);
        }

        public bool HandledExceptionVerboseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(57406, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Handling an exception  Exception details: {0}")]
        public void HandledExceptionVerbose(string data1, string SerializedException)
        {
            WriteEvent(57406, data1, SerializedException);
        }

        public bool ThrowingExceptionVerboseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(57407, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Throwing an exception. Source: {0}. Exception details: {1}")]
        public void ThrowingExceptionVerbose(string data1, string data2, string SerializedException)
        {
            WriteEvent(57407, data1, data2, SerializedException);
        }

        public bool EtwUnhandledExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Critical, Keywords.Infrastructure, EventChannel.Operational);
        }

        [Event(57408, Level = EventLevel.Critical, Channel = EventChannel.Operational, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Unhandled exception. Exception details: {0}")]
        public void EtwUnhandledException(string data1, string SerializedException)
        {
            WriteEvent(57408, data1, SerializedException);
        }

        public bool ThrowingEtwExceptionVerboseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(57409, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Throwing an exception. Source: {0}. Exception details: {1}")]
        public void ThrowingEtwExceptionVerbose(string data1, string data2, string SerializedException)
        {
            WriteEvent(57409, data1, data2, SerializedException);
        }

        public bool ThrowingEtwExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(57410, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "Throwing an exception. Source: {0}. Exception details: {1}")]
        public void ThrowingEtwException(string data1, string data2, string SerializedException)
        {
            WriteEvent(57410, data1, data2, SerializedException);
        }

        public bool HttpHandlerPickedForUrlIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(62326, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WebHost,
            Message = "The url '{0}' hosts XAML document with root element type '{1}'. The HTTP handler type '{2}' is picked to serve all the requests made to this url.")]
        public void HttpHandlerPickedForUrl(string data1, string data2, string data3)
        {
            WriteEvent(62326, data1, data2, data3);
        }


        #region Keywords / Tasks / Opcodes

        public class Tasks
        {
            public const EventTask BufferOutOfOrder = (EventTask)2508;
            public const EventTask BufferPooling = (EventTask)2509;
            public const EventTask ClientRuntime = (EventTask)2514;
            public const EventTask CompleteWorkItem = (EventTask)2518;
            public const EventTask Correlation = (EventTask)2523;
            public const EventTask DiscoveryClient = (EventTask)2529;
            public const EventTask DiscoveryClientChannel = (EventTask)2530;
            public const EventTask DiscoveryMessage = (EventTask)2531;
            public const EventTask DiscoverySynchronizationContext = (EventTask)2532;
            public const EventTask DispatchMessage = (EventTask)2533;
            public const EventTask EndpointDiscoverability = (EventTask)2534;
            public const EventTask ExecuteFlowchart = (EventTask)2536;
            public const EventTask InferDescription = (EventTask)2548;
            public const EventTask InvokeMethod = (EventTask)2551;
            public const EventTask MessageChannelCache = (EventTask)2554;
            public const EventTask ReliableSession = (EventTask)2561;
            public const EventTask RoutingService = (EventTask)2562;
            public const EventTask RoutingServiceClient = (EventTask)2563;
            public const EventTask RoutingServiceMessage = (EventTask)2565;
            public const EventTask RoutingServiceReceiveContext = (EventTask)2566;
            public const EventTask RoutingServiceTransaction = (EventTask)2567;
            public const EventTask RuntimeTransaction = (EventTask)2568;
            public const EventTask ScheduleWorkItem = (EventTask)2570;
            public const EventTask SessionUpgrade = (EventTask)2587;
            public const EventTask StartWorkItem = (EventTask)2590;
            public const EventTask TrackingProfile = (EventTask)2597;
            public const EventTask TrackingRecord = (EventTask)2598;
            public const EventTask TransportReceive = (EventTask)2599;
            public const EventTask TryCatchException = (EventTask)2601;
            public const EventTask WASActivation = (EventTask)2603;
            public const EventTask WFApplicationStateChange = (EventTask)2605;
            public const EventTask WorkflowInstanceRecord = (EventTask)2608;
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
