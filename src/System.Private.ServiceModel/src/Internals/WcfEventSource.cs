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
