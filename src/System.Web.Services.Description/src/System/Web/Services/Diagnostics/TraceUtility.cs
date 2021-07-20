// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace System.Web.Services.Diagnostics
{
    internal static class Tracing
    {
        private static bool s_tracingEnabled = true;
        private static bool s_tracingInitialized;
        private static bool s_appDomainShutdown;
        private const string TraceSourceAsmx = "System.Web.Services.Asmx";
        private static TraceSource s_asmxTraceSource;

        private static object s_internalSyncObject;
        private static object InternalSyncObject
        {
            get
            {
                if (s_internalSyncObject == null)
                {
                    object o = new Object();
                    Interlocked.CompareExchange(ref s_internalSyncObject, o, null);
                }
                return s_internalSyncObject;
            }
        }

        internal static bool IsVerbose
        {
            get
            {
                return ValidateSettings(Asmx, TraceEventType.Verbose);
            }
        }

        internal static TraceSource Asmx
        {
            get
            {
                if (!s_tracingInitialized)
                {
                    InitializeLogging();
                }

                if (!s_tracingEnabled)
                {
                    return null;
                }

                return s_asmxTraceSource;
            }
        }

        private static void InitializeLogging()
        {
            lock (InternalSyncObject)
            {
                if (!s_tracingInitialized)
                {
                    bool loggingEnabled = false;
                    s_asmxTraceSource = new TraceSource(TraceSourceAsmx);
                    if (s_asmxTraceSource.Switch.ShouldTrace(TraceEventType.Critical))
                    {
                        loggingEnabled = true;
                        AppDomain currentDomain = AppDomain.CurrentDomain;
                        currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
                        currentDomain.DomainUnload += new EventHandler(AppDomainUnloadEvent);
                        currentDomain.ProcessExit += new EventHandler(ProcessExitEvent);
                    }
                    s_tracingEnabled = loggingEnabled;
                    s_tracingInitialized = true;
                }
            }
        }

        private static void Close()
        {
            if (s_asmxTraceSource != null)
            {
                s_asmxTraceSource.Close();
            }
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            ExceptionCatch(TraceEventType.Error, sender, nameof(UnhandledExceptionHandler), e);
        }

        private static void ProcessExitEvent(object sender, EventArgs e)
        {
            Close();
            s_appDomainShutdown = true;
        }

        private static void AppDomainUnloadEvent(object sender, EventArgs e)
        {
            Close();
            s_appDomainShutdown = true;
        }

        private static bool ValidateSettings(TraceSource traceSource, TraceEventType traceLevel)
        {
            if (!s_tracingEnabled)
            {
                return false;
            }
            if (!s_tracingInitialized)
            {
                InitializeLogging();
            }
            if (traceSource == null || !traceSource.Switch.ShouldTrace(traceLevel))
            {
                return false;
            }
            if (s_appDomainShutdown)
            {
                return false;
            }
            return true;
        }

        private static void TraceEvent(TraceEventType eventType, string format)
        {
            Asmx.TraceEvent(eventType, 0, format);
        }

        internal static Exception ExceptionCatch(TraceEventType eventType, object target, string method, Exception e)
        {
            if (!ValidateSettings(Asmx, eventType))
            {
                return e;
            }

            TraceEvent(eventType, SR.Format(SR.TraceExceptionCaught, TraceMethod.MethodId(target, method), e.GetType(), e.Message));
            StackTrace(eventType, e);

            return e;
        }

        private static void StackTrace(TraceEventType eventType, Exception e)
        {
            if (IsVerbose && !string.IsNullOrEmpty(e.StackTrace))
            {
                TraceEvent(eventType, SR.Format(SR.TraceExceptionDetails, e.ToString()));
            }
        }

        internal static void OnUnknownElement(object sender, XmlElementEventArgs e)
        {
            if (!ValidateSettings(Asmx, TraceEventType.Warning))
            {
                return;
            }

            if (e.Element == null)
            {
                return;
            }

            string xml = RuntimeUtils.ElementString(e.Element);
            string format = e.ExpectedElements == null ? SR.WebUnknownElement : e.ExpectedElements.Length == 0 ? SR.WebUnknownElement1 : SR.WebUnknownElement2;
            TraceEvent(TraceEventType.Warning, SR.Format(format, xml, e.ExpectedElements));
        }
    }

    internal class TraceMethod
    {
        internal static string MethodId(object target, string method)
        {
            StringBuilder sb = new StringBuilder();
            WriteObjectId(sb, target);
            sb.Append(':');
            sb.Append(':');
            sb.Append(method);

            return sb.ToString();
        }

        private static void WriteObjectId(StringBuilder sb, object o)
        {
            if (o == null)
            {
                sb.Append("(null)");
            }
            else if (o is Type)
            {
                Type type = (Type)o;
                sb.Append(type.FullName);
                if (!(type.IsAbstract && type.IsSealed))
                {
                    sb.Append('#');
                    sb.Append(HashString(o));
                }
            }
            else
            {
                sb.Append(o.GetType().FullName);
                sb.Append('#');
                sb.Append(HashString(o));
            }
        }

        private static string HashString(object objectValue)
        {
            if (objectValue == null)
            {
                return "(null)";
            }
            else
            {
                return objectValue.GetHashCode().ToString(NumberFormatInfo.InvariantInfo);
            }
        }
    }
}
