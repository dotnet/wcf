// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Security;
using System.Text;
using Microsoft.Xml;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;

namespace System.Runtime.Diagnostics
{
    internal abstract class DiagnosticTraceBase
    {
        //Diagnostics trace
        protected const string DefaultTraceListenerName = "Default";
        protected const string TraceRecordVersion = "http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord";

        protected static string AppDomainFriendlyName = String.Empty;

        private const ushort TracingEventLogCategory = 4;

        private object _thisLock;
        protected string TraceSourceName;
        [Fx.Tag.SecurityNote(Critical = "This determines the event source name.")]
        [SecurityCritical]


        private string _eventSourceName;

        public DiagnosticTraceBase(string traceSourceName)
        {
            _thisLock = new object();
            this.TraceSourceName = traceSourceName;
            this.LastFailure = DateTime.MinValue;
        }

        protected DateTime LastFailure { get; set; }

        protected string EventSourceName
        {
            [Fx.Tag.SecurityNote(Critical = "Access critical eventSourceName field",
                Safe = "Doesn't leak info\\resources")]
            [SecuritySafeCritical]
            get
            {
                return _eventSourceName;
            }

            [Fx.Tag.SecurityNote(Critical = "This determines the event source name.")]
            [SecurityCritical]
            set
            {
                _eventSourceName = value;
            }
        }

        public bool TracingEnabled
        {
            get
            {
                return false;
            }
        }

        protected static string ProcessName
        {
            [Fx.Tag.SecurityNote(Critical = "Satisfies a LinkDemand for 'PermissionSetAttribute' on type 'Process' when calling method GetCurrentProcess",
            Safe = "Does not leak any resource and has been reviewed")]
            [SecuritySafeCritical]
            get
            {
                string retval = null;


                return retval;
            }
        }

        protected static int ProcessId
        {
            [Fx.Tag.SecurityNote(Critical = "Satisfies a LinkDemand for 'PermissionSetAttribute' on type 'Process' when calling method GetCurrentProcess",
            Safe = "Does not leak any resource and has been reviewed")]
            [SecuritySafeCritical]
            get
            {
                int retval = -1;
                return retval;
            }
        }


        //only used for exceptions, perf is not important
        public static string XmlEncode(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            int len = text.Length;
            StringBuilder encodedText = new StringBuilder(len + 8); //perf optimization, expecting no more than 2 > characters

            for (int i = 0; i < len; ++i)
            {
                char ch = text[i];
                switch (ch)
                {
                    case '<':
                        encodedText.Append("&lt;");
                        break;
                    case '>':
                        encodedText.Append("&gt;");
                        break;
                    case '&':
                        encodedText.Append("&amp;");
                        break;
                    default:
                        encodedText.Append(ch);
                        break;
                }
            }
            return encodedText.ToString();
        }

        [Fx.Tag.SecurityNote(Critical = "Sets global event handlers for the AppDomain",
            Safe = "Doesn't leak resources\\Information")]
        [SecuritySafeCritical]
        [SuppressMessage(FxCop.Category.Security, FxCop.Rule.DoNotIndirectlyExposeMethodsWithLinkDemands,
                Justification = "SecuritySafeCritical method, Does not expose critical resources returned by methods with Link Demands")]
        protected void AddDomainEventHandlersForCleanup()
        {
        }

        private void ExitOrUnloadEventHandler(object sender, EventArgs e)
        {
        }


        protected static string CreateSourceString(object source)
        {
            var traceSourceStringProvider = source as ITraceSourceStringProvider;
            if (traceSourceStringProvider != null)
            {
                return traceSourceStringProvider.GetSourceString();
            }

            return CreateDefaultSourceString(source);
        }

        internal static string CreateDefaultSourceString(object source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return String.Format(CultureInfo.CurrentCulture, "{0}/{1}", source.GetType().ToString(), source.GetHashCode());
        }

        protected static void AddExceptionToTraceString(XmlWriter xml, Exception exception)
        {
            xml.WriteElementString(DiagnosticStrings.ExceptionTypeTag, XmlEncode(exception.GetType().AssemblyQualifiedName));
            xml.WriteElementString(DiagnosticStrings.MessageTag, XmlEncode(exception.Message));
            xml.WriteElementString(DiagnosticStrings.StackTraceTag, XmlEncode(StackTraceString(exception)));
            xml.WriteElementString(DiagnosticStrings.ExceptionStringTag, XmlEncode(exception.ToString()));

            if (exception.Data != null && exception.Data.Count > 0)
            {
                xml.WriteStartElement(DiagnosticStrings.DataItemsTag);
                foreach (object dataItem in exception.Data.Keys)
                {
                    xml.WriteStartElement(DiagnosticStrings.DataTag);
                    xml.WriteElementString(DiagnosticStrings.KeyTag, XmlEncode(dataItem.ToString()));
                    xml.WriteElementString(DiagnosticStrings.ValueTag, XmlEncode(exception.Data[dataItem].ToString()));
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();
            }
            if (exception.InnerException != null)
            {
                xml.WriteStartElement(DiagnosticStrings.InnerExceptionTag);
                AddExceptionToTraceString(xml, exception.InnerException);
                xml.WriteEndElement();
            }
        }

        protected static string StackTraceString(Exception exception)
        {
            string retval = exception.StackTrace;

            return retval;
        }

        // Duplicate code from System.ServiceModel.Diagnostics
        [Fx.Tag.SecurityNote(Critical = "Calls unsafe methods, UnsafeCreateEventLogger and UnsafeLogEvent.",
            Safe = "Event identities cannot be spoofed as they are constants determined inside the method, Demands the same permission that is asserted by the unsafe method.")]
        [SecuritySafeCritical]
        [SuppressMessage(FxCop.Category.Security, FxCop.Rule.SecureAsserts,
            Justification = "Should not demand permission that is asserted by the EtwProvider ctor.")]
        protected void LogTraceFailure(string traceString, Exception exception)
        {
        }


        public static Guid ActivityId
        {
            [Fx.Tag.SecurityNote(Critical = "gets the CorrelationManager, which does a LinkDemand for UnmanagedCode",
                Safe = "only uses the CM to get the ActivityId, which is not protected data, doesn't leak the CM")]
            [SecuritySafeCritical]
            [SuppressMessage(FxCop.Category.Security, FxCop.Rule.DoNotIndirectlyExposeMethodsWithLinkDemands,
                Justification = "SecuritySafeCriticial method")]
            get
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            [Fx.Tag.SecurityNote(Critical = "gets the CorrelationManager, which does a LinkDemand for UnmanagedCode",
                Safe = "only uses the CM to get the ActivityId, which is not protected data, doesn't leak the CM")]
            [SecuritySafeCritical]
            set
            {
                throw ExceptionHelper.PlatformNotSupported();
            }
        }

#pragma warning restore 56500


        public abstract bool IsEnabled();
    }
}
