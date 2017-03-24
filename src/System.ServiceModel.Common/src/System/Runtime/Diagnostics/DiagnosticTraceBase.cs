// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;

namespace System.Runtime.Diagnostics
{
    internal abstract class DiagnosticTraceBase
    {
        private object _thisLock;

        protected string TraceSourceName;
        
        public DiagnosticTraceBase(string traceSourceName)
        {
            _thisLock = new object();
            this.TraceSourceName = traceSourceName;
            this.LastFailure = DateTime.MinValue;
        }
        
        protected DateTime LastFailure { get; set; }

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
            return retval;
        }
    }
}
  