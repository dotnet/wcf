// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
#if PRIVATE_RTLIB
using XmlNS = Microsoft.Xml;
#else
using XmlNS = System.Xml;
#endif
using WsdlNS = System.Web.Services.Description;

namespace Microsoft.Tools.ServiceModel.Svcutil.Metadata
{
    internal class MetadataFileInfo
    {
        internal MetadataFileInfo(object metadata, string filePath, string sourceUri, Action<XmlNS.XmlWriter> write)
        {
            Debug.Assert(metadata != null && filePath != null && write != null, "One or more parameters null!");

            this.Metadata = metadata;
            this.FilePath = filePath;
            this.SourceUri = sourceUri;
            this.Write = write;
        }

        public string FilePath { get; private set; }
        public string SourceUri { get; private set; }
        public object Metadata { get; private set; }
        public Action<XmlNS.XmlWriter> Write { get; private set; }

        public bool Equals(MetadataFileInfo other)
        {
            return other != null && (Object.ReferenceEquals(this, other) || this.Equals(other.FilePath));
        }

        public bool Equals(string otherFilePath)
        {
            return otherFilePath != null && MetadataFileNameManager.UriEqual(this.FilePath, otherFilePath);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MetadataFileInfo);
        }

        public override int GetHashCode()
        {
            return this.FilePath.ToUpperInvariant().GetHashCode();
        }

        public override string ToString()
        {
            return this.FilePath;
        }
    }

    internal class WsdlFileInfo : MetadataFileInfo
    {
        internal WsdlFileInfo(object metadata, string filePath, string sourceUri, Action<XmlNS.XmlWriter> write) : base(metadata, filePath, sourceUri, write)
        {
        }

        internal WsdlNS.ServiceDescription Wsdl
        {
            get
            {
                return this.Metadata as WsdlNS.ServiceDescription;
            }
        }
    }

    internal class SchemaFileInfo : MetadataFileInfo
    {
        internal SchemaFileInfo(object metadata, string filePath, string sourceUri, Action<XmlNS.XmlWriter> write) : base(metadata, filePath, sourceUri, write)
        {
        }

        internal XmlNS.Schema.XmlSchema Schema
        {
            get
            {
                return this.Metadata as XmlNS.Schema.XmlSchema;
            }
        }
    }
}
