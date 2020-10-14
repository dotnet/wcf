// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace System.Runtime.Diagnostics
{
    internal sealed partial class EtwDiagnosticTrace : DiagnosticTraceBase
    {
        private const int MaxExceptionDepth = 64;
        private const int XmlBracketsLength = 5; // "<></>".Length;

        public EtwDiagnosticTrace() : base()
        {
        }

        internal static string ExceptionToTraceString(Exception exception, int maxTraceStringLength)
        {
            StringBuilder sb = StringBuilderPool.Take();
            try
            {
                using (StringWriter stringWriter = new StringWriter(sb, CultureInfo.CurrentCulture))
                {
                    using (XmlWriter xml = XmlWriter.Create(stringWriter))
                    {
                        WriteExceptionToTraceString(xml, exception, maxTraceStringLength, MaxExceptionDepth);
                        xml.Flush();
                        stringWriter.Flush();

                        return sb.ToString();
                    }
                }
            }
            finally
            {
                StringBuilderPool.Return(sb);
            }
        }

        private static void WriteExceptionToTraceString(XmlWriter xml, Exception exception, int remainingLength, int remainingAllowedRecursionDepth)
        {
            if (remainingAllowedRecursionDepth < 1)
            {
                return;
            }

            if (!WriteStartElement(xml, DiagnosticStrings.ExceptionTag, ref remainingLength))
            {
                return;
            }

            try
            {
                IList<Tuple<string, string>> exceptionInfo = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string> (DiagnosticStrings.ExceptionTypeTag, XmlEncode(exception.GetType().AssemblyQualifiedName)),
                    new Tuple<string, string> (DiagnosticStrings.MessageTag, XmlEncode(exception.Message)),
                    new Tuple<string, string> (DiagnosticStrings.StackTraceTag, XmlEncode(StackTraceString(exception))), // Stack trace is sometimes null
                    new Tuple<string, string> (DiagnosticStrings.ExceptionStringTag, XmlEncode(exception.ToString())),
                };

                foreach (Tuple<string, string> item in exceptionInfo)
                {
                    if (!WriteXmlElementString(xml, item.Item1, item.Item2, ref remainingLength))
                    {
                        return;
                    }
                }

                if (exception.Data != null && exception.Data.Count > 0)
                {
                    string exceptionData = GetExceptionData(exception);
                    if (exceptionData.Length < remainingLength)
                    {
                        xml.WriteRaw(exceptionData);
                        remainingLength -= exceptionData.Length;
                    }
                }

                if (exception.InnerException != null)
                {
                    string innerException = GetInnerException(exception, remainingLength, remainingAllowedRecursionDepth - 1);
                    if (!string.IsNullOrEmpty(innerException) && innerException.Length < remainingLength)
                    {
                        xml.WriteRaw(innerException);
                    }
                }
            }
            finally
            {
                xml.WriteEndElement();
            }
        }

        private static bool WriteStartElement(XmlWriter xml, string localName, ref int remainingLength)
        {
            int minXmlLength = (localName.Length * 2) + XmlBracketsLength;
            if (minXmlLength <= remainingLength)
            {
                xml.WriteStartElement(localName);
                remainingLength -= minXmlLength;
                return true;
            }
            return false;
        }

        private static bool WriteXmlElementString(XmlWriter xml, string localName, string value, ref int remainingLength)
        {
            int xmlElementLength = (localName.Length * 2) + XmlBracketsLength + (value != null ? value.Length : 0);
            if (xmlElementLength <= remainingLength)
            {
                xml.WriteElementString(localName, value);
                remainingLength -= xmlElementLength;
                return true;
            }
            return false;
        }

        private static string GetExceptionData(Exception exception)
        {
            StringBuilder sb = StringBuilderPool.Take();
            try
            {
                using (StringWriter stringWriter = new StringWriter(sb, CultureInfo.CurrentCulture))
                {
                    using (XmlWriter xml = XmlWriter.Create(stringWriter))
                    {
                        xml.WriteStartElement(DiagnosticStrings.DataItemsTag);
                        foreach (object dataItem in exception.Data.Keys)
                        {
                            xml.WriteStartElement(DiagnosticStrings.DataTag);
                            xml.WriteElementString(DiagnosticStrings.KeyTag, XmlEncode(dataItem.ToString()));
                            if (exception.Data[dataItem] == null)
                            {
                                xml.WriteElementString(DiagnosticStrings.ValueTag, string.Empty);
                            }
                            else
                            {
                                xml.WriteElementString(DiagnosticStrings.ValueTag, XmlEncode(exception.Data[dataItem].ToString()));
                            }

                            xml.WriteEndElement();
                        }
                        xml.WriteEndElement();
                        xml.Flush();
                        stringWriter.Flush();

                        return sb.ToString();
                    }
                }
            }
            finally
            {
                StringBuilderPool.Return(sb);
            }
        }

        private static string GetInnerException(Exception exception, int remainingLength, int remainingAllowedRecursionDepth)
        {
            if (remainingAllowedRecursionDepth < 1)
            {
                return null;
            }

            StringBuilder sb = StringBuilderPool.Take();
            try
            {
                using (StringWriter stringWriter = new StringWriter(sb, CultureInfo.CurrentCulture))
                {
                    using (XmlWriter xml = XmlWriter.Create(stringWriter))
                    {
                        if (!WriteStartElement(xml, DiagnosticStrings.InnerExceptionTag, ref remainingLength))
                        {
                            return null;
                        }

                        WriteExceptionToTraceString(xml, exception.InnerException, remainingLength, remainingAllowedRecursionDepth);
                        xml.WriteEndElement();
                        xml.Flush();
                        stringWriter.Flush();

                        return sb.ToString();
                    }
                }
            }
            finally
            {
                StringBuilderPool.Return(sb);
            }
        }

        internal static class StringBuilderPool
        {
            private const int maxPooledStringBuilders = 64;
            private static readonly ConcurrentQueue<StringBuilder> s_freeStringBuilders = new ConcurrentQueue<StringBuilder>();

            public static StringBuilder Take()
            {
                StringBuilder sb = null;
                if (s_freeStringBuilders.TryDequeue(out sb))
                {
                    return sb;
                }

                return new StringBuilder();
            }

            public static void Return(StringBuilder sb)
            {
                Fx.Assert(sb != null, "'sb' MUST NOT be NULL.");
                if (s_freeStringBuilders.Count <= maxPooledStringBuilders)
                {
                    // There is a race condition here so the count could be off a little bit (but insignificantly)
                    sb.Clear();
                    s_freeStringBuilders.Enqueue(sb);
                }
            }
        }
    }
}
