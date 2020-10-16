// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Tracing;

namespace System.Runtime
{
    [EventSource(Name = "Microsoft-Windows-Application Server-Applications", Guid = "c651f5f6-1c0d-492e-8ae1-b4efd7c9d503")]
    internal sealed partial class WcfEventSource : EventSource
    {
        public static WcfEventSource Instance = new WcfEventSource();
#pragma warning disable CS0414 // The field is assigned but its value is never used - Not used in Federation but is in S.P.SM
        private bool _canTransferActivityId = false;
#pragma warning restore CS0414 // The field is assigned but its value is never used

        internal WcfEventSource()
#if DEBUG
             : base(throwOnEventWriteErrors: true)
#endif // DEBUG
        {
            if (Environment.Version.Major >= 5)
            {
                _canTransferActivityId = true;
            }
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

        #region Keywords / Tasks / Opcodes

        public partial class EventIds
        {
            public const int EtwUnhandledException = 57408;
            public const int ThrowingEtwExceptionVerbose = 57409;
            public const int ThrowingEtwException = 57410;
        }

        public partial class Keywords
        {
            public const EventKeywords Infrastructure = (EventKeywords)0x10000;

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
