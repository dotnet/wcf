// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Xml;

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
