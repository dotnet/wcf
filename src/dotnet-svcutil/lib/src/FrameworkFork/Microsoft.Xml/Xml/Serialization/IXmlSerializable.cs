// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Serialization
{
    using System;
    using Microsoft.Xml;

    using Microsoft.Xml.Schema;

    /// <include file='doc\IXmlSerializable.uex' path='docs/doc[@for="IXmlSerializable"]/*' />
    ///<internalonly/>
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public interface IXmlSerializable
    {
        /// <include file='doc\IXmlSerializable.uex' path='docs/doc[@for="IXmlSerializable.GetSchema"]/*' />
        XmlSchema GetSchema();
        /// <include file='doc\IXmlSerializable.uex' path='docs/doc[@for="IXmlSerializable.ReadXml"]/*' />
        void ReadXml(XmlReader reader);
        /// <include file='doc\IXmlSerializable.uex' path='docs/doc[@for="IXmlSerializable.WriteXml"]/*' />
        void WriteXml(XmlWriter writer);
    }
}
