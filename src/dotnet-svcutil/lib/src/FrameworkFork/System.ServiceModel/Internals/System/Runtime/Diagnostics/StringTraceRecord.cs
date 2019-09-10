// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Xml;

namespace System.Runtime.Diagnostics
{
    internal class StringTraceRecord : TraceRecord
    {
        private string _elementName;
        private string _content;

        internal StringTraceRecord(string elementName, string content)
        {
            _elementName = elementName;
            _content = content;
        }

        internal override string EventId
        {
            get { return BuildEventId("String"); }
        }

        internal override void WriteTo(XmlWriter writer)
        {
            writer.WriteElementString(_elementName, _content);
        }
    }
}
