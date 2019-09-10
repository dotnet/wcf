// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xml
{
    internal class XmlDictionaryAsyncCheckWriter : XmlDictionaryWriter
    {
        private readonly XmlDictionaryWriter _coreWriter = null;
        private Task _lastTask;

        public XmlDictionaryAsyncCheckWriter(XmlDictionaryWriter writer)
        {
            _coreWriter = writer;
        }

        internal XmlDictionaryWriter CoreWriter
        {
            get
            {
                return _coreWriter;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckAsync()
        {
            if (_lastTask != null && !_lastTask.IsCompleted)
            {
                throw new InvalidOperationException(SRSerialization.XmlAsyncIsRunningException);
            }
        }

        private Task SetLastTask(Task task)
        {
            _lastTask = task;
            return task;
        }

        public override XmlWriterSettings Settings
        {
            get
            {
                CheckAsync();
                return CoreWriter.Settings;
            }
        }

        public override WriteState WriteState
        {
            get
            {
                CheckAsync();
                return CoreWriter.WriteState;
            }
        }

        public override string XmlLang
        {
            get
            {
                CheckAsync();
                return CoreWriter.XmlLang;
            }
        }

        public override XmlSpace XmlSpace
        {
            get
            {
                CheckAsync();
                return CoreWriter.XmlSpace;
            }
        }

        public override void Flush()
        {
            CheckAsync();
            CoreWriter.Flush();
        }

#if disabled
        public override Task FlushAsync()
        {
            CheckAsync();
            return SetLastTask(CoreWriter.FlushAsync());
        }
#endif
        public override string LookupPrefix(string ns)
        {
            CheckAsync();
            return CoreWriter.LookupPrefix(ns);
        }

        public override void WriteAttributes(XmlReader reader, bool defattr)
        {
            CheckAsync();
            CoreWriter.WriteAttributes(reader, defattr);
        }

#if disabled
        public override Task WriteAttributesAsync(XmlReader reader, bool defattr)
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteAttributesAsync(reader, defattr));
        }
#endif
        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            CheckAsync();
            CoreWriter.WriteBase64(buffer, index, count);
        }

#if disabled
        public override Task WriteBase64Async(byte[] buffer, int index, int count)
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteBase64Async(buffer, index, count));
        }
#endif
        public override void WriteBinHex(byte[] buffer, int index, int count)
        {
            CheckAsync();
            CoreWriter.WriteBinHex(buffer, index, count);
        }

#if disabled
        public override Task WriteBinHexAsync(byte[] buffer, int index, int count)
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteBinHexAsync(buffer, index, count));
        }
#endif
        public override void WriteCData(string text)
        {
            CheckAsync();
            CoreWriter.WriteCData(text);
        }

#if disabled
        public override Task WriteCDataAsync(string text)
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteCDataAsync(text));
        }
#endif
        public override void WriteCharEntity(char ch)
        {
            CheckAsync();
            CoreWriter.WriteCharEntity(ch);
        }

#if disabled
        public override Task WriteCharEntityAsync(char ch)
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteCharEntityAsync(ch));
        }
#endif
        public override void WriteChars(char[] buffer, int index, int count)
        {
            CheckAsync();
            CoreWriter.WriteChars(buffer, index, count);
        }

#if disabled
        public override Task WriteCharsAsync(char[] buffer, int index, int count)
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteCharsAsync(buffer, index, count));
        }
#endif
        public override void WriteComment(string text)
        {
            CheckAsync();
            CoreWriter.WriteComment(text);
        }

#if disabled
        public override Task WriteCommentAsync(string text)
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteCommentAsync(text));
        }
#endif
        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            CheckAsync();
            CoreWriter.WriteDocType(name, pubid, sysid, subset);
        }

#if disabled
        public override Task WriteDocTypeAsync(string name, string pubid, string sysid, string subset)
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteDocTypeAsync(name, pubid, sysid, subset));
        }
#endif
        public override void WriteEndAttribute()
        {
            CheckAsync();
            CoreWriter.WriteEndAttribute();
        }

        public override void WriteEndDocument()
        {
            CheckAsync();
            CoreWriter.WriteEndDocument();
        }

#if disabled
        public override Task WriteEndDocumentAsync()
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteEndDocumentAsync());
        }
#endif
        public override void WriteEndElement()
        {
            CheckAsync();
            CoreWriter.WriteEndElement();
        }

#if disabled
        public override Task WriteEndElementAsync()
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteEndElementAsync());
        }
#endif
        public override void WriteEntityRef(string name)
        {
            CheckAsync();
            CoreWriter.WriteEntityRef(name);
        }

#if disabled
        public override Task WriteEntityRefAsync(string name)
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteEntityRefAsync(name));
        }
#endif
        public override void WriteFullEndElement()
        {
            CheckAsync();
            CoreWriter.WriteFullEndElement();
        }

#if disabled
        public override Task WriteFullEndElementAsync()
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteFullEndElementAsync());
        }
#endif
        public override void WriteName(string name)
        {
            CheckAsync();
            CoreWriter.WriteName(name);
        }

#if disabled
        public override Task WriteNameAsync(string name)
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteNameAsync(name));
        }
#endif
        public override void WriteNmToken(string name)
        {
            CheckAsync();
            CoreWriter.WriteNmToken(name);
        }

#if disabled
        public override Task WriteNmTokenAsync(string name)
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteNmTokenAsync(name));
        }

#endif
        public override void WriteNode(XmlReader reader, bool defattr)
        {
            CheckAsync();
            CoreWriter.WriteNode(reader, defattr);
        }

#if disabled
        public override Task WriteNodeAsync(XmlReader reader, bool defattr)
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteNodeAsync(reader, defattr));
        }
#endif
        public override void WriteProcessingInstruction(string name, string text)
        {
            CheckAsync();
            CoreWriter.WriteProcessingInstruction(name, text);
        }

#if disabled
        public override Task WriteProcessingInstructionAsync(string name, string text)
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteProcessingInstructionAsync(name, text));
        }
#endif
        public override void WriteQualifiedName(string localName, string ns)
        {
            CheckAsync();
            CoreWriter.WriteQualifiedName(localName, ns);
        }

#if disabled
        public override Task WriteQualifiedNameAsync(string localName, string ns)
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteQualifiedNameAsync(localName, ns));
        }
#endif
        public override void WriteRaw(string data)
        {
            CheckAsync();
            CoreWriter.WriteRaw(data);
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            CheckAsync();
            CoreWriter.WriteRaw(buffer, index, count);
        }

#if disabled
        public override Task WriteRawAsync(string data)
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteRawAsync(data));
        }

        public override Task WriteRawAsync(char[] buffer, int index, int count)
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteRawAsync(buffer, index, count));
        }
#endif
        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            CheckAsync();
            CoreWriter.WriteStartAttribute(prefix, localName, ns);
        }

        public override void WriteStartDocument()
        {
            CheckAsync();
            CoreWriter.WriteStartDocument();
        }

        public override void WriteStartDocument(bool standalone)
        {
            CheckAsync();
            CoreWriter.WriteStartDocument(standalone);
        }

#if disabled
        public override Task WriteStartDocumentAsync()
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteStartDocumentAsync());
        }

        public override Task WriteStartDocumentAsync(bool standalone)
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteStartDocumentAsync(standalone));
        }
#endif
        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            CheckAsync();
            CoreWriter.WriteStartElement(prefix, localName, ns);
        }

#if disabled
        public override Task WriteStartElementAsync(string prefix, string localName, string ns)
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteStartElementAsync(prefix, localName, ns));
        }
#endif
        public override void WriteString(string text)
        {
            CheckAsync();
            CoreWriter.WriteString(text);
        }

#if disabled
        public override Task WriteStringAsync(string text)
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteStringAsync(text));
        }
#endif
        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            CheckAsync();
            CoreWriter.WriteSurrogateCharEntity(lowChar, highChar);
        }

#if disabled
        public override Task WriteSurrogateCharEntityAsync(char lowChar, char highChar)
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteSurrogateCharEntityAsync(lowChar, highChar));
        }
#endif
        public override void WriteValue(string value)
        {
            CheckAsync();
            CoreWriter.WriteValue(value);
        }

        public override void WriteValue(double value)
        {
            CheckAsync();
            CoreWriter.WriteValue(value);
        }

        public override void WriteValue(int value)
        {
            CheckAsync();
            CoreWriter.WriteValue(value);
        }

        public override void WriteValue(long value)
        {
            CheckAsync();
            CoreWriter.WriteValue(value);
        }

        public override void WriteValue(object value)
        {
            CheckAsync();
            CoreWriter.WriteValue(value);
        }

        public override void WriteValue(float value)
        {
            CheckAsync();
            CoreWriter.WriteValue(value);
        }

        public override void WriteValue(decimal value)
        {
            CheckAsync();
            CoreWriter.WriteValue(value);
        }

        public override void WriteValue(DateTimeOffset value)
        {
            CheckAsync();
            CoreWriter.WriteValue(value);
        }

        public override void WriteValue(bool value)
        {
            CheckAsync();
            CoreWriter.WriteValue(value);
        }

        public override void WriteWhitespace(string ws)
        {
            CheckAsync();
            CoreWriter.WriteWhitespace(ws);
        }

#if disabled
        public override Task WriteWhitespaceAsync(string ws)
        {
            CheckAsync();
            return SetLastTask(CoreWriter.WriteWhitespaceAsync(ws));
        }
#endif
        public override void WriteStartElement(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri)
        {
            CheckAsync();
            CoreWriter.WriteStartElement(prefix, localName, namespaceUri);
        }

        public override void WriteStartAttribute(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri)
        {
            CheckAsync();
            CoreWriter.WriteStartAttribute(prefix, localName, namespaceUri);
        }

        public override void WriteXmlnsAttribute(string prefix, string namespaceUri)
        {
            CheckAsync();
            CoreWriter.WriteXmlnsAttribute(prefix, namespaceUri);
        }

        public override void WriteXmlnsAttribute(string prefix, XmlDictionaryString namespaceUri)
        {
            CheckAsync();
            CoreWriter.WriteXmlnsAttribute(prefix, namespaceUri);
        }

        public override void WriteXmlAttribute(string localName, string value)
        {
            CheckAsync();
            CoreWriter.WriteXmlAttribute(localName, value);
        }

        public override void WriteXmlAttribute(XmlDictionaryString localName, XmlDictionaryString value)
        {
            CheckAsync();
            CoreWriter.WriteXmlAttribute(localName, value);
        }

        public override void WriteString(XmlDictionaryString value)
        {
            CheckAsync();
            CoreWriter.WriteString(value);
        }

        public override void WriteQualifiedName(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
        {
            CheckAsync();
            CoreWriter.WriteQualifiedName(localName, namespaceUri);
        }

        public override void WriteValue(XmlDictionaryString value)
        {
            CheckAsync();
            CoreWriter.WriteValue(value);
        }

        public override void WriteValue(UniqueId value)
        {
            CheckAsync();
            CoreWriter.WriteValue(value);
        }

        public override void WriteValue(Guid value)
        {
            CheckAsync();
            CoreWriter.WriteValue(value);
        }

        public override void WriteValue(TimeSpan value)
        {
            CheckAsync();
            CoreWriter.WriteValue(value);
        }

        public override bool CanCanonicalize
        {
            get
            {
                CheckAsync();
                return CoreWriter.CanCanonicalize;
            }
        }

        public override void StartCanonicalization(Stream stream, bool includeComments, string[] inclusivePrefixes)
        {
            CheckAsync();
            CoreWriter.StartCanonicalization(stream, includeComments, inclusivePrefixes);
        }

        public override void EndCanonicalization()
        {
            CheckAsync();
            CoreWriter.EndCanonicalization();
        }

        public override void WriteNode(XmlDictionaryReader reader, bool defattr)
        {
            CheckAsync();
            CoreWriter.WriteNode(reader, defattr);
        }

        public override void WriteArray(string prefix, string localName, string namespaceUri, bool[] array, int offset, int count)
        {
            CheckAsync();
            CoreWriter.WriteArray(prefix, localName, namespaceUri, array, offset, count);
        }

        public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, bool[] array, int offset, int count)
        {
            CheckAsync();
            CoreWriter.WriteArray(prefix, localName, namespaceUri, array, offset, count);
        }

        public override void WriteArray(string prefix, string localName, string namespaceUri, Int16[] array, int offset, int count)
        {
            CheckAsync();
            CoreWriter.WriteArray(prefix, localName, namespaceUri, array, offset, count);
        }

        public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, Int16[] array, int offset, int count)
        {
            CheckAsync();
            CoreWriter.WriteArray(prefix, localName, namespaceUri, array, offset, count);
        }

        public override void WriteArray(string prefix, string localName, string namespaceUri, Int32[] array, int offset, int count)
        {
            CheckAsync();
            CoreWriter.WriteArray(prefix, localName, namespaceUri, array, offset, count);
        }

        public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, Int32[] array, int offset, int count)
        {
            CheckAsync();
            CoreWriter.WriteArray(prefix, localName, namespaceUri, array, offset, count);
        }

        public override void WriteArray(string prefix, string localName, string namespaceUri, Int64[] array, int offset, int count)
        {
            CheckAsync();
            CoreWriter.WriteArray(prefix, localName, namespaceUri, array, offset, count);
        }

        public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, Int64[] array, int offset, int count)
        {
            CheckAsync();
            CoreWriter.WriteArray(prefix, localName, namespaceUri, array, offset, count);
        }

        public override void WriteArray(string prefix, string localName, string namespaceUri, float[] array, int offset, int count)
        {
            CheckAsync();
            CoreWriter.WriteArray(prefix, localName, namespaceUri, array, offset, count);
        }

        public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, float[] array, int offset, int count)
        {
            CheckAsync();
            CoreWriter.WriteArray(prefix, localName, namespaceUri, array, offset, count);
        }

        public override void WriteArray(string prefix, string localName, string namespaceUri, double[] array, int offset, int count)
        {
            CheckAsync();
            CoreWriter.WriteArray(prefix, localName, namespaceUri, array, offset, count);
        }

        public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, double[] array, int offset, int count)
        {
            CheckAsync();
            CoreWriter.WriteArray(prefix, localName, namespaceUri, array, offset, count);
        }

        public override void WriteArray(string prefix, string localName, string namespaceUri, decimal[] array, int offset, int count)
        {
            CheckAsync();
            CoreWriter.WriteArray(prefix, localName, namespaceUri, array, offset, count);
        }

        public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, decimal[] array, int offset, int count)
        {
            CheckAsync();
            CoreWriter.WriteArray(prefix, localName, namespaceUri, array, offset, count);
        }

        public override void WriteArray(string prefix, string localName, string namespaceUri, DateTime[] array, int offset, int count)
        {
            CheckAsync();
            CoreWriter.WriteArray(prefix, localName, namespaceUri, array, offset, count);
        }

        public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, DateTime[] array, int offset, int count)
        {
            CheckAsync();
            CoreWriter.WriteArray(prefix, localName, namespaceUri, array, offset, count);
        }

        public override void WriteArray(string prefix, string localName, string namespaceUri, Guid[] array, int offset, int count)
        {
            CheckAsync();
            CoreWriter.WriteArray(prefix, localName, namespaceUri, array, offset, count);
        }

        public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, Guid[] array, int offset, int count)
        {
            CheckAsync();
            CoreWriter.WriteArray(prefix, localName, namespaceUri, array, offset, count);
        }

        public override void WriteArray(string prefix, string localName, string namespaceUri, TimeSpan[] array, int offset, int count)
        {
            CheckAsync();
            CoreWriter.WriteArray(prefix, localName, namespaceUri, array, offset, count);
        }

        public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, TimeSpan[] array, int offset, int count)
        {
            CheckAsync();
            CoreWriter.WriteArray(prefix, localName, namespaceUri, array, offset, count);
        }

        public override void Close()
        {
            CheckAsync();
            CoreWriter.Close();
        }

        protected override void Dispose(bool disposing)
        {
            CheckAsync();
            CoreWriter.Dispose();
        }
    }
}
