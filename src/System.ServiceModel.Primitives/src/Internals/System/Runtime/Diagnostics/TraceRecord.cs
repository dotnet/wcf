// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Xml;

namespace System.Runtime.Diagnostics
{
    public class TraceRecord
    {
        protected const string EventIdBase = "http://schemas.microsoft.com/2006/08/ServiceModel/";
        protected const string NamespaceSuffix = "TraceRecord";

        internal virtual string EventId { get { return BuildEventId("Empty"); } }

        internal virtual void WriteTo(XmlWriter writer)
        {
        }

        protected string BuildEventId(string eventId)
        {
            return TraceRecord.EventIdBase + eventId + TraceRecord.NamespaceSuffix;
        }

        protected string XmlEncode(string text)
        {
            return DiagnosticTraceBase.XmlEncode(text);
        }
    }
}
