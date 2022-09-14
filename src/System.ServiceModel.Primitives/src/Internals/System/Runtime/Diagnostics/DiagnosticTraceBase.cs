// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Text;
using System.Xml;
using System.ServiceModel;
using System.ServiceModel.Diagnostics;
using System.Diagnostics;

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

        protected static string StackTraceString(Exception exception)
        {
            string retval = exception.StackTrace;
            if (string.IsNullOrEmpty(retval))
            {
                // This means that the exception hasn't been thrown yet. We need to manufacture the stack then.
                StackTrace stackTrace = new StackTrace(false);
                // Figure out how many frames should be throw away
                StackFrame[] stackFrames = stackTrace.GetFrames();

                int frameCount = 0;
                bool breakLoop = false;
                foreach (StackFrame frame in stackFrames)
                {
                    string methodName = frame.GetMethod().Name;
                    switch (methodName)
                    {
                        case "StackTraceString":
                        case "AddExceptionToTraceString":
                        case "BuildTrace":
                        case "TraceEvent":
                        case "TraceException":
                        case "GetAdditionalPayload":
                            ++frameCount;
                            break;
                        default:
                            if (methodName.StartsWith("ThrowHelper", StringComparison.Ordinal))
                            {
                                ++frameCount;
                            }
                            else
                            {
                                breakLoop = true;
                            }
                            break;
                    }
                    if (breakLoop)
                    {
                        break;
                    }
                }

                stackTrace = new StackTrace(frameCount, false);
                retval = stackTrace.ToString();
            }
            return retval;
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
