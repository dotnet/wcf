// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace System.Runtime.Diagnostics
{
    internal sealed partial class EtwDiagnosticTrace : DiagnosticTraceBase
    {
        private const int MaxExceptionStringLength = 28 * 1024;

        public bool IsEtwProviderEnabled
        {
            get
            {
                return false;
            }
        }

        public bool IsEnd2EndActivityTracingEnabled
        {
            get
            {
                return WcfEventSource.Instance.IsEnabled() && 
                    (WcfEventSource.Instance.ActionItemScheduledIsEnabled() || WcfEventSource.Instance.ActionItemCallbackInvokedIsEnabled());
            }
        }

        private bool EtwTracingEnabled
        {
            get
            {
                return false;
            }
        }

        public void SetEnd2EndActivityTracingEnabled(bool isEnd2EndTracingEnabled)
        {
        }

        public void Event(ref EventDescriptor eventDescriptor, string description)
        {
            if (TracingEnabled)
            {
                TracePayload tracePayload = GetSerializedPayload(null, null, null);
                WriteTraceSource(ref eventDescriptor, description, tracePayload);
            }
        }

        public void SetAndTraceTransfer(Guid newId, bool emitTransfer)
        {
            if (emitTransfer)
            {
                TraceTransfer(newId);
            }
            EtwDiagnosticTrace.ActivityId = newId;
        }

        public void TraceTransfer(Guid newId)
        {
        }

        public void WriteTraceSource(ref EventDescriptor eventDescriptor, string description, TracePayload payload)
        {
        }

        private static string LookupChannel(TraceChannel traceChannel)
        {
            string channelName;
            switch (traceChannel)
            {
                case TraceChannel.Admin:
                    channelName = "Admin";
                    break;
                case TraceChannel.Analytic:
                    channelName = "Analytic";
                    break;
                case TraceChannel.Application:
                    channelName = "Application";
                    break;
                case TraceChannel.Debug:
                    channelName = "Debug";
                    break;
                case TraceChannel.Operational:
                    channelName = "Operational";
                    break;
                case TraceChannel.Perf:
                    channelName = "Perf";
                    break;
                default:
                    channelName = traceChannel.ToString();
                    break;
            }

            return channelName;
        }

        public TracePayload GetSerializedPayload(object source, TraceRecord traceRecord, Exception exception)
        {
            return GetSerializedPayload(source, traceRecord, exception, false);
        }

        public TracePayload GetSerializedPayload(object source, TraceRecord traceRecord, Exception exception, bool getServiceReference)
        {
            string eventSource = null;
            string extendedData = null;
            string serializedException = null;

            if (source != null)
            {
                eventSource = CreateSourceString(source);
            }

            if (traceRecord != null)
            {
                StringBuilder sb = StringBuilderPool.Take();
                try
                {
                    using (StringWriter stringWriter = new StringWriter(sb, CultureInfo.CurrentCulture))
                    {
                        using (XmlWriter writer = XmlWriter.Create(stringWriter))
                        {
                            writer.WriteStartElement(DiagnosticStrings.ExtendedDataTag);
                            traceRecord.WriteTo(writer);
                            writer.WriteEndElement();
                            writer.Flush();
                            stringWriter.Flush();

                            extendedData = sb.ToString();
                        }
                    }
                }
                finally
                {
                    StringBuilderPool.Return(sb);
                }
            }

            if (exception != null)
            {
                // We want to keep the ETW trace message to under 32k. So we keep the serialized exception to under 28k bytes.
                serializedException = ExceptionToTraceString(exception, MaxExceptionStringLength);
            }

            return new TracePayload(serializedException, eventSource, DiagnosticTraceBase.AppDomainFriendlyName, extendedData, string.Empty);
        }

        public bool IsEtwEventEnabled(ref EventDescriptor eventDescriptor)
        {
            return IsEtwEventEnabled(ref eventDescriptor, true);
        }

        public bool IsEtwEventEnabled(ref EventDescriptor eventDescriptor, bool fullCheck)
        {
            return false;
        }

        public override bool IsEnabled()
        {
            return false;
        }

        private static class TraceCodes
        {
            public const string AppDomainUnload = "AppDomainUnload";
            public const string TraceHandledException = "TraceHandledException";
            public const string ThrowingException = "ThrowingException";
            public const string UnhandledException = "UnhandledException";
        }

        private static class EventIdsWithMsdnTraceCode
        {
            // EventIds for which we need to translate the traceCode and the eventId
            // when system.diagnostics tracing is enabled.
            public const int AppDomainUnload = 57393;
            public const int ThrowingExceptionWarning = 57396;
            public const int ThrowingExceptionVerbose = 57407;
            public const int HandledExceptionInfo = 57394;
            public const int HandledExceptionWarning = 57404;
            public const int HandledExceptionError = 57405;
            public const int HandledExceptionVerbose = 57406;
            public const int UnhandledException = 57397;
        }

        private static class LegacyTraceEventIds
        {
            // Diagnostic trace codes
            public const int Diagnostics = 0X20000;
            public const int AppDomainUnload = LegacyTraceEventIds.Diagnostics | 0X0001;
            public const int EventLog = LegacyTraceEventIds.Diagnostics | 0X0002;
            public const int ThrowingException = LegacyTraceEventIds.Diagnostics | 0X0003;
            public const int TraceHandledException = LegacyTraceEventIds.Diagnostics | 0X0004;
            public const int UnhandledException = LegacyTraceEventIds.Diagnostics | 0X0005;
        }
    }
}
