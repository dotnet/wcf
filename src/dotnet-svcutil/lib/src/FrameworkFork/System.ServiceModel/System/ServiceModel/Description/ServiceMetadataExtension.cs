// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Description
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Reflection;
    using System.Runtime;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.Threading;
    using Microsoft.Xml;
    using Microsoft.Xml.Schema;
    using WsdlNS = System.Web.Services.Description;

    // the description/metadata "mix-in"
    public class ServiceMetadataExtension
    {
        const string BaseAddressPattern = "{%BaseAddress%}";

        internal abstract class WriteFilter : XmlDictionaryWriter
        {
            internal XmlWriter Writer;
            public abstract WriteFilter CloneWriteFilter();
            public override void Close()
            {
                this.Writer.Close();
            }

            public override void Flush()
            {
                this.Writer.Flush();
            }

            public override string LookupPrefix(string ns)
            {
                return this.Writer.LookupPrefix(ns);
            }

            public override void WriteBase64(byte[] buffer, int index, int count)
            {
                this.Writer.WriteBase64(buffer, index, count);
            }

            public override void WriteCData(string text)
            {
                this.Writer.WriteCData(text);
            }

            public override void WriteCharEntity(char ch)
            {
                this.Writer.WriteCharEntity(ch);
            }

            public override void WriteChars(char[] buffer, int index, int count)
            {
                this.Writer.WriteChars(buffer, index, count);
            }

            public override void WriteComment(string text)
            {
                this.Writer.WriteComment(text);
            }

            public override void WriteDocType(string name, string pubid, string sysid, string subset)
            {
                this.Writer.WriteDocType(name, pubid, sysid, subset);
            }

            public override void WriteEndAttribute()
            {
                this.Writer.WriteEndAttribute();
            }

            public override void WriteEndDocument()
            {
                this.Writer.WriteEndDocument();
            }

            public override void WriteEndElement()
            {
                this.Writer.WriteEndElement();
            }

            public override void WriteEntityRef(string name)
            {
                this.Writer.WriteEntityRef(name);
            }

            public override void WriteFullEndElement()
            {
                this.Writer.WriteFullEndElement();
            }

            public override void WriteProcessingInstruction(string name, string text)
            {
                this.Writer.WriteProcessingInstruction(name, text);
            }

            public override void WriteRaw(string data)
            {
                this.Writer.WriteRaw(data);
            }

            public override void WriteRaw(char[] buffer, int index, int count)
            {
                this.Writer.WriteRaw(buffer, index, count);
            }

            public override void WriteStartAttribute(string prefix, string localName, string ns)
            {
                this.Writer.WriteStartAttribute(prefix, localName, ns);
            }

            public override void WriteStartDocument(bool standalone)
            {
                this.Writer.WriteStartDocument(standalone);
            }

            public override void WriteStartDocument()
            {
                this.Writer.WriteStartDocument();
            }

            public override void WriteStartElement(string prefix, string localName, string ns)
            {
                this.Writer.WriteStartElement(prefix, localName, ns);
            }

            public override WriteState WriteState
            {
                get { return this.Writer.WriteState; }
            }

            public override void WriteString(string text)
            {
                this.Writer.WriteString(text);
            }

            public override void WriteSurrogateCharEntity(char lowChar, char highChar)
            {
                this.Writer.WriteSurrogateCharEntity(lowChar, highChar);
            }

            public override void WriteWhitespace(string ws)
            {
                this.Writer.WriteWhitespace(ws);
            }
        }

        class LocationUpdatingWriter : WriteFilter
        {
            readonly string oldValue;
            readonly string newValue;

            // passing null for newValue filters any string with oldValue as a prefix rather than replacing
            internal LocationUpdatingWriter(string oldValue, string newValue)
            {
                this.oldValue = oldValue;

                this.newValue = newValue;
            }

            public override WriteFilter CloneWriteFilter()
            {
                return new LocationUpdatingWriter(oldValue, newValue);
            }

            public override void WriteString(string text)
            {
                if (this.newValue != null)
                    text = text.Replace(this.oldValue, this.newValue);
                else if (text.StartsWith(this.oldValue, StringComparison.Ordinal))
                    text = String.Empty;

                base.WriteString(text);
            }
        }

        class DynamicAddressUpdateWriter : WriteFilter
        {
            readonly string oldHostName;
            readonly string newHostName;
            readonly string newBaseAddress;
            readonly bool removeBaseAddress;
            readonly string requestScheme;
            readonly int requestPort;
            readonly IDictionary<string, int> updatePortsByScheme;

            internal DynamicAddressUpdateWriter(Uri listenUri, string requestHost, int requestPort,
                IDictionary<string, int> updatePortsByScheme, bool removeBaseAddress)
                : this(listenUri.Host, requestHost, removeBaseAddress, listenUri.Scheme, requestPort, updatePortsByScheme)
            {
                this.newBaseAddress = UpdateUri(listenUri).ToString();
            }

            DynamicAddressUpdateWriter(string oldHostName, string newHostName, string newBaseAddress, bool removeBaseAddress, string requestScheme,
                int requestPort, IDictionary<string, int> updatePortsByScheme)
                : this(oldHostName, newHostName, removeBaseAddress, requestScheme, requestPort, updatePortsByScheme)
            {
                this.newBaseAddress = newBaseAddress;
            }

            DynamicAddressUpdateWriter(string oldHostName, string newHostName, bool removeBaseAddress, string requestScheme,
                int requestPort, IDictionary<string, int> updatePortsByScheme)
            {
                this.oldHostName = oldHostName;
                this.newHostName = newHostName;
                this.removeBaseAddress = removeBaseAddress;
                this.requestScheme = requestScheme;
                this.requestPort = requestPort;
                this.updatePortsByScheme = updatePortsByScheme;
            }

            public override WriteFilter CloneWriteFilter()
            {
                return new DynamicAddressUpdateWriter(this.oldHostName, this.newHostName, this.newBaseAddress, this.removeBaseAddress,
                    this.requestScheme, this.requestPort, this.updatePortsByScheme);
            }

            public override void WriteString(string text)
            {
                Uri uri;
                if (this.removeBaseAddress &&
                    text.StartsWith(ServiceMetadataExtension.BaseAddressPattern, StringComparison.Ordinal))
                {
                    text = string.Empty;
                }
                else if (!this.removeBaseAddress &&
                    text.Contains(ServiceMetadataExtension.BaseAddressPattern))
                {
                    text = text.Replace(ServiceMetadataExtension.BaseAddressPattern, this.newBaseAddress);
                }
                else if (Uri.TryCreate(text, UriKind.Absolute, out uri))
                {
                    Uri newUri = UpdateUri(uri);
                    if (newUri != null)
                    {
                        text = newUri.ToString();
                    }
                }
                base.WriteString(text);
            }

            public void UpdateUri(ref Uri uri, bool updateBaseAddressOnly = false)
            {
                Uri newUri = UpdateUri(uri, updateBaseAddressOnly);
                if (newUri != null)
                {
                    uri = newUri;
                }
            }

            Uri UpdateUri(Uri uri, bool updateBaseAddressOnly = false)
            {
                // Ordinal comparison okay: we're filtering for auto-generated URIs which will
                // always be based off the listenURI, so always match in case
                if (uri.Host != oldHostName)
                {
                    return null;
                }

                UriBuilder result = new UriBuilder(uri);
                result.Host = this.newHostName;

                if (!updateBaseAddressOnly)
                {
                    int port;
                    if (uri.Scheme == this.requestScheme)
                    {
                        port = requestPort;
                    }
                    else if (!this.updatePortsByScheme.TryGetValue(uri.Scheme, out port))
                    {
                        return null;
                    }
                    result.Port = port;
                }

                return result.Uri;
            }
        }
    }
}
