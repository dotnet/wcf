// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Text;
using System.Xml;
using System.ServiceModel;

namespace System.Runtime.Diagnostics
{
    internal abstract partial class DiagnosticTraceBase
    {
        //Diagnostics trace
        protected const string DefaultTraceListenerName = "Default";
        protected const string TraceRecordVersion = "http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord";

        protected static string AppDomainFriendlyName = String.Empty;

        private const ushort TracingEventLogCategory = 4;

        private string _eventSourceName;

        protected string EventSourceName
        {
            get
            {
                return _eventSourceName;
            }
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
            get
            {
                string retval = null;
                return retval;
            }
        }

        protected static int ProcessId
        {
            get
            {
                int retval = -1;
                return retval;
            }
        }

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

            return string.Format(CultureInfo.CurrentCulture, "{0}/{1}", source.GetType().ToString(), source.GetHashCode());
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

        public static Guid ActivityId
        {
            get
            {
                throw ExceptionHelper.PlatformNotSupported();
            }
            set
            {
                throw ExceptionHelper.PlatformNotSupported();
            }
        }

        public abstract bool IsEnabled();
    }
}
