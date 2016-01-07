using System.Diagnostics.Tracing;
using System.Runtime.Diagnostics;

namespace System.Runtime
{
    [EventSource(Name = "Microsoft-Windows-Application Server-Applications", Guid = "c651f5f6-1c0d-492e-8ae1-b4efd7c9d503")]
    public sealed class WcfEventSource : EventSource
    {
        public static WcfEventSource Instance = new WcfEventSource();

        public bool WorkflowInstanceRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(EventIds.WorkflowInstanceRecord, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.WorkflowInstanceRecord,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord= WorkflowInstanceRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, State = {4}, Annotations = {5}, ProfileName = {6}")]
        public void WorkflowInstanceRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string State, string Annotations, string ProfileName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowInstanceRecord, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, State, Annotations, ProfileName, HostReference, AppDomain);
        }

        [NonEvent]
        public void WorkflowInstanceRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string State, string Annotations, string ProfileName)
        {
            WorkflowInstanceRecord(InstanceId, RecordNumber, EventTime, ActivityDefinitionId, State, Annotations, ProfileName, "", "");
        }

        public bool WorkflowInstanceUnhandledExceptionRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(EventIds.WorkflowInstanceUnhandledExceptionRecord, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = Opcodes.WorkflowInstanceRecordUnhandledExceptionRecord, Task = Tasks.WorkflowInstanceRecord,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = WorkflowInstanceUnhandledExceptionRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, SourceName = {4}, SourceId = {5}, SourceInstanceId = {6}, SourceTypeName={7}, Exception={8}, Annotations= {9}, ProfileName = {10}")]
        public void WorkflowInstanceUnhandledExceptionRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string SourceName, string SourceId, string SourceInstanceId, string SourceTypeName, string Exception, string Annotations, string ProfileName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowInstanceUnhandledExceptionRecord, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, SourceName, SourceId, SourceInstanceId, SourceTypeName, Exception, Annotations, ProfileName, HostReference, AppDomain);
        }

        [NonEvent]
        public void WorkflowInstanceUnhandledExceptionRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string SourceName, string SourceId, string SourceInstanceId, string SourceTypeName, string Exception, string Annotations, string ProfileName)
        {
            WorkflowInstanceUnhandledExceptionRecord(InstanceId, RecordNumber, EventTime, ActivityDefinitionId, SourceName, SourceId, SourceInstanceId, SourceTypeName, Exception, Annotations, ProfileName, "", "");
        }

        public bool WorkflowInstanceAbortedRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(EventIds.WorkflowInstanceAbortedRecord, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = Opcodes.WorkflowInstanceRecordAbortedRecord, Task = Tasks.WorkflowInstanceRecord,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = WorkflowInstanceAbortedRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, Reason = {4}, Annotations = {5}, ProfileName = {6}")]
        public void WorkflowInstanceAbortedRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string Reason, string Annotations, string ProfileName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowInstanceAbortedRecord, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, Reason, Annotations, ProfileName, HostReference, AppDomain);
        }

        [NonEvent]
        public void WorkflowInstanceAbortedRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string Reason, string Annotations, string ProfileName)
        {
            WorkflowInstanceAbortedRecord(InstanceId, RecordNumber, EventTime, ActivityDefinitionId, Reason, Annotations, ProfileName, "", "");
        }

        public bool ActivityStateRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(EventIds.ActivityStateRecord, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.WorkflowTracking,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = ActivityStateRecord, InstanceID = {0}, RecordNumber={1}, EventTime={2}, State = {3}, Name={4}, ActivityId={5}, ActivityInstanceId={6}, ActivityTypeName={7}, Arguments={8}, Variables={9}, Annotations={10}, ProfileName = {11}")]
        public void ActivityStateRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string State, string Name, string ActivityId, string ActivityInstanceId, string ActivityTypeName, string Arguments, string Variables, string Annotations, string ProfileName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.ActivityStateRecord, InstanceId, RecordNumber, EventTime, State, Name, ActivityId, ActivityInstanceId, ActivityTypeName, Arguments, Variables, Annotations, ProfileName, HostReference, AppDomain);
        }

        [NonEvent]
        public void ActivityStateRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string State, string Name, string ActivityId, string ActivityInstanceId, string ActivityTypeName, string Arguments, string Variables, string Annotations, string ProfileName)
        {
            ActivityStateRecord(InstanceId, RecordNumber, EventTime, State, Name, ActivityId, ActivityInstanceId, ActivityTypeName, Arguments, Variables, Annotations, ProfileName, "", "");
        }

        public bool ActivityScheduledRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(EventIds.ActivityScheduledRecord, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.WorkflowTracking,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = ActivityScheduledRecord, InstanceID = {0},  RecordNumber = {1}, EventTime = {2}, Name = {3}, ActivityId = {4}, ActivityInstanceId = {5}, ActivityTypeName = {6}, ChildActivityName = {7}, ChildActivityId = {8}, ChildActivityInstanceId = {9}, ChildActivityTypeName ={10}, Annotations={11}, ProfileName = {12}")]
        public void ActivityScheduledRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string Name, string ActivityId, string ActivityInstanceId, string ActivityTypeName, string ChildActivityName, string ChildActivityId, string ChildActivityInstanceId, string ChildActivityTypeName, string Annotations, string ProfileName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.ActivityScheduledRecord, InstanceId, RecordNumber, EventTime, Name, ActivityId, ActivityInstanceId, ActivityTypeName, ChildActivityName, ChildActivityId, ChildActivityInstanceId, ChildActivityTypeName, Annotations, ProfileName, HostReference, AppDomain);
        }

        [NonEvent]
        public void ActivityScheduledRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string Name, string ActivityId, string ActivityInstanceId, string ActivityTypeName, string ChildActivityName, string ChildActivityId, string ChildActivityInstanceId, string ChildActivityTypeName, string Annotations, string ProfileName)
        {
            ActivityScheduledRecord(InstanceId, RecordNumber, EventTime, Name, ActivityId, ActivityInstanceId, ActivityTypeName, ChildActivityName, ChildActivityId, ChildActivityInstanceId, ChildActivityTypeName, Annotations, ProfileName, "", "");
        }

        public bool FaultPropagationRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(EventIds.FaultPropagationRecord, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.WorkflowTracking,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = FaultPropagationRecord, InstanceID={0}, RecordNumber={1}, EventTime={2}, FaultSourceActivityName={3}, FaultSourceActivityId={4}, FaultSourceActivityInstanceId={5}, FaultSourceActivityTypeName={6}, FaultHandlerActivityName={7},  FaultHandlerActivityId = {8}, FaultHandlerActivityInstanceId ={9}, FaultHandlerActivityTypeName={10}, Fault={11}, IsFaultSource={12}, Annotations={13}, ProfileName = {14}")]
        public void FaultPropagationRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string FaultSourceActivityName, string FaultSourceActivityId, string FaultSourceActivityInstanceId, string FaultSourceActivityTypeName, string FaultHandlerActivityName, string FaultHandlerActivityId, string FaultHandlerActivityInstanceId, string FaultHandlerActivityTypeName, string Fault, byte IsFaultSource, string Annotations, string ProfileName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.FaultPropagationRecord, InstanceId, RecordNumber, EventTime, FaultSourceActivityName, FaultSourceActivityId, FaultSourceActivityInstanceId, FaultSourceActivityTypeName, FaultHandlerActivityName, FaultHandlerActivityId, FaultHandlerActivityInstanceId, FaultHandlerActivityTypeName, Fault, IsFaultSource, Annotations, ProfileName, HostReference, AppDomain);
        }

        [NonEvent]
        public void FaultPropagationRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string FaultSourceActivityName, string FaultSourceActivityId, string FaultSourceActivityInstanceId, string FaultSourceActivityTypeName, string FaultHandlerActivityName, string FaultHandlerActivityId, string FaultHandlerActivityInstanceId, string FaultHandlerActivityTypeName, string Fault, byte IsFaultSource, string Annotations, string ProfileName)
        {
            FaultPropagationRecord(InstanceId, RecordNumber, EventTime, FaultSourceActivityName, FaultSourceActivityId, FaultSourceActivityInstanceId, FaultSourceActivityTypeName, FaultHandlerActivityName, FaultHandlerActivityId, FaultHandlerActivityInstanceId, FaultHandlerActivityTypeName, Fault, IsFaultSource, Annotations, ProfileName, "", "");
        }

        public bool CancelRequestedRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(EventIds.CancelRequestedRecord, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.WorkflowTracking,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = CancelRequestedRecord, InstanceID={0}, RecordNumber={1}, EventTime={2}, Name={3}, ActivityId={4}, ActivityInstanceId={5}, ActivityTypeName = {6}, ChildActivityName = {7}, ChildActivityId = {8}, ChildActivityInstanceId = {9}, ChildActivityTypeName ={10}, Annotations={11}, ProfileName = {12}")]
        public void CancelRequestedRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string Name, string ActivityId, string ActivityInstanceId, string ActivityTypeName, string ChildActivityName, string ChildActivityId, string ChildActivityInstanceId, string ChildActivityTypeName, string Annotations, string ProfileName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.CancelRequestedRecord, InstanceId, RecordNumber, EventTime, Name, ActivityId, ActivityInstanceId, ActivityTypeName, ChildActivityName, ChildActivityId, ChildActivityInstanceId, ChildActivityTypeName, Annotations, ProfileName, HostReference, AppDomain);
        }

        [NonEvent]
        public void CancelRequestedRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string Name, string ActivityId, string ActivityInstanceId, string ActivityTypeName, string ChildActivityName, string ChildActivityId, string ChildActivityInstanceId, string ChildActivityTypeName, string Annotations, string ProfileName)
        {
            CancelRequestedRecord(InstanceId, RecordNumber, EventTime, Name, ActivityId, ActivityInstanceId, ActivityTypeName, ChildActivityName, ChildActivityId, ChildActivityInstanceId, ChildActivityTypeName, Annotations, ProfileName, "", "");
        }

        public bool BookmarkResumptionRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(EventIds.BookmarkResumptionRecord, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.WorkflowTracking,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = BookmarkResumptionRecord, InstanceID={0}, RecordNumber={1},EventTime={2}, Name={3}, SubInstanceID={4},  OwnerActivityName={5}, OwnerActivityId ={6}, OwnerActivityInstanceId={7}, OwnerActivityTypeName={8}, Annotations={9}, ProfileName = {10}")]
        public void BookmarkResumptionRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string Name, Guid SubInstanceID, string OwnerActivityName, string OwnerActivityId, string OwnerActivityInstanceId, string OwnerActivityTypeName, string Annotations, string ProfileName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.BookmarkResumptionRecord, InstanceId, RecordNumber, EventTime, Name, SubInstanceID, OwnerActivityName, OwnerActivityId, OwnerActivityInstanceId, OwnerActivityTypeName, Annotations, ProfileName, HostReference, AppDomain);
        }

        [NonEvent]
        public void BookmarkResumptionRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string Name, Guid SubInstanceID, string OwnerActivityName, string OwnerActivityId, string OwnerActivityInstanceId, string OwnerActivityTypeName, string Annotations, string ProfileName)
        {
            BookmarkResumptionRecord(InstanceId, RecordNumber, EventTime, Name, SubInstanceID, OwnerActivityName, OwnerActivityId, OwnerActivityInstanceId, OwnerActivityTypeName, Annotations, ProfileName, "", "");
        }

        public bool CustomTrackingRecordInfoIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.UserEvents | Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(EventIds.CustomTrackingRecordInfo, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.CustomTrackingRecord,
            Keywords = Keywords.UserEvents | Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = CustomTrackingRecord, InstanceID = {0}, RecordNumber={1}, EventTime={2},  Name={3}, ActivityName={4}, ActivityId={5}, ActivityInstanceId={6}, ActivityTypeName={7}, Data={8}, Annotations={9}, ProfileName = {10}")]
        public void CustomTrackingRecordInfo(Guid InstanceId, long RecordNumber, DateTime EventTime, string Name, string ActivityName, string ActivityId, string ActivityInstanceId, string ActivityTypeName, string Data, string Annotations, string ProfileName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.CustomTrackingRecordInfo, InstanceId, RecordNumber, EventTime, Name, ActivityName, ActivityId, ActivityInstanceId, ActivityTypeName, Data, Annotations, ProfileName, HostReference, AppDomain);
        }

        [NonEvent]
        public void CustomTrackingRecordInfo(Guid InstanceId, long RecordNumber, DateTime EventTime, string Name, string ActivityName, string ActivityId, string ActivityInstanceId, string ActivityTypeName, string Data, string Annotations, string ProfileName)
        {
            CustomTrackingRecordInfo(InstanceId, RecordNumber, EventTime, Name, ActivityName, ActivityId, ActivityInstanceId, ActivityTypeName, Data, Annotations, ProfileName, "", "");
        }

        public bool CustomTrackingRecordWarningIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.UserEvents | Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(EventIds.CustomTrackingRecordWarning, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.CustomTrackingRecord,
            Keywords = Keywords.UserEvents | Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = CustomTrackingRecord, InstanceID = {0}, RecordNumber={1}, EventTime={2}, Name={3}, ActivityName={4}, ActivityId={5}, ActivityInstanceId={6}, ActivityTypeName={7}, Data={8}, Annotations={9}, ProfileName = {10}")]
        public void CustomTrackingRecordWarning(Guid InstanceId, long RecordNumber, DateTime EventTime, string Name, string ActivityName, string ActivityId, string ActivityInstanceId, string ActivityTypeName, string Data, string Annotations, string ProfileName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.CustomTrackingRecordWarning, InstanceId, RecordNumber, EventTime, Name, ActivityName, ActivityId, ActivityInstanceId, ActivityTypeName, Data, Annotations, ProfileName, HostReference, AppDomain);
        }

        [NonEvent]
        public void CustomTrackingRecordWarning(Guid InstanceId, long RecordNumber, DateTime EventTime, string Name, string ActivityName, string ActivityId, string ActivityInstanceId, string ActivityTypeName, string Data, string Annotations, string ProfileName)
        {
            CustomTrackingRecordWarning(InstanceId, RecordNumber, EventTime, Name, ActivityName, ActivityId, ActivityInstanceId, ActivityTypeName, Data, Annotations, ProfileName, "", "");
        }

        public bool CustomTrackingRecordErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.UserEvents | Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(EventIds.CustomTrackingRecordError, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.CustomTrackingRecord,
            Keywords = Keywords.UserEvents | Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = CustomTrackingRecord, InstanceID = {0}, RecordNumber={1}, EventTime={2}, Name={3}, ActivityName={4}, ActivityId={5}, ActivityInstanceId={6}, ActivityTypeName={7}, Data={8}, Annotations={9}, ProfileName = {10}")]
        public void CustomTrackingRecordError(Guid InstanceId, long RecordNumber, DateTime EventTime, string Name, string ActivityName, string ActivityId, string ActivityInstanceId, string ActivityTypeName, string Data, string Annotations, string ProfileName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.CustomTrackingRecordError, InstanceId, RecordNumber, EventTime, Name, ActivityName, ActivityId, ActivityInstanceId, ActivityTypeName, Data, Annotations, ProfileName, HostReference, AppDomain);
        }

        [NonEvent]
        public void CustomTrackingRecordError(Guid InstanceId, long RecordNumber, DateTime EventTime, string Name, string ActivityName, string ActivityId, string ActivityInstanceId, string ActivityTypeName, string Data, string Annotations, string ProfileName)
        {
            CustomTrackingRecordError(InstanceId, RecordNumber, EventTime, Name, ActivityName, ActivityId, ActivityInstanceId, ActivityTypeName, Data, Annotations, ProfileName, "", "");
        }

        public bool WorkflowInstanceSuspendedRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(EventIds.WorkflowInstanceSuspendedRecord, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.WorkflowInstanceRecordSuspendedRecord, Task = Tasks.WorkflowInstanceRecord,
            Keywords = Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = WorkflowInstanceSuspendedRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, Reason = {4}, Annotations = {5}, ProfileName = {6}")]
        public void WorkflowInstanceSuspendedRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string Reason, string Annotations, string ProfileName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowInstanceSuspendedRecord, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, Reason, Annotations, ProfileName, HostReference, AppDomain);
        }

        [NonEvent]
        public void WorkflowInstanceSuspendedRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string Reason, string Annotations, string ProfileName)
        {
            WorkflowInstanceSuspendedRecord(InstanceId, RecordNumber, EventTime, ActivityDefinitionId, Reason, Annotations, ProfileName, "", "");
        }

        public bool WorkflowInstanceTerminatedRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(EventIds.WorkflowInstanceTerminatedRecord, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = Opcodes.WorkflowInstanceRecordTerminatedRecord, Task = Tasks.WorkflowInstanceRecord,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = WorkflowInstanceTerminatedRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, Reason = {4}, Annotations = {5}, ProfileName = {6}")]
        public void WorkflowInstanceTerminatedRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string Reason, string Annotations, string ProfileName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowInstanceTerminatedRecord, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, Reason, Annotations, ProfileName, HostReference, AppDomain);
        }

        [NonEvent]
        public void WorkflowInstanceTerminatedRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string Reason, string Annotations, string ProfileName)
        {
            WorkflowInstanceTerminatedRecord(InstanceId, RecordNumber, EventTime, ActivityDefinitionId, Reason, Annotations, ProfileName, "", "");
        }

        public bool WorkflowInstanceRecordWithIdIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(EventIds.WorkflowInstanceRecordWithId, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.WorkflowInstanceRecord,
            Keywords = Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord= WorkflowInstanceRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, State = {4}, Annotations = {5}, ProfileName = {6}, WorkflowDefinitionIdentity = {7}")]
        public void WorkflowInstanceRecordWithId(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string State, string Annotations, string ProfileName, string WorkflowDefinitionIdentity, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowInstanceRecordWithId, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, State, Annotations, ProfileName, WorkflowDefinitionIdentity, HostReference, AppDomain);
        }

        [NonEvent]
        public void WorkflowInstanceRecordWithId(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string State, string Annotations, string ProfileName, string WorkflowDefinitionIdentity)
        {
            WorkflowInstanceRecordWithId(InstanceId, RecordNumber, EventTime, ActivityDefinitionId, State, Annotations, ProfileName, WorkflowDefinitionIdentity, "", "");
        }

        public bool WorkflowInstanceAbortedRecordWithIdIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(EventIds.WorkflowInstanceAbortedRecordWithId, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = Opcodes.WorkflowInstanceRecordAbortedWithId, Task = Tasks.WorkflowInstanceRecord,
            Keywords = Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = WorkflowInstanceAbortedRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, Reason = {4},  Annotations = {5}, ProfileName = {6}, WorkflowDefinitionIdentity = {7}")]
        public void WorkflowInstanceAbortedRecordWithId(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string Reason, string Annotations, string ProfileName, string WorkflowDefinitionIdentity, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowInstanceAbortedRecordWithId, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, Reason, Annotations, ProfileName, WorkflowDefinitionIdentity, HostReference, AppDomain);
        }

        [NonEvent]
        public void WorkflowInstanceAbortedRecordWithId(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string Reason, string Annotations, string ProfileName, string WorkflowDefinitionIdentity)
        {
            WorkflowInstanceAbortedRecordWithId(InstanceId, RecordNumber, EventTime, ActivityDefinitionId, Reason, Annotations, ProfileName, WorkflowDefinitionIdentity, "", "");
        }

        public bool WorkflowInstanceSuspendedRecordWithIdIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(EventIds.WorkflowInstanceSuspendedRecordWithId, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.WorkflowInstanceRecordSuspendedWithId, Task = Tasks.WorkflowInstanceRecord,
            Keywords = Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = WorkflowInstanceSuspendedRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, Reason = {4}, Annotations = {5}, ProfileName = {6}, WorkflowDefinitionIdentity = {7}")]
        public void WorkflowInstanceSuspendedRecordWithId(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string Reason, string Annotations, string ProfileName, string WorkflowDefinitionIdentity, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowInstanceSuspendedRecordWithId, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, Reason, Annotations, ProfileName, WorkflowDefinitionIdentity, HostReference, AppDomain);
        }

        [NonEvent]
        public void WorkflowInstanceSuspendedRecordWithId(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string Reason, string Annotations, string ProfileName, string WorkflowDefinitionIdentity)
        {
            WorkflowInstanceSuspendedRecordWithId(InstanceId, RecordNumber, EventTime, ActivityDefinitionId, Reason, Annotations, ProfileName, WorkflowDefinitionIdentity, "", "");
        }

        public bool WorkflowInstanceTerminatedRecordWithIdIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(EventIds.WorkflowInstanceTerminatedRecordWithId, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = Opcodes.WorkflowInstanceRecordTerminatedWithId, Task = Tasks.WorkflowInstanceRecord,
            Keywords = Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = WorkflowInstanceTerminatedRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, Reason = {4},  Annotations = {5}, ProfileName = {6}, WorkflowDefinitionIdentity = {7}")]
        public void WorkflowInstanceTerminatedRecordWithId(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string Reason, string Annotations, string ProfileName, string WorkflowDefinitionIdentity, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowInstanceTerminatedRecordWithId, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, Reason, Annotations, ProfileName, WorkflowDefinitionIdentity, HostReference, AppDomain);
        }

        [NonEvent]
        public void WorkflowInstanceTerminatedRecordWithId(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string Reason, string Annotations, string ProfileName, string WorkflowDefinitionIdentity)
        {
            WorkflowInstanceTerminatedRecordWithId(InstanceId, RecordNumber, EventTime, ActivityDefinitionId, Reason, Annotations, ProfileName, WorkflowDefinitionIdentity, "", "");
        }

        public bool WorkflowInstanceUnhandledExceptionRecordWithIdIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(EventIds.WorkflowInstanceUnhandledExceptionRecordWithId, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = Opcodes.WorkflowInstanceRecordUnhandledExceptionWithId, Task = Tasks.WorkflowInstanceRecord,
            Keywords = Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord = WorkflowInstanceUnhandledExceptionRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, SourceName = {4}, SourceId = {5}, SourceInstanceId = {6}, SourceTypeName={7}, Exception={8},  Annotations= {9}, ProfileName = {10}, WorkflowDefinitionIdentity = {11}")]
        public void WorkflowInstanceUnhandledExceptionRecordWithId(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string SourceName, string SourceId, string SourceInstanceId, string SourceTypeName, string Exception, string Annotations, string ProfileName, string WorkflowDefinitionIdentity, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowInstanceUnhandledExceptionRecordWithId, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, SourceName, SourceId, SourceInstanceId, SourceTypeName, Exception, Annotations, ProfileName, WorkflowDefinitionIdentity, HostReference, AppDomain);
        }

        [NonEvent]
        public void WorkflowInstanceUnhandledExceptionRecordWithId(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string SourceName, string SourceId, string SourceInstanceId, string SourceTypeName, string Exception, string Annotations, string ProfileName, string WorkflowDefinitionIdentity)
        {
            WorkflowInstanceUnhandledExceptionRecordWithId(InstanceId, RecordNumber, EventTime, ActivityDefinitionId, SourceName, SourceId, SourceInstanceId, SourceTypeName, Exception, Annotations, ProfileName, WorkflowDefinitionIdentity, "", "");
        }

        public bool WorkflowInstanceUpdatedRecordIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.HealthMonitoring | Keywords.WFTracking, EventChannel.Analytic);
        }

        [Event(EventIds.WorkflowInstanceUpdatedRecord, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.WorkflowInstanceRecordUpdatedRecord, Task = Tasks.WorkflowInstanceRecord,
            Keywords = Keywords.HealthMonitoring | Keywords.WFTracking,
            Message = "TrackRecord= WorkflowInstanceUpdatedRecord, InstanceID = {0}, RecordNumber = {1}, EventTime = {2}, ActivityDefinitionId = {3}, State = {4}, OriginalDefinitionIdentity = {5}, UpdatedDefinitionIdentity = {6}, Annotations = {7}, ProfileName = {8}")]
        public void WorkflowInstanceUpdatedRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string State, string OriginalDefinitionIdentity, string UpdatedDefinitionIdentity, string Annotations, string ProfileName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowInstanceUpdatedRecord, InstanceId, RecordNumber, EventTime, ActivityDefinitionId, State, OriginalDefinitionIdentity, UpdatedDefinitionIdentity, Annotations, ProfileName, HostReference, AppDomain);
        }

        [NonEvent]
        public void WorkflowInstanceUpdatedRecord(Guid InstanceId, long RecordNumber, DateTime EventTime, string ActivityDefinitionId, string State, string OriginalDefinitionIdentity, string UpdatedDefinitionIdentity, string Annotations, string ProfileName)
        {
            WorkflowInstanceUpdatedRecord(InstanceId, RecordNumber, EventTime, ActivityDefinitionId, State, OriginalDefinitionIdentity, UpdatedDefinitionIdentity, Annotations, ProfileName, "", "");
        }

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

        public bool MessageInspectorAfterReceiveInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.MessageInspectorAfterReceiveInvoked, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.DispatchMessageDispathMessageInspectorAfterReceiveInvoked, Task = Tasks.DispatchMessage,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'AfterReceiveReply' on a MessageInspector of type '{0}'.")]
        public void MessageInspectorAfterReceiveInvoked(string TypeName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.MessageInspectorAfterReceiveInvoked, TypeName, HostReference, AppDomain);
        }

        [NonEvent]
        public void MessageInspectorAfterReceiveInvoked(string TypeName)
        {
            MessageInspectorAfterReceiveInvoked(TypeName, "", "");
        }

        public bool MessageInspectorBeforeSendInvokedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.MessageInspectorBeforeSendInvoked, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.DispatchMessageDispathMessageInspectorBeforeSendInvoked, Task = Tasks.DispatchMessage,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "The Dispatcher invoked 'BeforeSendRequest' on a MessageInspector of type '{0}'.")]
        public void MessageInspectorBeforeSendInvoked(string TypeName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.MessageInspectorBeforeSendInvoked, TypeName, HostReference, AppDomain);
        }

        [NonEvent]
        public void MessageInspectorBeforeSendInvoked(string TypeName)
        {
            MessageInspectorBeforeSendInvoked(TypeName, "", "");
        }

        public bool MessageThrottleExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.Quota | Keywords.HealthMonitoring | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.MessageThrottleExceeded, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.Quota | Keywords.HealthMonitoring | Keywords.ServiceModel,
            Message = "The '{0}' throttle limit of '{1}' was hit.")]
        public void MessageThrottleExceeded(string ThrottleName, long Limit, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.MessageThrottleExceeded, ThrottleName, Limit, HostReference, AppDomain);
        }

        [NonEvent]
        public void MessageThrottleExceeded(string ThrottleName, long Limit)
        {
            MessageThrottleExceeded(ThrottleName, Limit, "", "");
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

        public bool ServiceHostStartedIsEnabled()
        {
            return base.IsEnabled(EventLevel.LogAlways, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceHost, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceHostStarted, Level = EventLevel.LogAlways, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.ServiceHostActivation,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.HealthMonitoring | Keywords.ServiceHost,
            Message = "ServiceHost started: '{0}'.")]
        public void ServiceHostStarted(string ServiceTypeName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.ServiceHostStarted, ServiceTypeName, HostReference, AppDomain);
        }

        [NonEvent]
        public void ServiceHostStarted(string ServiceTypeName)
        {
            ServiceHostStarted(ServiceTypeName, "", "");
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

        public bool MessageThrottleAtSeventyPercentIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.Quota | Keywords.HealthMonitoring | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.MessageThrottleAtSeventyPercent, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.DispatchMessage,
            Keywords = Keywords.EndToEndMonitoring | Keywords.Troubleshooting | Keywords.Quota | Keywords.HealthMonitoring | Keywords.ServiceModel,
            Message = "The '{0}' throttle limit of '{1}' is at 70%%.")]
        public void MessageThrottleAtSeventyPercent(string ThrottleName, long Limit, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.MessageThrottleAtSeventyPercent, ThrottleName, Limit, HostReference, AppDomain);
        }

        [NonEvent]
        public void MessageThrottleAtSeventyPercent(string ThrottleName, long Limit)
        {
            MessageThrottleAtSeventyPercent(ThrottleName, Limit, "", "");
        }

        public bool TraceCorrelationKeysIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.TraceCorrelationKeys, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Correlation,
            Keywords = Keywords.Troubleshooting | Keywords.WFServices,
            Message = "Calculated correlation key '{0}' using values '{1}' in parent scope '{2}'.")]
        public void TraceCorrelationKeys(Guid InstanceKey, string Values, string ParentScope, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.TraceCorrelationKeys, InstanceKey, Values, ParentScope, HostReference, AppDomain);
        }

        [NonEvent]
        public void TraceCorrelationKeys(Guid InstanceKey, string Values, string ParentScope)
        {
            TraceCorrelationKeys(InstanceKey, Values, ParentScope, "", "");
        }

        public bool IdleServicesClosedIsEnabled()
        {
            return base.IsEnabled(EventLevel.LogAlways, Keywords.HealthMonitoring | Keywords.WebHost, EventChannel.Analytic);
        }

        [Event(EventIds.IdleServicesClosed, Level = EventLevel.LogAlways, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.ServiceShutdown,
            Keywords = Keywords.HealthMonitoring | Keywords.WebHost,
            Message = "{0} idle services out of total {1} activated services closed.")]
        public void IdleServicesClosed(int ClosedCount, int TotalCount, string AppDomain)
        {
            WriteEvent(EventIds.IdleServicesClosed, ClosedCount, TotalCount, AppDomain);
        }

        [NonEvent]
        public void IdleServicesClosed(int ClosedCount, int TotalCount)
        {
            IdleServicesClosed(ClosedCount, TotalCount, "");
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

        [NonEvent]
        public void UserDefinedErrorOccurred(string Name, string Payload)
        {
            UserDefinedErrorOccurred(Name, "", Payload);
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

        [NonEvent]
        public void UserDefinedWarningOccurred(string Name, string Payload)
        {
            UserDefinedWarningOccurred(Name, "", Payload);
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

        [NonEvent]
        public void UserDefinedInformationEventOccured(string Name, string Payload)
        {
            UserDefinedInformationEventOccured(Name, "", Payload);
        }

        public bool StopSignpostEventIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.StopSignpostEvent, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.Signpost,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "Activity boundary.")]
        public void StopSignpostEvent(string ExtendedData, string AppDomain)
        {
            WriteEvent(EventIds.StopSignpostEvent, ExtendedData, AppDomain);
        }

        [NonEvent]
        public void StopSignpostEvent(string ExtendedData)
        {
            StopSignpostEvent(ExtendedData, "");
        }

        public bool StartSignpostEventIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.StartSignpostEvent, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.Signpost,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "Activity boundary.")]
        public void StartSignpostEvent(string ExtendedData, string AppDomain)
        {
            WriteEvent(EventIds.StartSignpostEvent, ExtendedData, AppDomain);
        }

        [NonEvent]
        public void StartSignpostEvent(string ExtendedData)
        {
            StartSignpostEvent(ExtendedData, "");
        }

        public bool SuspendSignpostEventIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.SuspendSignpostEvent, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Suspend, Task = Tasks.Signpost,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "Activity boundary.")]
        public void SuspendSignpostEvent(string ExtendedData, string AppDomain)
        {
            WriteEvent(EventIds.SuspendSignpostEvent, ExtendedData, AppDomain);
        }

        [NonEvent]
        public void SuspendSignpostEvent(string ExtendedData)
        {
            SuspendSignpostEvent(ExtendedData, "");
        }

        public bool ResumeSignpostEventIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ResumeSignpostEvent, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Resume, Task = Tasks.Signpost,
            Keywords = Keywords.Troubleshooting | Keywords.ServiceModel,
            Message = "Activity boundary.")]
        public void ResumeSignpostEvent(string ExtendedData, string AppDomain)
        {
            WriteEvent(EventIds.ResumeSignpostEvent, ExtendedData, AppDomain);
        }

        [NonEvent]
        public void ResumeSignpostEvent(string ExtendedData)
        {
            ResumeSignpostEvent(ExtendedData, "");
        }

        public bool StartSignpostEvent1IsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.StartSignpostEvent1, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.Signpost,
            Keywords = Keywords.Troubleshooting | Keywords.WFServices,
            Message = "Activity boundary.")]
        public void StartSignpostEvent1(string ExtendedData, string AppDomain)
        {
            WriteEvent(EventIds.StartSignpostEvent1, ExtendedData, AppDomain);
        }

        [NonEvent]
        public void StartSignpostEvent1(string ExtendedData)
        {
            StartSignpostEvent1(ExtendedData, "");
        }

        public bool StopSignpostEvent1IsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.StopSignpostEvent1, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.Signpost,
            Keywords = Keywords.Troubleshooting | Keywords.WFServices,
            Message = "Activity boundary.")]
        public void StopSignpostEvent1(string ExtendedData, string AppDomain)
        {
            WriteEvent(EventIds.StopSignpostEvent1, ExtendedData, AppDomain);
        }

        [NonEvent]
        public void StopSignpostEvent1(string ExtendedData)
        {
            StopSignpostEvent1(ExtendedData, "");
        }

        public bool MessageLogInfoIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Troubleshooting | Keywords.WCFMessageLogging, EventChannel.Analytic);
        }

        [Event(EventIds.MessageLogInfo, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.WCFMessageLogging,
            Message = "{0}")]
        public void MessageLogInfo(string data1, string AppDomain)
        {
            WriteEvent(EventIds.MessageLogInfo, data1, AppDomain);
        }

        [NonEvent]
        public void MessageLogInfo(string data1)
        {
            MessageLogInfo(data1, "");
        }

        public bool MessageLogWarningIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Troubleshooting | Keywords.WCFMessageLogging, EventChannel.Analytic);
        }

        [Event(EventIds.MessageLogWarning, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.WCFMessageLogging,
            Message = "{0}")]
        public void MessageLogWarning(string data1, string AppDomain)
        {
            WriteEvent(EventIds.MessageLogWarning, data1, AppDomain);
        }

        [NonEvent]
        public void MessageLogWarning(string data1)
        {
            MessageLogWarning(data1, "");
        }

        public bool TransferEmittedIsEnabled()
        {
            return base.IsEnabled(EventLevel.LogAlways, Keywords.Troubleshooting | Keywords.UserEvents | Keywords.EndToEndMonitoring | Keywords.ServiceModel | Keywords.WFTracking | Keywords.ServiceHost | Keywords.WCFMessageLogging, EventChannel.Analytic);
        }

        [Event(EventIds.TransferEmitted, Level = EventLevel.LogAlways, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Troubleshooting | Keywords.UserEvents | Keywords.EndToEndMonitoring | Keywords.ServiceModel | Keywords.WFTracking | Keywords.ServiceHost | Keywords.WCFMessageLogging,
            Message = "Transfer event emitted.")]
        public void TransferEmitted(string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.TransferEmitted, HostReference, AppDomain);
        }

        [NonEvent]
        public void TransferEmitted()
        {
            TransferEmitted("", "");
        }

        public bool CompilationStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.CompilationStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ServiceHostCompilation,
            Keywords = Keywords.WebHost,
            Message = "Begin compilation")]
        public void CompilationStart(string AppDomain)
        {
            WriteEvent(EventIds.CompilationStart, AppDomain);
        }

        [NonEvent]
        public void CompilationStart()
        {
            CompilationStart("");
        }

        public bool CompilationStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.CompilationStop, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ServiceHostCompilation,
            Keywords = Keywords.WebHost,
            Message = "End compilation")]
        public void CompilationStop(string AppDomain)
        {
            WriteEvent(EventIds.CompilationStop, AppDomain);
        }

        [NonEvent]
        public void CompilationStop()
        {
            CompilationStop("");
        }

        public bool ServiceHostFactoryCreationStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.ServiceHostFactoryCreationStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ServiceHostFactoryCreation,
            Keywords = Keywords.WebHost,
            Message = "ServiceHostFactory begin creation")]
        public void ServiceHostFactoryCreationStart(string AppDomain)
        {
            WriteEvent(EventIds.ServiceHostFactoryCreationStart, AppDomain);
        }

        [NonEvent]
        public void ServiceHostFactoryCreationStart()
        {
            ServiceHostFactoryCreationStart("");
        }

        public bool ServiceHostFactoryCreationStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.ServiceHostFactoryCreationStop, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ServiceHostFactoryCreation,
            Keywords = Keywords.WebHost,
            Message = "ServiceHostFactory end creation")]
        public void ServiceHostFactoryCreationStop(string AppDomain)
        {
            WriteEvent(EventIds.ServiceHostFactoryCreationStop, AppDomain);
        }

        [NonEvent]
        public void ServiceHostFactoryCreationStop()
        {
            ServiceHostFactoryCreationStop("");
        }

        public bool CreateServiceHostStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.CreateServiceHostStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ServiceHostCreate,
            Keywords = Keywords.WebHost,
            Message = "Begin CreateServiceHost")]
        public void CreateServiceHostStart(string AppDomain)
        {
            WriteEvent(EventIds.CreateServiceHostStart, AppDomain);
        }

        [NonEvent]
        public void CreateServiceHostStart()
        {
            CreateServiceHostStart("");
        }

        public bool CreateServiceHostStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.CreateServiceHostStop, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ServiceHostCreate,
            Keywords = Keywords.WebHost,
            Message = "End CreateServiceHost")]
        public void CreateServiceHostStop(string AppDomain)
        {
            WriteEvent(EventIds.CreateServiceHostStop, AppDomain);
        }

        [NonEvent]
        public void CreateServiceHostStop()
        {
            CreateServiceHostStop("");
        }

        public bool HostedTransportConfigurationManagerConfigInitStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.HostedTransportConfigurationManagerConfigInitStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.HostedTransportConfigurationManagerConfigInit,
            Keywords = Keywords.WebHost,
            Message = "HostedTransportConfigurationManager begin configuration initialization")]
        public void HostedTransportConfigurationManagerConfigInitStart(string AppDomain)
        {
            WriteEvent(EventIds.HostedTransportConfigurationManagerConfigInitStart, AppDomain);
        }

        [NonEvent]
        public void HostedTransportConfigurationManagerConfigInitStart()
        {
            HostedTransportConfigurationManagerConfigInitStart("");
        }

        public bool HostedTransportConfigurationManagerConfigInitStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.HostedTransportConfigurationManagerConfigInitStop, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.HostedTransportConfigurationManagerConfigInit,
            Keywords = Keywords.WebHost,
            Message = "HostedTransportConfigurationManager end configuration initialization")]
        public void HostedTransportConfigurationManagerConfigInitStop(string AppDomain)
        {
            WriteEvent(EventIds.HostedTransportConfigurationManagerConfigInitStop, AppDomain);
        }

        [NonEvent]
        public void HostedTransportConfigurationManagerConfigInitStop()
        {
            HostedTransportConfigurationManagerConfigInitStop("");
        }

        public bool ServiceHostOpenStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceHost, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceHostOpenStart, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.ServiceHostOpen,
            Keywords = Keywords.ServiceHost,
            Message = "ServiceHost Open started.")]
        public void ServiceHostOpenStart(string AppDomain)
        {
            WriteEvent(EventIds.ServiceHostOpenStart, AppDomain);
        }

        [NonEvent]
        public void ServiceHostOpenStart()
        {
            ServiceHostOpenStart("");
        }

        public bool ServiceHostOpenStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceHost, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceHostOpenStop, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.ServiceHostOpen,
            Keywords = Keywords.ServiceHost,
            Message = "ServiceHost Open completed.")]
        public void ServiceHostOpenStop(string AppDomain)
        {
            WriteEvent(EventIds.ServiceHostOpenStop, AppDomain);
        }

        [NonEvent]
        public void ServiceHostOpenStop()
        {
            ServiceHostOpenStop("");
        }

        public bool WebHostRequestStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.WebHostRequestStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.WebHostRequest,
            Keywords = Keywords.WebHost,
            Message = "Received request with virtual path '{1}' from the AppDomain '{0}'.")]
        public void WebHostRequestStart(string AppDomainFriendlyName, string VirtualPath, string AppDomain)
        {
            WriteEvent(EventIds.WebHostRequestStart, AppDomainFriendlyName, VirtualPath, AppDomain);
        }

        [NonEvent]
        public void WebHostRequestStart(string VirtualPath)
        {
            WebHostRequestStart("", VirtualPath, "");
        }

        public bool WebHostRequestStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.WebHostRequestStop, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.WebHostRequest,
            Keywords = Keywords.WebHost,
            Message = "WebHostRequest stop.")]
        public void WebHostRequestStop(string AppDomain)
        {
            WriteEvent(EventIds.WebHostRequestStop, AppDomain);
        }

        [NonEvent]
        public void WebHostRequestStop()
        {
            WebHostRequestStop("");
        }

        public bool CBAEntryReadIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, EventKeywords.None, EventChannel.Debug);
        }

        [Event(EventIds.CBAEntryRead, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = EventKeywords.None,
            Message = "Processed ServiceActivation Element Relative Address:'{0}', Normalized Relative Address '{1}' .")]
        public void CBAEntryRead(string RelativeAddress, string NormalizedAddress, string AppDomain)
        {
            WriteEvent(EventIds.CBAEntryRead, RelativeAddress, NormalizedAddress, AppDomain);
        }

        [NonEvent]
        public void CBAEntryRead(string RelativeAddress, string NormalizedAddress)
        {
            CBAEntryRead(RelativeAddress, NormalizedAddress, "");
        }

        public bool CBAMatchFoundIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, EventKeywords.None, EventChannel.Debug);
        }

        [Event(EventIds.CBAMatchFound, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = EventKeywords.None,
            Message = "Incoming request matches a ServiceActivation element with address '{0}'. ")]
        public void CBAMatchFound(string IncomingAddress, string AppDomain)
        {
            WriteEvent(EventIds.CBAMatchFound, IncomingAddress, AppDomain);
        }

        [NonEvent]
        public void CBAMatchFound(string IncomingAddress)
        {
            CBAMatchFound(IncomingAddress, "");
        }

        public bool AspNetRoutingServiceIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.AspNetRoutingService, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.RoutingServices,
            Message = "Incoming request matches a WCF Service defined in Asp.Net route with address {0}.  ")]
        public void AspNetRoutingService(string IncomingAddress, string AppDomain)
        {
            WriteEvent(EventIds.AspNetRoutingService, IncomingAddress, AppDomain);
        }

        [NonEvent]
        public void AspNetRoutingService(string IncomingAddress)
        {
            AspNetRoutingService(IncomingAddress, "");
        }

        public bool AspNetRouteIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.AspNetRoute, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.RoutingServices,
            Message = "A new Asp.Net route '{0}' with serviceType '{1}' and serviceHostFactoryType '{2}' is added.")]
        public void AspNetRoute(string AspNetRoutePrefix, string ServiceType, string ServiceHostFactoryType, string AppDomain)
        {
            WriteEvent(EventIds.AspNetRoute, AspNetRoutePrefix, ServiceType, ServiceHostFactoryType, AppDomain);
        }

        [NonEvent]
        public void AspNetRoute(string AspNetRoutePrefix, string ServiceType, string ServiceHostFactoryType)
        {
            AspNetRoute(AspNetRoutePrefix, ServiceType, ServiceHostFactoryType, "");
        }

        public bool IncrementBusyCountIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.IncrementBusyCount, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.WebHostRequest,
            Keywords = Keywords.WebHost,
            Message = "IncrementBusyCount called. Source : {0}")]
        public void IncrementBusyCount(string Data, string AppDomain)
        {
            WriteEvent(EventIds.IncrementBusyCount, Data, AppDomain);
        }

        [NonEvent]
        public void IncrementBusyCount(string Data)
        {
            IncrementBusyCount(Data, "");
        }

        public bool DecrementBusyCountIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.DecrementBusyCount, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.WebHostRequest,
            Keywords = Keywords.WebHost,
            Message = "DecrementBusyCount called. Source : {0}")]
        public void DecrementBusyCount(string Data, string AppDomain)
        {
            WriteEvent(EventIds.DecrementBusyCount, Data, AppDomain);
        }

        [NonEvent]
        public void DecrementBusyCount(string Data)
        {
            DecrementBusyCount(Data, "");
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

        public bool ServiceChannelBeginCallStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
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

        public bool HttpSendMessageStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.HttpSendMessageStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportSend,
            Keywords = Keywords.HTTP,
            Message = "Http Send Request Start.")]
        public void HttpSendMessageStart(string AppDomain)
        {
            WriteEvent(EventIds.HttpSendMessageStart, AppDomain);
        }

        [NonEvent]
        public void HttpSendMessageStart()
        {
            HttpSendMessageStart("");
        }

        public bool HttpSendStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.HttpSendStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.TransportSend,
            Keywords = Keywords.HTTP,
            Message = "Http Send Request Stop.")]
        public void HttpSendStop(string AppDomain)
        {
            WriteEvent(EventIds.HttpSendStop, AppDomain);
        }

        [NonEvent]
        public void HttpSendStop()
        {
            HttpSendStop("");
        }

        public bool HttpMessageReceiveStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.HttpMessageReceiveStart, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "Message received from http transport.")]
        public void HttpMessageReceiveStart(string AppDomain)
        {
            WriteEvent(EventIds.HttpMessageReceiveStart, AppDomain);
        }

        [NonEvent]
        public void HttpMessageReceiveStart()
        {
            HttpMessageReceiveStart("");
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

        public bool HttpContextBeforeProcessAuthenticationIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.HttpContextBeforeProcessAuthentication, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.TransportReceiveBeforeAuthentication, Task = Tasks.TransportReceive,
            Keywords = Keywords.ServiceModel,
            Message = "Start authentication for message dispatching")]
        public void HttpContextBeforeProcessAuthentication(string AppDomain)
        {
            WriteEvent(EventIds.HttpContextBeforeProcessAuthentication, AppDomain);
        }

        [NonEvent]
        public void HttpContextBeforeProcessAuthentication()
        {
            HttpContextBeforeProcessAuthentication("");
        }

        public bool DispatchMessageBeforeAuthorizationIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.DispatchMessageBeforeAuthorization, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.DispatchMessageBeforeAuthorization, Task = Tasks.DispatchMessage,
            Keywords = Keywords.ServiceModel,
            Message = "Start authorization for message dispatching")]
        public void DispatchMessageBeforeAuthorization(string AppDomain)
        {
            WriteEvent(EventIds.DispatchMessageBeforeAuthorization, AppDomain);
        }

        [NonEvent]
        public void DispatchMessageBeforeAuthorization()
        {
            DispatchMessageBeforeAuthorization("");
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

        public bool HttpSendStreamedMessageStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.HttpSendStreamedMessageStart, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.TransportSend,
            Keywords = Keywords.HTTP,
            Message = "Http Send streamed message started.")]
        public void HttpSendStreamedMessageStart(string AppDomain)
        {
            WriteEvent(EventIds.HttpSendStreamedMessageStart, AppDomain);
        }

        [NonEvent]
        public void HttpSendStreamedMessageStart()
        {
            HttpSendStreamedMessageStart("");
        }

        public bool WorkflowApplicationCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowApplicationCompleted, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.WFApplicationStateChangeCompleted, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' has completed in the Closed state.")]
        public void WorkflowApplicationCompleted(string data1, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowApplicationCompleted, data1, AppDomain);
        }

        [NonEvent]
        public void WorkflowApplicationCompleted(string data1)
        {
            WorkflowApplicationCompleted(data1, "");
        }

        public bool WorkflowApplicationTerminatedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowApplicationTerminated, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.WFApplicationStateChangeTerminated, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowApplication Id: '{0}' was terminated. It has completed in the Faulted state with an exception.")]
        public void WorkflowApplicationTerminated(string data1, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowApplicationTerminated, data1, SerializedException, AppDomain);
        }

        [NonEvent]
        public void WorkflowApplicationTerminated(string data1, string SerializedException)
        {
            WorkflowApplicationTerminated(data1, SerializedException, "");
        }

        public bool WorkflowInstanceCanceledIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowInstanceCanceled, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.WFApplicationStateChangeInstanceCanceled, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' has completed in the Canceled state.")]
        public void WorkflowInstanceCanceled(string data1, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowInstanceCanceled, data1, AppDomain);
        }

        [NonEvent]
        public void WorkflowInstanceCanceled(string data1)
        {
            WorkflowInstanceCanceled(data1, "");
        }

        public bool WorkflowInstanceAbortedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowInstanceAborted, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.WFApplicationStateChangeInstanceAborted, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' was aborted with an exception.")]
        public void WorkflowInstanceAborted(string data1, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowInstanceAborted, data1, SerializedException, AppDomain);
        }

        [NonEvent]
        public void WorkflowInstanceAborted(string data1, string SerializedException)
        {
            WorkflowInstanceAborted(data1, SerializedException, "");
        }

        public bool WorkflowApplicationIdledIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowApplicationIdled, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.WFApplicationStateChangeIdled, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowApplication Id: '{0}' went idle.")]
        public void WorkflowApplicationIdled(string data1, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowApplicationIdled, data1, AppDomain);
        }

        [NonEvent]
        public void WorkflowApplicationIdled(string data1)
        {
            WorkflowApplicationIdled(data1, "");
        }

        public bool WorkflowApplicationUnhandledExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowApplicationUnhandledException, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = Opcodes.WFApplicationStateChangeUnhandledException, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' has encountered an unhandled exception.  The exception originated from Activity '{1}', DisplayName: '{2}'.  The following action will be taken: {3}.")]
        public void WorkflowApplicationUnhandledException(string data1, string data2, string data3, string data4, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowApplicationUnhandledException, data1, data2, data3, data4, SerializedException, AppDomain);
        }

        [NonEvent]
        public void WorkflowApplicationUnhandledException(string data1, string data2, string data3, string data4, string SerializedException)
        {
            WorkflowApplicationUnhandledException(data1, data2, data3, data4, SerializedException, "");
        }

        public bool WorkflowApplicationPersistedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowApplicationPersisted, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.WFApplicationStateChangePersisted, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowApplication Id: '{0}' was Persisted.")]
        public void WorkflowApplicationPersisted(string data1, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowApplicationPersisted, data1, AppDomain);
        }

        [NonEvent]
        public void WorkflowApplicationPersisted(string data1)
        {
            WorkflowApplicationPersisted(data1, "");
        }

        public bool WorkflowApplicationUnloadedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowApplicationUnloaded, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.WFApplicationStateChangeUnloaded, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' was Unloaded.")]
        public void WorkflowApplicationUnloaded(string data1, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowApplicationUnloaded, data1, AppDomain);
        }

        [NonEvent]
        public void WorkflowApplicationUnloaded(string data1)
        {
            WorkflowApplicationUnloaded(data1, "");
        }

        public bool ActivityScheduledIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ActivityScheduled, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ScheduleActivity,
            Keywords = Keywords.WFRuntime,
            Message = "Parent Activity '{0}', DisplayName: '{1}', InstanceId: '{2}' scheduled child Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void ActivityScheduled(string data1, string data2, string data3, string data4, string data5, string data6, string AppDomain)
        {
            WriteEvent(EventIds.ActivityScheduled, data1, data2, data3, data4, data5, data6, AppDomain);
        }

        [NonEvent]
        public void ActivityScheduled(string data1, string data2, string data3, string data4, string data5, string data6)
        {
            ActivityScheduled(data1, data2, data3, data4, data5, data6, "");
        }

        public bool ActivityCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ActivityCompleted, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.CompleteActivity,
            Keywords = Keywords.WFRuntime,
            Message = "Activity '{0}', DisplayName: '{1}', InstanceId: '{2}' has completed in the '{3}' state.")]
        public void ActivityCompleted(string data1, string data2, string data3, string data4, string AppDomain)
        {
            WriteEvent(EventIds.ActivityCompleted, data1, data2, data3, data4, AppDomain);
        }

        [NonEvent]
        public void ActivityCompleted(string data1, string data2, string data3, string data4)
        {
            ActivityCompleted(data1, data2, data3, data4, "");
        }

        public bool ScheduleExecuteActivityWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ScheduleExecuteActivityWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleWorkItemScheduleExecuteActivity, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "An ExecuteActivityWorkItem has been scheduled for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void ScheduleExecuteActivityWorkItem(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.ScheduleExecuteActivityWorkItem, data1, data2, data3, AppDomain);
        }

        [NonEvent]
        public void ScheduleExecuteActivityWorkItem(string data1, string data2, string data3)
        {
            ScheduleExecuteActivityWorkItem(data1, data2, data3, "");
        }

        public bool StartExecuteActivityWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.StartExecuteActivityWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.StartWorkItemStartExecuteActivity, Task = Tasks.StartWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Starting execution of an ExecuteActivityWorkItem for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void StartExecuteActivityWorkItem(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.StartExecuteActivityWorkItem, data1, data2, data3, AppDomain);
        }

        [NonEvent]
        public void StartExecuteActivityWorkItem(string data1, string data2, string data3)
        {
            StartExecuteActivityWorkItem(data1, data2, data3, "");
        }

        public bool CompleteExecuteActivityWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CompleteExecuteActivityWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompleteWorkItemCompleteExecuteActivity, Task = Tasks.CompleteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "An ExecuteActivityWorkItem has completed for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void CompleteExecuteActivityWorkItem(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.CompleteExecuteActivityWorkItem, data1, data2, data3, AppDomain);
        }

        [NonEvent]
        public void CompleteExecuteActivityWorkItem(string data1, string data2, string data3)
        {
            CompleteExecuteActivityWorkItem(data1, data2, data3, "");
        }

        public bool ScheduleCompletionWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ScheduleCompletionWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleWorkItemScheduleCompletion, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A CompletionWorkItem has been scheduled for parent Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.  Completed Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void ScheduleCompletionWorkItem(string data1, string data2, string data3, string data4, string data5, string data6, string AppDomain)
        {
            WriteEvent(EventIds.ScheduleCompletionWorkItem, data1, data2, data3, data4, data5, data6, AppDomain);
        }

        [NonEvent]
        public void ScheduleCompletionWorkItem(string data1, string data2, string data3, string data4, string data5, string data6)
        {
            ScheduleCompletionWorkItem(data1, data2, data3, data4, data5, data6, "");
        }

        public bool StartCompletionWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.StartCompletionWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.StartWorkItemStartCompletion, Task = Tasks.StartWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Starting execution of a CompletionWorkItem for parent Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'. Completed Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void StartCompletionWorkItem(string data1, string data2, string data3, string data4, string data5, string data6, string AppDomain)
        {
            WriteEvent(EventIds.StartCompletionWorkItem, data1, data2, data3, data4, data5, data6, AppDomain);
        }

        [NonEvent]
        public void StartCompletionWorkItem(string data1, string data2, string data3, string data4, string data5, string data6)
        {
            StartCompletionWorkItem(data1, data2, data3, data4, data5, data6, "");
        }

        public bool CompleteCompletionWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CompleteCompletionWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompleteWorkItemCompleteCompletion, Task = Tasks.CompleteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A CompletionWorkItem has completed for parent Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'. Completed Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void CompleteCompletionWorkItem(string data1, string data2, string data3, string data4, string data5, string data6, string AppDomain)
        {
            WriteEvent(EventIds.CompleteCompletionWorkItem, data1, data2, data3, data4, data5, data6, AppDomain);
        }

        [NonEvent]
        public void CompleteCompletionWorkItem(string data1, string data2, string data3, string data4, string data5, string data6)
        {
            CompleteCompletionWorkItem(data1, data2, data3, data4, data5, data6, "");
        }

        public bool ScheduleCancelActivityWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ScheduleCancelActivityWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleWorkItemScheduleCancelActivity, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A CancelActivityWorkItem has been scheduled for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void ScheduleCancelActivityWorkItem(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.ScheduleCancelActivityWorkItem, data1, data2, data3, AppDomain);
        }

        [NonEvent]
        public void ScheduleCancelActivityWorkItem(string data1, string data2, string data3)
        {
            ScheduleCancelActivityWorkItem(data1, data2, data3, "");
        }

        public bool StartCancelActivityWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.StartCancelActivityWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.StartWorkItemStartCancelActivity, Task = Tasks.StartWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Starting execution of a CancelActivityWorkItem for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void StartCancelActivityWorkItem(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.StartCancelActivityWorkItem, data1, data2, data3, AppDomain);
        }

        [NonEvent]
        public void StartCancelActivityWorkItem(string data1, string data2, string data3)
        {
            StartCancelActivityWorkItem(data1, data2, data3, "");
        }

        public bool CompleteCancelActivityWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CompleteCancelActivityWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompleteWorkItemCompleteCancelActivity, Task = Tasks.CompleteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A CancelActivityWorkItem has completed for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void CompleteCancelActivityWorkItem(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.CompleteCancelActivityWorkItem, data1, data2, data3, AppDomain);
        }

        [NonEvent]
        public void CompleteCancelActivityWorkItem(string data1, string data2, string data3)
        {
            CompleteCancelActivityWorkItem(data1, data2, data3, "");
        }

        public bool CreateBookmarkIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CreateBookmark, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.CreateBookmark,
            Keywords = Keywords.WFRuntime,
            Message = "A Bookmark has been created for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.  BookmarkName: {3}, BookmarkScope: {4}.")]
        public void CreateBookmark(string data1, string data2, string data3, string data4, string data5, string AppDomain)
        {
            WriteEvent(EventIds.CreateBookmark, data1, data2, data3, data4, data5, AppDomain);
        }

        [NonEvent]
        public void CreateBookmark(string data1, string data2, string data3, string data4, string data5)
        {
            CreateBookmark(data1, data2, data3, data4, data5, "");
        }

        public bool ScheduleBookmarkWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ScheduleBookmarkWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleWorkItemScheduleBookmark, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A BookmarkWorkItem has been scheduled for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.  BookmarkName: {3}, BookmarkScope: {4}.")]
        public void ScheduleBookmarkWorkItem(string data1, string data2, string data3, string data4, string data5, string AppDomain)
        {
            WriteEvent(EventIds.ScheduleBookmarkWorkItem, data1, data2, data3, data4, data5, AppDomain);
        }

        [NonEvent]
        public void ScheduleBookmarkWorkItem(string data1, string data2, string data3, string data4, string data5)
        {
            ScheduleBookmarkWorkItem(data1, data2, data3, data4, data5, "");
        }

        public bool StartBookmarkWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.StartBookmarkWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.StartWorkItemStartBookmark, Task = Tasks.StartWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Starting execution of a BookmarkWorkItem for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.  BookmarkName: {3}, BookmarkScope: {4}.")]
        public void StartBookmarkWorkItem(string data1, string data2, string data3, string data4, string data5, string AppDomain)
        {
            WriteEvent(EventIds.StartBookmarkWorkItem, data1, data2, data3, data4, data5, AppDomain);
        }

        [NonEvent]
        public void StartBookmarkWorkItem(string data1, string data2, string data3, string data4, string data5)
        {
            StartBookmarkWorkItem(data1, data2, data3, data4, data5, "");
        }

        public bool CompleteBookmarkWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CompleteBookmarkWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompleteWorkItemCompleteBookmark, Task = Tasks.CompleteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A BookmarkWorkItem has completed for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'. BookmarkName: {3}, BookmarkScope: {4}.")]
        public void CompleteBookmarkWorkItem(string data1, string data2, string data3, string data4, string data5, string AppDomain)
        {
            WriteEvent(EventIds.CompleteBookmarkWorkItem, data1, data2, data3, data4, data5, AppDomain);
        }

        [NonEvent]
        public void CompleteBookmarkWorkItem(string data1, string data2, string data3, string data4, string data5)
        {
            CompleteBookmarkWorkItem(data1, data2, data3, data4, data5, "");
        }

        public bool CreateBookmarkScopeIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CreateBookmarkScope, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.CreateBookmark,
            Keywords = Keywords.WFRuntime,
            Message = "A BookmarkScope has been created: {0}.")]
        public void CreateBookmarkScope(string data1, string AppDomain)
        {
            WriteEvent(EventIds.CreateBookmarkScope, data1, AppDomain);
        }

        [NonEvent]
        public void CreateBookmarkScope(string data1)
        {
            CreateBookmarkScope(data1, "");
        }

        public bool BookmarkScopeInitializedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.BookmarkScopeInitialized, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.InitializeBookmarkScope,
            Keywords = Keywords.WFRuntime,
            Message = "The BookmarkScope that had TemporaryId: '{0}' has been initialized with Id: '{1}'.")]
        public void BookmarkScopeInitialized(string data1, string data2, string AppDomain)
        {
            WriteEvent(EventIds.BookmarkScopeInitialized, data1, data2, AppDomain);
        }

        [NonEvent]
        public void BookmarkScopeInitialized(string data1, string data2)
        {
            BookmarkScopeInitialized(data1, data2, "");
        }

        public bool ScheduleTransactionContextWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ScheduleTransactionContextWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleWorkItemScheduleTransactionContext, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A TransactionContextWorkItem has been scheduled for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void ScheduleTransactionContextWorkItem(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.ScheduleTransactionContextWorkItem, data1, data2, data3, AppDomain);
        }

        [NonEvent]
        public void ScheduleTransactionContextWorkItem(string data1, string data2, string data3)
        {
            ScheduleTransactionContextWorkItem(data1, data2, data3, "");
        }

        public bool StartTransactionContextWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.StartTransactionContextWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.StartWorkItemStartTransactionContext, Task = Tasks.StartWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Starting execution of a TransactionContextWorkItem for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void StartTransactionContextWorkItem(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.StartTransactionContextWorkItem, data1, data2, data3, AppDomain);
        }

        [NonEvent]
        public void StartTransactionContextWorkItem(string data1, string data2, string data3)
        {
            StartTransactionContextWorkItem(data1, data2, data3, "");
        }

        public bool CompleteTransactionContextWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CompleteTransactionContextWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompleteWorkItemCompleteTransactionContext, Task = Tasks.CompleteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A TransactionContextWorkItem has completed for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void CompleteTransactionContextWorkItem(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.CompleteTransactionContextWorkItem, data1, data2, data3, AppDomain);
        }

        [NonEvent]
        public void CompleteTransactionContextWorkItem(string data1, string data2, string data3)
        {
            CompleteTransactionContextWorkItem(data1, data2, data3, "");
        }

        public bool ScheduleFaultWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ScheduleFaultWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleWorkItemScheduleFault, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A FaultWorkItem has been scheduled for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.  The exception was propagated from Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void ScheduleFaultWorkItem(string data1, string data2, string data3, string data4, string data5, string data6, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.ScheduleFaultWorkItem, data1, data2, data3, data4, data5, data6, SerializedException, AppDomain);
        }

        [NonEvent]
        public void ScheduleFaultWorkItem(string data1, string data2, string data3, string data4, string data5, string data6, string SerializedException)
        {
            ScheduleFaultWorkItem(data1, data2, data3, data4, data5, data6, SerializedException, "");
        }

        public bool StartFaultWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.StartFaultWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.StartWorkItemStartFault, Task = Tasks.StartWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Starting execution of a FaultWorkItem for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.  The exception was propagated from Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void StartFaultWorkItem(string data1, string data2, string data3, string data4, string data5, string data6, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.StartFaultWorkItem, data1, data2, data3, data4, data5, data6, SerializedException, AppDomain);
        }

        [NonEvent]
        public void StartFaultWorkItem(string data1, string data2, string data3, string data4, string data5, string data6, string SerializedException)
        {
            StartFaultWorkItem(data1, data2, data3, data4, data5, data6, SerializedException, "");
        }

        public bool CompleteFaultWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CompleteFaultWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompleteWorkItemCompleteFault, Task = Tasks.CompleteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A FaultWorkItem has completed for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'. The exception was propagated from Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void CompleteFaultWorkItem(string data1, string data2, string data3, string data4, string data5, string data6, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.CompleteFaultWorkItem, data1, data2, data3, data4, data5, data6, SerializedException, AppDomain);
        }

        [NonEvent]
        public void CompleteFaultWorkItem(string data1, string data2, string data3, string data4, string data5, string data6, string SerializedException)
        {
            CompleteFaultWorkItem(data1, data2, data3, data4, data5, data6, SerializedException, "");
        }

        public bool ScheduleRuntimeWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ScheduleRuntimeWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ScheduleWorkItemScheduleRuntime, Task = Tasks.ScheduleWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A runtime work item has been scheduled for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void ScheduleRuntimeWorkItem(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.ScheduleRuntimeWorkItem, data1, data2, data3, AppDomain);
        }

        [NonEvent]
        public void ScheduleRuntimeWorkItem(string data1, string data2, string data3)
        {
            ScheduleRuntimeWorkItem(data1, data2, data3, "");
        }

        public bool StartRuntimeWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.StartRuntimeWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.StartWorkItemStartRuntime, Task = Tasks.StartWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Starting execution of a runtime work item for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void StartRuntimeWorkItem(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.StartRuntimeWorkItem, data1, data2, data3, AppDomain);
        }

        [NonEvent]
        public void StartRuntimeWorkItem(string data1, string data2, string data3)
        {
            StartRuntimeWorkItem(data1, data2, data3, "");
        }

        public bool CompleteRuntimeWorkItemIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CompleteRuntimeWorkItem, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.CompleteWorkItemCompleteRuntime, Task = Tasks.CompleteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "A runtime work item has completed for Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.")]
        public void CompleteRuntimeWorkItem(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.CompleteRuntimeWorkItem, data1, data2, data3, AppDomain);
        }

        [NonEvent]
        public void CompleteRuntimeWorkItem(string data1, string data2, string data3)
        {
            CompleteRuntimeWorkItem(data1, data2, data3, "");
        }

        public bool RuntimeTransactionSetIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFServices, EventChannel.Debug);
        }

        [Event(EventIds.RuntimeTransactionSet, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.RuntimeTransactionSet, Task = Tasks.RuntimeTransaction,
            Keywords = Keywords.WFServices,
            Message = "The runtime transaction has been set by Activity '{0}', DisplayName: '{1}', InstanceId: '{2}'.  Execution isolated to Activity '{3}', DisplayName: '{4}', InstanceId: '{5}'.")]
        public void RuntimeTransactionSet(string data1, string data2, string data3, string data4, string data5, string data6, string AppDomain)
        {
            WriteEvent(EventIds.RuntimeTransactionSet, data1, data2, data3, data4, data5, data6, AppDomain);
        }

        [NonEvent]
        public void RuntimeTransactionSet(string data1, string data2, string data3, string data4, string data5, string data6)
        {
            RuntimeTransactionSet(data1, data2, data3, data4, data5, data6, "");
        }

        public bool RuntimeTransactionCompletionRequestedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFServices, EventChannel.Debug);
        }

        [Event(EventIds.RuntimeTransactionCompletionRequested, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.RuntimeTransactionCompletionRequested, Task = Tasks.RuntimeTransaction,
            Keywords = Keywords.WFServices,
            Message = "Activity '{0}', DisplayName: '{1}', InstanceId: '{2}' has scheduled completion of the runtime transaction.")]
        public void RuntimeTransactionCompletionRequested(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.RuntimeTransactionCompletionRequested, data1, data2, data3, AppDomain);
        }

        [NonEvent]
        public void RuntimeTransactionCompletionRequested(string data1, string data2, string data3)
        {
            RuntimeTransactionCompletionRequested(data1, data2, data3, "");
        }

        public bool RuntimeTransactionCompleteIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFServices, EventChannel.Debug);
        }

        [Event(EventIds.RuntimeTransactionComplete, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.RuntimeTransactionComplete, Task = Tasks.RuntimeTransaction,
            Keywords = Keywords.WFServices,
            Message = "The runtime transaction has completed with the state '{0}'.")]
        public void RuntimeTransactionComplete(string data1, string AppDomain)
        {
            WriteEvent(EventIds.RuntimeTransactionComplete, data1, AppDomain);
        }

        [NonEvent]
        public void RuntimeTransactionComplete(string data1)
        {
            RuntimeTransactionComplete(data1, "");
        }

        public bool EnterNoPersistBlockIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.EnterNoPersistBlock, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.NoPersistBlock,
            Keywords = Keywords.WFRuntime,
            Message = "Entering a no persist block.")]
        public void EnterNoPersistBlock(string AppDomain)
        {
            WriteEvent(EventIds.EnterNoPersistBlock, AppDomain);
        }

        [NonEvent]
        public void EnterNoPersistBlock()
        {
            EnterNoPersistBlock("");
        }

        public bool ExitNoPersistBlockIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ExitNoPersistBlock, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.NoPersistBlock,
            Keywords = Keywords.WFRuntime,
            Message = "Exiting a no persist block.")]
        public void ExitNoPersistBlock(string AppDomain)
        {
            WriteEvent(EventIds.ExitNoPersistBlock, AppDomain);
        }

        [NonEvent]
        public void ExitNoPersistBlock()
        {
            ExitNoPersistBlock("");
        }

        public bool InArgumentBoundIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.InArgumentBound, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ExecuteActivity,
            Keywords = Keywords.WFActivities,
            Message = "In argument '{0}' on Activity '{1}', DisplayName: '{2}', InstanceId: '{3}' has been bound with value: {4}.")]
        public void InArgumentBound(string data1, string data2, string data3, string data4, string data5, string AppDomain)
        {
            WriteEvent(EventIds.InArgumentBound, data1, data2, data3, data4, data5, AppDomain);
        }

        [NonEvent]
        public void InArgumentBound(string data1, string data2, string data3, string data4, string data5)
        {
            InArgumentBound(data1, data2, data3, data4, data5, "");
        }

        public bool WorkflowApplicationPersistableIdleIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowApplicationPersistableIdle, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.WFApplicationStateChangePersistableIdle, Task = Tasks.WFApplicationStateChange,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowApplication Id: '{0}' is idle and persistable.  The following action will be taken: {1}.")]
        public void WorkflowApplicationPersistableIdle(string data1, string data2, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowApplicationPersistableIdle, data1, data2, AppDomain);
        }

        [NonEvent]
        public void WorkflowApplicationPersistableIdle(string data1, string data2)
        {
            WorkflowApplicationPersistableIdle(data1, data2, "");
        }

        public bool WorkflowActivityStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowActivityStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.WorkflowActivity,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' E2E Activity")]
        public void WorkflowActivityStart(Guid Id, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowActivityStart, Id, AppDomain);
        }

        [NonEvent]
        public void WorkflowActivityStart(Guid Id)
        {
            WorkflowActivityStart(Id, "");
        }

        public bool WorkflowActivityStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowActivityStop, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.WorkflowActivity,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' E2E Activity")]
        public void WorkflowActivityStop(Guid Id, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowActivityStop, Id, AppDomain);
        }

        [NonEvent]
        public void WorkflowActivityStop(Guid Id)
        {
            WorkflowActivityStop(Id, "");
        }

        public bool WorkflowActivitySuspendIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowActivitySuspend, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Suspend, Task = Tasks.WorkflowActivity,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' E2E Activity")]
        public void WorkflowActivitySuspend(Guid Id, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowActivitySuspend, Id, AppDomain);
        }

        [NonEvent]
        public void WorkflowActivitySuspend(Guid Id)
        {
            WorkflowActivitySuspend(Id, "");
        }

        public bool WorkflowActivityResumeIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.WorkflowActivityResume, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Resume, Task = Tasks.WorkflowActivity,
            Keywords = Keywords.WFRuntime,
            Message = "WorkflowInstance Id: '{0}' E2E Activity")]
        public void WorkflowActivityResume(Guid Id, string AppDomain)
        {
            WriteEvent(EventIds.WorkflowActivityResume, Id, AppDomain);
        }

        [NonEvent]
        public void WorkflowActivityResume(Guid Id)
        {
            WorkflowActivityResume(Id, "");
        }

        public bool InvokeMethodIsStaticIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.InvokeMethodIsStatic, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.InvokeMethodIsStatic, Task = Tasks.InvokeMethod,
            Keywords = Keywords.WFRuntime,
            Message = "InvokeMethod '{0}' - method is Static.")]
        public void InvokeMethodIsStatic(string data1, string AppDomain)
        {
            WriteEvent(EventIds.InvokeMethodIsStatic, data1, AppDomain);
        }

        [NonEvent]
        public void InvokeMethodIsStatic(string data1)
        {
            InvokeMethodIsStatic(data1, "");
        }

        public bool InvokeMethodIsNotStaticIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.InvokeMethodIsNotStatic, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.InvokeMethodIsNotStatic, Task = Tasks.InvokeMethod,
            Keywords = Keywords.WFRuntime,
            Message = "InvokeMethod '{0}' - method is not Static.")]
        public void InvokeMethodIsNotStatic(string data1, string AppDomain)
        {
            WriteEvent(EventIds.InvokeMethodIsNotStatic, data1, AppDomain);
        }

        [NonEvent]
        public void InvokeMethodIsNotStatic(string data1)
        {
            InvokeMethodIsNotStatic(data1, "");
        }

        public bool InvokedMethodThrewExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.InvokedMethodThrewException, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.InvokeMethodThrewException, Task = Tasks.InvokeMethod,
            Keywords = Keywords.WFRuntime,
            Message = "An exception was thrown in the method called by the activity '{0}'. {1}")]
        public void InvokedMethodThrewException(string data1, string data2, string AppDomain)
        {
            WriteEvent(EventIds.InvokedMethodThrewException, data1, data2, AppDomain);
        }

        [NonEvent]
        public void InvokedMethodThrewException(string data1, string data2)
        {
            InvokedMethodThrewException(data1, data2, "");
        }

        public bool InvokeMethodUseAsyncPatternIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.InvokeMethodUseAsyncPattern, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.InvokeMethodUseAsyncPattern, Task = Tasks.InvokeMethod,
            Keywords = Keywords.WFRuntime,
            Message = "InvokeMethod '{0}' - method uses asynchronous pattern of '{1}' and '{2}'.")]
        public void InvokeMethodUseAsyncPattern(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.InvokeMethodUseAsyncPattern, data1, data2, data3, AppDomain);
        }

        [NonEvent]
        public void InvokeMethodUseAsyncPattern(string data1, string data2, string data3)
        {
            InvokeMethodUseAsyncPattern(data1, data2, data3, "");
        }

        public bool InvokeMethodDoesNotUseAsyncPatternIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.InvokeMethodDoesNotUseAsyncPattern, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.InvokeMethodDoesNotUseAsyncPattern, Task = Tasks.InvokeMethod,
            Keywords = Keywords.WFRuntime,
            Message = "InvokeMethod '{0}' - method does not use asynchronous pattern.")]
        public void InvokeMethodDoesNotUseAsyncPattern(string data1, string AppDomain)
        {
            WriteEvent(EventIds.InvokeMethodDoesNotUseAsyncPattern, data1, AppDomain);
        }

        [NonEvent]
        public void InvokeMethodDoesNotUseAsyncPattern(string data1)
        {
            InvokeMethodDoesNotUseAsyncPattern(data1, "");
        }

        public bool FlowchartStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.FlowchartStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ExecuteFlowchartBegin, Task = Tasks.ExecuteFlowchart,
            Keywords = Keywords.WFActivities,
            Message = "Flowchart '{0}' - Start has been scheduled.")]
        public void FlowchartStart(string data1, string AppDomain)
        {
            WriteEvent(EventIds.FlowchartStart, data1, AppDomain);
        }

        [NonEvent]
        public void FlowchartStart(string data1)
        {
            FlowchartStart(data1, "");
        }

        public bool FlowchartEmptyIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.FlowchartEmpty, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.ExecuteFlowchartEmpty, Task = Tasks.ExecuteFlowchart,
            Keywords = Keywords.WFActivities,
            Message = "Flowchart '{0}' - was executed with no Nodes.")]
        public void FlowchartEmpty(string data1, string AppDomain)
        {
            WriteEvent(EventIds.FlowchartEmpty, data1, AppDomain);
        }

        [NonEvent]
        public void FlowchartEmpty(string data1)
        {
            FlowchartEmpty(data1, "");
        }

        public bool FlowchartNextNullIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.FlowchartNextNull, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ExecuteFlowchartNextNull, Task = Tasks.ExecuteFlowchart,
            Keywords = Keywords.WFActivities,
            Message = "Flowchart '{0}'/FlowStep - Next node is null. Flowchart execution will end.")]
        public void FlowchartNextNull(string data1, string AppDomain)
        {
            WriteEvent(EventIds.FlowchartNextNull, data1, AppDomain);
        }

        [NonEvent]
        public void FlowchartNextNull(string data1)
        {
            FlowchartNextNull(data1, "");
        }

        public bool FlowchartSwitchCaseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.FlowchartSwitchCase, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ExecuteFlowchartSwitchCase, Task = Tasks.ExecuteFlowchart,
            Keywords = Keywords.WFActivities,
            Message = "Flowchart '{0}'/FlowSwitch - Case '{1}' was selected.")]
        public void FlowchartSwitchCase(string data1, string data2, string AppDomain)
        {
            WriteEvent(EventIds.FlowchartSwitchCase, data1, data2, AppDomain);
        }

        [NonEvent]
        public void FlowchartSwitchCase(string data1, string data2)
        {
            FlowchartSwitchCase(data1, data2, "");
        }

        public bool FlowchartSwitchDefaultIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.FlowchartSwitchDefault, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ExecuteFlowchartSwitchDefault, Task = Tasks.ExecuteFlowchart,
            Keywords = Keywords.WFActivities,
            Message = "Flowchart '{0}'/FlowSwitch - Default Case was selected.")]
        public void FlowchartSwitchDefault(string data1, string AppDomain)
        {
            WriteEvent(EventIds.FlowchartSwitchDefault, data1, AppDomain);
        }

        [NonEvent]
        public void FlowchartSwitchDefault(string data1)
        {
            FlowchartSwitchDefault(data1, "");
        }

        public bool FlowchartSwitchCaseNotFoundIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.FlowchartSwitchCaseNotFound, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ExecuteFlowchartSwitchCaseNotFound, Task = Tasks.ExecuteFlowchart,
            Keywords = Keywords.WFActivities,
            Message = "Flowchart '{0}'/FlowSwitch - could find neither a Case activity nor a Default Case matching the Expression result. Flowchart execution will end.")]
        public void FlowchartSwitchCaseNotFound(string data1, string AppDomain)
        {
            WriteEvent(EventIds.FlowchartSwitchCaseNotFound, data1, AppDomain);
        }

        [NonEvent]
        public void FlowchartSwitchCaseNotFound(string data1)
        {
            FlowchartSwitchCaseNotFound(data1, "");
        }

        public bool CompensationStateIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.CompensationState, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.CompensationState,
            Keywords = Keywords.WFActivities,
            Message = "CompensableActivity '{0}' is in the '{1}' state.")]
        public void CompensationState(string data1, string data2, string AppDomain)
        {
            WriteEvent(EventIds.CompensationState, data1, data2, AppDomain);
        }

        [NonEvent]
        public void CompensationState(string data1, string data2)
        {
            CompensationState(data1, data2, "");
        }

        public bool SwitchCaseNotFoundIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.SwitchCaseNotFound, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ExecuteActivity,
            Keywords = Keywords.WFActivities,
            Message = "The Switch activity '{0}' could not find a Case activity matching the Expression result.")]
        public void SwitchCaseNotFound(string data1, string AppDomain)
        {
            WriteEvent(EventIds.SwitchCaseNotFound, data1, AppDomain);
        }

        [NonEvent]
        public void SwitchCaseNotFound(string data1)
        {
            SwitchCaseNotFound(data1, "");
        }

        public bool ChannelInitializationTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ChannelInitializationTimeout, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TimeoutException,
            Keywords = Keywords.ServiceModel,
            Message = "{0}")]
        public void ChannelInitializationTimeout(string data1, string AppDomain)
        {
            WriteEvent(EventIds.ChannelInitializationTimeout, data1, AppDomain);
        }

        [NonEvent]
        public void ChannelInitializationTimeout(string data1)
        {
            ChannelInitializationTimeout(data1, "");
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

        public bool OpenTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.OpenTimeout, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TimeoutException,
            Keywords = Keywords.ServiceModel,
            Message = "{0}")]
        public void OpenTimeout(string data1, string AppDomain)
        {
            WriteEvent(EventIds.OpenTimeout, data1, AppDomain);
        }

        [NonEvent]
        public void OpenTimeout(string data1)
        {
            OpenTimeout(data1, "");
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

        public bool SendTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.SendTimeout, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TimeoutException,
            Keywords = Keywords.ServiceModel,
            Message = "{0}")]
        public void SendTimeout(string data1, string AppDomain)
        {
            WriteEvent(EventIds.SendTimeout, data1, AppDomain);
        }

        [NonEvent]
        public void SendTimeout(string data1)
        {
            SendTimeout(data1, "");
        }

        public bool InactivityTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.InactivityTimeout, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TimeoutException,
            Keywords = Keywords.ServiceModel,
            Message = "{0}")]
        public void InactivityTimeout(string data1, string AppDomain)
        {
            WriteEvent(EventIds.InactivityTimeout, data1, AppDomain);
        }

        [NonEvent]
        public void InactivityTimeout(string data1)
        {
            InactivityTimeout(data1, "");
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

        public bool MaxPendingConnectionsExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Quota, EventChannel.Debug);
        }

        [Event(EventIds.MaxPendingConnectionsExceeded, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void MaxPendingConnectionsExceeded(string data1, string AppDomain)
        {
            WriteEvent(EventIds.MaxPendingConnectionsExceeded, data1, AppDomain);
        }

        [NonEvent]
        public void MaxPendingConnectionsExceeded(string data1)
        {
            MaxPendingConnectionsExceeded(data1, "");
        }

        public bool ReaderQuotaExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.ReaderQuotaExceeded, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void ReaderQuotaExceeded(string data1, string AppDomain)
        {
            WriteEvent(EventIds.ReaderQuotaExceeded, data1, AppDomain);
        }

        [NonEvent]
        public void ReaderQuotaExceeded(string data1)
        {
            ReaderQuotaExceeded(data1, "");
        }

        public bool NegotiateTokenAuthenticatorStateCacheExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.NegotiateTokenAuthenticatorStateCacheExceeded, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void NegotiateTokenAuthenticatorStateCacheExceeded(string msg, string AppDomain)
        {
            WriteEvent(EventIds.NegotiateTokenAuthenticatorStateCacheExceeded, msg, AppDomain);
        }

        [NonEvent]
        public void NegotiateTokenAuthenticatorStateCacheExceeded(string msg)
        {
            NegotiateTokenAuthenticatorStateCacheExceeded(msg, "");
        }

        public bool NegotiateTokenAuthenticatorStateCacheRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Debug);
        }

        [Event(EventIds.NegotiateTokenAuthenticatorStateCacheRatio, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Negotiate token authenticator state cache ratio: {0}/{1}")]
        public void NegotiateTokenAuthenticatorStateCacheRatio(int cur, int max, string AppDomain)
        {
            WriteEvent(EventIds.NegotiateTokenAuthenticatorStateCacheRatio, cur, max, AppDomain);
        }

        [NonEvent]
        public void NegotiateTokenAuthenticatorStateCacheRatio(int cur, int max)
        {
            NegotiateTokenAuthenticatorStateCacheRatio(cur, max, "");
        }

        public bool SecuritySessionRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Debug);
        }

        [Event(EventIds.SecuritySessionRatio, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Security session ratio: {0}/{1}")]
        public void SecuritySessionRatio(int cur, int max, string AppDomain)
        {
            WriteEvent(EventIds.SecuritySessionRatio, cur, max, AppDomain);
        }

        [NonEvent]
        public void SecuritySessionRatio(int cur, int max)
        {
            SecuritySessionRatio(cur, max, "");
        }

        public bool PendingConnectionsRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.PendingConnectionsRatio, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Pending connections ratio: {0}/{1}")]
        public void PendingConnectionsRatio(int cur, int max, string AppDomain)
        {
            WriteEvent(EventIds.PendingConnectionsRatio, cur, max, AppDomain);
        }

        [NonEvent]
        public void PendingConnectionsRatio(int cur, int max)
        {
            PendingConnectionsRatio(cur, max, "");
        }

        public bool ConcurrentCallsRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.ConcurrentCallsRatio, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Concurrent calls ratio: {0}/{1}")]
        public void ConcurrentCallsRatio(int cur, int max, string AppDomain)
        {
            WriteEvent(EventIds.ConcurrentCallsRatio, cur, max, AppDomain);
        }

        [NonEvent]
        public void ConcurrentCallsRatio(int cur, int max)
        {
            ConcurrentCallsRatio(cur, max, "");
        }

        public bool ConcurrentSessionsRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.ConcurrentSessionsRatio, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Concurrent sessions ratio: {0}/{1}")]
        public void ConcurrentSessionsRatio(int cur, int max, string AppDomain)
        {
            WriteEvent(EventIds.ConcurrentSessionsRatio, cur, max, AppDomain);
        }

        [NonEvent]
        public void ConcurrentSessionsRatio(int cur, int max)
        {
            ConcurrentSessionsRatio(cur, max, "");
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

        public bool PendingMessagesPerChannelRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.PendingMessagesPerChannelRatio, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Pending messages per channel ratio: {0}/{1}")]
        public void PendingMessagesPerChannelRatio(int cur, int max, string AppDomain)
        {
            WriteEvent(EventIds.PendingMessagesPerChannelRatio, cur, max, AppDomain);
        }

        [NonEvent]
        public void PendingMessagesPerChannelRatio(int cur, int max)
        {
            PendingMessagesPerChannelRatio(cur, max, "");
        }

        public bool ConcurrentInstancesRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.ConcurrentInstancesRatio, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Concurrent instances ratio: {0}/{1}")]
        public void ConcurrentInstancesRatio(int cur, int max, string AppDomain)
        {
            WriteEvent(EventIds.ConcurrentInstancesRatio, cur, max, AppDomain);
        }

        [NonEvent]
        public void ConcurrentInstancesRatio(int cur, int max)
        {
            ConcurrentInstancesRatio(cur, max, "");
        }

        public bool PendingAcceptsAtZeroIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Quota, EventChannel.Debug);
        }

        [Event(EventIds.PendingAcceptsAtZero, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Zero pending accepts left")]
        public void PendingAcceptsAtZero(string AppDomain)
        {
            WriteEvent(EventIds.PendingAcceptsAtZero, AppDomain);
        }

        [NonEvent]
        public void PendingAcceptsAtZero()
        {
            PendingAcceptsAtZero("");
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

        public bool ReceiveRetryCountReachedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.ReceiveRetryCountReached, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.MsmqQuotas,
            Keywords = Keywords.Quota,
            Message = "Receive retry count reached on MSMQ message with id '{0}'")]
        public void ReceiveRetryCountReached(string data1, string AppDomain)
        {
            WriteEvent(EventIds.ReceiveRetryCountReached, data1, AppDomain);
        }

        [NonEvent]
        public void ReceiveRetryCountReached(string data1)
        {
            ReceiveRetryCountReached(data1, "");
        }

        public bool MaxRetryCyclesExceededMsmqIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.MaxRetryCyclesExceededMsmq, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.MsmqQuotas,
            Keywords = Keywords.Quota,
            Message = "Max retry cycles exceeded on MSMQ message with id '{0}'")]
        public void MaxRetryCyclesExceededMsmq(string data1, string AppDomain)
        {
            WriteEvent(EventIds.MaxRetryCyclesExceededMsmq, data1, AppDomain);
        }

        [NonEvent]
        public void MaxRetryCyclesExceededMsmq(string data1)
        {
            MaxRetryCyclesExceededMsmq(data1, "");
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

        public bool WfMessageReceivedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Debug);
        }

        [Event(EventIds.WfMessageReceived, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Receive, Task = Tasks.WFMessage,
            Keywords = Keywords.WFServices,
            Message = "Message received by workflow")]
        public void WfMessageReceived(string AppDomain)
        {
            WriteEvent(EventIds.WfMessageReceived, AppDomain);
        }

        [NonEvent]
        public void WfMessageReceived()
        {
            WfMessageReceived("");
        }

        public bool WfMessageSentIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Debug);
        }

        [Event(EventIds.WfMessageSent, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Send, Task = Tasks.WFMessage,
            Keywords = Keywords.WFServices,
            Message = "Message sent from workflow")]
        public void WfMessageSent(string AppDomain)
        {
            WriteEvent(EventIds.WfMessageSent, AppDomain);
        }

        [NonEvent]
        public void WfMessageSent()
        {
            WfMessageSent("");
        }

        public bool MaxRetryCyclesExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.MaxRetryCyclesExceeded, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "{0}")]
        public void MaxRetryCyclesExceeded(string data1, string AppDomain)
        {
            WriteEvent(EventIds.MaxRetryCyclesExceeded, data1, AppDomain);
        }

        [NonEvent]
        public void MaxRetryCyclesExceeded(string data1)
        {
            MaxRetryCyclesExceeded(data1, "");
        }

        public bool ExecuteWorkItemStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ExecuteWorkItemStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ExecuteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Execute work item start")]
        public void ExecuteWorkItemStart(string AppDomain)
        {
            WriteEvent(EventIds.ExecuteWorkItemStart, AppDomain);
        }

        [NonEvent]
        public void ExecuteWorkItemStart()
        {
            ExecuteWorkItemStart("");
        }

        public bool ExecuteWorkItemStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.ExecuteWorkItemStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ExecuteWorkItem,
            Keywords = Keywords.WFRuntime,
            Message = "Execute work item stop")]
        public void ExecuteWorkItemStop(string AppDomain)
        {
            WriteEvent(EventIds.ExecuteWorkItemStop, AppDomain);
        }

        [NonEvent]
        public void ExecuteWorkItemStop()
        {
            ExecuteWorkItemStop("");
        }

        public bool SendMessageChannelCacheMissIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.SendMessageChannelCacheMiss, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.MessageChannelCacheMissed, Task = Tasks.MessageChannelCache,
            Keywords = Keywords.WFRuntime,
            Message = "SendMessageChannelCache miss")]
        public void SendMessageChannelCacheMiss(string AppDomain)
        {
            WriteEvent(EventIds.SendMessageChannelCacheMiss, AppDomain);
        }

        [NonEvent]
        public void SendMessageChannelCacheMiss()
        {
            SendMessageChannelCacheMiss("");
        }

        public bool InternalCacheMetadataStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.InternalCacheMetadataStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.InternalCacheMetadata,
            Keywords = Keywords.WFRuntime,
            Message = "InternalCacheMetadata started on activity '{0}'.")]
        public void InternalCacheMetadataStart(string id, string AppDomain)
        {
            WriteEvent(EventIds.InternalCacheMetadataStart, id, AppDomain);
        }

        [NonEvent]
        public void InternalCacheMetadataStart(string id)
        {
            InternalCacheMetadataStart(id, "");
        }

        public bool InternalCacheMetadataStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.InternalCacheMetadataStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.InternalCacheMetadata,
            Keywords = Keywords.WFRuntime,
            Message = "InternalCacheMetadata stopped on activity '{0}'.")]
        public void InternalCacheMetadataStop(string id, string AppDomain)
        {
            WriteEvent(EventIds.InternalCacheMetadataStop, id, AppDomain);
        }

        [NonEvent]
        public void InternalCacheMetadataStop(string id)
        {
            InternalCacheMetadataStop(id, "");
        }

        public bool CompileVbExpressionStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CompileVbExpressionStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.VBExpressionCompile,
            Keywords = Keywords.WFRuntime,
            Message = "Compiling VB expression '{0}'")]
        public void CompileVbExpressionStart(string expr, string AppDomain)
        {
            WriteEvent(EventIds.CompileVbExpressionStart, expr, AppDomain);
        }

        [NonEvent]
        public void CompileVbExpressionStart(string expr)
        {
            CompileVbExpressionStart(expr, "");
        }

        public bool CacheRootMetadataStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CacheRootMetadataStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.CacheRootMetadata,
            Keywords = Keywords.WFRuntime,
            Message = "CacheRootMetadata started on activity '{0}'")]
        public void CacheRootMetadataStart(string activityName, string AppDomain)
        {
            WriteEvent(EventIds.CacheRootMetadataStart, activityName, AppDomain);
        }

        [NonEvent]
        public void CacheRootMetadataStart(string activityName)
        {
            CacheRootMetadataStart(activityName, "");
        }

        public bool CacheRootMetadataStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CacheRootMetadataStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.CacheRootMetadata,
            Keywords = Keywords.WFRuntime,
            Message = "CacheRootMetadata stopped on activity {0}.")]
        public void CacheRootMetadataStop(string activityName, string AppDomain)
        {
            WriteEvent(EventIds.CacheRootMetadataStop, activityName, AppDomain);
        }

        [NonEvent]
        public void CacheRootMetadataStop(string activityName)
        {
            CacheRootMetadataStop(activityName, "");
        }

        public bool CompileVbExpressionStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.CompileVbExpressionStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.VBExpressionCompile,
            Keywords = Keywords.WFRuntime,
            Message = "Finished compiling VB expression.")]
        public void CompileVbExpressionStop(string AppDomain)
        {
            WriteEvent(EventIds.CompileVbExpressionStop, AppDomain);
        }

        [NonEvent]
        public void CompileVbExpressionStop()
        {
            CompileVbExpressionStop("");
        }

        public bool TryCatchExceptionFromTryIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.TryCatchExceptionFromTry, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.TryCatchExceptionFromTry, Task = Tasks.TryCatchException,
            Keywords = Keywords.WFActivities,
            Message = "The TryCatch activity '{0}' has caught an exception of type '{1}'.")]
        public void TryCatchExceptionFromTry(string data1, string data2, string AppDomain)
        {
            WriteEvent(EventIds.TryCatchExceptionFromTry, data1, data2, AppDomain);
        }

        [NonEvent]
        public void TryCatchExceptionFromTry(string data1, string data2)
        {
            TryCatchExceptionFromTry(data1, data2, "");
        }

        public bool TryCatchExceptionDuringCancelationIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.TryCatchExceptionDuringCancelation, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.TryCatchExceptionDuringCancelation, Task = Tasks.TryCatchException,
            Keywords = Keywords.WFActivities,
            Message = "A child activity of the TryCatch activity '{0}' has thrown an exception during cancelation.")]
        public void TryCatchExceptionDuringCancelation(string data1, string AppDomain)
        {
            WriteEvent(EventIds.TryCatchExceptionDuringCancelation, data1, AppDomain);
        }

        [NonEvent]
        public void TryCatchExceptionDuringCancelation(string data1)
        {
            TryCatchExceptionDuringCancelation(data1, "");
        }

        public bool TryCatchExceptionFromCatchOrFinallyIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFActivities, EventChannel.Debug);
        }

        [Event(EventIds.TryCatchExceptionFromCatchOrFinally, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.TryCatchExceptionFromCatchOrFinally, Task = Tasks.TryCatchException,
            Keywords = Keywords.WFActivities,
            Message = "A Catch or Finally activity that is associated with the TryCatch activity '{0}' has thrown an exception.")]
        public void TryCatchExceptionFromCatchOrFinally(string data1, string AppDomain)
        {
            WriteEvent(EventIds.TryCatchExceptionFromCatchOrFinally, data1, AppDomain);
        }

        [NonEvent]
        public void TryCatchExceptionFromCatchOrFinally(string data1)
        {
            TryCatchExceptionFromCatchOrFinally(data1, "");
        }

        public bool ReceiveContextCompleteFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Channel, EventChannel.Analytic);
        }

        [Event(EventIds.ReceiveContextCompleteFailed, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info,
            Keywords = Keywords.Channel,
            Message = "Failed to Complete {0}.")]
        public void ReceiveContextCompleteFailed(string TypeName, string AppDomain)
        {
            WriteEvent(EventIds.ReceiveContextCompleteFailed, TypeName, AppDomain);
        }

        [NonEvent]
        public void ReceiveContextCompleteFailed(string TypeName)
        {
            ReceiveContextCompleteFailed(TypeName, "");
        }

        public bool ReceiveContextAbandonFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ReceiveContextAbandonFailed, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Channel,
            Message = "Failed to Abandon {0}.")]
        public void ReceiveContextAbandonFailed(string TypeName, string AppDomain)
        {
            WriteEvent(EventIds.ReceiveContextAbandonFailed, TypeName, AppDomain);
        }

        [NonEvent]
        public void ReceiveContextAbandonFailed(string TypeName)
        {
            ReceiveContextAbandonFailed(TypeName, "");
        }

        public bool ReceiveContextFaultedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.ReceiveContextFaulted, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.DispatchMessage,
            Keywords = Keywords.ServiceModel,
            Message = "Receive Context faulted.")]
        public void ReceiveContextFaulted(string EventSource, string AppDomain)
        {
            WriteEvent(EventIds.ReceiveContextFaulted, EventSource, AppDomain);
        }

        [NonEvent]
        public void ReceiveContextFaulted(object source)
        {
            TracePayload payload = FxTrace.Trace.GetSerializedPayload(source, null, null);
            ReceiveContextFaulted(payload.EventSource, "");
        }

        public bool ReceiveContextAbandonWithExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ReceiveContextAbandonWithException, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Channel,
            Message = "{0} was Abandoned with exception {1}.")]
        public void ReceiveContextAbandonWithException(string TypeName, string ExceptionToString, string AppDomain)
        {
            WriteEvent(EventIds.ReceiveContextAbandonWithException, TypeName, ExceptionToString, AppDomain);
        }

        [NonEvent]
        public void ReceiveContextAbandonWithException(string TypeName, string ExceptionToString)
        {
            ReceiveContextAbandonWithException(TypeName, ExceptionToString, "");
        }

        public bool ClientBaseCachedChannelFactoryCountIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ClientBaseCachedChannelFactoryCount, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ChannelFactoryCaching,
            Keywords = Keywords.ServiceModel,
            Message = "Number of cached channel factories is: '{0}'.  At most '{1}' channel factories can be cached.")]
        public void ClientBaseCachedChannelFactoryCount(int Count, int MaxNum, string EventSource, string AppDomain)
        {
            WriteEvent(EventIds.ClientBaseCachedChannelFactoryCount, Count, MaxNum, EventSource, AppDomain);
        }

        [NonEvent]
        public void ClientBaseCachedChannelFactoryCount(int Count, int MaxNum, object source)
        {
            TracePayload payload = FxTrace.Trace.GetSerializedPayload(source, null, null);
            ClientBaseCachedChannelFactoryCount(Count, MaxNum, payload.EventSource, "");
        }

        public bool ClientBaseChannelFactoryAgedOutofCacheIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ClientBaseChannelFactoryAgedOutofCache, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ChannelFactoryCaching,
            Keywords = Keywords.ServiceModel,
            Message = "A channel factory has been aged out of the cache because the cache has reached its limit of '{0}'.")]
        public void ClientBaseChannelFactoryAgedOutofCache(int Count, string EventSource, string AppDomain)
        {
            WriteEvent(EventIds.ClientBaseChannelFactoryAgedOutofCache, Count, EventSource, AppDomain);
        }

        [NonEvent]
        public void ClientBaseChannelFactoryAgedOutofCache(int Count, object source)
        {
            TracePayload payload = FxTrace.Trace.GetSerializedPayload(source, null, null);
            ClientBaseChannelFactoryAgedOutofCache(Count, payload.EventSource, "");
        }

        public bool ClientBaseChannelFactoryCacheHitIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ClientBaseChannelFactoryCacheHit, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ChannelFactoryCaching,
            Keywords = Keywords.ServiceModel,
            Message = "Used matching channel factory found in cache.")]
        public void ClientBaseChannelFactoryCacheHit(string EventSource, string AppDomain)
        {
            WriteEvent(EventIds.ClientBaseChannelFactoryCacheHit, EventSource, AppDomain);
        }

        [NonEvent]
        public void ClientBaseChannelFactoryCacheHit(object source)
        {
            TracePayload payload = FxTrace.Trace.GetSerializedPayload(source, null, null);
            ClientBaseChannelFactoryCacheHit(payload.EventSource, "");
        }

        public bool ClientBaseUsingLocalChannelFactoryIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.ClientBaseUsingLocalChannelFactory, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ChannelFactoryCaching,
            Keywords = Keywords.ServiceModel,
            Message = "Not using channel factory from cache, i.e. caching disabled for instance.")]
        public void ClientBaseUsingLocalChannelFactory(string EventSource, string AppDomain)
        {
            WriteEvent(EventIds.ClientBaseUsingLocalChannelFactory, EventSource, AppDomain);
        }

        [NonEvent]
        public void ClientBaseUsingLocalChannelFactory(object source)
        {
            TracePayload payload = FxTrace.Trace.GetSerializedPayload(source, null, null);
            ClientBaseUsingLocalChannelFactory(payload.EventSource, "");
        }

        public bool QueryCompositionExecutedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.QueryCompositionExecuted, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.ServiceModel,
            Message = "Query composition using '{0}' was executed on the Request Uri: '{1}'.")]
        public void QueryCompositionExecuted(string TypeName, string Uri, string EventSource, string AppDomain)
        {
            WriteEvent(EventIds.QueryCompositionExecuted, TypeName, Uri, EventSource, AppDomain);
        }

        [NonEvent]
        public void QueryCompositionExecuted(string TypeName, string Uri, object source)
        {
            TracePayload payload = FxTrace.Trace.GetSerializedPayload(source, null, null);
            QueryCompositionExecuted(TypeName, Uri, payload.EventSource, "");
        }

        public bool DispatchFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.DispatchFailed, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.DispatchMessage,
            Keywords = Keywords.ServiceModel,
            Message = "The '{0}' operation was dispatched with errors.")]
        public void DispatchFailed(string OperationName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.DispatchFailed, OperationName, HostReference, AppDomain);
        }

        [NonEvent]
        public void DispatchFailed(string OperationName)
        {
            DispatchFailed(OperationName, "", "");
        }

        public bool DispatchSuccessfulIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Analytic);
        }

        [Event(EventIds.DispatchSuccessful, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.DispatchMessage,
            Keywords = Keywords.ServiceModel,
            Message = "The '{0}' operation was dispatched successfully.")]
        public void DispatchSuccessful(string OperationName, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.DispatchSuccessful, OperationName, HostReference, AppDomain);
        }

        [NonEvent]
        public void DispatchSuccessful(string OperationName)
        {
            DispatchSuccessful(OperationName, "", "");
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

        public bool SocketAcceptEnqueuedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.TCP, EventChannel.Debug);
        }

        [Event(EventIds.SocketAcceptEnqueued, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ConnectionAccept,
            Keywords = Keywords.TCP,
            Message = "Connection accept started.")]
        public void SocketAcceptEnqueued(string AppDomain)
        {
            WriteEvent(EventIds.SocketAcceptEnqueued, AppDomain);
        }

        [NonEvent]
        public void SocketAcceptEnqueued()
        {
            SocketAcceptEnqueued("");
        }

        public bool SocketAcceptedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.TCP, EventChannel.Debug);
        }

        [Event(EventIds.SocketAccepted, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ConnectionAccept,
            Keywords = Keywords.TCP,
            Message = "ListenerId:{0} accepted SocketId:{1}.")]
        public void SocketAccepted(int ListenerHashCode, int SocketHashCode, string AppDomain)
        {
            WriteEvent(EventIds.SocketAccepted, ListenerHashCode, SocketHashCode, AppDomain);
        }

        [NonEvent]
        public void SocketAccepted(int ListenerHashCode, int SocketHashCode)
        {
            SocketAccepted(ListenerHashCode, SocketHashCode, "");
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

        public bool SecurityNegotiationStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.SecurityNegotiationStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.SecurityNegotiation,
            Keywords = Keywords.Security,
            Message = "Security negotiation started.")]
        public void SecurityNegotiationStart(string AppDomain)
        {
            WriteEvent(EventIds.SecurityNegotiationStart, AppDomain);
        }

        [NonEvent]
        public void SecurityNegotiationStart()
        {
            SecurityNegotiationStart("");
        }

        public bool SecurityNegotiationStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.SecurityNegotiationStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.SecurityNegotiation,
            Keywords = Keywords.Security,
            Message = "Security negotiation completed.")]
        public void SecurityNegotiationStop(string AppDomain)
        {
            WriteEvent(EventIds.SecurityNegotiationStop, AppDomain);
        }

        [NonEvent]
        public void SecurityNegotiationStop()
        {
            SecurityNegotiationStop("");
        }

        public bool SecurityTokenProviderOpenedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.SecurityTokenProviderOpened, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.SecureMessage,
            Keywords = Keywords.Security,
            Message = "SecurityTokenProvider opening completed.")]
        public void SecurityTokenProviderOpened(string AppDomain)
        {
            WriteEvent(EventIds.SecurityTokenProviderOpened, AppDomain);
        }

        [NonEvent]
        public void SecurityTokenProviderOpened()
        {
            SecurityTokenProviderOpened("");
        }

        public bool OutgoingMessageSecuredIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.OutgoingMessageSecured, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.SecureMessage,
            Keywords = Keywords.Security,
            Message = "Outgoing message has been secured.")]
        public void OutgoingMessageSecured(string AppDomain)
        {
            WriteEvent(EventIds.OutgoingMessageSecured, AppDomain);
        }

        [NonEvent]
        public void OutgoingMessageSecured()
        {
            OutgoingMessageSecured("");
        }

        public bool IncomingMessageVerifiedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security | Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.IncomingMessageVerified, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SecurityVerification,
            Keywords = Keywords.Security | Keywords.ServiceModel,
            Message = "Incoming message has been verified.")]
        public void IncomingMessageVerified(string AppDomain)
        {
            WriteEvent(EventIds.IncomingMessageVerified, AppDomain);
        }

        [NonEvent]
        public void IncomingMessageVerified()
        {
            IncomingMessageVerified("");
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

        public bool PipeConnectionAcceptStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.PipeConnectionAcceptStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ConnectionAccept,
            Keywords = Keywords.Channel,
            Message = "Pipe connection accept started on {0} .")]
        public void PipeConnectionAcceptStart(string uri, string AppDomain)
        {
            WriteEvent(EventIds.PipeConnectionAcceptStart, uri, AppDomain);
        }

        [NonEvent]
        public void PipeConnectionAcceptStart(string uri)
        {
            PipeConnectionAcceptStart(uri, "");
        }

        public bool PipeConnectionAcceptStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.PipeConnectionAcceptStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ConnectionAccept,
            Keywords = Keywords.Channel,
            Message = "Pipe connection accepted.")]
        public void PipeConnectionAcceptStop(string AppDomain)
        {
            WriteEvent(EventIds.PipeConnectionAcceptStop, AppDomain);
        }

        [NonEvent]
        public void PipeConnectionAcceptStop()
        {
            PipeConnectionAcceptStop("");
        }

        public bool EstablishConnectionStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.EstablishConnectionStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.Connect,
            Keywords = Keywords.Channel,
            Message = "Connection establishment started for {0}.")]
        public void EstablishConnectionStart(string Key, string AppDomain)
        {
            WriteEvent(EventIds.EstablishConnectionStart, Key, AppDomain);
        }

        [NonEvent]
        public void EstablishConnectionStart(string Key)
        {
            EstablishConnectionStart(Key, "");
        }

        public bool EstablishConnectionStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.EstablishConnectionStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.Connect,
            Keywords = Keywords.Channel,
            Message = "Connection established.")]
        public void EstablishConnectionStop(string AppDomain)
        {
            WriteEvent(EventIds.EstablishConnectionStop, AppDomain);
        }

        [NonEvent]
        public void EstablishConnectionStop()
        {
            EstablishConnectionStop("");
        }

        public bool SessionPreambleUnderstoodIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.SessionPreambleUnderstood, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.Channel,
            Message = "Session preamble for '{0}' understood.")]
        public void SessionPreambleUnderstood(string Via, string AppDomain)
        {
            WriteEvent(EventIds.SessionPreambleUnderstood, Via, AppDomain);
        }

        [NonEvent]
        public void SessionPreambleUnderstood(string Via)
        {
            SessionPreambleUnderstood(Via, "");
        }

        public bool ConnectionReaderSendFaultIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ConnectionReaderSendFault, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.Channel,
            Message = "Connection reader sending fault '{0}'. ")]
        public void ConnectionReaderSendFault(string FaultString, string AppDomain)
        {
            WriteEvent(EventIds.ConnectionReaderSendFault, FaultString, AppDomain);
        }

        [NonEvent]
        public void ConnectionReaderSendFault(string FaultString)
        {
            ConnectionReaderSendFault(FaultString, "");
        }

        public bool SocketAcceptClosedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.TCP, EventChannel.Debug);
        }

        [Event(EventIds.SocketAcceptClosed, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ConnectionAccept,
            Keywords = Keywords.TCP,
            Message = "Socket accept closed.")]
        public void SocketAcceptClosed(string AppDomain)
        {
            WriteEvent(EventIds.SocketAcceptClosed, AppDomain);
        }

        [NonEvent]
        public void SocketAcceptClosed()
        {
            SocketAcceptClosed("");
        }

        public bool ServiceHostFaultedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Critical, Keywords.TCP, EventChannel.Analytic);
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

        public bool ServerMaxPooledConnectionsQuotaReachedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.ServerMaxPooledConnectionsQuotaReached, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Server max pooled connections quota reached.")]
        public void ServerMaxPooledConnectionsQuotaReached(string AppDomain)
        {
            WriteEvent(EventIds.ServerMaxPooledConnectionsQuotaReached, AppDomain);
        }

        [NonEvent]
        public void ServerMaxPooledConnectionsQuotaReached()
        {
            ServerMaxPooledConnectionsQuotaReached("");
        }

        public bool TcpConnectionTimedOutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.TCP, EventChannel.Analytic);
        }

        [Event(EventIds.TcpConnectionTimedOut, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.TCP,
            Message = "SocketId:{0} to remote address {1} timed out.")]
        public void TcpConnectionTimedOut(int SocketId, string Uri, string AppDomain)
        {
            WriteEvent(EventIds.TcpConnectionTimedOut, SocketId, Uri, AppDomain);
        }

        [NonEvent]
        public void TcpConnectionTimedOut(int SocketId, string Uri)
        {
            TcpConnectionTimedOut(SocketId, Uri, "");
        }

        public bool TcpConnectionResetErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.TCP, EventChannel.Analytic);
        }

        [Event(EventIds.TcpConnectionResetError, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.TCP,
            Message = "SocketId:{0} to remote address {1} had a connection reset error.")]
        public void TcpConnectionResetError(int SocketId, string Uri, string AppDomain)
        {
            WriteEvent(EventIds.TcpConnectionResetError, SocketId, Uri, AppDomain);
        }

        [NonEvent]
        public void TcpConnectionResetError(int SocketId, string Uri)
        {
            TcpConnectionResetError(SocketId, Uri, "");
        }

        public bool ServiceSecurityNegotiationCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.ServiceSecurityNegotiationCompleted, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SecurityNegotiation,
            Keywords = Keywords.Security,
            Message = "Service security negotiation completed.")]
        public void ServiceSecurityNegotiationCompleted(string AppDomain)
        {
            WriteEvent(EventIds.ServiceSecurityNegotiationCompleted, AppDomain);
        }

        [NonEvent]
        public void ServiceSecurityNegotiationCompleted()
        {
            ServiceSecurityNegotiationCompleted("");
        }

        public bool SecurityNegotiationProcessingFailureIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Security, EventChannel.Analytic);
        }

        [Event(EventIds.SecurityNegotiationProcessingFailure, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.SecurityNegotiation,
            Keywords = Keywords.Security,
            Message = "Security negotiation processing failed.")]
        public void SecurityNegotiationProcessingFailure(string AppDomain)
        {
            WriteEvent(EventIds.SecurityNegotiationProcessingFailure, AppDomain);
        }

        [NonEvent]
        public void SecurityNegotiationProcessingFailure()
        {
            SecurityNegotiationProcessingFailure("");
        }

        public bool SecurityIdentityVerificationSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.SecurityIdentityVerificationSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SecurityVerification,
            Keywords = Keywords.Security,
            Message = "Security verification succeeded.")]
        public void SecurityIdentityVerificationSuccess(string AppDomain)
        {
            WriteEvent(EventIds.SecurityIdentityVerificationSuccess, AppDomain);
        }

        [NonEvent]
        public void SecurityIdentityVerificationSuccess()
        {
            SecurityIdentityVerificationSuccess("");
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

        public bool PortSharingDuplicatedSocketIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Debug);
        }

        [Event(EventIds.PortSharingDuplicatedSocket, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ActivationDuplicateSocket,
            Keywords = Keywords.ActivationServices,
            Message = "Socket duplicated for {0}.")]
        public void PortSharingDuplicatedSocket(string Uri, string AppDomain)
        {
            WriteEvent(EventIds.PortSharingDuplicatedSocket, Uri, AppDomain);
        }

        [NonEvent]
        public void PortSharingDuplicatedSocket(string Uri)
        {
            PortSharingDuplicatedSocket(Uri, "");
        }

        public bool SecurityImpersonationSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.SecurityImpersonationSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SecurityImpersonation,
            Keywords = Keywords.Security,
            Message = "Security impersonation succeeded.")]
        public void SecurityImpersonationSuccess(string AppDomain)
        {
            WriteEvent(EventIds.SecurityImpersonationSuccess, AppDomain);
        }

        [NonEvent]
        public void SecurityImpersonationSuccess()
        {
            SecurityImpersonationSuccess("");
        }

        public bool SecurityImpersonationFailureIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Security, EventChannel.Analytic);
        }

        [Event(EventIds.SecurityImpersonationFailure, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.SecurityImpersonation,
            Keywords = Keywords.Security,
            Message = "Security impersonation failed.")]
        public void SecurityImpersonationFailure(string AppDomain)
        {
            WriteEvent(EventIds.SecurityImpersonationFailure, AppDomain);
        }

        [NonEvent]
        public void SecurityImpersonationFailure()
        {
            SecurityImpersonationFailure("");
        }

        public bool HttpChannelRequestAbortedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.HttpChannelRequestAborted, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "Http channel request aborted.")]
        public void HttpChannelRequestAborted(string AppDomain)
        {
            WriteEvent(EventIds.HttpChannelRequestAborted, AppDomain);
        }

        [NonEvent]
        public void HttpChannelRequestAborted()
        {
            HttpChannelRequestAborted("");
        }

        public bool HttpChannelResponseAbortedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.HttpChannelResponseAborted, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TransportSend,
            Keywords = Keywords.HTTP,
            Message = "Http channel response aborted.")]
        public void HttpChannelResponseAborted(string AppDomain)
        {
            WriteEvent(EventIds.HttpChannelResponseAborted, AppDomain);
        }

        [NonEvent]
        public void HttpChannelResponseAborted()
        {
            HttpChannelResponseAborted("");
        }

        public bool HttpAuthFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.HttpAuthFailed, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.SecurityVerification,
            Keywords = Keywords.HTTP,
            Message = "Http authentication failed.")]
        public void HttpAuthFailed(string AppDomain)
        {
            WriteEvent(EventIds.HttpAuthFailed, AppDomain);
        }

        [NonEvent]
        public void HttpAuthFailed()
        {
            HttpAuthFailed("");
        }

        public bool SharedListenerProxyRegisterStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.SharedListenerProxyRegisterStart, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.ActivationListenerOpen,
            Keywords = Keywords.ActivationServices,
            Message = "SharedListenerProxy registration started for uri '{0}'.")]
        public void SharedListenerProxyRegisterStart(string Uri, string AppDomain)
        {
            WriteEvent(EventIds.SharedListenerProxyRegisterStart, Uri, AppDomain);
        }

        [NonEvent]
        public void SharedListenerProxyRegisterStart(string Uri)
        {
            SharedListenerProxyRegisterStart(Uri, "");
        }

        public bool SharedListenerProxyRegisterStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.SharedListenerProxyRegisterStop, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.ActivationListenerOpen,
            Keywords = Keywords.ActivationServices,
            Message = "SharedListenerProxy Register Stop.")]
        public void SharedListenerProxyRegisterStop(string AppDomain)
        {
            WriteEvent(EventIds.SharedListenerProxyRegisterStop, AppDomain);
        }

        [NonEvent]
        public void SharedListenerProxyRegisterStop()
        {
            SharedListenerProxyRegisterStop("");
        }

        public bool SharedListenerProxyRegisterFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.SharedListenerProxyRegisterFailed, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.ActivationListenerOpen,
            Keywords = Keywords.ActivationServices,
            Message = "SharedListenerProxy register failed with status '{0}'.")]
        public void SharedListenerProxyRegisterFailed(string Status, string AppDomain)
        {
            WriteEvent(EventIds.SharedListenerProxyRegisterFailed, Status, AppDomain);
        }

        [NonEvent]
        public void SharedListenerProxyRegisterFailed(string Status)
        {
            SharedListenerProxyRegisterFailed(Status, "");
        }

        public bool ConnectionPoolPreambleFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Channel, EventChannel.Analytic);
        }

        [Event(EventIds.ConnectionPoolPreambleFailed, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.SessionStart,
            Keywords = Keywords.Channel,
            Message = "ConnectionPoolPreambleFailed.")]
        public void ConnectionPoolPreambleFailed(string AppDomain)
        {
            WriteEvent(EventIds.ConnectionPoolPreambleFailed, AppDomain);
        }

        [NonEvent]
        public void ConnectionPoolPreambleFailed()
        {
            ConnectionPoolPreambleFailed("");
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

        public bool MtomMessageEncodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.MtomMessageEncodingStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.MessageEncoding,
            Keywords = Keywords.Channel,
            Message = "MtomMessageEncoder started encoding the message.")]
        public void MtomMessageEncodingStart(string AppDomain)
        {
            WriteEvent(EventIds.MtomMessageEncodingStart, AppDomain);
        }

        [NonEvent]
        public void MtomMessageEncodingStart()
        {
            MtomMessageEncodingStart("");
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

        public bool MtomMessageDecodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.MtomMessageDecodingStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.MessageDecoding,
            Keywords = Keywords.Channel,
            Message = "MtomMessageEncoder started decoding  the message.")]
        public void MtomMessageDecodingStart(string AppDomain)
        {
            WriteEvent(EventIds.MtomMessageDecodingStart, AppDomain);
        }

        [NonEvent]
        public void MtomMessageDecodingStart()
        {
            MtomMessageDecodingStart("");
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

        public bool HttpResponseReceiveStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.HttpResponseReceiveStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "Http transport started receiving a message.")]
        public void HttpResponseReceiveStart(string AppDomain)
        {
            WriteEvent(EventIds.HttpResponseReceiveStart, AppDomain);
        }

        [NonEvent]
        public void HttpResponseReceiveStart()
        {
            HttpResponseReceiveStart("");
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

        public bool SequenceAcknowledgementSentIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.SequenceAcknowledgementSent, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.ReliableSessionSequenceAck, Task = Tasks.ReliableSession,
            Keywords = Keywords.Channel,
            Message = "SessionId:{0} acknowledgement sent.")]
        public void SequenceAcknowledgementSent(string SessionId, string AppDomain)
        {
            WriteEvent(EventIds.SequenceAcknowledgementSent, SessionId, AppDomain);
        }

        [NonEvent]
        public void SequenceAcknowledgementSent(string SessionId)
        {
            SequenceAcknowledgementSent(SessionId, "");
        }

        public bool ClientReliableSessionReconnectIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ClientReliableSessionReconnect, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ReliableSessionReconnect, Task = Tasks.ReliableSession,
            Keywords = Keywords.Channel,
            Message = "SessionId:{0} reconnecting.")]
        public void ClientReliableSessionReconnect(string SessionId, string AppDomain)
        {
            WriteEvent(EventIds.ClientReliableSessionReconnect, SessionId, AppDomain);
        }

        [NonEvent]
        public void ClientReliableSessionReconnect(string SessionId)
        {
            ClientReliableSessionReconnect(SessionId, "");
        }

        public bool ReliableSessionChannelFaultedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ReliableSessionChannelFaulted, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.ReliableSessionFaulted, Task = Tasks.ReliableSession,
            Keywords = Keywords.Channel,
            Message = "SessionId:{0} faulted.")]
        public void ReliableSessionChannelFaulted(string SessionId, string AppDomain)
        {
            WriteEvent(EventIds.ReliableSessionChannelFaulted, SessionId, AppDomain);
        }

        [NonEvent]
        public void ReliableSessionChannelFaulted(string SessionId)
        {
            ReliableSessionChannelFaulted(SessionId, "");
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

        public bool SocketConnectionAbortIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.TCP, EventChannel.Analytic);
        }

        [Event(EventIds.SocketConnectionAbort, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.ConnectionAbort,
            Keywords = Keywords.TCP,
            Message = "SocketId:{0} is aborting.")]
        public void SocketConnectionAbort(int SocketId, string AppDomain)
        {
            WriteEvent(EventIds.SocketConnectionAbort, SocketId, AppDomain);
        }

        [NonEvent]
        public void SocketConnectionAbort(int SocketId)
        {
            SocketConnectionAbort(SocketId, "");
        }

        public bool HttpGetContextStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.HttpGetContextStart, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "HttpGetContext start.")]
        public void HttpGetContextStart(string AppDomain)
        {
            WriteEvent(EventIds.HttpGetContextStart, AppDomain);
        }

        [NonEvent]
        public void HttpGetContextStart()
        {
            HttpGetContextStart("");
        }

        public bool ClientSendPreambleStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ClientSendPreambleStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ClientSendPreamble,
            Keywords = Keywords.Channel,
            Message = "Client sending preamble start.")]
        public void ClientSendPreambleStart(string AppDomain)
        {
            WriteEvent(EventIds.ClientSendPreambleStart, AppDomain);
        }

        [NonEvent]
        public void ClientSendPreambleStart()
        {
            ClientSendPreambleStart("");
        }

        public bool ClientSendPreambleStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.ClientSendPreambleStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ClientSendPreamble,
            Keywords = Keywords.Channel,
            Message = "Client sending preamble stop.")]
        public void ClientSendPreambleStop(string AppDomain)
        {
            WriteEvent(EventIds.ClientSendPreambleStop, AppDomain);
        }

        [NonEvent]
        public void ClientSendPreambleStop()
        {
            ClientSendPreambleStop("");
        }

        public bool HttpMessageReceiveFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.HttpMessageReceiveFailed, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "Http Message receive failed.")]
        public void HttpMessageReceiveFailed(string AppDomain)
        {
            WriteEvent(EventIds.HttpMessageReceiveFailed, AppDomain);
        }

        [NonEvent]
        public void HttpMessageReceiveFailed()
        {
            HttpMessageReceiveFailed("");
        }

        public bool TransactionScopeCreateIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ServiceModel, EventChannel.Debug);
        }

        [Event(EventIds.TransactionScopeCreate, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.DispatchMessageTransactionScopeCreate, Task = Tasks.DispatchMessage,
            Keywords = Keywords.ServiceModel,
            Message = "TransactionScope is being created with LocalIdentifier:'{0}' and DistributedIdentifier:'{1}'.")]
        public void TransactionScopeCreate(string LocalId, Guid Distributed, string AppDomain)
        {
            WriteEvent(EventIds.TransactionScopeCreate, LocalId, Distributed, AppDomain);
        }

        [NonEvent]
        public void TransactionScopeCreate(string LocalId, Guid Distributed)
        {
            TransactionScopeCreate(LocalId, Distributed, "");
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

        public bool MessageWrittenAsynchronouslyByEncoderIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.MessageWrittenAsynchronouslyByEncoder, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.MessageEncoding,
            Keywords = Keywords.Channel,
            Message = "A message was written asynchronously by the encoder.")]
        public void MessageWrittenAsynchronouslyByEncoder(string AppDomain)
        {
            WriteEvent(EventIds.MessageWrittenAsynchronouslyByEncoder, AppDomain);
        }

        [NonEvent]
        public void MessageWrittenAsynchronouslyByEncoder()
        {
            MessageWrittenAsynchronouslyByEncoder("");
        }

        public bool BufferedAsyncWriteStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.BufferedAsyncWriteStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportSend,
            Keywords = Keywords.Channel,
            Message = "BufferId:{0} completed writing '{1}' bytes to underlying stream.")]
        public void BufferedAsyncWriteStart(int BufferId, int Size, string AppDomain)
        {
            WriteEvent(EventIds.BufferedAsyncWriteStart, BufferId, Size, AppDomain);
        }

        [NonEvent]
        public void BufferedAsyncWriteStart(int BufferId, int Size)
        {
            BufferedAsyncWriteStart(BufferId, Size, "");
        }

        public bool BufferedAsyncWriteStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.BufferedAsyncWriteStop, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.TransportSend,
            Keywords = Keywords.Channel,
            Message = "A message was written asynchronously by the encoder.")]
        public void BufferedAsyncWriteStop(string AppDomain)
        {
            WriteEvent(EventIds.BufferedAsyncWriteStop, AppDomain);
        }

        [NonEvent]
        public void BufferedAsyncWriteStop()
        {
            BufferedAsyncWriteStop("");
        }

        public bool PipeSharedMemoryCreatedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.PipeSharedMemoryCreated, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ListenerOpen,
            Keywords = Keywords.Channel,
            Message = "Pipe shared memory created at '{0}' .")]
        public void PipeSharedMemoryCreated(string sharedMemoryName, string AppDomain)
        {
            WriteEvent(EventIds.PipeSharedMemoryCreated, sharedMemoryName, AppDomain);
        }

        [NonEvent]
        public void PipeSharedMemoryCreated(string sharedMemoryName)
        {
            PipeSharedMemoryCreated(sharedMemoryName, "");
        }

        public bool NamedPipeCreatedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.NamedPipeCreated, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.ListenerOpen,
            Keywords = Keywords.Channel,
            Message = "NamedPipe '{0}' created.")]
        public void NamedPipeCreated(string pipeName, string AppDomain)
        {
            WriteEvent(EventIds.NamedPipeCreated, pipeName, AppDomain);
        }

        [NonEvent]
        public void NamedPipeCreated(string pipeName)
        {
            NamedPipeCreated(pipeName, "");
        }

        public bool SignatureVerificationStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.SignatureVerificationStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.SignatureVerification,
            Keywords = Keywords.Security,
            Message = "Signature verification started.")]
        public void SignatureVerificationStart(string AppDomain)
        {
            WriteEvent(EventIds.SignatureVerificationStart, AppDomain);
        }

        [NonEvent]
        public void SignatureVerificationStart()
        {
            SignatureVerificationStart("");
        }

        public bool SignatureVerificationSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.SignatureVerificationSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SignatureVerification,
            Keywords = Keywords.Security,
            Message = "Signature verification succeeded")]
        public void SignatureVerificationSuccess(string AppDomain)
        {
            WriteEvent(EventIds.SignatureVerificationSuccess, AppDomain);
        }

        [NonEvent]
        public void SignatureVerificationSuccess()
        {
            SignatureVerificationSuccess("");
        }

        public bool WrappedKeyDecryptionStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.WrappedKeyDecryptionStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.WrappedKeyDecryption,
            Keywords = Keywords.Security,
            Message = "Wrapped key decryption started.")]
        public void WrappedKeyDecryptionStart(string AppDomain)
        {
            WriteEvent(EventIds.WrappedKeyDecryptionStart, AppDomain);
        }

        [NonEvent]
        public void WrappedKeyDecryptionStart()
        {
            WrappedKeyDecryptionStart("");
        }

        public bool WrappedKeyDecryptionSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.WrappedKeyDecryptionSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.WrappedKeyDecryption,
            Keywords = Keywords.Security,
            Message = "Wrapped key decryption succeeded.")]
        public void WrappedKeyDecryptionSuccess(string AppDomain)
        {
            WriteEvent(EventIds.WrappedKeyDecryptionSuccess, AppDomain);
        }

        [NonEvent]
        public void WrappedKeyDecryptionSuccess()
        {
            WrappedKeyDecryptionSuccess("");
        }

        public bool EncryptedDataProcessingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.EncryptedDataProcessingStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.EncryptedDataProcessing,
            Keywords = Keywords.Security,
            Message = "Encrypted data processing started.")]
        public void EncryptedDataProcessingStart(string AppDomain)
        {
            WriteEvent(EventIds.EncryptedDataProcessingStart, AppDomain);
        }

        [NonEvent]
        public void EncryptedDataProcessingStart()
        {
            EncryptedDataProcessingStart("");
        }

        public bool EncryptedDataProcessingSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.EncryptedDataProcessingSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.EncryptedDataProcessing,
            Keywords = Keywords.Security,
            Message = "Encrypted data processing succeeded.")]
        public void EncryptedDataProcessingSuccess(string AppDomain)
        {
            WriteEvent(EventIds.EncryptedDataProcessingSuccess, AppDomain);
        }

        [NonEvent]
        public void EncryptedDataProcessingSuccess()
        {
            EncryptedDataProcessingSuccess("");
        }

        public bool HttpPipelineProcessInboundRequestStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.HttpPipelineProcessInboundRequestStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "Http message handler started processing the inbound request.")]
        public void HttpPipelineProcessInboundRequestStart(string AppDomain)
        {
            WriteEvent(EventIds.HttpPipelineProcessInboundRequestStart, AppDomain);
        }

        [NonEvent]
        public void HttpPipelineProcessInboundRequestStart()
        {
            HttpPipelineProcessInboundRequestStart("");
        }

        public bool HttpPipelineBeginProcessInboundRequestStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.HttpPipelineBeginProcessInboundRequestStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "Http message handler started processing the inbound request asynchronously.")]
        public void HttpPipelineBeginProcessInboundRequestStart(string AppDomain)
        {
            WriteEvent(EventIds.HttpPipelineBeginProcessInboundRequestStart, AppDomain);
        }

        [NonEvent]
        public void HttpPipelineBeginProcessInboundRequestStart()
        {
            HttpPipelineBeginProcessInboundRequestStart("");
        }

        public bool HttpPipelineProcessInboundRequestStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.HttpPipelineProcessInboundRequestStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "Http message handler completed processing an inbound request.")]
        public void HttpPipelineProcessInboundRequestStop(string AppDomain)
        {
            WriteEvent(EventIds.HttpPipelineProcessInboundRequestStop, AppDomain);
        }

        [NonEvent]
        public void HttpPipelineProcessInboundRequestStop()
        {
            HttpPipelineProcessInboundRequestStop("");
        }

        public bool HttpPipelineFaultedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.HttpPipelineFaulted, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.TransportReceive,
            Keywords = Keywords.HTTP,
            Message = "Http message handler is faulted.")]
        public void HttpPipelineFaulted(string AppDomain)
        {
            WriteEvent(EventIds.HttpPipelineFaulted, AppDomain);
        }

        [NonEvent]
        public void HttpPipelineFaulted()
        {
            HttpPipelineFaulted("");
        }

        public bool HttpPipelineTimeoutExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.HttpPipelineTimeoutException, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocket connection timed out.")]
        public void HttpPipelineTimeoutException(string AppDomain)
        {
            WriteEvent(EventIds.HttpPipelineTimeoutException, AppDomain);
        }

        [NonEvent]
        public void HttpPipelineTimeoutException()
        {
            HttpPipelineTimeoutException("");
        }

        public bool HttpPipelineProcessResponseStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.HttpPipelineProcessResponseStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportSend,
            Keywords = Keywords.HTTP,
            Message = "Http message handler started processing the response.")]
        public void HttpPipelineProcessResponseStart(string AppDomain)
        {
            WriteEvent(EventIds.HttpPipelineProcessResponseStart, AppDomain);
        }

        [NonEvent]
        public void HttpPipelineProcessResponseStart()
        {
            HttpPipelineProcessResponseStart("");
        }

        public bool HttpPipelineBeginProcessResponseStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.HttpPipelineBeginProcessResponseStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.TransportSend,
            Keywords = Keywords.HTTP,
            Message = "Http message handler started processing the response asynchronously.")]
        public void HttpPipelineBeginProcessResponseStart(string AppDomain)
        {
            WriteEvent(EventIds.HttpPipelineBeginProcessResponseStart, AppDomain);
        }

        [NonEvent]
        public void HttpPipelineBeginProcessResponseStart()
        {
            HttpPipelineBeginProcessResponseStart("");
        }

        public bool HttpPipelineProcessResponseStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.HttpPipelineProcessResponseStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.TransportSend,
            Keywords = Keywords.HTTP,
            Message = "Http message handler completed processing the response.")]
        public void HttpPipelineProcessResponseStop(string AppDomain)
        {
            WriteEvent(EventIds.HttpPipelineProcessResponseStop, AppDomain);
        }

        [NonEvent]
        public void HttpPipelineProcessResponseStop()
        {
            HttpPipelineProcessResponseStop("");
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

        public bool WebSocketConnectionAcceptStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketConnectionAcceptStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocket connection accept start.")]
        public void WebSocketConnectionAcceptStart(string AppDomain)
        {
            WriteEvent(EventIds.WebSocketConnectionAcceptStart, AppDomain);
        }

        [NonEvent]
        public void WebSocketConnectionAcceptStart()
        {
            WebSocketConnectionAcceptStart("");
        }

        public bool WebSocketConnectionAcceptedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketConnectionAccepted, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocketId:{0} connection accepted.")]
        public void WebSocketConnectionAccepted(int websocketId, string AppDomain)
        {
            WriteEvent(EventIds.WebSocketConnectionAccepted, websocketId, AppDomain);
        }

        [NonEvent]
        public void WebSocketConnectionAccepted(int websocketId)
        {
            WebSocketConnectionAccepted(websocketId, "");
        }

        public bool WebSocketConnectionDeclinedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.HTTP, EventChannel.Analytic);
        }

        [Event(EventIds.WebSocketConnectionDeclined, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "WebSocket connection declined with status code '{0}'")]
        public void WebSocketConnectionDeclined(string errorMessage, string AppDomain)
        {
            WriteEvent(EventIds.WebSocketConnectionDeclined, errorMessage, AppDomain);
        }

        [NonEvent]
        public void WebSocketConnectionDeclined(string errorMessage)
        {
            WebSocketConnectionDeclined(errorMessage, "");
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

        public bool WebSocketUseVersionFromClientWebSocketFactoryIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.HTTP, EventChannel.Debug);
        }

        [Event(EventIds.WebSocketUseVersionFromClientWebSocketFactory, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Connect,
            Keywords = Keywords.HTTP,
            Message = "Using the WebSocketVersion from a client WebSocket factory of type '{0}'.")]
        public void WebSocketUseVersionFromClientWebSocketFactory(string clientWebSocketFactoryType, string AppDomain)
        {
            WriteEvent(EventIds.WebSocketUseVersionFromClientWebSocketFactory, clientWebSocketFactoryType, AppDomain);
        }

        [NonEvent]
        public void WebSocketUseVersionFromClientWebSocketFactory(string clientWebSocketFactoryType)
        {
            WebSocketUseVersionFromClientWebSocketFactory(clientWebSocketFactoryType, "");
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

        public bool InferredContractDescriptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.InferredContractDescription, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.InferDescriptionContract, Task = Tasks.InferDescription,
            Keywords = Keywords.WFServices,
            Message = "ContractDescription with Name='{0}' and Namespace='{1}' has been inferred from WorkflowService.")]
        public void InferredContractDescription(string data1, string data2, string AppDomain)
        {
            WriteEvent(EventIds.InferredContractDescription, data1, data2, AppDomain);
        }

        [NonEvent]
        public void InferredContractDescription(string data1, string data2)
        {
            InferredContractDescription(data1, data2, "");
        }

        public bool InferredOperationDescriptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.InferredOperationDescription, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.InferDescriptionOperation, Task = Tasks.InferDescription,
            Keywords = Keywords.WFServices,
            Message = "OperationDescription with Name='{0}' in contract '{1}' has been inferred from WorkflowService. IsOneWay={2}.")]
        public void InferredOperationDescription(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.InferredOperationDescription, data1, data2, data3, AppDomain);
        }

        [NonEvent]
        public void InferredOperationDescription(string data1, string data2, string data3)
        {
            InferredOperationDescription(data1, data2, data3, "");
        }

        public bool DuplicateCorrelationQueryIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.DuplicateCorrelationQuery, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = Opcodes.CorrelationDuplicateQuery, Task = Tasks.Correlation,
            Keywords = Keywords.WFServices,
            Message = "A duplicate CorrelationQuery was found with Where='{0}'. This duplicate query will not be used when calculating correlation.")]
        public void DuplicateCorrelationQuery(string data1, string AppDomain)
        {
            WriteEvent(EventIds.DuplicateCorrelationQuery, data1, AppDomain);
        }

        [NonEvent]
        public void DuplicateCorrelationQuery(string data1)
        {
            DuplicateCorrelationQuery(data1, "");
        }

        public bool ServiceEndpointAddedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceEndpointAdded, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.AddServiceEndpoint,
            Keywords = Keywords.WFServices,
            Message = "A service endpoint has been added for address '{0}', binding '{1}', and contract '{2}'.")]
        public void ServiceEndpointAdded(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.ServiceEndpointAdded, data1, data2, data3, AppDomain);
        }

        [NonEvent]
        public void ServiceEndpointAdded(string data1, string data2, string data3)
        {
            ServiceEndpointAdded(data1, data2, data3, "");
        }

        public bool TrackingProfileNotFoundIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.TrackingProfileNotFound, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.TrackingProfileNotFound, Task = Tasks.TrackingProfile,
            Keywords = Keywords.WFServices,
            Message = "TrackingProfile '{0}' for the ActivityDefinitionId '{1}' not found. Either the TrackingProfile is not found in the config file or the ActivityDefinitionId does not match.")]
        public void TrackingProfileNotFound(string TrackingProfile, string ActivityDefinitionId, string AppDomain)
        {
            WriteEvent(EventIds.TrackingProfileNotFound, TrackingProfile, ActivityDefinitionId, AppDomain);
        }

        [NonEvent]
        public void TrackingProfileNotFound(string TrackingProfile, string ActivityDefinitionId)
        {
            TrackingProfileNotFound(TrackingProfile, ActivityDefinitionId, "");
        }

        public bool BufferOutOfOrderMessageNoInstanceIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.BufferOutOfOrderMessageNoInstance, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.BufferOutOfOrderNoInstance, Task = Tasks.BufferOutOfOrder,
            Keywords = Keywords.WFServices,
            Message = "Operation '{0}' cannot be performed at this time. Another attempt will be made when the service instance is ready to process this particular operation.")]
        public void BufferOutOfOrderMessageNoInstance(string data1, string AppDomain)
        {
            WriteEvent(EventIds.BufferOutOfOrderMessageNoInstance, data1, AppDomain);
        }

        [NonEvent]
        public void BufferOutOfOrderMessageNoInstance(string data1)
        {
            BufferOutOfOrderMessageNoInstance(data1, "");
        }

        public bool BufferOutOfOrderMessageNoBookmarkIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.BufferOutOfOrderMessageNoBookmark, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = Opcodes.BufferOutOfOrderNoBookmark, Task = Tasks.BufferOutOfOrder,
            Keywords = Keywords.WFServices,
            Message = "Operation '{1}' on service instance '{0}' cannot be performed at this time. Another attempt will be made when the service instance is ready to process this particular operation.")]
        public void BufferOutOfOrderMessageNoBookmark(string data1, string data2, string AppDomain)
        {
            WriteEvent(EventIds.BufferOutOfOrderMessageNoBookmark, data1, data2, AppDomain);
        }

        [NonEvent]
        public void BufferOutOfOrderMessageNoBookmark(string data1, string data2)
        {
            BufferOutOfOrderMessageNoBookmark(data1, data2, "");
        }

        public bool MaxPendingMessagesPerChannelExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Quota | Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.MaxPendingMessagesPerChannelExceeded, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota | Keywords.WFServices,
            Message = "The throttle 'MaxPendingMessagesPerChannel' limit of  '{0}' was hit. To increase this limit, adjust the MaxPendingMessagesPerChannel property on BufferedReceiveServiceBehavior.")]
        public void MaxPendingMessagesPerChannelExceeded(int limit, string AppDomain)
        {
            WriteEvent(EventIds.MaxPendingMessagesPerChannelExceeded, limit, AppDomain);
        }

        [NonEvent]
        public void MaxPendingMessagesPerChannelExceeded(int limit)
        {
            MaxPendingMessagesPerChannelExceeded(limit, "");
        }

        public bool XamlServicesLoadStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.XamlServicesLoadStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.XamlServicesLoad,
            Keywords = Keywords.WebHost,
            Message = "XamlServicesLoad start")]
        public void XamlServicesLoadStart(string AppDomain)
        {
            WriteEvent(EventIds.XamlServicesLoadStart, AppDomain);
        }

        [NonEvent]
        public void XamlServicesLoadStart()
        {
            XamlServicesLoadStart("");
        }

        public bool XamlServicesLoadStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.XamlServicesLoadStop, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.XamlServicesLoad,
            Keywords = Keywords.WebHost,
            Message = "XamlServicesLoad Stop")]
        public void XamlServicesLoadStop(string AppDomain)
        {
            WriteEvent(EventIds.XamlServicesLoadStop, AppDomain);
        }

        [NonEvent]
        public void XamlServicesLoadStop()
        {
            XamlServicesLoadStop("");
        }

        public bool CreateWorkflowServiceHostStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.CreateWorkflowServiceHostStart, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.CreateWorkflowServiceHost,
            Keywords = Keywords.WebHost,
            Message = "CreateWorkflowServiceHost start")]
        public void CreateWorkflowServiceHostStart(string AppDomain)
        {
            WriteEvent(EventIds.CreateWorkflowServiceHostStart, AppDomain);
        }

        [NonEvent]
        public void CreateWorkflowServiceHostStart()
        {
            CreateWorkflowServiceHostStart("");
        }

        public bool CreateWorkflowServiceHostStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.CreateWorkflowServiceHostStop, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.CreateWorkflowServiceHost,
            Keywords = Keywords.WebHost,
            Message = "CreateWorkflowServiceHost Stop")]
        public void CreateWorkflowServiceHostStop(string AppDomain)
        {
            WriteEvent(EventIds.CreateWorkflowServiceHostStop, AppDomain);
        }

        [NonEvent]
        public void CreateWorkflowServiceHostStop()
        {
            CreateWorkflowServiceHostStop("");
        }

        public bool TransactedReceiveScopeEndCommitFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.TransactedReceiveScopeEndCommitFailed, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.RuntimeTransaction,
            Keywords = Keywords.WFServices,
            Message = "The call to EndCommit on the CommittableTransaction with id = '{0}' threw a TransactionException with the following message: '{1}'.")]
        public void TransactedReceiveScopeEndCommitFailed(string data1, string data2, string AppDomain)
        {
            WriteEvent(EventIds.TransactedReceiveScopeEndCommitFailed, data1, data2, AppDomain);
        }

        [NonEvent]
        public void TransactedReceiveScopeEndCommitFailed(string data1, string data2)
        {
            TransactedReceiveScopeEndCommitFailed(data1, data2, "");
        }

        public bool ServiceActivationStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceActivationStart, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.ServiceActivation,
            Keywords = Keywords.WebHost,
            Message = "Service activation start")]
        public void ServiceActivationStart(string AppDomain)
        {
            WriteEvent(EventIds.ServiceActivationStart, AppDomain);
        }

        [NonEvent]
        public void ServiceActivationStart()
        {
            ServiceActivationStart("");
        }

        public bool ServiceActivationStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceActivationStop, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.ServiceActivation,
            Keywords = Keywords.WebHost,
            Message = "Service activation Stop")]
        public void ServiceActivationStop(string AppDomain)
        {
            WriteEvent(EventIds.ServiceActivationStop, AppDomain);
        }

        [NonEvent]
        public void ServiceActivationStop()
        {
            ServiceActivationStop("");
        }

        public bool ServiceActivationAvailableMemoryIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Analytic);
        }

        public bool ServiceActivationExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WebHost, EventChannel.Operational);
        }

        [Event(EventIds.ServiceActivationException, Level = EventLevel.Error, Channel = EventChannel.Operational, Opcode = EventOpcode.Info,
            Keywords = Keywords.WebHost,
            Message = "The service could not be activated. Exception details: {0}")]
        public void ServiceActivationException(string data1, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.ServiceActivationException, data1, SerializedException, AppDomain);
        }

        [NonEvent]
        public void ServiceActivationException(string data1, string SerializedException)
        {
            ServiceActivationException(data1, SerializedException, "");
        }

        public bool RoutingServiceClosingClientIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceClosingClient, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceClientClosing, Task = Tasks.RoutingServiceClient,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is closing client '{0}'.")]
        public void RoutingServiceClosingClient(string data1, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceClosingClient, data1, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceClosingClient(string data1)
        {
            RoutingServiceClosingClient(data1, "");
        }

        public bool RoutingServiceChannelFaultedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceChannelFaulted, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceClientChannelFaulted, Task = Tasks.RoutingServiceClient,
            Keywords = Keywords.RoutingServices,
            Message = "Routing Service client '{0}' has faulted.")]
        public void RoutingServiceChannelFaulted(string data1, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceChannelFaulted, data1, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceChannelFaulted(string data1)
        {
            RoutingServiceChannelFaulted(data1, "");
        }

        public bool RoutingServiceCompletingOneWayIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceCompletingOneWay, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceMessageCompletingOneWay, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "A Routing Service one way message is completing.")]
        public void RoutingServiceCompletingOneWay(string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceCompletingOneWay, SerializedException, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceCompletingOneWay(string SerializedException)
        {
            RoutingServiceCompletingOneWay(SerializedException, "");
        }

        public bool RoutingServiceProcessingFailureIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceProcessingFailure, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceMessageProcessingFailure, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service failed while processing a message on the endpoint with address '{0}'.")]
        public void RoutingServiceProcessingFailure(string data1, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceProcessingFailure, data1, SerializedException, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceProcessingFailure(string data1, string SerializedException)
        {
            RoutingServiceProcessingFailure(data1, SerializedException, "");
        }

        public bool RoutingServiceCreatingClientForEndpointIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceCreatingClientForEndpoint, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceClientCreatingForEndpoint, Task = Tasks.RoutingServiceClient,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is creating a client for endpoint: '{0}'.")]
        public void RoutingServiceCreatingClientForEndpoint(string data1, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceCreatingClientForEndpoint, data1, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceCreatingClientForEndpoint(string data1)
        {
            RoutingServiceCreatingClientForEndpoint(data1, "");
        }

        public bool RoutingServiceDisplayConfigIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceDisplayConfig, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is configured with RouteOnHeadersOnly: {0}, SoapProcessingEnabled: {1}, EnsureOrderedDispatch: {2}.")]
        public void RoutingServiceDisplayConfig(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceDisplayConfig, data1, data2, data3, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceDisplayConfig(string data1, string data2, string data3)
        {
            RoutingServiceDisplayConfig(data1, data2, data3, "");
        }

        public bool RoutingServiceCompletingTwoWayIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceCompletingTwoWay, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceMessageCompletingTwoWay, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "A Routing Service request reply message is completing.")]
        public void RoutingServiceCompletingTwoWay(string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceCompletingTwoWay, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceCompletingTwoWay()
        {
            RoutingServiceCompletingTwoWay("");
        }

        public bool RoutingServiceMessageRoutedToEndpointsIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceMessageRoutedToEndpoints, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceMessageRoutedToEndpoints, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service routed message with ID: '{0}' to {1} endpoint lists.")]
        public void RoutingServiceMessageRoutedToEndpoints(string data1, string data2, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceMessageRoutedToEndpoints, data1, data2, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceMessageRoutedToEndpoints(string data1, string data2)
        {
            RoutingServiceMessageRoutedToEndpoints(data1, data2, "");
        }

        public bool RoutingServiceConfigurationAppliedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceConfigurationApplied, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceConfigurationApplied, Task = Tasks.RoutingService,
            Keywords = Keywords.RoutingServices,
            Message = "A new RoutingConfiguration has been applied to the Routing Service.")]
        public void RoutingServiceConfigurationApplied(string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceConfigurationApplied, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceConfigurationApplied()
        {
            RoutingServiceConfigurationApplied("");
        }

        public bool RoutingServiceProcessingMessageIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceProcessingMessage, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceMessageProcessingMessage, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is processing a message with ID: '{0}', Action: '{1}', Inbound URL: '{2}' Received in Transaction: {3}.")]
        public void RoutingServiceProcessingMessage(string data1, string data2, string data3, string data4, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceProcessingMessage, data1, data2, data3, data4, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceProcessingMessage(string data1, string data2, string data3, string data4)
        {
            RoutingServiceProcessingMessage(data1, data2, data3, data4, "");
        }

        public bool RoutingServiceTransmittingMessageIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceTransmittingMessage, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceMessageTransmittingMessage, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is transmitting the message with ID: '{0}' [operation {1}] to '{2}'.")]
        public void RoutingServiceTransmittingMessage(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceTransmittingMessage, data1, data2, data3, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceTransmittingMessage(string data1, string data2, string data3)
        {
            RoutingServiceTransmittingMessage(data1, data2, data3, "");
        }

        public bool RoutingServiceCommittingTransactionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceCommittingTransaction, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceTransactionCommittingTransaction, Task = Tasks.RoutingServiceTransaction,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is committing a transaction with id: '{0}'.")]
        public void RoutingServiceCommittingTransaction(string data1, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceCommittingTransaction, data1, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceCommittingTransaction(string data1)
        {
            RoutingServiceCommittingTransaction(data1, "");
        }

        public bool RoutingServiceDuplexCallbackExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceDuplexCallbackException, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceDuplexCallbackException, Task = Tasks.RoutingService,
            Keywords = Keywords.RoutingServices,
            Message = "Routing Service component {0} encountered a duplex callback exception.")]
        public void RoutingServiceDuplexCallbackException(string data1, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceDuplexCallbackException, data1, SerializedException, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceDuplexCallbackException(string data1, string SerializedException)
        {
            RoutingServiceDuplexCallbackException(data1, SerializedException, "");
        }

        public bool RoutingServiceMovedToBackupIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceMovedToBackup, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceMessageMovedToBackup, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "Routing Service message with ID: '{0}' [operation {1}] moved to backup endpoint '{2}'.")]
        public void RoutingServiceMovedToBackup(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceMovedToBackup, data1, data2, data3, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceMovedToBackup(string data1, string data2, string data3)
        {
            RoutingServiceMovedToBackup(data1, data2, data3, "");
        }

        public bool RoutingServiceCreatingTransactionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceCreatingTransaction, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceTransactionCreating, Task = Tasks.RoutingServiceTransaction,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service created a new Transaction with id '{0}' for processing message(s).")]
        public void RoutingServiceCreatingTransaction(string data1, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceCreatingTransaction, data1, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceCreatingTransaction(string data1)
        {
            RoutingServiceCreatingTransaction(data1, "");
        }

        public bool RoutingServiceCloseFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceCloseFailed, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceCloseFailed, Task = Tasks.RoutingService,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service failed while closing outbound client '{0}'.")]
        public void RoutingServiceCloseFailed(string data1, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceCloseFailed, data1, SerializedException, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceCloseFailed(string data1, string SerializedException)
        {
            RoutingServiceCloseFailed(data1, SerializedException, "");
        }

        public bool RoutingServiceSendingResponseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceSendingResponse, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceMessageSendingResponse, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is sending back a response message with Action '{0}'.")]
        public void RoutingServiceSendingResponse(string data1, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceSendingResponse, data1, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceSendingResponse(string data1)
        {
            RoutingServiceSendingResponse(data1, "");
        }

        public bool RoutingServiceSendingFaultResponseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceSendingFaultResponse, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceMessageSendingFaultResponse, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is sending back a Fault response message with Action '{0}'.")]
        public void RoutingServiceSendingFaultResponse(string data1, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceSendingFaultResponse, data1, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceSendingFaultResponse(string data1)
        {
            RoutingServiceSendingFaultResponse(data1, "");
        }

        public bool RoutingServiceCompletingReceiveContextIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceCompletingReceiveContext, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceReceiveContextCompleting, Task = Tasks.RoutingServiceReceiveContext,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is calling ReceiveContext.Complete for Message with ID: '{0}'.")]
        public void RoutingServiceCompletingReceiveContext(string data1, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceCompletingReceiveContext, data1, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceCompletingReceiveContext(string data1)
        {
            RoutingServiceCompletingReceiveContext(data1, "");
        }

        public bool RoutingServiceAbandoningReceiveContextIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceAbandoningReceiveContext, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceReceiveContextAbandoning, Task = Tasks.RoutingServiceReceiveContext,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is calling ReceiveContext.Abandon for Message with ID: '{0}'.")]
        public void RoutingServiceAbandoningReceiveContext(string data1, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceAbandoningReceiveContext, data1, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceAbandoningReceiveContext(string data1)
        {
            RoutingServiceAbandoningReceiveContext(data1, "");
        }

        public bool RoutingServiceUsingExistingTransactionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceUsingExistingTransaction, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceTransactionUsingExisting, Task = Tasks.RoutingServiceTransaction,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service will send messages using existing transaction '{0}'.")]
        public void RoutingServiceUsingExistingTransaction(string data1, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceUsingExistingTransaction, data1, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceUsingExistingTransaction(string data1)
        {
            RoutingServiceUsingExistingTransaction(data1, "");
        }

        public bool RoutingServiceTransmitFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceTransmitFailed, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceTransmitFailed, Task = Tasks.RoutingService,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service failed while sending to '{0}'.")]
        public void RoutingServiceTransmitFailed(string data1, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceTransmitFailed, data1, SerializedException, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceTransmitFailed(string data1, string SerializedException)
        {
            RoutingServiceTransmitFailed(data1, SerializedException, "");
        }

        public bool RoutingServiceFilterTableMatchStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Analytic);
        }

        [Event(EventIds.RoutingServiceFilterTableMatchStart, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.RoutingServiceFilterTableMatch,
            Keywords = Keywords.RoutingServices,
            Message = "Routing Service MessageFilterTable Match Start.")]
        public void RoutingServiceFilterTableMatchStart(string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceFilterTableMatchStart, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceFilterTableMatchStart()
        {
            RoutingServiceFilterTableMatchStart("");
        }

        public bool RoutingServiceFilterTableMatchStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Analytic);
        }

        [Event(EventIds.RoutingServiceFilterTableMatchStop, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.RoutingServiceFilterTableMatch,
            Keywords = Keywords.RoutingServices,
            Message = "Routing Service MessageFilterTable Match Stop.")]
        public void RoutingServiceFilterTableMatchStop(string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceFilterTableMatchStop, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceFilterTableMatchStop()
        {
            RoutingServiceFilterTableMatchStop("");
        }

        public bool RoutingServiceAbortingChannelIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceAbortingChannel, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceAbortingChannel, Task = Tasks.RoutingService,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service is calling abort on channel: '{0}'.")]
        public void RoutingServiceAbortingChannel(string data1, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceAbortingChannel, data1, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceAbortingChannel(string data1)
        {
            RoutingServiceAbortingChannel(data1, "");
        }

        public bool RoutingServiceHandledExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceHandledException, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceHandledException, Task = Tasks.RoutingService,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service has handled an exception.")]
        public void RoutingServiceHandledException(string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceHandledException, SerializedException, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceHandledException(string SerializedException)
        {
            RoutingServiceHandledException(SerializedException, "");
        }

        public bool RoutingServiceTransmitSucceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.RoutingServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingServiceTransmitSucceeded, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.RoutingServiceMessageTransmitSucceeded, Task = Tasks.RoutingServiceMessage,
            Keywords = Keywords.RoutingServices,
            Message = "The Routing Service successfully transmitted Message with ID: '{0} [operation {1}] to '{2}'.")]
        public void RoutingServiceTransmitSucceeded(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.RoutingServiceTransmitSucceeded, data1, data2, data3, AppDomain);
        }

        [NonEvent]
        public void RoutingServiceTransmitSucceeded(string data1, string data2, string data3)
        {
            RoutingServiceTransmitSucceeded(data1, data2, data3, "");
        }

        public bool TransportListenerSessionsReceivedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.TransportListenerSessionsReceived, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Receive, Task = Tasks.ActivationDispatchSession,
            Keywords = Keywords.ActivationServices,
            Message = "Transport listener session received with via '{0}'")]
        public void TransportListenerSessionsReceived(string via, string AppDomain)
        {
            WriteEvent(EventIds.TransportListenerSessionsReceived, via, AppDomain);
        }

        [NonEvent]
        public void TransportListenerSessionsReceived(string via)
        {
            TransportListenerSessionsReceived(via, "");
        }

        public bool FailFastExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Critical, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.FailFastException, Level = EventLevel.Critical, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.WASActivation,
            Keywords = Keywords.ActivationServices,
            Message = "FailFastException.")]
        public void FailFastException(string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.FailFastException, SerializedException, AppDomain);
        }

        [NonEvent]
        public void FailFastException(string SerializedException)
        {
            FailFastException(SerializedException, "");
        }

        public bool ServiceStartPipeErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.ServiceStartPipeError, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.ActivationServiceStart,
            Keywords = Keywords.ActivationServices,
            Message = "Service start pipe error.")]
        public void ServiceStartPipeError(string Endpoint, string AppDomain)
        {
            WriteEvent(EventIds.ServiceStartPipeError, Endpoint, AppDomain);
        }

        [NonEvent]
        public void ServiceStartPipeError(string Endpoint)
        {
            ServiceStartPipeError(Endpoint, "");
        }

        public bool DispatchSessionStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.DispatchSessionStart, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.ActivationDispatchSession,
            Keywords = Keywords.ActivationServices,
            Message = "Session dispatch started.")]
        public void DispatchSessionStart(string AppDomain)
        {
            WriteEvent(EventIds.DispatchSessionStart, AppDomain);
        }

        [NonEvent]
        public void DispatchSessionStart()
        {
            DispatchSessionStart("");
        }

        public bool PendingSessionQueueFullIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.PendingSessionQueueFull, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.ActivationDispatchSession,
            Keywords = Keywords.ActivationServices,
            Message = "Session dispatch for '{0}' failed since pending session queue is full with '{1}' pending items.")]
        public void PendingSessionQueueFull(string Uri, int count, string AppDomain)
        {
            WriteEvent(EventIds.PendingSessionQueueFull, Uri, count, AppDomain);
        }

        [NonEvent]
        public void PendingSessionQueueFull(string Uri, int count)
        {
            PendingSessionQueueFull(Uri, count, "");
        }

        public bool MessageQueueRegisterStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.MessageQueueRegisterStart, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.MessageQueueRegister,
            Keywords = Keywords.ActivationServices,
            Message = "Message queue register start.")]
        public void MessageQueueRegisterStart(string AppDomain)
        {
            WriteEvent(EventIds.MessageQueueRegisterStart, AppDomain);
        }

        [NonEvent]
        public void MessageQueueRegisterStart()
        {
            MessageQueueRegisterStart("");
        }

        public bool MessageQueueRegisterAbortIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.MessageQueueRegisterAbort, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.MessageQueueRegister,
            Keywords = Keywords.ActivationServices,
            Message = "Message queue registration aborted with status:'{0}' for uri:'{1}'.")]
        public void MessageQueueRegisterAbort(string Status, string Uri, string AppDomain)
        {
            WriteEvent(EventIds.MessageQueueRegisterAbort, Status, Uri, AppDomain);
        }

        [NonEvent]
        public void MessageQueueRegisterAbort(string Status, string Uri)
        {
            MessageQueueRegisterAbort(Status, Uri, "");
        }

        public bool MessageQueueUnregisterSucceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.MessageQueueUnregisterSucceeded, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.MessageQueueRegister,
            Keywords = Keywords.ActivationServices,
            Message = "Message queue unregister succeeded for uri:'{0}'.")]
        public void MessageQueueUnregisterSucceeded(string Uri, string AppDomain)
        {
            WriteEvent(EventIds.MessageQueueUnregisterSucceeded, Uri, AppDomain);
        }

        [NonEvent]
        public void MessageQueueUnregisterSucceeded(string Uri)
        {
            MessageQueueUnregisterSucceeded(Uri, "");
        }

        public bool MessageQueueRegisterFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.MessageQueueRegisterFailed, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.MessageQueueRegister,
            Keywords = Keywords.ActivationServices,
            Message = "Message queue registration for uri:'{0}' failed with status:'{1}'.")]
        public void MessageQueueRegisterFailed(string Uri, string Status, string AppDomain)
        {
            WriteEvent(EventIds.MessageQueueRegisterFailed, Uri, Status, AppDomain);
        }

        [NonEvent]
        public void MessageQueueRegisterFailed(string Uri, string Status)
        {
            MessageQueueRegisterFailed(Uri, Status, "");
        }

        public bool MessageQueueRegisterCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.MessageQueueRegisterCompleted, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.MessageQueueRegister,
            Keywords = Keywords.ActivationServices,
            Message = "Message queue registration completed for uri '{0}'.")]
        public void MessageQueueRegisterCompleted(string Uri, string AppDomain)
        {
            WriteEvent(EventIds.MessageQueueRegisterCompleted, Uri, AppDomain);
        }

        [NonEvent]
        public void MessageQueueRegisterCompleted(string Uri)
        {
            MessageQueueRegisterCompleted(Uri, "");
        }

        public bool MessageQueueDuplicatedSocketErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.MessageQueueDuplicatedSocketError, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.ActivationDispatchSession,
            Keywords = Keywords.ActivationServices,
            Message = "Message queue failed duplicating socket.")]
        public void MessageQueueDuplicatedSocketError(string AppDomain)
        {
            WriteEvent(EventIds.MessageQueueDuplicatedSocketError, AppDomain);
        }

        [NonEvent]
        public void MessageQueueDuplicatedSocketError()
        {
            MessageQueueDuplicatedSocketError("");
        }

        public bool MessageQueueDuplicatedSocketCompleteIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.MessageQueueDuplicatedSocketComplete, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.ActivationDispatchSession,
            Keywords = Keywords.ActivationServices,
            Message = "MessageQueueDuplicatedSocketComplete")]
        public void MessageQueueDuplicatedSocketComplete(string AppDomain)
        {
            WriteEvent(EventIds.MessageQueueDuplicatedSocketComplete, AppDomain);
        }

        [NonEvent]
        public void MessageQueueDuplicatedSocketComplete()
        {
            MessageQueueDuplicatedSocketComplete("");
        }

        public bool TcpTransportListenerListeningStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.TcpTransportListenerListeningStart, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.ActivationTcpListenerListening,
            Keywords = Keywords.ActivationServices,
            Message = "Tcp transport listener starting to listen on uri:'{0}'.")]
        public void TcpTransportListenerListeningStart(string Uri, string AppDomain)
        {
            WriteEvent(EventIds.TcpTransportListenerListeningStart, Uri, AppDomain);
        }

        [NonEvent]
        public void TcpTransportListenerListeningStart(string Uri)
        {
            TcpTransportListenerListeningStart(Uri, "");
        }

        public bool TcpTransportListenerListeningStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.TcpTransportListenerListeningStop, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.ActivationTcpListenerListening,
            Keywords = Keywords.ActivationServices,
            Message = "Tcp transport listener listening.")]
        public void TcpTransportListenerListeningStop(string AppDomain)
        {
            WriteEvent(EventIds.TcpTransportListenerListeningStop, AppDomain);
        }

        [NonEvent]
        public void TcpTransportListenerListeningStop()
        {
            TcpTransportListenerListeningStop("");
        }

        public bool WebhostUnregisterProtocolFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.WebhostUnregisterProtocolFailed, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.WASActivation,
            Keywords = Keywords.ActivationServices,
            Message = "Error Code:{0}")]
        public void WebhostUnregisterProtocolFailed(string hresult, string AppDomain)
        {
            WriteEvent(EventIds.WebhostUnregisterProtocolFailed, hresult, AppDomain);
        }

        [NonEvent]
        public void WebhostUnregisterProtocolFailed(string hresult)
        {
            WebhostUnregisterProtocolFailed(hresult, "");
        }

        public bool WasCloseAllListenerChannelInstancesCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.WasCloseAllListenerChannelInstancesCompleted, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.WASActivation,
            Keywords = Keywords.ActivationServices,
            Message = "Was closing all listener channel instances completed.")]
        public void WasCloseAllListenerChannelInstancesCompleted(string AppDomain)
        {
            WriteEvent(EventIds.WasCloseAllListenerChannelInstancesCompleted, AppDomain);
        }

        [NonEvent]
        public void WasCloseAllListenerChannelInstancesCompleted()
        {
            WasCloseAllListenerChannelInstancesCompleted("");
        }

        public bool WasCloseAllListenerChannelInstancesFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.WasCloseAllListenerChannelInstancesFailed, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.WASActivation,
            Keywords = Keywords.ActivationServices,
            Message = "Error Code:{0}")]
        public void WasCloseAllListenerChannelInstancesFailed(string hresult, string AppDomain)
        {
            WriteEvent(EventIds.WasCloseAllListenerChannelInstancesFailed, hresult, AppDomain);
        }

        [NonEvent]
        public void WasCloseAllListenerChannelInstancesFailed(string hresult)
        {
            WasCloseAllListenerChannelInstancesFailed(hresult, "");
        }

        public bool OpenListenerChannelInstanceFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.OpenListenerChannelInstanceFailed, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.WASActivation,
            Keywords = Keywords.ActivationServices,
            Message = "Error Code:{0}")]
        public void OpenListenerChannelInstanceFailed(string hresult, string AppDomain)
        {
            WriteEvent(EventIds.OpenListenerChannelInstanceFailed, hresult, AppDomain);
        }

        [NonEvent]
        public void OpenListenerChannelInstanceFailed(string hresult)
        {
            OpenListenerChannelInstanceFailed(hresult, "");
        }

        public bool WasConnectedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.WasConnected, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.WASActivationConnected, Task = Tasks.WASActivation,
            Keywords = Keywords.ActivationServices,
            Message = "WAS Connected.")]
        public void WasConnected(string AppDomain)
        {
            WriteEvent(EventIds.WasConnected, AppDomain);
        }

        [NonEvent]
        public void WasConnected()
        {
            WasConnected("");
        }

        public bool WasDisconnectedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.WasDisconnected, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = Opcodes.WASActivationDisconnect, Task = Tasks.WASActivation,
            Keywords = Keywords.ActivationServices,
            Message = "WAS Disconnected.")]
        public void WasDisconnected(string AppDomain)
        {
            WriteEvent(EventIds.WasDisconnected, AppDomain);
        }

        [NonEvent]
        public void WasDisconnected()
        {
            WasDisconnected("");
        }

        public bool PipeTransportListenerListeningStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.PipeTransportListenerListeningStart, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Start, Task = Tasks.ActivationPipeListenerListening,
            Keywords = Keywords.ActivationServices,
            Message = "Pipe transport listener listening start on uri:{0}.")]
        public void PipeTransportListenerListeningStart(string Uri, string AppDomain)
        {
            WriteEvent(EventIds.PipeTransportListenerListeningStart, Uri, AppDomain);
        }

        [NonEvent]
        public void PipeTransportListenerListeningStart(string Uri)
        {
            PipeTransportListenerListeningStart(Uri, "");
        }

        public bool PipeTransportListenerListeningStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.PipeTransportListenerListeningStop, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.ActivationPipeListenerListening,
            Keywords = Keywords.ActivationServices,
            Message = "Pipe transport listener listening stop.")]
        public void PipeTransportListenerListeningStop(string AppDomain)
        {
            WriteEvent(EventIds.PipeTransportListenerListeningStop, AppDomain);
        }

        [NonEvent]
        public void PipeTransportListenerListeningStop()
        {
            PipeTransportListenerListeningStop("");
        }

        public bool DispatchSessionSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.DispatchSessionSuccess, Level = EventLevel.Informational, Channel = EventChannel.Analytic, Opcode = EventOpcode.Stop, Task = Tasks.ActivationDispatchSession,
            Keywords = Keywords.ActivationServices,
            Message = "Session dispatch succeeded.")]
        public void DispatchSessionSuccess(string AppDomain)
        {
            WriteEvent(EventIds.DispatchSessionSuccess, AppDomain);
        }

        [NonEvent]
        public void DispatchSessionSuccess()
        {
            DispatchSessionSuccess("");
        }

        public bool DispatchSessionFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.DispatchSessionFailed, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.ActivationDispatchSession,
            Keywords = Keywords.ActivationServices,
            Message = "Session dispatch failed.")]
        public void DispatchSessionFailed(string AppDomain)
        {
            WriteEvent(EventIds.DispatchSessionFailed, AppDomain);
        }

        [NonEvent]
        public void DispatchSessionFailed()
        {
            DispatchSessionFailed("");
        }

        public bool WasConnectionTimedoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Critical, Keywords.ActivationServices, EventChannel.Analytic);
        }

        [Event(EventIds.WasConnectionTimedout, Level = EventLevel.Critical, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.WASActivation,
            Keywords = Keywords.ActivationServices,
            Message = "WAS connection timed out.")]
        public void WasConnectionTimedout(string AppDomain)
        {
            WriteEvent(EventIds.WasConnectionTimedout, AppDomain);
        }

        [NonEvent]
        public void WasConnectionTimedout()
        {
            WasConnectionTimedout("");
        }

        public bool RoutingTableLookupStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingTableLookupStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ActivationRoutingTableLookup,
            Keywords = Keywords.ActivationServices,
            Message = "Routing table lookup started.")]
        public void RoutingTableLookupStart(string AppDomain)
        {
            WriteEvent(EventIds.RoutingTableLookupStart, AppDomain);
        }

        [NonEvent]
        public void RoutingTableLookupStart()
        {
            RoutingTableLookupStart("");
        }

        public bool RoutingTableLookupStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.ActivationServices, EventChannel.Debug);
        }

        [Event(EventIds.RoutingTableLookupStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ActivationRoutingTableLookup,
            Keywords = Keywords.ActivationServices,
            Message = "Routing table lookup completed.")]
        public void RoutingTableLookupStop(string AppDomain)
        {
            WriteEvent(EventIds.RoutingTableLookupStop, AppDomain);
        }

        [NonEvent]
        public void RoutingTableLookupStop()
        {
            RoutingTableLookupStop("");
        }

        public bool PendingSessionQueueRatioIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Quota, EventChannel.Debug);
        }

        [Event(EventIds.PendingSessionQueueRatio, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.Quotas,
            Keywords = Keywords.Quota,
            Message = "Pending session queue ratio: {0}/{1}")]
        public void PendingSessionQueueRatio(int curr, int max, string AppDomain)
        {
            WriteEvent(EventIds.PendingSessionQueueRatio, curr, max, AppDomain);
        }

        [NonEvent]
        public void PendingSessionQueueRatio(int curr, int max)
        {
            PendingSessionQueueRatio(curr, max, "");
        }

        public bool EndSqlCommandExecuteIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.EndSqlCommandExecute, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.SqlCommandExecute,
            Keywords = Keywords.WFInstanceStore,
            Message = "End SQL command execution: {0}")]
        public void EndSqlCommandExecute(string data1, string AppDomain)
        {
            WriteEvent(EventIds.EndSqlCommandExecute, data1, AppDomain);
        }

        [NonEvent]
        public void EndSqlCommandExecute(string data1)
        {
            EndSqlCommandExecute(data1, "");
        }

        public bool StartSqlCommandExecuteIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.StartSqlCommandExecute, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.SqlCommandExecute,
            Keywords = Keywords.WFInstanceStore,
            Message = "Starting SQL command execution: {0}")]
        public void StartSqlCommandExecute(string data1, string AppDomain)
        {
            WriteEvent(EventIds.StartSqlCommandExecute, data1, AppDomain);
        }

        [NonEvent]
        public void StartSqlCommandExecute(string data1)
        {
            StartSqlCommandExecute(data1, "");
        }

        public bool RenewLockSystemErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.RenewLockSystemError, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.LockWorkflowInstance,
            Keywords = Keywords.WFInstanceStore,
            Message = "Failed to extend lock expiration, lock expiration already passed or the lock owner was deleted. Aborting SqlWorkflowInstanceStore.")]
        public void RenewLockSystemError(string AppDomain)
        {
            WriteEvent(EventIds.RenewLockSystemError, AppDomain);
        }

        [NonEvent]
        public void RenewLockSystemError()
        {
            RenewLockSystemError("");
        }

        public bool FoundProcessingErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.FoundProcessingError, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SqlCommandExecute,
            Keywords = Keywords.WFInstanceStore,
            Message = "Command failed: {0}")]
        public void FoundProcessingError(string data1, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.FoundProcessingError, data1, SerializedException, AppDomain);
        }

        [NonEvent]
        public void FoundProcessingError(string data1, string SerializedException)
        {
            FoundProcessingError(data1, SerializedException, "");
        }

        public bool UnlockInstanceExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.UnlockInstanceException, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.LockWorkflowInstance,
            Keywords = Keywords.WFInstanceStore,
            Message = "Encountered exception {0} while attempting to unlock instance.")]
        public void UnlockInstanceException(string data1, string AppDomain)
        {
            WriteEvent(EventIds.UnlockInstanceException, data1, AppDomain);
        }

        [NonEvent]
        public void UnlockInstanceException(string data1)
        {
            UnlockInstanceException(data1, "");
        }

        public bool MaximumRetriesExceededForSqlCommandIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Quota | Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.MaximumRetriesExceededForSqlCommand, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SqlCommandExecute,
            Keywords = Keywords.Quota | Keywords.WFInstanceStore,
            Message = "Giving up retrying a SQL command as the maximum number of retries have been performed.")]
        public void MaximumRetriesExceededForSqlCommand(string AppDomain)
        {
            WriteEvent(EventIds.MaximumRetriesExceededForSqlCommand, AppDomain);
        }

        [NonEvent]
        public void MaximumRetriesExceededForSqlCommand()
        {
            MaximumRetriesExceededForSqlCommand("");
        }

        public bool RetryingSqlCommandDueToSqlErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.RetryingSqlCommandDueToSqlError, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SqlCommandExecute,
            Keywords = Keywords.WFInstanceStore,
            Message = "Retrying a SQL command due to SQL error number {0}.")]
        public void RetryingSqlCommandDueToSqlError(string data1, string AppDomain)
        {
            WriteEvent(EventIds.RetryingSqlCommandDueToSqlError, data1, AppDomain);
        }

        [NonEvent]
        public void RetryingSqlCommandDueToSqlError(string data1)
        {
            RetryingSqlCommandDueToSqlError(data1, "");
        }

        public bool TimeoutOpeningSqlConnectionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.TimeoutOpeningSqlConnection, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SqlCommandExecute,
            Keywords = Keywords.WFInstanceStore,
            Message = "Timeout trying to open a SQL connection. The operation did not complete within the allotted timeout of {0}. The time allotted to this operation may have been a portion of a longer timeout.")]
        public void TimeoutOpeningSqlConnection(string data1, string AppDomain)
        {
            WriteEvent(EventIds.TimeoutOpeningSqlConnection, data1, AppDomain);
        }

        [NonEvent]
        public void TimeoutOpeningSqlConnection(string data1)
        {
            TimeoutOpeningSqlConnection(data1, "");
        }

        public bool SqlExceptionCaughtIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.SqlExceptionCaught, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SqlCommandExecute,
            Keywords = Keywords.WFInstanceStore,
            Message = "Caught SQL Exception number {0} message {1}.")]
        public void SqlExceptionCaught(string data1, string data2, string AppDomain)
        {
            WriteEvent(EventIds.SqlExceptionCaught, data1, data2, AppDomain);
        }

        [NonEvent]
        public void SqlExceptionCaught(string data1, string data2)
        {
            SqlExceptionCaught(data1, data2, "");
        }

        public bool QueuingSqlRetryIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.QueuingSqlRetry, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.LockWorkflowInstance,
            Keywords = Keywords.WFInstanceStore,
            Message = "Queuing SQL retry with delay {0} milliseconds.")]
        public void QueuingSqlRetry(string data1, string AppDomain)
        {
            WriteEvent(EventIds.QueuingSqlRetry, data1, AppDomain);
        }

        [NonEvent]
        public void QueuingSqlRetry(string data1)
        {
            QueuingSqlRetry(data1, "");
        }

        public bool LockRetryTimeoutIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.LockRetryTimeout, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SqlCommandExecute,
            Keywords = Keywords.WFInstanceStore,
            Message = "Timeout trying to acquire the instance lock.  The operation did not complete within the allotted timeout of {0}. The time allotted to this operation may have been a portion of a longer timeout.")]
        public void LockRetryTimeout(string data1, string AppDomain)
        {
            WriteEvent(EventIds.LockRetryTimeout, data1, AppDomain);
        }

        [NonEvent]
        public void LockRetryTimeout(string data1)
        {
            LockRetryTimeout(data1, "");
        }

        public bool RunnableInstancesDetectionErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.RunnableInstancesDetectionError, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.LockWorkflowInstance,
            Keywords = Keywords.WFInstanceStore,
            Message = "Detection of runnable instances failed due to the following exception")]
        public void RunnableInstancesDetectionError(string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.RunnableInstancesDetectionError, SerializedException, AppDomain);
        }

        [NonEvent]
        public void RunnableInstancesDetectionError(string SerializedException)
        {
            RunnableInstancesDetectionError(SerializedException, "");
        }

        public bool InstanceLocksRecoveryErrorIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.WFInstanceStore, EventChannel.Debug);
        }

        [Event(EventIds.InstanceLocksRecoveryError, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.LockWorkflowInstance,
            Keywords = Keywords.WFInstanceStore,
            Message = "Recovering instance locks failed due to the following exception")]
        public void InstanceLocksRecoveryError(string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.InstanceLocksRecoveryError, SerializedException, AppDomain);
        }

        [NonEvent]
        public void InstanceLocksRecoveryError(string SerializedException)
        {
            InstanceLocksRecoveryError(SerializedException, "");
        }

        public bool MessageLogEventSizeExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WCFMessageLogging, EventChannel.Debug);
        }

        [Event(EventIds.MessageLogEventSizeExceeded, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.WCFMessageLogging,
            Message = "Message could not be logged as it exceeds the ETW event size")]
        public void MessageLogEventSizeExceeded(string AppDomain)
        {
            WriteEvent(EventIds.MessageLogEventSizeExceeded, AppDomain);
        }

        [NonEvent]
        public void MessageLogEventSizeExceeded()
        {
            MessageLogEventSizeExceeded("");
        }

        public bool DiscoveryClientInClientChannelFailedToCloseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryClientInClientChannelFailedToClose, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.DiscoveryClientFailedToClose, Task = Tasks.DiscoveryClient,
            Keywords = Keywords.Discovery,
            Message = "The DiscoveryClient created inside DiscoveryClientChannel failed to close and hence has been aborted.")]
        public void DiscoveryClientInClientChannelFailedToClose(string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.DiscoveryClientInClientChannelFailedToClose, SerializedException, AppDomain);
        }

        [NonEvent]
        public void DiscoveryClientInClientChannelFailedToClose(string SerializedException)
        {
            DiscoveryClientInClientChannelFailedToClose(SerializedException, "");
        }

        public bool DiscoveryClientProtocolExceptionSuppressedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryClientProtocolExceptionSuppressed, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.DiscoveryClientExceptionSuppressed, Task = Tasks.DiscoveryClient,
            Keywords = Keywords.Discovery,
            Message = "A ProtocolException was suppressed while closing the DiscoveryClient. This could be because a DiscoveryService is still trying to send response to the DiscoveryClient.")]
        public void DiscoveryClientProtocolExceptionSuppressed(string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.DiscoveryClientProtocolExceptionSuppressed, SerializedException, AppDomain);
        }

        [NonEvent]
        public void DiscoveryClientProtocolExceptionSuppressed(string SerializedException)
        {
            DiscoveryClientProtocolExceptionSuppressed(SerializedException, "");
        }

        public bool DiscoveryClientReceivedMulticastSuppressionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryClientReceivedMulticastSuppression, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.DiscoveryClientReceivedMulticastSuppression, Task = Tasks.DiscoveryClient,
            Keywords = Keywords.Discovery,
            Message = "The DiscoveryClient received a multicast suppression message from a DiscoveryProxy.")]
        public void DiscoveryClientReceivedMulticastSuppression(string AppDomain)
        {
            WriteEvent(EventIds.DiscoveryClientReceivedMulticastSuppression, AppDomain);
        }

        [NonEvent]
        public void DiscoveryClientReceivedMulticastSuppression()
        {
            DiscoveryClientReceivedMulticastSuppression("");
        }

        public bool DiscoveryMessageReceivedAfterOperationCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryMessageReceivedAfterOperationCompleted, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.DiscoveryMessageReceivedAfterOperationCompleted, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message with messageId='{1}' was dropped by the DiscoveryClient because the corresponding {2} operation was completed.")]
        public void DiscoveryMessageReceivedAfterOperationCompleted(string discoveryMessageName, string messageId, string discoveryOperationName, string AppDomain)
        {
            WriteEvent(EventIds.DiscoveryMessageReceivedAfterOperationCompleted, discoveryMessageName, messageId, discoveryOperationName, AppDomain);
        }

        [NonEvent]
        public void DiscoveryMessageReceivedAfterOperationCompleted(string discoveryMessageName, string messageId, string discoveryOperationName)
        {
            DiscoveryMessageReceivedAfterOperationCompleted(discoveryMessageName, messageId, discoveryOperationName, "");
        }

        public bool DiscoveryMessageWithInvalidContentIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryMessageWithInvalidContent, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.DiscoveryMessageInvalidContent, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message with messageId='{1}' was dropped because it had invalid content.")]
        public void DiscoveryMessageWithInvalidContent(string messageType, string messageId, string AppDomain)
        {
            WriteEvent(EventIds.DiscoveryMessageWithInvalidContent, messageType, messageId, AppDomain);
        }

        [NonEvent]
        public void DiscoveryMessageWithInvalidContent(string messageType, string messageId)
        {
            DiscoveryMessageWithInvalidContent(messageType, messageId, "");
        }

        public bool DiscoveryMessageWithInvalidRelatesToOrOperationCompletedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryMessageWithInvalidRelatesToOrOperationCompleted, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.DiscoveryMessageInvalidRelatesToOrOperationCompleted, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message with messageId='{1}' and relatesTo='{2}' was dropped by the DiscoveryClient because either the corresponding {3} operation was completed or the relatesTo value is invalid.")]
        public void DiscoveryMessageWithInvalidRelatesToOrOperationCompleted(string discoveryMessageName, string messageId, string relatesTo, string discoveryOperationName, string AppDomain)
        {
            WriteEvent(EventIds.DiscoveryMessageWithInvalidRelatesToOrOperationCompleted, discoveryMessageName, messageId, relatesTo, discoveryOperationName, AppDomain);
        }

        [NonEvent]
        public void DiscoveryMessageWithInvalidRelatesToOrOperationCompleted(string discoveryMessageName, string messageId, string relatesTo, string discoveryOperationName)
        {
            DiscoveryMessageWithInvalidRelatesToOrOperationCompleted(discoveryMessageName, messageId, relatesTo, discoveryOperationName, "");
        }

        public bool DiscoveryMessageWithInvalidReplyToIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryMessageWithInvalidReplyTo, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.DiscoveryMessageInvalidReplyTo, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A discovery request message with messageId='{0}' was dropped because it had an invalid ReplyTo address.")]
        public void DiscoveryMessageWithInvalidReplyTo(string messageId, string AppDomain)
        {
            WriteEvent(EventIds.DiscoveryMessageWithInvalidReplyTo, messageId, AppDomain);
        }

        [NonEvent]
        public void DiscoveryMessageWithInvalidReplyTo(string messageId)
        {
            DiscoveryMessageWithInvalidReplyTo(messageId, "");
        }

        public bool DiscoveryMessageWithNoContentIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryMessageWithNoContent, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.DiscoveryMessageNoContent, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message was dropped because it did not have any content.")]
        public void DiscoveryMessageWithNoContent(string messageType, string AppDomain)
        {
            WriteEvent(EventIds.DiscoveryMessageWithNoContent, messageType, AppDomain);
        }

        [NonEvent]
        public void DiscoveryMessageWithNoContent(string messageType)
        {
            DiscoveryMessageWithNoContent(messageType, "");
        }

        public bool DiscoveryMessageWithNullMessageIdIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryMessageWithNullMessageId, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.DiscoveryMessageNullMessageId, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message was dropped because the message header did not contain the required MessageId property.")]
        public void DiscoveryMessageWithNullMessageId(string messageType, string AppDomain)
        {
            WriteEvent(EventIds.DiscoveryMessageWithNullMessageId, messageType, AppDomain);
        }

        [NonEvent]
        public void DiscoveryMessageWithNullMessageId(string messageType)
        {
            DiscoveryMessageWithNullMessageId(messageType, "");
        }

        public bool DiscoveryMessageWithNullMessageSequenceIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryMessageWithNullMessageSequence, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.DiscoveryMessageNullMessageSequence, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message with messageId='{1}' was dropped by the DiscoveryClient because it did not have the DiscoveryMessageSequence property.")]
        public void DiscoveryMessageWithNullMessageSequence(string discoveryMessageName, string messageId, string AppDomain)
        {
            WriteEvent(EventIds.DiscoveryMessageWithNullMessageSequence, discoveryMessageName, messageId, AppDomain);
        }

        [NonEvent]
        public void DiscoveryMessageWithNullMessageSequence(string discoveryMessageName, string messageId)
        {
            DiscoveryMessageWithNullMessageSequence(discoveryMessageName, messageId, "");
        }

        public bool DiscoveryMessageWithNullRelatesToIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryMessageWithNullRelatesTo, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.DiscoveryMessageNullRelatesTo, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message with messageId='{1}' was dropped by the DiscoveryClient because the message header did not contain the required RelatesTo property.")]
        public void DiscoveryMessageWithNullRelatesTo(string discoveryMessageName, string messageId, string AppDomain)
        {
            WriteEvent(EventIds.DiscoveryMessageWithNullRelatesTo, discoveryMessageName, messageId, AppDomain);
        }

        [NonEvent]
        public void DiscoveryMessageWithNullRelatesTo(string discoveryMessageName, string messageId)
        {
            DiscoveryMessageWithNullRelatesTo(discoveryMessageName, messageId, "");
        }

        public bool DiscoveryMessageWithNullReplyToIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DiscoveryMessageWithNullReplyTo, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.DiscoveryMessageNullReplyTo, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A discovery request message with messageId='{0}' was dropped because it did not have a ReplyTo address.")]
        public void DiscoveryMessageWithNullReplyTo(string messageId, string AppDomain)
        {
            WriteEvent(EventIds.DiscoveryMessageWithNullReplyTo, messageId, AppDomain);
        }

        [NonEvent]
        public void DiscoveryMessageWithNullReplyTo(string messageId)
        {
            DiscoveryMessageWithNullReplyTo(messageId, "");
        }

        public bool DuplicateDiscoveryMessageIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.DuplicateDiscoveryMessage, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.DiscoveryMessageDuplicate, Task = Tasks.DiscoveryMessage,
            Keywords = Keywords.Discovery,
            Message = "A {0} message with messageId='{1}' was dropped because it was a duplicate.")]
        public void DuplicateDiscoveryMessage(string messageType, string messageId, string AppDomain)
        {
            WriteEvent(EventIds.DuplicateDiscoveryMessage, messageType, messageId, AppDomain);
        }

        [NonEvent]
        public void DuplicateDiscoveryMessage(string messageType, string messageId)
        {
            DuplicateDiscoveryMessage(messageType, messageId, "");
        }

        public bool EndpointDiscoverabilityDisabledIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.EndpointDiscoverabilityDisabled, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.EndpointDiscoverabilityDisabled, Task = Tasks.EndpointDiscoverability,
            Keywords = Keywords.Discovery,
            Message = "The discoverability of endpoint with EndpointAddress='{0}' and ListenUri='{1}' has been disabled.")]
        public void EndpointDiscoverabilityDisabled(string endpointAddress, string listenUri, string AppDomain)
        {
            WriteEvent(EventIds.EndpointDiscoverabilityDisabled, endpointAddress, listenUri, AppDomain);
        }

        [NonEvent]
        public void EndpointDiscoverabilityDisabled(string endpointAddress, string listenUri)
        {
            EndpointDiscoverabilityDisabled(endpointAddress, listenUri, "");
        }

        public bool EndpointDiscoverabilityEnabledIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.EndpointDiscoverabilityEnabled, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.EndpointDiscoverabilityEnabled, Task = Tasks.EndpointDiscoverability,
            Keywords = Keywords.Discovery,
            Message = "The discoverability of endpoint with EndpointAddress='{0}' and ListenUri='{1}' has been enabled.")]
        public void EndpointDiscoverabilityEnabled(string endpointAddress, string listenUri, string AppDomain)
        {
            WriteEvent(EventIds.EndpointDiscoverabilityEnabled, endpointAddress, listenUri, AppDomain);
        }

        [NonEvent]
        public void EndpointDiscoverabilityEnabled(string endpointAddress, string listenUri)
        {
            EndpointDiscoverabilityEnabled(endpointAddress, listenUri, "");
        }

        public bool FindInitiatedInDiscoveryClientChannelIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.FindInitiatedInDiscoveryClientChannel, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.DiscoveryClientChannelFindInitiated, Task = Tasks.DiscoveryClientChannel,
            Keywords = Keywords.Discovery,
            Message = "A Find operation was initiated in the DiscoveryClientChannel to discover endpoint(s).")]
        public void FindInitiatedInDiscoveryClientChannel(string AppDomain)
        {
            WriteEvent(EventIds.FindInitiatedInDiscoveryClientChannel, AppDomain);
        }

        [NonEvent]
        public void FindInitiatedInDiscoveryClientChannel()
        {
            FindInitiatedInDiscoveryClientChannel("");
        }

        public bool InnerChannelCreationFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.InnerChannelCreationFailed, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.DiscoveryClientChannelCreationFailed, Task = Tasks.DiscoveryClientChannel,
            Keywords = Keywords.Discovery,
            Message = "The DiscoveryClientChannel failed to create the channel with a discovered endpoint with EndpointAddress='{0}' and Via='{1}'. The DiscoveryClientChannel will now attempt to use the next available discovered endpoint.")]
        public void InnerChannelCreationFailed(string endpointAddress, string via, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.InnerChannelCreationFailed, endpointAddress, via, SerializedException, AppDomain);
        }

        [NonEvent]
        public void InnerChannelCreationFailed(string endpointAddress, string via, string SerializedException)
        {
            InnerChannelCreationFailed(endpointAddress, via, SerializedException, "");
        }

        public bool InnerChannelOpenFailedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.InnerChannelOpenFailed, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.DiscoveryClientChannelOpenFailed, Task = Tasks.DiscoveryClientChannel,
            Keywords = Keywords.Discovery,
            Message = "The DiscoveryClientChannel failed to open the channel with a discovered endpoint with EndpointAddress='{0}' and Via='{1}'. The DiscoveryClientChannel will now attempt to use the next available discovered endpoint.")]
        public void InnerChannelOpenFailed(string endpointAddress, string via, string SerializedException, string AppDomain)
        {
            WriteEvent(EventIds.InnerChannelOpenFailed, endpointAddress, via, SerializedException, AppDomain);
        }

        [NonEvent]
        public void InnerChannelOpenFailed(string endpointAddress, string via, string SerializedException)
        {
            InnerChannelOpenFailed(endpointAddress, via, SerializedException, "");
        }

        public bool InnerChannelOpenSucceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.InnerChannelOpenSucceeded, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.DiscoveryClientChannelOpenSucceeded, Task = Tasks.DiscoveryClientChannel,
            Keywords = Keywords.Discovery,
            Message = "The DiscoveryClientChannel successfully discovered an endpoint and opened the channel using it. The client is connected to a service using EndpointAddress='{0}' and Via='{1}'.")]
        public void InnerChannelOpenSucceeded(string endpointAddress, string via, string AppDomain)
        {
            WriteEvent(EventIds.InnerChannelOpenSucceeded, endpointAddress, via, AppDomain);
        }

        [NonEvent]
        public void InnerChannelOpenSucceeded(string endpointAddress, string via)
        {
            InnerChannelOpenSucceeded(endpointAddress, via, "");
        }

        public bool SynchronizationContextResetIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.SynchronizationContextReset, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.DiscoverySynchronizationContextReset, Task = Tasks.DiscoverySynchronizationContext,
            Keywords = Keywords.Discovery,
            Message = "The SynchronizationContext has been reset to its original value of {0} by DiscoveryClientChannel.")]
        public void SynchronizationContextReset(string synchronizationContextType, string AppDomain)
        {
            WriteEvent(EventIds.SynchronizationContextReset, synchronizationContextType, AppDomain);
        }

        [NonEvent]
        public void SynchronizationContextReset(string synchronizationContextType)
        {
            SynchronizationContextReset(synchronizationContextType, "");
        }

        public bool SynchronizationContextSetToNullIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Discovery, EventChannel.Debug);
        }

        [Event(EventIds.SynchronizationContextSetToNull, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.DiscoverySynchronizationContextSetToNull, Task = Tasks.DiscoverySynchronizationContext,
            Keywords = Keywords.Discovery,
            Message = "The SynchronizationContext has been set to null by DiscoveryClientChannel before initiating the Find operation.")]
        public void SynchronizationContextSetToNull(string AppDomain)
        {
            WriteEvent(EventIds.SynchronizationContextSetToNull, AppDomain);
        }

        [NonEvent]
        public void SynchronizationContextSetToNull()
        {
            SynchronizationContextSetToNull("");
        }

        public bool DCSerializeWithSurrogateStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCSerializeWithSurrogateStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.SurrogateSerialize,
            Keywords = Keywords.Serialization,
            Message = "DataContract serialize {0} with surrogates start.")]
        public void DCSerializeWithSurrogateStart(string SurrogateType, string AppDomain)
        {
            WriteEvent(EventIds.DCSerializeWithSurrogateStart, SurrogateType, AppDomain);
        }

        [NonEvent]
        public void DCSerializeWithSurrogateStart(string SurrogateType)
        {
            DCSerializeWithSurrogateStart(SurrogateType, "");
        }

        public bool DCSerializeWithSurrogateStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCSerializeWithSurrogateStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.SurrogateSerialize,
            Keywords = Keywords.Serialization,
            Message = "DataContract serialize with surrogates stop.")]
        public void DCSerializeWithSurrogateStop(string AppDomain)
        {
            WriteEvent(EventIds.DCSerializeWithSurrogateStop, AppDomain);
        }

        [NonEvent]
        public void DCSerializeWithSurrogateStop()
        {
            DCSerializeWithSurrogateStop("");
        }

        public bool DCDeserializeWithSurrogateStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCDeserializeWithSurrogateStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.SurrogateDeserialize,
            Keywords = Keywords.Serialization,
            Message = "DataContract deserialize {0} with surrogates start.")]
        public void DCDeserializeWithSurrogateStart(string SurrogateType, string AppDomain)
        {
            WriteEvent(EventIds.DCDeserializeWithSurrogateStart, SurrogateType, AppDomain);
        }

        [NonEvent]
        public void DCDeserializeWithSurrogateStart(string SurrogateType)
        {
            DCDeserializeWithSurrogateStart(SurrogateType, "");
        }

        public bool DCDeserializeWithSurrogateStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCDeserializeWithSurrogateStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.SurrogateDeserialize,
            Keywords = Keywords.Serialization,
            Message = "DataContract deserialize with surrogates stop.")]
        public void DCDeserializeWithSurrogateStop(string AppDomain)
        {
            WriteEvent(EventIds.DCDeserializeWithSurrogateStop, AppDomain);
        }

        [NonEvent]
        public void DCDeserializeWithSurrogateStop()
        {
            DCDeserializeWithSurrogateStop("");
        }

        public bool ImportKnownTypesStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.ImportKnownTypesStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.ImportKnownType,
            Keywords = Keywords.Serialization,
            Message = "ImportKnownTypes start.")]
        public void ImportKnownTypesStart(string AppDomain)
        {
            WriteEvent(EventIds.ImportKnownTypesStart, AppDomain);
        }

        [NonEvent]
        public void ImportKnownTypesStart()
        {
            ImportKnownTypesStart("");
        }

        public bool ImportKnownTypesStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.ImportKnownTypesStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.ImportKnownType,
            Keywords = Keywords.Serialization,
            Message = "ImportKnownTypes stop.")]
        public void ImportKnownTypesStop(string AppDomain)
        {
            WriteEvent(EventIds.ImportKnownTypesStop, AppDomain);
        }

        [NonEvent]
        public void ImportKnownTypesStop()
        {
            ImportKnownTypesStop("");
        }

        public bool DCResolverResolveIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCResolverResolve, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.DataContractResolver,
            Keywords = Keywords.Serialization,
            Message = "DataContract resolver resolving {0} start.")]
        public void DCResolverResolve(string TypeName, string AppDomain)
        {
            WriteEvent(EventIds.DCResolverResolve, TypeName, AppDomain);
        }

        [NonEvent]
        public void DCResolverResolve(string TypeName)
        {
            DCResolverResolve(TypeName, "");
        }

        public bool DCGenWriterStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCGenWriterStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.GenerateSerializer,
            Keywords = Keywords.Serialization,
            Message = "DataContract generate {0} writer for {1} start.")]
        public void DCGenWriterStart(string Kind, string TypeName, string AppDomain)
        {
            WriteEvent(EventIds.DCGenWriterStart, Kind, TypeName, AppDomain);
        }

        [NonEvent]
        public void DCGenWriterStart(string Kind, string TypeName)
        {
            DCGenWriterStart(Kind, TypeName, "");
        }

        public bool DCGenWriterStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCGenWriterStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.GenerateSerializer,
            Keywords = Keywords.Serialization,
            Message = "DataContract generate writer stop.")]
        public void DCGenWriterStop(string AppDomain)
        {
            WriteEvent(EventIds.DCGenWriterStop, AppDomain);
        }

        [NonEvent]
        public void DCGenWriterStop()
        {
            DCGenWriterStop("");
        }

        public bool DCGenReaderStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCGenReaderStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.GenerateDeserializer,
            Keywords = Keywords.Serialization,
            Message = "DataContract generate {0} reader for {1} start.")]
        public void DCGenReaderStart(string Kind, string TypeName, string AppDomain)
        {
            WriteEvent(EventIds.DCGenReaderStart, Kind, TypeName, AppDomain);
        }

        [NonEvent]
        public void DCGenReaderStart(string Kind, string TypeName)
        {
            DCGenReaderStart(Kind, TypeName, "");
        }

        public bool DCGenReaderStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCGenReaderStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.GenerateDeserializer,
            Keywords = Keywords.Serialization,
            Message = "DataContract generation stop.")]
        public void DCGenReaderStop(string AppDomain)
        {
            WriteEvent(EventIds.DCGenReaderStop, AppDomain);
        }

        [NonEvent]
        public void DCGenReaderStop()
        {
            DCGenReaderStop("");
        }

        public bool DCJsonGenReaderStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCJsonGenReaderStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.GenerateDeserializer,
            Keywords = Keywords.Serialization,
            Message = "Json generate {0} reader for {1} start.")]
        public void DCJsonGenReaderStart(string Kind, string TypeName, string AppDomain)
        {
            WriteEvent(EventIds.DCJsonGenReaderStart, Kind, TypeName, AppDomain);
        }

        [NonEvent]
        public void DCJsonGenReaderStart(string Kind, string TypeName)
        {
            DCJsonGenReaderStart(Kind, TypeName, "");
        }

        public bool DCJsonGenReaderStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCJsonGenReaderStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.GenerateDeserializer,
            Keywords = Keywords.Serialization,
            Message = "Json reader generation stop.")]
        public void DCJsonGenReaderStop(string AppDomain)
        {
            WriteEvent(EventIds.DCJsonGenReaderStop, AppDomain);
        }

        [NonEvent]
        public void DCJsonGenReaderStop()
        {
            DCJsonGenReaderStop("");
        }

        public bool DCJsonGenWriterStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCJsonGenWriterStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.GenerateSerializer,
            Keywords = Keywords.Serialization,
            Message = "Json generate {0} writer for {1} start.")]
        public void DCJsonGenWriterStart(string Kind, string TypeName, string AppDomain)
        {
            WriteEvent(EventIds.DCJsonGenWriterStart, Kind, TypeName, AppDomain);
        }

        [NonEvent]
        public void DCJsonGenWriterStart(string Kind, string TypeName)
        {
            DCJsonGenWriterStart(Kind, TypeName, "");
        }

        public bool DCJsonGenWriterStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.DCJsonGenWriterStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.GenerateSerializer,
            Keywords = Keywords.Serialization,
            Message = "Json generate writer stop.")]
        public void DCJsonGenWriterStop(string AppDomain)
        {
            WriteEvent(EventIds.DCJsonGenWriterStop, AppDomain);
        }

        [NonEvent]
        public void DCJsonGenWriterStop()
        {
            DCJsonGenWriterStop("");
        }

        public bool GenXmlSerializableStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.GenXmlSerializableStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.GenerateXmlSerializable,
            Keywords = Keywords.Serialization,
            Message = "Generate Xml serializable for '{0}' start.")]
        public void GenXmlSerializableStart(string DCType, string AppDomain)
        {
            WriteEvent(EventIds.GenXmlSerializableStart, DCType, AppDomain);
        }

        [NonEvent]
        public void GenXmlSerializableStart(string DCType)
        {
            GenXmlSerializableStart(DCType, "");
        }

        public bool GenXmlSerializableStopIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Serialization, EventChannel.Debug);
        }

        [Event(EventIds.GenXmlSerializableStop, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Stop, Task = Tasks.GenerateXmlSerializable,
            Keywords = Keywords.Serialization,
            Message = "Generate Xml serializable stop.")]
        public void GenXmlSerializableStop(string AppDomain)
        {
            WriteEvent(EventIds.GenXmlSerializableStop, AppDomain);
        }

        [NonEvent]
        public void GenXmlSerializableStop()
        {
            GenXmlSerializableStop("");
        }

        public bool JsonMessageDecodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.JsonMessageDecodingStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.MessageDecoding,
            Keywords = Keywords.Channel,
            Message = "JsonMessageEncoder started decoding the message.")]
        public void JsonMessageDecodingStart(string AppDomain)
        {
            WriteEvent(EventIds.JsonMessageDecodingStart, AppDomain);
        }

        [NonEvent]
        public void JsonMessageDecodingStart()
        {
            JsonMessageDecodingStart("");
        }

        public bool JsonMessageEncodingStartIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Channel, EventChannel.Debug);
        }

        [Event(EventIds.JsonMessageEncodingStart, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.MessageEncoding,
            Keywords = Keywords.Channel,
            Message = "JsonMessageEncoder started encoding the message.")]
        public void JsonMessageEncodingStart(string AppDomain)
        {
            WriteEvent(EventIds.JsonMessageEncodingStart, AppDomain);
        }

        [NonEvent]
        public void JsonMessageEncodingStart()
        {
            JsonMessageEncodingStart("");
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

        public bool GetIssuerNameSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.GetIssuerNameSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.GetIssuerName,
            Keywords = Keywords.Security,
            Message = "Retrieval of issuer name:{0} from tokenId:{1} succeeded.")]
        public void GetIssuerNameSuccess(string issuerName, string tokenID, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.GetIssuerNameSuccess, issuerName, tokenID, HostReference, AppDomain);
        }

        [NonEvent]
        public void GetIssuerNameSuccess(string issuerName, string tokenID)
        {
            GetIssuerNameSuccess(issuerName, tokenID, "", "");
        }

        public bool GetIssuerNameFailureIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.GetIssuerNameFailure, Level = EventLevel.Error, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.GetIssuerName,
            Keywords = Keywords.Security,
            Message = "Retrieval of issuer name from tokenId:{0} failed.")]
        public void GetIssuerNameFailure(string tokenID, string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.GetIssuerNameFailure, tokenID, HostReference, AppDomain);
        }

        [NonEvent]
        public void GetIssuerNameFailure(string tokenID)
        {
            GetIssuerNameFailure(tokenID, "", "");
        }

        public bool FederationMessageProcessingStartedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.FederationMessageProcessingStarted, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.FederationMessageProcessing,
            Keywords = Keywords.Security,
            Message = "Federation message processing started.")]
        public void FederationMessageProcessingStarted(string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.FederationMessageProcessingStarted, HostReference, AppDomain);
        }

        [NonEvent]
        public void FederationMessageProcessingStarted()
        {
            FederationMessageProcessingStarted("", "");
        }

        public bool FederationMessageProcessingSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.FederationMessageProcessingSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.FederationMessageProcessing,
            Keywords = Keywords.Security,
            Message = "Federation message processing succeeded.")]
        public void FederationMessageProcessingSuccess(string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.FederationMessageProcessingSuccess, HostReference, AppDomain);
        }

        [NonEvent]
        public void FederationMessageProcessingSuccess()
        {
            FederationMessageProcessingSuccess("", "");
        }

        public bool FederationMessageCreationStartedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.FederationMessageCreationStarted, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.FederationMessageCreation,
            Keywords = Keywords.Security,
            Message = "Creating federation message from form post started.")]
        public void FederationMessageCreationStarted(string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.FederationMessageCreationStarted, HostReference, AppDomain);
        }

        [NonEvent]
        public void FederationMessageCreationStarted()
        {
            FederationMessageCreationStarted("", "");
        }

        public bool FederationMessageCreationSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.FederationMessageCreationSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.FederationMessageCreation,
            Keywords = Keywords.Security,
            Message = "Creating federation message from form post succeeded.")]
        public void FederationMessageCreationSuccess(string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.FederationMessageCreationSuccess, HostReference, AppDomain);
        }

        [NonEvent]
        public void FederationMessageCreationSuccess()
        {
            FederationMessageCreationSuccess("", "");
        }

        public bool SessionCookieReadingStartedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.SessionCookieReadingStarted, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SessionCookieReading,
            Keywords = Keywords.Security,
            Message = "Reading session token from session cookie started.")]
        public void SessionCookieReadingStarted(string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.SessionCookieReadingStarted, HostReference, AppDomain);
        }

        [NonEvent]
        public void SessionCookieReadingStarted()
        {
            SessionCookieReadingStarted("", "");
        }

        public bool SessionCookieReadingSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.SessionCookieReadingSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.SessionCookieReading,
            Keywords = Keywords.Security,
            Message = "Reading session token from session cookie succeeded.")]
        public void SessionCookieReadingSuccess(string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.SessionCookieReadingSuccess, HostReference, AppDomain);
        }

        [NonEvent]
        public void SessionCookieReadingSuccess()
        {
            SessionCookieReadingSuccess("", "");
        }

        public bool PrincipalSettingFromSessionTokenStartedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.PrincipalSettingFromSessionTokenStarted, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.PrincipalSetting,
            Keywords = Keywords.Security,
            Message = "Principal setting from session token started.")]
        public void PrincipalSettingFromSessionTokenStarted(string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.PrincipalSettingFromSessionTokenStarted, HostReference, AppDomain);
        }

        [NonEvent]
        public void PrincipalSettingFromSessionTokenStarted()
        {
            PrincipalSettingFromSessionTokenStarted("", "");
        }

        public bool PrincipalSettingFromSessionTokenSuccessIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.PrincipalSettingFromSessionTokenSuccess, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.PrincipalSetting,
            Keywords = Keywords.Security,
            Message = "Principal setting from session token succeeded.")]
        public void PrincipalSettingFromSessionTokenSuccess(string HostReference, string AppDomain)
        {
            WriteEvent(EventIds.PrincipalSettingFromSessionTokenSuccess, HostReference, AppDomain);
        }

        [NonEvent]
        public void PrincipalSettingFromSessionTokenSuccess()
        {
            PrincipalSettingFromSessionTokenSuccess("", "");
        }

        public bool TrackingRecordDroppedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFTracking, EventChannel.Debug);
        }

        [Event(EventIds.TrackingRecordDropped, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.TrackingRecordDropped, Task = Tasks.TrackingRecord,
            Keywords = Keywords.WFTracking,
            Message = "Size of tracking record {0} exceeds maximum allowed by the ETW session for provider {1}")]
        public void TrackingRecordDropped(long RecordNumber, Guid ProviderId, string AppDomain)
        {
            WriteEvent(EventIds.TrackingRecordDropped, RecordNumber, ProviderId, AppDomain);
        }

        [NonEvent]
        public void TrackingRecordDropped(long RecordNumber, Guid ProviderId)
        {
            TrackingRecordDropped(RecordNumber, ProviderId, "");
        }

        public bool TrackingRecordRaisedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.TrackingRecordRaised, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = Opcodes.TrackingRecordRaised, Task = Tasks.TrackingRecord,
            Keywords = Keywords.WFRuntime,
            Message = "Tracking Record {0} raised to {1}.")]
        public void TrackingRecordRaised(string data1, string data2, string AppDomain)
        {
            WriteEvent(EventIds.TrackingRecordRaised, data1, data2, AppDomain);
        }

        [NonEvent]
        public void TrackingRecordRaised(string data1, string data2)
        {
            TrackingRecordRaised(data1, data2, "");
        }

        public bool TrackingRecordTruncatedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFTracking, EventChannel.Debug);
        }

        [Event(EventIds.TrackingRecordTruncated, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = Opcodes.TrackingRecordTruncated, Task = Tasks.TrackingRecord,
            Keywords = Keywords.WFTracking,
            Message = "Truncated tracking record {0} written to ETW session with provider {1}. Variables/annotations/user data have been removed")]
        public void TrackingRecordTruncated(long RecordNumber, Guid ProviderId, string AppDomain)
        {
            WriteEvent(EventIds.TrackingRecordTruncated, RecordNumber, ProviderId, AppDomain);
        }

        [NonEvent]
        public void TrackingRecordTruncated(long RecordNumber, Guid ProviderId)
        {
            TrackingRecordTruncated(RecordNumber, ProviderId, "");
        }

        public bool TrackingDataExtractedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.WFRuntime, EventChannel.Debug);
        }

        [Event(EventIds.TrackingDataExtracted, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.TrackingProfile,
            Keywords = Keywords.WFRuntime,
            Message = "Tracking data {0} extracted in activity {1}.")]
        public void TrackingDataExtracted(string Data, string Activity, string AppDomain)
        {
            WriteEvent(EventIds.TrackingDataExtracted, Data, Activity, AppDomain);
        }

        [NonEvent]
        public void TrackingDataExtracted(string Data, string Activity)
        {
            TrackingDataExtracted(Data, Activity, "");
        }

        public bool TrackingValueNotSerializableIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFTracking, EventChannel.Debug);
        }

        [Event(EventIds.TrackingValueNotSerializable, Level = EventLevel.Warning, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.TrackingProfile,
            Keywords = Keywords.WFTracking,
            Message = "The extracted argument/variable '{0}' is not serializable.")]
        public void TrackingValueNotSerializable(string name, string AppDomain)
        {
            WriteEvent(EventIds.TrackingValueNotSerializable, name, AppDomain);
        }

        [NonEvent]
        public void TrackingValueNotSerializable(string name)
        {
            TrackingValueNotSerializable(name, "");
        }

        public bool AppDomainUnloadIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.Infrastructure, EventChannel.Debug);
        }

        [Event(EventIds.AppDomainUnload, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info,
            Keywords = Keywords.Infrastructure,
            Message = "AppDomain unloading. AppDomain.FriendlyName {0}, ProcessName {1}, ProcessId {2}.")]
        public void AppDomainUnload(string appdomainName, string processName, string processId, string AppDomain)
        {
            WriteEvent(EventIds.AppDomainUnload, appdomainName, processName, processId, AppDomain);
        }

        [NonEvent]
        public void AppDomainUnload(string processName, string processId)
        {
            AppDomainUnload("", processName, processId, "");
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

        public bool MaxInstancesExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.WFServices, EventChannel.Analytic);
        }

        [Event(EventIds.MaxInstancesExceeded, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Throttles,
            Keywords = Keywords.WFServices,
            Message = "The system hit the limit set for throttle 'MaxConcurrentInstances'. Limit for this throttle was set to {0}. Throttle value can be changed by modifying attribute 'maxConcurrentInstances' in serviceThrottle element or by modifying 'MaxConcurrentInstances' property on behavior ServiceThrottlingBehavior.")]
        public void MaxInstancesExceeded(int limit, string AppDomain)
        {
            WriteEvent(EventIds.MaxInstancesExceeded, limit, AppDomain);
        }

        [NonEvent]
        public void MaxInstancesExceeded(int limit)
        {
            MaxInstancesExceeded(limit, "");
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

        public bool HttpHandlerPickedForUrlIsEnabled()
        {
            return base.IsEnabled(EventLevel.Informational, Keywords.WebHost, EventChannel.Debug);
        }

        [Event(EventIds.HttpHandlerPickedForUrl, Level = EventLevel.Informational, Channel = EventChannel.Debug, Opcode = EventOpcode.Info, Task = Tasks.CreateWorkflowServiceHost,
            Keywords = Keywords.WebHost,
            Message = "The url '{0}' hosts XAML document with root element type '{1}'. The HTTP handler type '{2}' is picked to serve all the requests made to this url.")]
        public void HttpHandlerPickedForUrl(string data1, string data2, string data3, string AppDomain)
        {
            WriteEvent(EventIds.HttpHandlerPickedForUrl, data1, data2, data3, AppDomain);
        }

        [NonEvent]
        public void HttpHandlerPickedForUrl(string data1, string data2, string data3)
        {
            HttpHandlerPickedForUrl(data1, data2, data3, "");
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
