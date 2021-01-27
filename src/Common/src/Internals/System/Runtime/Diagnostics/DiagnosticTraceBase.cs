// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Text;

namespace System.Runtime.Diagnostics
{
    internal abstract partial class DiagnosticTraceBase
    {
        public DiagnosticTraceBase()
        {
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
    }
}
