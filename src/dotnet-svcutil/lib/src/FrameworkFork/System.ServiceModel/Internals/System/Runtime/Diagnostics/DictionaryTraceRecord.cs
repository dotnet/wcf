// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Xml;
using System.Collections;

namespace System.Runtime.Diagnostics
{
    internal class DictionaryTraceRecord : TraceRecord
    {
        private IDictionary _dictionary;

        internal DictionaryTraceRecord(IDictionary dictionary)
        {
            _dictionary = dictionary;
        }

        internal override string EventId { get { return TraceRecord.EventIdBase + "Dictionary" + TraceRecord.NamespaceSuffix; } }

        internal override void WriteTo(XmlWriter xml)
        {
            if (_dictionary != null)
            {
                foreach (object key in _dictionary.Keys)
                {
                    object value = _dictionary[key];
                    xml.WriteElementString(key.ToString(), value == null ? string.Empty : value.ToString());
                }
            }
        }
    }
}
