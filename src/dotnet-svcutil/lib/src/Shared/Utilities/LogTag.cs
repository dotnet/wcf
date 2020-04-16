// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    /// <summary>
    /// This class enables communication between the Svcutil and the process that invokes it
    /// so that process messages can be distiguished between error, warning or just info messages.
    /// </summary>
    internal class LogTag
    {
        private readonly string _value;

        private static string s_tagStart = "$";
        private static string s_tagEnd = "!#";
        private static int s_tagLength = 4;

        public static LogTag Information { get; } = new LogTag("$I!#");
        public static LogTag Important { get; } = new LogTag("$T!#");
        public static LogTag Warning { get; } = new LogTag("$W!#");
        public static LogTag Error { get; } = new LogTag("$E!#");
        public static LogTag LogMessage { get; } = new LogTag("$L!#");

        public static LogTag NewLine { get; } = new LogTag("$N!#");

        public static LogTag TraceSuccess { get; } = new LogTag("$S!#");
        public static LogTag TraceFailure { get; } = new LogTag("$F!#");
        public static LogTag TraceProperty { get; } = new LogTag("$P!#");
        public static LogTag TraceException { get; } = new LogTag("$X!#");


        private LogTag(string tag)
        {
            if (string.IsNullOrEmpty(tag) || tag.Length != LogTag.s_tagLength || !tag.StartsWith(s_tagStart, StringComparison.Ordinal) || !tag.EndsWith(s_tagEnd, StringComparison.Ordinal))
            {
                throw new ArgumentException(string.Empty, nameof(tag));
            }
            _value = tag;
        }

        // Enable LogTag objects to be treated as strings, but not the opposite to ensure strong type validation for function parameters.
        public static implicit operator string(LogTag tag)
        {
            return tag.ToString();
        }

        public static bool IsTrace(LogTag tag)
        {
            return tag == LogTag.TraceSuccess || tag == LogTag.TraceProperty || tag == LogTag.TraceFailure || tag == LogTag.TraceException || tag == LogTag.LogMessage;
        }

        public static bool IsTag(string message, out string tag, out string value)
        {
            tag = null;
            value = string.Empty;

            if (!string.IsNullOrEmpty(message) && message.Length >= LogTag.s_tagLength)
            {
                tag = message.Substring(0, LogTag.s_tagLength);
                if (tag != Important && tag != Information && tag != Warning && tag != Error && tag != LogMessage && tag != TraceSuccess && tag != TraceFailure && tag != TraceProperty && tag != TraceException)
                {
                    tag = null;
                }
                else if (message.Length > tag.Length)
                {
                    value = message.Substring(tag.Length);
                }
            }
            return tag != null;
        }

        public override string ToString()
        {
            return _value;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}
