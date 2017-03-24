// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Tracing;

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

        [Event(EventIds.BufferPoolAllocation, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.BufferPoolingAllocate, Task = Tasks.BufferPooling, Keywords = Keywords.Infrastructure | ChannelKeywords.Debug,
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

        [Event(EventIds.BufferPoolChangeQuota, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = Opcodes.BufferPoolingTune, Task = Tasks.BufferPooling, Keywords = Keywords.Infrastructure | ChannelKeywords.Debug,
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

        public bool EtwUnhandledExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Critical, Keywords.Infrastructure, EventChannel.Operational);
        }

        [Event(EventIds.EtwUnhandledException, Level = EventLevel.Critical, Channel = EventChannel.Operational, Opcode = EventOpcode.Info, Keywords = Keywords.Infrastructure | ChannelKeywords.Operational,
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

        public bool MaxSentMessageSizeExceededIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Quota, EventChannel.Analytic);
        }

        [Event(EventIds.MaxSentMessageSizeExceeded, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Task = Tasks.Quotas, Keywords = Keywords.Quota | ChannelKeywords.Analytic,
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

        public bool ShipAssertExceptionMessageIsEnabled()
        {
            return base.IsEnabled(EventLevel.Error, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(EventIds.ShipAssertExceptionMessage, Level = EventLevel.Error, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Keywords = Keywords.Infrastructure | ChannelKeywords.Analytic,
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

        public bool ThrowingEtwExceptionVerboseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(EventIds.ThrowingEtwExceptionVerbose, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Keywords = Keywords.Infrastructure | ChannelKeywords.Analytic,
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

        [Event(EventIds.ThrowingEtwException, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Keywords = Keywords.Infrastructure | ChannelKeywords.Analytic,
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

        public bool UnhandledExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Critical, Keywords.Infrastructure, EventChannel.Operational);
        }

        [Event(EventIds.UnhandledException, Level = EventLevel.Critical, Channel = EventChannel.Operational, Opcode = EventOpcode.Info, Keywords = Keywords.Infrastructure | ChannelKeywords.Operational,
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

        public bool ThrowingExceptionVerboseIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(EventIds.ThrowingExceptionVerbose, Level = EventLevel.Verbose, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Keywords = Keywords.Infrastructure | ChannelKeywords.Analytic,
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

        public bool ThrowingExceptionIsEnabled()
        {
            return base.IsEnabled(EventLevel.Warning, Keywords.Infrastructure, EventChannel.Analytic);
        }

        [Event(EventIds.ThrowingException, Level = EventLevel.Warning, Channel = EventChannel.Analytic, Opcode = EventOpcode.Info, Keywords = Keywords.Infrastructure | ChannelKeywords.Analytic,
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

        #region Keywords / Tasks / Opcodes

        public class EventIds
        {
            public const int BufferPoolAllocation = 131;
            public const int BufferPoolChangeQuota = 132;
            public const int MaxSentMessageSizeExceeded = 1417;
            public const int ShipAssertExceptionMessage = 57395;
            public const int ThrowingException = 57396;
            public const int UnhandledException = 57397;
            public const int EtwUnhandledException = 57408;
            public const int ThrowingEtwExceptionVerbose = 57409;
            public const int ThrowingEtwException = 57410;
            public const int ThrowingExceptionVerbose = 57407;
        }

        public class Tasks
        {
            public const EventTask BufferPooling = (EventTask)2509;
            public const EventTask Quotas = (EventTask)2560;
        }

        public class Opcodes
        {
            public const EventOpcode BufferPoolingAllocate = (EventOpcode)12;
            public const EventOpcode BufferPoolingTune = (EventOpcode)13;
        }

        public class Keywords
        {
            public const EventKeywords Infrastructure = (EventKeywords)0x10000;
            public const EventKeywords Quota = (EventKeywords)0x400000;
        }

        public class ChannelKeywords
        {
            public const EventKeywords Admin = unchecked((EventKeywords)0x8000000000000000);
            public const EventKeywords Operational = unchecked((EventKeywords)0x4000000000000000);
            public const EventKeywords Analytic = unchecked((EventKeywords)0x2000000000000000);
            public const EventKeywords Debug = unchecked((EventKeywords)0x1000000000000000);
        }

        #endregion
    }
}
